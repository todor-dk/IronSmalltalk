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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    internal interface ICallSiteBinderCacheItem<TKey>
        where TKey : class
    {
        TKey CacheKey { get; }
        ICallSiteBinderCacheFinalizationManager<TKey> FinalizationManager { get; set; }
    }

    internal interface ICallSiteBinderCacheFinalizationManager<TKey>
    {
        void InternalRemoveItem(TKey key);
    }

    internal class CallSiteBinderCacheTable<TKey, TValue>
        where TKey : class
        where TValue : class, ICallSiteBinderCacheItem<TKey>
    {
        private readonly WeakCallSiteBinderCache WeakCache;
        private readonly ConcurrentDictionary<TKey, TValue> StrongCache;

        public CallSiteBinderCacheTable(IEnumerable<TKey> strongKeys, IEqualityComparer<TKey> comparer)
        {
            this.WeakCache = new WeakCallSiteBinderCache(comparer);
            this.StrongCache = new ConcurrentDictionary<TKey, TValue>(Environment.ProcessorCount, 250, comparer);
            if (strongKeys != null)
            {
                foreach (TKey key in strongKeys)
                    this.StrongCache[key] = null;
            }
        }

        public TValue GetBinder(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();

            TValue result;
            this.StrongCache.TryGetValue(key, out result);
            if (result != null)
                return result;

            return this.WeakCache.GetItem(key);
        }

        public TValue AddBinder(TValue binder)
        {
            if (binder == null)
                throw new ArgumentNullException();

            TValue result;
            if (this.StrongCache.TryGetValue(binder.CacheKey, out result))
            {
                // It is one of the common selectors
                if (result != null)
                    // Already cached
                    return result;
                // Cache it and return ...
                this.StrongCache.TryUpdate(binder.CacheKey, binder, null);
                return this.StrongCache[binder.CacheKey];
            }

            return this.WeakCache.AddItem(binder);
        }

        /// <summary>
        /// A weak dictionary of call-site-binders.
        /// </summary>
        /// <remarks>
        /// This is roughly based on the SymbolTable
        /// </remarks>
        private class WeakCallSiteBinderCache : ICallSiteBinderCacheFinalizationManager<TKey>
        {
            private ConcurrentDictionary<TKey, WeakReference> _Contents;

            /// <summary>
            /// Create and initialize an empty weak table.
            /// </summary>
            internal WeakCallSiteBinderCache(IEqualityComparer<TKey> comparer)
            {
                // We expect very low concurrency on writing ... 
                // Use StringComparer.InvariantCulture ... because keys are case-sensitive etc.
                this._Contents = new ConcurrentDictionary<TKey, WeakReference>(Environment.ProcessorCount, 250, comparer);
            }

            /// <summary>
            /// Get a Call-Site-Binder that represent a message send with the given selector.
            /// </summary>
            /// <param name="selector">String value of the selector.</param>
            /// <returns>An existing Call-Site-Binder or null if none.</returns>
            internal TValue GetItem(TKey key)
            {
                if (key == null)
                    throw new ArgumentNullException();

                // 1. Try to get the CSB from the dictionary. There are good changes that:
                WeakReference reference;
                this._Contents.TryGetValue(key, out reference);
                if (reference == null)
                    return null;
                // 2. Get the CSB from the weak reference holding it
                return reference.Target as TValue;
            }

            internal TValue AddItem(TValue binder)
            {
                if (binder == null)
                    throw new ArgumentNullException();

                // 1. Try to get the CSB from the dictionary. 
                binder.FinalizationManager = this;
                WeakReference reference = this._Contents.GetOrAdd(binder.CacheKey, na => new WeakReference(binder, false));
                // 2. Get the CSB from the weak reference holding it
                TValue result = reference.Target as TValue;
                // Once here, it can't be GC'ed.
                if (result != null)
                    // somebody else managed to put 
                    return result;
                reference.Target = binder;
                return binder;
            }

            /// <summary>
            /// A Call-Site-Binder was GC'ed. Remove the Call-Site-Binder info from the internal string-CSB dictionary.
            /// </summary>
            /// <param name="selector">String value of the Call-Site-Binder that was GC'ed.</param>
            void ICallSiteBinderCacheFinalizationManager<TKey>.InternalRemoveItem(TKey key)
            {
                WeakReference reference;
                this._Contents.TryGetValue(key, out reference);
                if (reference == null)
                    return;

                // Check if the weak reference's Target reference a CSB. It may:
                //      a. CSB is null ... 1.a. ... CSB was GC'ed and GC set Target to null.
                //      b. CSB is NOT null ... 1.b. ... CSB was GC'ed and GC set Target to null,
                //         however, before this code managed to run, somebody requested a CSB with the
                //         same selector, and a new CSB object was created.
                //         Therefore, we cannot throw the weak reference away!
                TValue csb = reference.Target as TValue;
                if (csb == null)
                    // Remove the weak reference from the contents dictionary ... this is case a).
                    this._Contents.TryRemove(key, out reference);
            }
        }
    }
}
