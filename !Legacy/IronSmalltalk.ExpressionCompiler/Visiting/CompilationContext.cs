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
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.ExpressionCompiler.Bindings;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public abstract class CompilationContext
    {
        internal CompilationContext(ExpressionCompiler compiler, BindingScope globalScope, BindingScope reservedScope, Expression self, Expression executionContext, string superLookupScope)
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
            this.ExecutionContextArgument = executionContext;
            this.SuperLookupScope = superLookupScope;
            this.LocalScope = new BindingScope();
            this._ReturnLabel = null; // Lazy init
        }

        public ExpressionCompiler Compiler { get; private set; }

        public abstract RootCompilationContext RootContext { get; }

        #region Bindings and Arguments

        /// <summary>
        /// Binding lookup scope for identifiers of globals and similar, e.g. global variables, class or instance variables, pool variables etc.
        /// </summary>
        public BindingScope GlobalScope { get; private set; }

        /// <summary>
        /// Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.
        /// </summary>
        public BindingScope ReservedScope { get; private set; }

        /// <summary>
        /// Binding lookup scope with locally defined identifiers, e.g. arguments and temporary variables.
        /// </summary>
        public BindingScope LocalScope { get; private set; }

        public Expression Self { get; private set; }

        public Expression ExecutionContextArgument { get; private set; }

        /// <summary>
        /// Collection of temporary variables bindings. We need this to define the vars in the AST block.
        /// </summary>
        internal readonly List<TemporaryBinding> Temporaries = new List<TemporaryBinding>();

        internal void DefineArgument(string name)
        {
            this.DefineArgument(new ArgumentBinding(name));
        }

        internal void DefineArgument(string name, Expression expression)
        {
            this.DefineArgument(new ArgumentBinding(name, expression));
        }

        internal virtual void DefineArgument(ArgumentBinding argument)
        {
            if (argument == null)
                throw new ArgumentNullException();
            this.LocalScope.DefineBinding(argument);
        }

        internal void DefineTemporary(string name)
        {
            TemporaryBinding temporary = new TemporaryBinding(name);
            this.Temporaries.Add(temporary);
            this.LocalScope.DefineBinding(temporary);
        }

        internal abstract NameBinding GetBinding(string name);

        internal NameBinding GetLocalVariable(string name)
        {
            NameBinding result = this.LocalScope.GetBinding(name);
            if (result != null)
                return result;

            return new ErrorBinding(name);
        }

        #endregion

        #region Compilation

        public string SuperLookupScope { get; private set; } // For initializers, always null

        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, Expression receiver)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContextArgument));
        }

        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, Expression receiver, Expression argument)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContextArgument, argument));
        }

        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, Expression receiver, IEnumerable<Expression> arguments)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, this.SuperLookupScope, receiver, this.ExecutionContextArgument, arguments));
        }

        // Helper for normal send binary messages - because it's used often when inlining
        public Expression CompileDynamicCall(EncoderVisitor visitor, string selector, Expression receiver, Expression argument)
        {
            return this.CompileLightweightExceptionCheck(visitor,
                this.Compiler.DynamicCallStrategy.CompileDynamicCall(this, selector, selector, false, false, this.SuperLookupScope, receiver, this.ExecutionContextArgument, argument));
        }

        internal abstract Expression CompileLightweightExceptionCheck(EncoderVisitor visitor, Expression expression);

        internal abstract Expression GeneratePrologAndEpilogue(List<Expression> expressions);

        public Expression CompileGetClass(Expression receiver)
        {
            return this.Compiler.DynamicCallStrategy.CompileGetClass(this, receiver, this.ExecutionContextArgument);
        }

        public Expression CompileDynamicConvert(Expression parameter, Type type, Conversion conversion)
        {
            return this.Compiler.DynamicCallStrategy.CompileDynamicConvert(this, parameter, type, conversion);
        }

        protected LabelTarget _ReturnLabel;
        protected LabelTarget ReturnLabel
        {
            get
            {
                if (this._ReturnLabel == null)
                    this._ReturnLabel = Expression.Label(typeof(object), "return");
                return this._ReturnLabel;
            }
        }

        protected ParameterExpression _TempValue;
        protected Expression TempValue
        {
            get
            {
                if (this._TempValue == null)
                    this._TempValue = Expression.Variable(typeof(object), "_TempValue");
                return this._TempValue;
            }
        }

        internal abstract Expression Return(Expression value);

        internal Expression ReturnLocal(Expression value)
        {
            return Expression.Return(this.ReturnLabel, value, typeof(object));
        }

        #endregion
    }
}
