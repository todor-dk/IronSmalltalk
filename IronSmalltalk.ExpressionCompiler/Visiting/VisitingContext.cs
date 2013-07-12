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

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.Runtime.Execution.Internals;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public sealed class VisitingContext
    {
        public VisitingContext(ExpressionCompiler compiler, BindingScope globalScope, BindingScope reservedScope, DynamicMetaObject self, DynamicMetaObject executionContext, string superLookupScope)
        {
            if (compiler == null)
                throw new ArgumentNullException("compiler");
            if (globalScope == null)
                throw new ArgumentNullException("globalScope");
            if (reservedScope == null)
                throw new ArgumentNullException("reservedScope");
            if (self == null)
                throw new ArgumentNullException("self");
            if (executionContext == null)
                throw new ArgumentNullException("executionContext");
            this.Compiler = compiler;
            this.GlobalScope = globalScope;
            this.ReservedScope = reservedScope;
            this.Self = self;
            this.ExecutionContext = executionContext;
            this.SuperLookupScope = superLookupScope;
            this._ReturnLabel = null; // Lazy init
            this._HomeContext = null; // Lazy init
        }

        /// <summary>
        /// Binding lookup scope for identifiers of globals and similar, e.g. global variables, class or instance variables, pool variables etc.
        /// </summary>
        public BindingScope GlobalScope { get; private set; }

        /// <summary>
        /// Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.
        /// </summary>
        public BindingScope ReservedScope { get; private set; }

        /// <summary>
        /// Gets or sets binding restrictions to be applied together with the visited executable code.
        /// </summary>
        public BindingRestrictions BindingRestrictions { get; set; }

        public ExpressionCompiler Compiler { get; private set; }

        public DynamicMetaObject ExecutionContext { get; private set; }

        public DynamicMetaObject Self { get; private set; }

        public string SuperLookupScope { get; private set; } // For initializers, always null

        private LabelTarget _ReturnLabel;
        public LabelTarget ReturnLabel
        {
            get
            {
                if (this._ReturnLabel == null)
                    this._ReturnLabel = Expression.Label(typeof(object), "return");
                return this._ReturnLabel;
            }
        }

        private Expression _HomeContext;
        private ParameterExpression _HomeContextVariable;
        public Expression HomeContext
        {
            get
            {
                if (this._HomeContext == null)
                {
                    System.Reflection.ConstructorInfo ctor = typeof(HomeContext).GetConstructor(new Type[0]);
                    if (ctor == null)
                        throw new InternalCodeGenerationException(CodeGenerationErrors.InternalError);

                    // Semantics are as follows:
                    // .... = ( (_HomeContext == null) ? (_HomeContext = new HomeContext()) : _HomeContext );
                    ParameterExpression homeContextVariable = Expression.Variable(typeof(HomeContext), "_HomeContext");
                    this._HomeContext = Expression.Condition(
                        Expression.ReferenceEqual(homeContextVariable, Expression.Constant(null, typeof(HomeContext))),
                        Expression.Assign(homeContextVariable, Expression.New(ctor)),
                        homeContextVariable,
                        typeof(HomeContext));
                    this._HomeContextVariable = homeContextVariable;
                }
                return this._HomeContext;
            }
        }

        public Expression GeneratePrologAndEpilogue(Expression result)
        {
            // Somebody requested explicit return
            if (this._ReturnLabel != null)
                result = Expression.Label(this._ReturnLabel, result);

            // Somebody requested the home context used for block returns ... we must handle this correcty with try ... catch ...
            if (this._HomeContext != null)
            {
                // Semantics:
                // HomeContext homeContext = new HomeContext();     ... however, this is lazy init'ed
                // try
                // {
                //      return <<result>>;
                // } 
                // .... this is how we would like to have it ... if we could. CLR limitations do not allow filters in dynamic methos ....
                // catch (BlockResult blockResult) where (blockResult.HomeContext == homeContext)       ... the where semantics are not part of C#
                // {
                //      return blockResult.Result;
                // }
                // .... therefore the following implementation ....
                // catch (BlockResult blockResult)
                // {
                //      if (blockResult.HomeContext == homeContext)
                //          return blockResult.Result;
                //      else
                //          throw;
                // }

                ParameterExpression blockResult = Expression.Parameter(typeof(BlockResult), "blockResult");
                CatchBlock catchBlock = Expression.Catch(
                    blockResult,
                    Expression.Condition(
                        Expression.ReferenceEqual(Expression.Field(blockResult, BlockResult.HomeContextField), this._HomeContextVariable),
                        Expression.Field(blockResult, BlockResult.ValueField),
                        Expression.Rethrow(typeof(object))));

                result = Expression.Block(new ParameterExpression[] { this._HomeContextVariable }, Expression.TryCatch(result, catchBlock));
            }

            return result;
        }

        public Expression CompileDynamicCall(string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, Expression receiver)
        {
            return this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContext.Expression);
        }

        public Expression CompileDynamicCall(string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, Expression receiver, Expression argument)
        {
            return this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContext.Expression, argument);
        }

        public Expression CompileDynamicCall(string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, Expression receiver, IEnumerable<Expression> arguments)
        {
            return this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContext.Expression, arguments);
        }

        // Helper for normal send binary messages - because it's used often when inlining
        public Expression CompileDynamicCall(string selector, Expression receiver, Expression argument)
        {
            return this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, selector, false, false, this.SuperLookupScope, receiver, this.ExecutionContext.Expression, argument);
        }
    }
}
