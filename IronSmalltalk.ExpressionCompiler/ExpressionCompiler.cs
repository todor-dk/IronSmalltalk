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
        /// Service providing information necessary to emit 
        /// debugging symbol information for a source file.
        /// 
        /// If this property is set, the generator will emit debug(able) code.
        /// </summary>
        public IDebugInfoService DebugInfoService { get; private set; }

        protected SmalltalkRuntime Runtime { get; private set; }

        public ILiteralEncodingStrategy LiteralEncoding { get; private set; }

        public IDynamicCallStrategy DynamicCallStrategy { get; private set; }

        public CompilerOptions CompilerOptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="compilerOptions">Options that control the workings of the compiler.</param>
        protected ExpressionCompiler(SmalltalkRuntime runtime, CompilerOptions compilerOptions)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (compilerOptions == null)
                compilerOptions = new CompilerOptions(); // Default options

            this.Runtime = runtime;
            this.CompilerOptions = compilerOptions;
            this.DebugInfoService = compilerOptions.DebugInfoService; // Optional, null is OK
            this.LiteralEncoding = compilerOptions.LiteralEncodingStrategy ?? new LiteralEncodingStrategy();
            this.DynamicCallStrategy = compilerOptions.DynamicCallStrategy ?? new DynamicCallStrategy();
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

        internal CallSiteBinder GetBinder(string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
        {
            CallSiteBinder binder;
            if (isSuperSend)
            {
                return CSB.RuntimeHelpers.CreateSuperSendCallSiteBinder(selector, superLookupScope, argumentCount);
            }
            else if (isConstantReceiver)
            {
                binder = this.BinderCache.ConstantSendCache.GetBinder(selector);
                if (binder == null)
                    binder = this.BinderCache.ConstantSendCache.AddBinder(
                        CSB.RuntimeHelpers.CreateConstantCallSiteBinder(selector, nativeName, argumentCount));
            }
            else
            {
                binder = this.BinderCache.MessageSendCache.GetBinder(selector);
                if (binder == null)
                    binder = this.BinderCache.MessageSendCache.AddBinder(
                        CSB.RuntimeHelpers.CreateCallSiteBinder(selector, nativeName, argumentCount));
            }

            return binder;
        }

        internal ObjectClassCallSiteBinder GetClassBinder()
        {
            ObjectClassCallSiteBinder binder = this.BinderCache.CachedObjectClassCallSiteBinder;
            if (binder == null)
            {
                binder = new ObjectClassCallSiteBinder();
                this.BinderCache.CachedObjectClassCallSiteBinder = binder;
            }
            return binder;
        }
    }
}
