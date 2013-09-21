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
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using System.Collections;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.NativeCompiler.CompilationStrategies;

namespace IronSmalltalk.NativeCompiler.Generators.Initializers
{
    /// <summary>
    /// Generates initializers (class, global (variable/constant), pool item (variable/constant) and program).
    /// </summary>
    internal abstract class InitializerGenerator : GeneratorBase
    {
        internal static InitializerGenerator GetInitializerGenerator(NativeCompiler compiler, CompiledInitializer initializer)
        {
            if (initializer == null)
                throw new ArgumentNullException("initializer");
            switch (initializer.Type)
            {
                case InitializerType.ProgramInitializer:
                    RuntimeProgramInitializer programInitializer = initializer as RuntimeProgramInitializer;
                    if (programInitializer == null)
                        throw new Exception("Expected to see a RuntimeProgramInitializer"); // ... since we have compiled everything ourself.
                    return new ProgramInitializerGenerator(compiler, programInitializer);
                case InitializerType.GlobalInitializer:
                    RuntimeGlobalInitializer globalInitializer = initializer as RuntimeGlobalInitializer;
                    if (globalInitializer == null)
                        throw new Exception("Expected to see a RuntimeGlobalInitializer"); // ... since we have compiled everything ourself.
                    return new GlobalInitializerGenerator(compiler, globalInitializer);
                case InitializerType.ClassInitializer:
                    RuntimeGlobalInitializer classInitializer = initializer as RuntimeGlobalInitializer;
                    if (classInitializer == null)
                        throw new Exception("Expected to see a RuntimeGlobalInitializer"); // ... since we have compiled everything ourself.
                    return new ClassInitializerGenerator(compiler, classInitializer);
                case InitializerType.PoolVariableInitializer:
                    RuntimePoolItemInitializer poolItemInitializer = initializer as RuntimePoolItemInitializer;
                    if (poolItemInitializer == null)
                        throw new Exception("Expected to see a RuntimePoolItemInitializer"); // ... since we have compiled everything ourself.
                    return new PoolVariableInitializerGenerator(compiler, poolItemInitializer);
                default:
                    throw new Exception(String.Format("Unrecognized initializer type {0}", initializer.Type));
            }
        }

        internal InitializerGenerator(NativeCompiler compiler)
            : base(compiler)
        {
        }

        protected string MethodName { get; private set; }

        /// <summary>
        /// Generate initializer methods with the logic of each initializer.
        /// Those are contained in a class named ".Initializers.ScopeName_Initializers".
        /// </summary>
        internal void GenerateInitializerMethod(TypeBuilder type, NativeLiteralEncodingStrategy literalEncodingStrategy, NativeDynamicCallStrategy dynamicCallStrategy, NativeDiscreteBindingEncodingStrategy discreteBindingEncodingStrategy, ISet<string> names)
        {
            string name = this.GetInitializerName(names);
            this.MethodName = name;
            MethodBuilder method = type.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static);
            var lambda = this.GenerateInitializerLambda(literalEncodingStrategy, dynamicCallStrategy, discreteBindingEncodingStrategy, name);
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

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

        protected abstract string GetInitializerNameSuggestion();

        protected abstract Expression<Func<object, ExecutionContext, object>> GenerateInitializerLambda(NativeLiteralEncodingStrategy literalEncodingStrategy, NativeDynamicCallStrategy dynamicCallStrategy, NativeDiscreteBindingEncodingStrategy discreteBindingEncodingStrategy, string name);

        /// <summary>
        /// Generate an expression to call a helper method to add the initializer to the name scope.
        /// Example: CompiledInitializer initializer = NativeLoadHelper.AddClassInitializer(runtime, scope, delegateType, "BigDecimal_Class_Initializer", "BigDecimal");
        /// </summary>
        /// <param name="initializer">Parameter representing the variable where to store the initializer.</param>
        /// <param name="runtime">Parameter representing the SmalltalkRuntime.</param>
        /// <param name="scope">Parameter representing the NameScope.</param>
        /// <param name="initializersType">Parameter representing a variable with the type containing the initializer methods.</param>
        /// <returns></returns>
        internal abstract IEnumerable<Expression> GenerateCreateInitializer(ParameterExpression initializer, ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType);
    }

