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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    /// <summary>
    /// CallSiteBinderCache caches the Smalltalk call-site-binders across a SmalltalkRuntime.
    /// </summary>
    /// <remarks>
    /// One CallSiteBinderCache exists per SmalltalkRuntime and the cached binders in it
    /// belong to the same SmalltalkRuntime.
    /// </remarks>
    public class CallSiteBinderCache
    {
        public static readonly CallSiteBinderCache Current = new CallSiteBinderCache();

        private readonly CallSiteBinderCacheTable<string, MessageSendCallSiteBinder> MessageSendCache
            = new CallSiteBinderCacheTable<string, MessageSendCallSiteBinder>(CallSiteBinderCache.CommonSelectors, StringComparer.InvariantCulture);
        private readonly CallSiteBinderCacheTable<string, ConstantSendCallSiteBinder> ConstantSendCache
            = new CallSiteBinderCacheTable<string, ConstantSendCallSiteBinder>(CallSiteBinderCache.CommonSelectors, StringComparer.InvariantCulture);
        private readonly CallSiteBinderCacheTable<string, SymbolCallSiteBinder> SymbolCache
            = new CallSiteBinderCacheTable<string, SymbolCallSiteBinder>(null, StringComparer.InvariantCulture);
        private readonly CallSiteBinderCacheTable<string, DiscreteBindingVariableValueCallSiteBinder> DiscreteVariableBindingCache
            = new CallSiteBinderCacheTable<string, DiscreteBindingVariableValueCallSiteBinder>(null, StringComparer.InvariantCulture);
        private readonly CallSiteBinderCacheTable<string, DiscreteBindingConstantValueCallSiteBinder> DiscreteConstantBindingCache
            = new CallSiteBinderCacheTable<string, DiscreteBindingConstantValueCallSiteBinder>(null, StringComparer.InvariantCulture);
        private readonly CallSiteBinderCacheTable<string, DiscreteBindingObjectCallSiteBinder> DiscreteObjectBindingCache
            = new CallSiteBinderCacheTable<string, DiscreteBindingObjectCallSiteBinder>(null, StringComparer.InvariantCulture);

        /// <summary>
        /// Cached ObjectClassCallSiteBinder.
        /// </summary>
        /// <remarks>
        /// Only one ObjectClassCallSiteBinder exists per SmalltalkRuntime.
        /// This is due to the fact that the ObjectClassCallSiteBinder does
        /// not contain any instance specific information.
        /// </remarks>
        public readonly ObjectClassCallSiteBinder ObjectClassCallSiteBinder = new ObjectClassCallSiteBinder();

        /// <summary>
        /// Selectors of messages we consider common and worth caching aggressively.
        /// </summary>
        /// <remarks>
        /// The list below was creating by examining an existing Smalltalk source code
        /// and determining the most often sent messages (as number of call-sites).
        /// </remarks>
        public static readonly string[] CommonSelectors = new string[] {
            "=", "~=", "==", "~~", ">", ">=", "<", "<=",    // comparison operations 
            "+", "-", "*", "/", "\\\\", "//",               // arithmetic operations 
            "&", "|",                                       // logical operations 
            "@", ",",                                       // miscellaneous 
            "add:", "addAll:", "and:", "asString", "at:", "at:ifAbsent:", "at:put:", "atEnd", 
            "basicAt:", "basicAt:put:", "basicHash", "basicHash:", "basicNew", "basicNew:", "basicSize", 
            "between:and:", "class", "do:", "doesNotUnderstand:", "ensure:",
            "ifFalse:", "ifFalse:ifTrue:", "ifTrue:", "ifTrue:ifFalse:", 
            "isEmpty", "isNil", "key", "max:", "min:", "new", "new:", 
            "nextPut:", "nextPutAll:", "not", "notNil", "on:do:", "or:", "printOn:", "printString", 
            "propertyAt:", "propertyAt:ifAbsent:", "propertyAt:ifAbsentPut:", "propertyAt:put:", 
            "release", "size", "to:by:do:", "to:do:", "triggerEvent:", "value", "value:", "value:value:", 
            "vmInterrupt:", "when:send:to:", "when:send:to:with:", 
            "whileFalse", "whileFalse:", "whileTrue", "whileTrue:", "with:", "with:with:", "x", "y", "yourself"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="nativeName"></param>
        /// <param name="argumentCount"></param>
        /// <param name="isSuperSend"></param>
        /// <param name="isConstantReceiver"></param>
        /// <param name="superLookupScope"></param>
        /// <returns></returns>
        /// <remarks>
        /// There ARE senders of this method!
        /// </remarks>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static CallSiteBinder GetMessageBinder(string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
        {
            if (isSuperSend)
            {
                // Those are not cached
                return new SuperSendCallSiteBinder(selector, superLookupScope);
            }
            else if (isConstantReceiver)
            {
                CallSiteBinder binder = CallSiteBinderCache.Current.ConstantSendCache.GetBinder(selector);
                if (binder == null)
                    binder = CallSiteBinderCache.Current.ConstantSendCache.AddBinder(new ConstantSendCallSiteBinder(selector, nativeName, argumentCount));
                return binder;
            }
            else
            {
                CallSiteBinder binder = CallSiteBinderCache.Current.MessageSendCache.GetBinder(selector);
                if (binder == null)
                    binder = CallSiteBinderCache.Current.MessageSendCache.AddBinder(new MessageSendCallSiteBinder(selector, nativeName, argumentCount));
                return binder;
            }
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static SymbolCallSiteBinder GetSymbolBinder(string symbolKey)
        {
            if (symbolKey == null)
                throw new ArgumentNullException();
            SymbolCallSiteBinder binder = CallSiteBinderCache.Current.SymbolCache.GetBinder(symbolKey);
            if (binder == null)
                binder = CallSiteBinderCache.Current.SymbolCache.AddBinder(new SymbolCallSiteBinder(symbolKey));
            return binder;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static ObjectClassCallSiteBinder GetObjectClassBinder()
        {
            return CallSiteBinderCache.Current.ObjectClassCallSiteBinder;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static DiscreteBindingVariableValueCallSiteBinder GetDiscreteVariableBinding(string moniker)
        {
            if (moniker == null)
                throw new ArgumentNullException();
            DiscreteBindingVariableValueCallSiteBinder binder = CallSiteBinderCache.Current.DiscreteVariableBindingCache.GetBinder(moniker);
            if (binder == null)
                binder = CallSiteBinderCache.Current.DiscreteVariableBindingCache.AddBinder(new DiscreteBindingVariableValueCallSiteBinder(moniker));
            return binder;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static DiscreteBindingConstantValueCallSiteBinder GetDiscreteConstantBinding(string moniker)
        {
            if (moniker == null)
                throw new ArgumentNullException();
            DiscreteBindingConstantValueCallSiteBinder binder = CallSiteBinderCache.Current.DiscreteConstantBindingCache.GetBinder(moniker);
            if (binder == null)
                binder = CallSiteBinderCache.Current.DiscreteConstantBindingCache.AddBinder(new DiscreteBindingConstantValueCallSiteBinder(moniker));
            return binder;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static DiscreteBindingObjectCallSiteBinder GetDiscreteObjectBinding(string moniker)
        {
            if (moniker == null)
                throw new ArgumentNullException();
            DiscreteBindingObjectCallSiteBinder binder = CallSiteBinderCache.Current.DiscreteObjectBindingCache.GetBinder(moniker);
            if (binder == null)
                binder = CallSiteBinderCache.Current.DiscreteObjectBindingCache.AddBinder(new DiscreteBindingObjectCallSiteBinder(moniker));
            return binder;
        }
    }
}
