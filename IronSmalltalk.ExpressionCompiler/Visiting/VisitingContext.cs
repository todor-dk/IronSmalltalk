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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.Runtime.Execution.Internals;
using IronSmalltalk.Runtime.Internal;
using IronSmalltalk.ExpressionCompiler.Primitives;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public sealed class VisitingContext
    {
        public VisitingContext(ExpressionCompiler compiler, BindingScope globalScope, BindingScope reservedScope, Expression self, Expression executionContext, IEnumerable<Expression> arguments, string superLookupScope, string codeName)
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
            if (arguments == null)
                throw new ArgumentNullException("Arguments");
            this.Compiler = compiler;
            this.GlobalScope = globalScope;
            this.ReservedScope = reservedScope;
            this.Self = self;
            this.ExecutionContext = executionContext;
            this.Arguments = arguments.ToList().AsReadOnly();
            this.SuperLookupScope = superLookupScope;
            this._ReturnLabel = null; // Lazy init
            this._HomeContext = null; // Lazy init
            this._TempValue = null; // Lazy init

            this.CodeName = codeName;
        }

        /// <summary>
        /// Binding lookup scope for identifiers of globals and similar, e.g. global variables, class or instance variables, pool variables etc.
        /// </summary>
        public BindingScope GlobalScope { get; private set; }

        /// <summary>
        /// Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.
        /// </summary>
        public BindingScope ReservedScope { get; private set; }

        public ExpressionCompiler Compiler { get; private set; }

        public Expression ExecutionContext { get; private set; }

        public Expression Self { get; private set; }

        private string CodeName { get; set; }

        /// <summary>
        /// Arguments passed to the method. This excludes "self" and "executionContext"
        /// </summary>
        public IReadOnlyList<Expression> Arguments { get; private set; }

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

        private static readonly ConstructorInfo HomeContextCtor = TypeUtilities.Constructor(typeof(HomeContext));

        private Expression _HomeContext;
        private ParameterExpression _HomeContextVariable;
        public Expression HomeContext
        {
            get
            {
                if (this._HomeContext == null)
                {
                    // Semantics are as follows:
                    // .... = ( (_HomeContext == null) ? (_HomeContext = new HomeContext()) : _HomeContext );
                    ParameterExpression homeContextVariable = Expression.Variable(typeof(HomeContext), "_HomeContext");
                    this._HomeContext = Expression.Condition(
                        Expression.ReferenceEqual(homeContextVariable, Expression.Constant(null, typeof(HomeContext))),
                        Expression.Assign(homeContextVariable, Expression.New(VisitingContext.HomeContextCtor)),
                        homeContextVariable,
                        typeof(HomeContext));
                    this._HomeContextVariable = homeContextVariable;
                }
                return this._HomeContext;
            }
        }

        private ParameterExpression _TempValue;
        internal Expression TempValue
        {
            get
            {
                if (this._TempValue == null)
                    this._TempValue = Expression.Variable(typeof(object), "_TempValue");
                return this._TempValue;
            }
        }

        public Expression GeneratePrologAndEpilogue(Expression result)
        {
            // Somebody requested explicit return
            if (this._ReturnLabel != null)
                result = Expression.Label(this._ReturnLabel, result);

            // Somebody requested the home context used for block returns ... we must handle this correctly with try ... catch ...
            if (this._HomeContext != null)
            {


                if (this.Compiler.CompilerOptions.LightweightExceptions)
                {
                    result = Expression.Block(new ParameterExpression[] { this._HomeContextVariable }, result);
                }
                else
                {
                    // Semantics:
                    // HomeContext homeContext = new HomeContext();     ... however, this is lazy init'ed
                    // try
                    // {
                    //      return <<result>>;
                    // } 
                    // .... this is how we would like to have it ... if we could. CLR limitations do not allow filters in dynamic methods ....
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
            }

            if (this._TempValue != null)
                result = Expression.Block(new ParameterExpression[] { this._TempValue }, result);

            return result;
        }

        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, Expression receiver)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContext));
        }

        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, Expression receiver, Expression argument)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContext, argument));
        }

        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, Expression receiver, IEnumerable<Expression> arguments)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContext, arguments));
        }

        // Helper for normal send binary messages - because it's used often when inlining
        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, Expression receiver, Expression argument)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, selector, false, false, this.SuperLookupScope, receiver, this.ExecutionContext, argument));
        }

        private Expression CompileLightweightExceptionCheck(EncoderVisitor visitor, Expression expression)
        {
            if (!this.Compiler.CompilerOptions.LightweightExceptions)
                return expression;

            // Improve ... this needs clean-up and refactore to use polymorphism
            if (visitor.TempValue == this._TempValue)
            {
                var na = this.HomeContext; // Init needed stuff
                return Expression.Block(
                    Expression.Assign(visitor.TempValue, expression),
                    Expression.IfThen(
                        Expression.TypeIs(visitor.TempValue, typeof(BlockResult)),
                        Expression.IfThenElse(
                            // Improve ... unnecessary test if the  methods doesn't declare blocks.
                            Expression.Equal(Expression.Field(Expression.Convert(visitor.TempValue, typeof(BlockResult)), BlockResult.HomeContextField), this._HomeContextVariable),
                            visitor.ReturnLocal(Expression.Field(Expression.Convert(visitor.TempValue, typeof(BlockResult)), BlockResult.ValueField)),
                            visitor.ReturnLocal(visitor.TempValue))),
                    visitor.TempValue);
            }
            else
            {
                return Expression.Block(
                    Expression.Assign(visitor.TempValue, expression),
                    Expression.IfThen(
                        Expression.TypeIs(visitor.TempValue, typeof(BlockResult)),
                        visitor.ReturnLocal(visitor.TempValue)),
                    visitor.TempValue);
            }
        }

        public Expression CompileGetClass(Expression receiver)
        {
            return this.Compiler.DynamicCallStrategy.CompileGetClass(this, receiver, this.ExecutionContext);
        }

        public Expression CompileDynamicConvert(Expression parameter, Type type, Conversion conversion)
        {
            return this.Compiler.DynamicCallStrategy.CompileDynamicConvert(this, parameter, type, conversion);
        }

        private int BlockNumber = 0;

        internal string GetLambdaName(BlockVisitor visitor, BlockNode block)
        {
            this.BlockNumber++;
            if (String.IsNullOrWhiteSpace(this.CodeName))
                return null;
            else
                return String.Format(CultureInfo.InvariantCulture, "[{0}] in {1}", this.BlockNumber, this.CodeName);
        }
    }
}