    internal abstract class InitializerGenerator<TInitializer> : InitializerGenerator
        where TInitializer : RuntimeCompiledInitializer
    {
        protected readonly TInitializer Initializer;
        
        internal InitializerGenerator(NativeCompiler compiler, TInitializer initializer)
            : base(compiler)
        {
            if (initializer == null)
                throw new ArgumentNullException("initializer");
            this.Initializer = initializer;
        }

        protected abstract InitializerCompiler GetInitializerCompiler(NativeLiteralEncodingStrategy literalEncodingStrategy, NativeDynamicCallStrategy dynamicCallStrategy, NativeDiscreteBindingEncodingStrategy discreteBindingEncodingStrategy);

        protected InitializerCompiler GetInitializerCompiler(BindingScope globalScope, BindingScope reservedScope, NativeLiteralEncodingStrategy literalEncodingStrategy, NativeDynamicCallStrategy dynamicCallStrategy, NativeDiscreteBindingEncodingStrategy discreteBindingEncodingStrategy)
        {
            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = this.Initializer.GetDebugInfoService();
            options.LiteralEncodingStrategy = literalEncodingStrategy;
            options.DynamicCallStrategy = dynamicCallStrategy;
            options.DiscreteBindingEncodingStrategy = discreteBindingEncodingStrategy;

            return new InitializerCompiler(this.Compiler.Parameters.Runtime, options, globalScope, reservedScope);
        }

        protected override Expression<Func<object, ExecutionContext, object>> GenerateInitializerLambda(NativeLiteralEncodingStrategy literalEncodingStrategy, NativeDynamicCallStrategy dynamicCallStrategy, NativeDiscreteBindingEncodingStrategy discreteBindingEncodingStrategy, string name)
        {
            return this.GetInitializerCompiler(literalEncodingStrategy, dynamicCallStrategy, discreteBindingEncodingStrategy).CompileInitializer(this.Initializer.ParseTree, name);
        }

        /// <summary>
        /// Generate an expression to call a helper method to add the initializer to the name scope.
        /// Example: CompiledInitializer initializer = NativeLoadHelper.AddClassInitializer(runtime, scope, delegateType, "BigDecimal_Class_Initializer", "BigDecimal");
        /// </summary>
        /// <param name="initializer">Parameter representing the variable where to store the initializer.</param>
        /// <param name="runtime">Parameter representing the SmalltalkRuntime.</param>
        /// <param name="scope">Parameter representing the NameScope.</param>
        /// <param name="initializersType">Parameter representing a variable with the type containing the initializer methods.</param>
        /// <returns></returns>
        internal override IEnumerable<Expression> GenerateCreateInitializer(ParameterExpression initializer, ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType)
        {
            MethodCallExpression initializerCall = this.GenerateInitializerCall(runtime, scope, initializersType);
            if (initializerCall == null)
                return new Expression[0];

            List<Expression> expressions = new List<Expression>();
            expressions.Add(Expression.Assign(initializer, initializerCall));

            foreach (KeyValuePair<string, string> pair in this.Initializer.Annotations)
            {
                expressions.Add(Expression.Call(InitializerGenerator <TInitializer>.AnnotateObjectMethod,
                    initializer,
                    Expression.Constant(pair.Key, typeof(string)),
                    Expression.Constant(pair.Value, typeof(string))));
            }

            return expressions;
        }

        protected abstract MethodCallExpression GenerateInitializerCall(ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType);

        private static readonly MethodInfo AnnotateObjectMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AnnotateObject",
            typeof(CompiledInitializer), typeof(string), typeof(string));
    }
}
