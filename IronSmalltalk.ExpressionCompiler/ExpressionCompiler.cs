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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using CSB = IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.ExpressionCompiler
{
    public abstract class ExpressionCompiler
    {
        /// <summary>
        /// Binding lookup scope for identifiers of globals and similar, e.g. global variables, class or instance variables, pool variables etc.
        /// </summary>
        public BindingScope GlobalScope { get; private set; }

        /// <summary>
        /// Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.
        /// </summary>
        public BindingScope ReservedScope { get; private set; }

        /// <summary>
        /// Service providing information necessary to emit 
        /// debugging symbol information for a source file.
        /// 
        /// If this property is set, the generator will emit debug(able) code.
        /// </summary>
        public IDebugInfoService DebugInfoService { get; private set; }


        protected SmalltalkRuntime Runtime { get; private set; }

        public LiteralEncodingStrategy LiteralEncoding { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="globalScope">Binding lookup scope with global identifiers, e.g. globals, class variables, instance variables etc.</param>
        /// <param name="reservedScope">Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.</param>
        /// <param name="debugInfoService">Optional debug info service if the generator is to emit debug symbols.</param>
        public ExpressionCompiler(SmalltalkRuntime runtime, BindingScope globalScope, BindingScope reservedScope, IDebugInfoService debugInfoService)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (globalScope == null)
                throw new ArgumentNullException("globalScope");
            if (reservedScope == null)
                throw new ArgumentNullException("reservedScope");

            this.Runtime = runtime;
            this.GlobalScope = globalScope;
            this.ReservedScope = reservedScope;
            this.DebugInfoService = debugInfoService; // Optional, null is OK
            this.LiteralEncoding = new LiteralEncodingStrategy();
        }

        private CallSiteBinderCache _binderCache;
        public CallSiteBinderCache BinderCache
        {
            get
            {
                if (this._binderCache == null)
                    this._binderCache = CallSiteBinderCache.GetCache(this.Runtime);
                return this._binderCache;
            }
        }

        public Expression AddDebugInfo(Expression expression, IParseNode node)
        {
            if (this.DebugInfoService == null)
                return expression;
            var tokens = node.GetTokens().Concat(node.GetChildNodes().SelectMany(n => n.GetTokens()));
            SourceLocation start = tokens.Min(t => t.StartPosition);
            SourceLocation end = tokens.Min(t => t.StopPosition);

            start = this.DebugInfoService.TranslateSourcePosition(start);
            end = this.DebugInfoService.TranslateSourcePosition(end);

            DebugInfoExpression debugInfo = Expression.DebugInfo(this.DebugInfoService.SymbolDocument,
                start.Line, start.Column, end.Line, end.Column);
            return Expression.Block(debugInfo, expression);
        }

        public Symbol GetSymbol(string value)
        {
            return this.Runtime.GetSymbol(value);
        }

        public Expression CompileDynamicCall(string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, 
            Expression receiver, Expression executionContext)
        {
            CallSiteBinder binder = this.GetBinder(selector, nativeName, 0, isSuperSend, isConstantReceiver, superLookupScope);
            return Expression.Dynamic(binder, typeof(Object), receiver, executionContext);
        }

        public Expression CompileDynamicCall(string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext, Expression argument)
        {
            CallSiteBinder binder = this.GetBinder(selector, nativeName, 1, isSuperSend, isConstantReceiver, superLookupScope);
            return Expression.Dynamic(binder, typeof(Object), receiver, executionContext, argument);
        }

        public Expression CompileDynamicCall(string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            CallSiteBinder binder = this.GetBinder(selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, superLookupScope);
            List<Expression> args = new List<Expression>();
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);
            return Expression.Dynamic(binder, typeof(Object), args);
        }

        private CallSiteBinder GetBinder(string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
        {
            CallSiteBinder binder;
            if (isSuperSend)
            {
                return CSB.RuntimeHelpers.CreateSuperSendCallSiteBinder(this.Runtime, selector, superLookupScope, argumentCount);
            }
            else if (isConstantReceiver)
            {
                binder = this.BinderCache.ConstantSendCache.GetBinder(selector);
                if (binder == null)
                    binder = this.BinderCache.ConstantSendCache.AddBinder(
                        CSB.RuntimeHelpers.CreateConstantCallSiteBinder(this.Runtime, selector, nativeName, argumentCount));
            }
            else
            {
                binder = this.BinderCache.MessageSendCache.GetBinder(selector);
                if (binder == null)
                    binder = this.BinderCache.MessageSendCache.AddBinder(
                        CSB.RuntimeHelpers.CreateCallSiteBinder(this.Runtime, selector, nativeName, argumentCount));
            }

            return binder;
        }

        public ObjectClassCallSiteBinder GetClassBinder()
        {
            ObjectClassCallSiteBinder binder = this.BinderCache.CachedObjectClassCallSiteBinder;
            if (binder == null)
            {
                binder = new ObjectClassCallSiteBinder(this.Runtime);
                this.BinderCache.CachedObjectClassCallSiteBinder = binder;
            }
            return binder;
        }
    }
}
