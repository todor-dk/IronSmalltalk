/*
 * **************************************************************************
 *
 * Copyright (c) The IronSmalltalk Project. 
 *
 * This source code is subject to terms and conditions of the 
 * license agreement found in the solution directory. 
 * See: $(SolutionDir)\License.htm ... in the root of this distribution.
 * By using this source code in any fashion, you are agreeing 
 * to be bound by the terms of the license agreement.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * **************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.NativeCompiler.Generators;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.NativeCompiler.Generators.Initializers
{
    /// <summary>
    /// Generates initializers (class, global (variable/constant), pool item (variable/constant) and program).
    /// </summary>
    internal sealed class InitializerGenerator : GeneratorBase
    {
        private readonly CompiledInitializer Initializer;

        internal InitializerGenerator(NativeCompiler compiler, CompiledInitializer initializer)
            : base(compiler)
        {
            this.Initializer = initializer;
        }

        /// <summary>
        /// Generate initializer methods with the logic of each initializer.
        /// Those are contained in a class named ".Initializers.ScopeName_Initializers".
        /// </summary>
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
            ParameterExpression self = Expression.Parameter(typeof(object), "self");
            ParameterExpression context = Expression.Parameter(typeof(ExecutionContext), "executionContext");

            // BUG BUG BUG TO-DO ... compile the initializer!

            return Expression.Lambda<Func<object, ExecutionContext, object>>(self, name, new ParameterExpression[] { self, context });
        }

        private InitializerCompiler _InitializerCompiler = null;
        private InitializerCompiler InitializerCompiler
        {
            get
            {
                if (this._InitializerCompiler == null)
                    this._InitializerCompiler = this.GetInitializerCompiler();
                return this._InitializerCompiler;
            }
        }
        private InitializerCompiler GetInitializerCompiler()
        {
            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = null;    // BUG-BUG 
            options.LiteralEncodingStrategy = this.LiteralEncodingStrategy;
            options.DynamicCallStrategy = this.DynamicCallStrategy;
            return new InitializerCompiler(this.Compiler.Parameters.Runtime, options,
        }



        /// <summary>
        /// Generate an expression to call a helper method to add the initializer to the name scope.
        /// Example: CompiledInitializer initializer = NativeLoadHelper.AddClassInitializer(runtime, scope, delegateType, "BigDecimal_Class_Initializer", "BigDecimal");
        /// </summary>
        /// <param name="initializer">Parameter representing the variable where to store the initializer.</param>
        /// <param name="runtime">Parameter representing the SmalltalkRuntime.</param>
        /// <param name="scope">Parameter representing the NameScope.</param>
        /// <param name="initializersType">Parameter representing a variable with the type containing the initializor methods.</param>
        /// <returns></returns>
        internal IEnumerable<Expression> GenerateCreateInitializer(ParameterExpression initializer, ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType)
        {
            MethodCallExpression initializerCall;

            switch (this.Initializer.Type)
            {
                case InitializerType.ProgramInitializer:
                    initializerCall = Expression.Call(InitializerGenerator.AddProgramInitializerMethod,
                        runtime,
                        scope,
                        initializersType,
                        Expression.Constant(this.MethodName, typeof(string)));
                    break;
                case InitializerType.GlobalInitializer:
                    initializerCall = Expression.Call(InitializerGenerator.AddGlobalInitializerMethod,
                        runtime,
                        scope,
                        initializersType,
                        Expression.Constant(this.MethodName, typeof(string)),
                        Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
                    break;
                case InitializerType.ClassInitializer:
                    initializerCall = Expression.Call(InitializerGenerator.AddClassInitializerMethod,
                        runtime,
                        scope,
                        initializersType,
                        Expression.Constant(this.MethodName, typeof(string)),
                        Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
                    break;
                case InitializerType.PoolVariableInitializer:
                    initializerCall = null;
                    foreach (PoolBinding pool in this.Compiler.Parameters.Runtime.Pools)
                    {
                        if ((pool.Value != null) && pool.Value.Contains((PoolVariableOrConstantBinding)this.Initializer.Binding))
                        {
                            initializerCall = Expression.Call(InitializerGenerator.AddPoolInitializerMethod,
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

            foreach (KeyValuePair<string, string> pair in this.Initializer.Annotations)
            {
                expressions.Add(Expression.Call(InitializerGenerator.AnnotateObjectMethod,
                    initializer,
                    Expression.Constant(pair.Key, typeof(string)),
                    Expression.Constant(pair.Value, typeof(string))));
            }

            return expressions;
        }

        private static readonly MethodInfo AddProgramInitializerMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AddProgramInitializer",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string));

        private static readonly MethodInfo AddClassInitializerMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AddClassInitializer",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string));

        private static readonly MethodInfo AddGlobalInitializerMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AddGlobalInitializer",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string));

        private static readonly MethodInfo AddPoolInitializerMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AddPoolInitializer",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string), typeof(string));

        private static readonly MethodInfo AnnotateObjectMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AnnotateObject",
            typeof(CompiledInitializer), typeof(string), typeof(string));
    }
}
