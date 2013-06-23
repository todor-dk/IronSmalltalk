using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class PoolGenerator : GlobalBindingGenerator<PoolBinding>
    {
        internal PoolGenerator(NativeCompiler compiler, PoolBinding binding)
            : base(compiler, binding)
        {
        }


        private TypeBuilder Type;

        internal override void GenerateTypes()
        {
            //this.Type = this.Compiler.NativeGenerator.DefineType(
            //    this.Compiler.GetTypeName("Pools", this.Binding.Name),
            //    typeof(Object),
            //    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);
        }

        protected override string AddBindingMethodName
        {
            get { return "AddPoolBinding"; }
        }

        private static readonly Type[] CreateObjectMethodParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(PoolBinding)
        };

        private MethodInfo GetCreateObjectMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("CreatePool", BindingFlags.Static | BindingFlags.Public, null, PoolGenerator.CreateObjectMethodParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method CreatePool in class {0}.", helperType.FullName));
            return method;
        }

        internal override IEnumerable<Expression> GenerateCreateObject(ParameterExpression runtime, NameScopeGenerator scopeGenerator, ParameterExpression scope, ParameterExpression binding)
        {
            string name = String.Format("Init_{0}", this.Binding.Name.Value);
            Type initializerType = scopeGenerator.PoolsInitializerType;
            MethodInfo initializer = initializerType.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(SmalltalkRuntime), typeof(PoolBinding) }, null);

            MethodInfo method = this.GetCreateObjectMethod();
            return new Expression[] 
            {
                Expression.Call(method, runtime, binding),
                Expression.Call(initializer, runtime, binding)
            };
        }

        internal string GeneratePoolInitializer(TypeBuilder type)
        {
            string name = String.Format("Init_{0}", this.Binding.Name.Value);
            this.GeneratePoolInitializer(type, name);
            return name;
        }

        private void GeneratePoolInitializer(TypeBuilder type, string name)
        {
            MethodBuilder method = type.DefineMethod(name, MethodAttributes.Assembly | MethodAttributes.Static);

            var lambda = this.GeneratePoolInitializerLambda(name);
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

        private Expression<Action<SmalltalkRuntime, PoolBinding>> GeneratePoolInitializerLambda(string name)
        {
            MethodInfo constantMethod = this.GetCreatePoolConstantBindingMethod();
            MethodInfo variableMethod = this.GetCreatePoolVariableBindingMethod();
            MethodInfo annotateMethod = this.GetAnnotateObjectMethod();

            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            ParameterExpression binding = Expression.Parameter(typeof(PoolBinding), "poolBinding");
            ParameterExpression tempBinding = Expression.Parameter(typeof(PoolVariableOrConstantBinding), "tempBinding");
            List<Expression> expressions = new List<Expression>();

            foreach (PoolVariableOrConstantBinding varBinding in this.Binding.Value.Values)
            {
                MethodInfo method = varBinding.IsConstantBinding ? constantMethod : variableMethod;
                expressions.Add(Expression.Assign(
                    tempBinding,
                    Expression.Convert(
                        Expression.Call(method, runtime, binding, Expression.Constant(varBinding.Name.Value, typeof(string))),
                        typeof(PoolVariableOrConstantBinding))));

                foreach (KeyValuePair<string, string> pair in varBinding.Annotations)
                {
                    expressions.Add(Expression.Call(annotateMethod,
                        tempBinding,
                        Expression.Constant(pair.Key, typeof(string)),
                        Expression.Constant(pair.Value, typeof(string))));
                }
            }

            return Expression.Lambda<Action<SmalltalkRuntime, PoolBinding>>(
                Expression.Block(new ParameterExpression[] { tempBinding }, expressions), name, new ParameterExpression[] { runtime, binding });
        }

        private static readonly Type[] CreatePoolVariableBindingParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(PoolBinding), typeof(string)
        };

        private MethodInfo GetCreatePoolConstantBindingMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("CreatePoolConstantBinding", BindingFlags.Static | BindingFlags.Public, null, PoolGenerator.CreatePoolVariableBindingParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method CreatePoolConstantBinding in class {0}.", helperType.FullName));
            return method;
        }

        private MethodInfo GetCreatePoolVariableBindingMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("CreatePoolVariableBinding", BindingFlags.Static | BindingFlags.Public, null, PoolGenerator.CreatePoolVariableBindingParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method CreatePoolVariableBinding in class {0}.", helperType.FullName));
            return method;
        }
    }
}
