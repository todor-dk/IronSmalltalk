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

using IronSmalltalk.Common.Internal;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.NativeCompiler.CompilationStrategies;
using IronSmalltalk.Runtime.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.Generators.Initializers
{
    internal sealed class GlobalInitializerGenerator : InitializerGenerator<RuntimeGlobalInitializer>
    {
        public GlobalInitializerGenerator(NativeCompiler compiler, RuntimeGlobalInitializer initializer)
            : base(compiler, initializer)
        {
        }

        protected override string GetInitializerNameSuggestion()
        {
            return String.Format("{0}_Global_Initializer", this.Initializer.Binding.Name.Value);
        }

        protected override InitializerCompiler GetInitializerCompiler(NativeLiteralEncodingStrategy literalEncodingStrategy, NativeDynamicCallStrategy dynamicCallStrategy, NativeDiscreteBindingEncodingStrategy discreteBindingEncodingStrategy)
        {
            BindingScope globalScope = BindingScope.ForGlobalInitializer(this.Compiler.Parameters.Runtime.GlobalScope);
            BindingScope reservedScope = ReservedScope.ForGlobalInitializer();
            return this.GetInitializerCompiler(globalScope, reservedScope, literalEncodingStrategy, dynamicCallStrategy, discreteBindingEncodingStrategy);
        }

        protected override MethodCallExpression GenerateInitializerCall(ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType)
        {
            return Expression.Call(GlobalInitializerGenerator.AddGlobalInitializerMethod,
                runtime,
                scope,
                initializersType,
                Expression.Constant(this.MethodName, typeof(string)),
                Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
        }

        private static readonly MethodInfo AddGlobalInitializerMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AddGlobalInitializer",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string));
    }
}
