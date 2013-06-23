using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class InitializerGenerator : GeneratorBase
    {
        private readonly CompiledInitializer Initializer;

        internal InitializerGenerator(NativeCompiler compiler, CompiledInitializer initializer)
            : base(compiler)
        {
            this.Initializer = initializer;
        }

        internal void GenerateInitializerMethod(TypeBuilder type, ISet<string> names)
        {
            string name = this.GetInitializerName(names);
            this.MethodName = name;
            MethodBuilder method = type.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static);
            var lambda = this.GenerateInitializerLambda(name);
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

        private string MethodName;

        private string GetInitializerName(ISet<string> names)
        {
            string name = this.GetInitializerNameSuggestion();
            if (names.Contains(name))
            {
                int i = 1;
                string tmp;
                do
                {
                    tmp = String.Format("{0}_{1}", name, i);
                    i++;
                } while (names.Contains(tmp));
                name = tmp;
            }
            names.Add(name);
            return name;
        }

        private string GetInitializerNameSuggestion()
        {
            switch (this.Initializer.Type)
            {
                case InitializerType.ProgramInitializer:
                    return "Program_Initializer";
                case InitializerType.GlobalInitializer:
                    return String.Format("{0}_Global_Initializer", this.Initializer.Binding.Name.Value);
                case InitializerType.ClassInitializer:
                    return String.Format("{0}_Class_Initializer", this.Initializer.Binding.Name.Value);
                case InitializerType.PoolVariableInitializer:
                    foreach (PoolBinding pool in this.Compiler.Parameters.Runtime.Pools)
                    {
                        if ((pool.Value != null) && pool.Value.Contains((PoolVariableOrConstantBinding) this.Initializer.Binding))
                            return String.Format("{0}_{1}_PoolVariable_Initializer", pool.Name.Value, this.Initializer.Binding.Name.Value);
                    }
                    return String.Format("{0}_PoolVariable_Initializer", this.Initializer.Binding.Name.Value); // Just in case ...
                default:
                    throw new InvalidOperationException();
            }
        }

        private Expression<Func<object, ExecutionContext, object>> GenerateInitializerLambda(string name)
        {
            ParameterExpression self = Expression.Parameter(typeof(object));
            ParameterExpression context = Expression.Parameter(typeof(ExecutionContext), "executionContext");

            return Expression.Lambda<Func<object, ExecutionContext, object>>(self, name, new ParameterExpression[] { self, context });
        } 

        internal IEnumerable<Expression> GenerateCreateInitializer(ParameterExpression initializer, ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType)
        {
            MethodInfo method;
            MethodCallExpression initializerCall;

            switch (this.Initializer.Type)
            {
                case InitializerType.ProgramInitializer:
                    method = this.GetAddProgramInitializerMethod();
                    initializerCall = Expression.Call(method,
                        runtime,
                        scope,
                        initializersType,
                        Expression.Constant(this.MethodName, typeof(string)));
                    break;
                case InitializerType.GlobalInitializer:
                    method = this.GetAddGlobalInitializerMethod();
                    initializerCall = Expression.Call(method,
                        runtime,
                        scope,
                        initializersType,
                        Expression.Constant(this.MethodName, typeof(string)),
                        Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
                    break;
                case InitializerType.ClassInitializer:
                    method = this.GetAddClassInitializerMethod();
                    initializerCall = Expression.Call(method,
                        runtime,
                        scope,
                        initializersType,
                        Expression.Constant(this.MethodName, typeof(string)),
                        Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
                    break;
                case InitializerType.PoolVariableInitializer:
                    method = this.GetAddPoolInitializerMethod();
                    initializerCall = null;
                    foreach (PoolBinding pool in this.Compiler.Parameters.Runtime.Pools)
                    {
                        if ((pool.Value != null) && pool.Value.Contains((PoolVariableOrConstantBinding)this.Initializer.Binding))
                        {
                            initializerCall = Expression.Call(method,
                                runtime,
                                scope,
                                initializersType,
                                Expression.Constant(this.MethodName, typeof(string)),
                                Expression.Constant(pool.Name.Value, typeof(string)),
                                Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
                            break;
                        }
                    }
                    if (initializerCall == null)
                        return new Expression[0];
                    break;
                default:
                    throw new Exception(String.Format("Unrecognized initializer type {0}", this.Initializer.Type));
            }

            List<Expression> expressions = new List<Expression>();
            expressions.Add(Expression.Assign(initializer, initializerCall));

            method = this.GetAnnotateObjectMethod();
            foreach (KeyValuePair<string, string> pair in this.Initializer.Annotations)
            {
                expressions.Add(Expression.Call(method,
                    initializer,
                    Expression.Constant(pair.Key, typeof(string)),
                    Expression.Constant(pair.Value, typeof(string))));
            }

            return expressions;
        }

        private static readonly Type[] AddProgramInitializerParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string)
        };

        private MethodInfo GetAddProgramInitializerMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AddProgramInitializer", BindingFlags.Static | BindingFlags.Public, null, InitializerGenerator.AddProgramInitializerParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AddProgramInitializer in class {0}.", helperType.FullName));
            return method;
        }

        private static readonly Type[] AddClassInitializerParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string)
        };

        private MethodInfo GetAddClassInitializerMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AddClassInitializer", BindingFlags.Static | BindingFlags.Public, null, InitializerGenerator.AddClassInitializerParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AddClassInitializer in class {0}.", helperType.FullName));
            return method;
        }

        private static readonly Type[] AddGlobalInitializerParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string)
        };

        private MethodInfo GetAddGlobalInitializerMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AddGlobalInitializer", BindingFlags.Static | BindingFlags.Public, null, InitializerGenerator.AddGlobalInitializerParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AddGlobalInitializer in class {0}.", helperType.FullName));
            return method;
        }

        private static readonly Type[] AddPoolInitializerParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string), typeof(string)
        };

        private MethodInfo GetAddPoolInitializerMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AddPoolInitializer", BindingFlags.Static | BindingFlags.Public, null, InitializerGenerator.AddPoolInitializerParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AddPoolInitializer in class {0}.", helperType.FullName));
            return method;
        }

        private static readonly Type[] AnnotateObjectParameterTypes = new Type[]
        {
            typeof(CompiledInitializer), typeof(string), typeof(string)
        };

        private MethodInfo GetAnnotateObjectMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AnnotateObject", BindingFlags.Static | BindingFlags.Public, null, AnnotateObjectParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AnnotateObject in class {0}.", helperType.FullName));
            return method;
        }
    }
}
