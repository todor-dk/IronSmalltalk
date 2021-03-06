﻿/*
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
    internal sealed class PoolVariableInitializerGenerator : InitializerGenerator<RuntimePoolItemInitializer>
    {
        public PoolVariableInitializerGenerator(NativeCompiler compiler, RuntimePoolItemInitializer initializer)
            : base(compiler, initializer)
        {
        }

        protected override string GetInitializerNameSuggestion()
        {
            return String.Format("{0}_{1}_PoolVariable_Initializer", this.Initializer.PoolName, this.Initializer.Binding.Name.Value);
        }

        protected override InitializerCompiler GetInitializerCompiler(NativeLiteralEncodingStrategy literalEncodingStrategy, NativeDynamicCallStrategy dynamicCallStrategy, NativeDiscreteBindingEncodingStrategy discreteBindingEncodingStrategy)
        {
            PoolBinding poolBinding = this.Compiler.Parameters.Runtime.GlobalScope.GetPoolBinding(this.Initializer.PoolName);
            if ((poolBinding == null) || (poolBinding.Value == null))
                throw new Exception(String.Format("Cannot find pool named {0}", this.Initializer.PoolName)); // May be better exception type

            BindingScope globalScope = BindingScope.ForPoolInitializer(poolBinding.Value, this.Compiler.Parameters.Runtime.GlobalScope);
            BindingScope reservedScope = ReservedScope.ForPoolInitializer();
            return this.GetInitializerCompiler(globalScope, reservedScope, literalEncodingStrategy, dynamicCallStrategy, discreteBindingEncodingStrategy);
        }

        protected override MethodCallExpression GenerateInitializerCall(ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType)
        {
            return Expression.Call(PoolVariableInitializerGenerator.AddPoolInitializerMethod,
                runtime,
                scope,
                initializersType,
                Expression.Constant(this.MethodName, typeof(string)),
                Expression.Constant(this.Initializer.PoolName, typeof(string)),
                Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
        }

        private static readonly MethodInfo AddPoolInitializerMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AddPoolInitializer",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string), typeof(string));
    }
}
