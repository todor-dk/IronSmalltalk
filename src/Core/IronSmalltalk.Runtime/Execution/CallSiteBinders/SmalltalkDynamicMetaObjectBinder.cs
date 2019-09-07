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
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
//using Microsoft.Scripting.Generation;

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    /// <summary>
    /// Base class for IromSmalltalk's DynamicMetaObjectBinders.
    /// This class contains very little functionality, but is 
    /// used to logically group the binders.
    /// </summary>
    /// <remarks>
    /// Each concrete subclass will be responsible for implementing the Bind method.
    /// </remarks>
    public abstract class SmalltalkDynamicMetaObjectBinder : DynamicMetaObjectBinder
    {
        /// <summary>
        /// Create a new SmalltalkDynamicMetaObjectBinder.
        /// </summary>
        protected SmalltalkDynamicMetaObjectBinder()
        {
        }

#if DEBUGCALLSITE

        public override T BindDelegate<T>(System.Runtime.CompilerServices.CallSite<T> site, object[] args)
        {
            //LambdaExpression lambda = null; // this.Bind()
            //Delegate precompiled = lambda.Compile(true); // Cache this
            //T result = (T) (object) precompiled;
            //this.CacheTarget(result);
            //return result;

            //
            // Get the Expression for the binding 
            //
            var signature = LambdaSignature<T>.Instance;
            Expression binding = Bind(args, signature.Parameters, signature.ReturnLabel);

            //
            // Check the produced rule 
            // 
            if (binding == null)
            {
                throw new InvalidOperationException("No or Invalid Rule Produced");
            }

            //
            // finally produce the new rule if we need to 
            //

            // We cannot compile rules in the heterogeneous app domains since they 
            // may come from less trusted sources
            // Silverlight always uses a homogenous appdomain, so we don’t need this check 
            if (!AppDomain.CurrentDomain.IsHomogenous)
            {
                throw new InvalidOperationException("Homogenous AppDomain Required");
            }

            Expression<T> e = Stitch(binding, signature);
            T newRule = e.Compile(true);

            CacheTarget(newRule);

            return newRule;
            //return base.BindDelegate<T>(site, args);
        }

        private static Expression<T> Stitch<T>(Expression binding, LambdaSignature<T> signature) where T : class
        {
            ParameterExpression expression;
            Type type = typeof(CallSite<T>);
            ReadOnlyCollectionBuilder<Expression> expressions = new ReadOnlyCollectionBuilder<Expression>(3) {
                binding
            };
            ParameterExpression[] source = signature.Parameters.AddFirst(expression = Expression.Parameter(typeof(CallSite), "$site"));
            Expression item = Expression.Label(UpdateLabel);
            expressions.Add(item);
            expressions.Add(Expression.Label(signature.ReturnLabel, Expression.Condition(Expression.Call(typeof(CallSiteOps).GetMethod("SetNotMatched"), source.First()), Expression.Default(signature.ReturnLabel.Type), Expression.Invoke(Expression.Property(Expression.Convert(expression, type), typeof(CallSite<T>).GetProperty("Update")), new ReadOnlyCollection<Expression>(source)))));
            return Expression.Lambda<T>(Expression.Block(expressions)
                , "CallSite.Target", 
                true, new ReadOnlyCollection<ParameterExpression>(source));
        }

        private sealed class LambdaSignature<T> where T : class
        {
            internal static readonly LambdaSignature<T> Instance;
            internal readonly ReadOnlyCollection<ParameterExpression> Parameters;
            internal readonly LabelTarget ReturnLabel;

            static LambdaSignature()
            {
                LambdaSignature<T>.Instance = new LambdaSignature<T>();
            }

            private LambdaSignature()
            {
                Type type = typeof(T);
                if (!type.IsSubclassOf(typeof(MulticastDelegate)))
                {
                    throw new InvalidOperationException(String.Format("Type {0} Parameter Is Not Delegate", type));
                }
                MethodInfo method = type.GetMethod("Invoke");
                ParameterInfo[] parametersCached = method.GetParametersCached();
                if (parametersCached[0].ParameterType != typeof(CallSite))
                {
                    throw new ArgumentException("First Argument Must Be CallSite");
                }
                ParameterExpression[] list = new ParameterExpression[parametersCached.Length - 1];
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = Expression.Parameter(parametersCached[i + 1].ParameterType, "$arg" + i);
                }
                this.Parameters = new ReadOnlyCollection<ParameterExpression>(list);
                this.ReturnLabel = Expression.Label(method.GetReturnType());
            }
        }
#endif
    }

    internal static class HackExtensions
    {
        private static readonly CacheDict<MethodBase, ParameterInfo[]> ParamInfoCache;
        private static readonly Assembly Mscorlib;
        private static readonly Assembly SystemCore;

        static HackExtensions()
        {
            Mscorlib = typeof(object).Assembly;
            SystemCore = typeof(Expression).Assembly;
            ParamInfoCache = new CacheDict<MethodBase, ParameterInfo[]>(0x4b);
        }

        internal class CacheDict<TKey, TValue>
        {
            // Fields
            private readonly Dictionary<TKey, KeyInfo> _dict;
            private readonly LinkedList<TKey> _list;
            private readonly int _maxSize;

            internal CacheDict(int maxSize)
            {
                this._dict = new Dictionary<TKey, KeyInfo>();
                this._list = new LinkedList<TKey>();
                this._maxSize = maxSize;
            }

            internal void Add(TKey key, TValue value)
            {
                KeyInfo info;
                if (this._dict.TryGetValue(key, out info))
                {
                    this._list.Remove(info.List);
                }
                else if (this._list.Count == this._maxSize)
                {
                    LinkedListNode<TKey> last = this._list.Last;
                    this._list.RemoveLast();
                    this._dict.Remove(last.Value);
                }
                LinkedListNode<TKey> node2 = new LinkedListNode<TKey>(key);
                this._list.AddFirst(node2);
                this._dict[key] = new KeyInfo(value, node2);
            }

            internal bool TryGetValue(TKey key, out TValue value)
            {
                KeyInfo info;
                if (this._dict.TryGetValue(key, out info))
                {
                    LinkedListNode<TKey> list = info.List;
                    if (list.Previous != null)
                    {
                        this._list.Remove(list);
                        this._list.AddFirst(list);
                    }
                    value = info.Value;
                    return true;
                }
                value = default(TValue);
                return false;
            }

            internal TValue this[TKey key]
            {
                get
                {
                    TValue local;
                    if (!this.TryGetValue(key, out local))
                    {
                        throw new KeyNotFoundException();
                    }
                    return local;
                }
                [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
                set
                {
                    this.Add(key, value);
                }
            }



            // Nested Types
            [StructLayout(LayoutKind.Sequential)]
            private struct KeyInfo
            {
                internal readonly TValue Value;
                internal readonly LinkedListNode<TKey> List;
                internal KeyInfo(TValue value, LinkedListNode<TKey> list)
                {
                    this.Value = value;
                    this.List = list;
                }
            }
        }

 
 


        internal static bool CanCache(this Type t)
        {
            Assembly assembly = t.Assembly;
            if ((assembly != Mscorlib) && (assembly != SystemCore))
            {
                return false;
            }
            if (t.IsGenericType)
            {
                foreach (Type type in t.GetGenericArguments())
                {
                    if (!type.CanCache())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
 
        internal static ParameterInfo[] GetParametersCached(this MethodBase method)
        {
            ParameterInfo[] parameters = null;
            lock (ParamInfoCache)
            {
                if (!ParamInfoCache.TryGetValue(method, out parameters))
                {
                    parameters = method.GetParameters();
                    Type declaringType = method.DeclaringType;
                    if ((declaringType != null) && declaringType.CanCache())
                    {
                        ParamInfoCache[method] = parameters;
                    }
                }
            }
            return parameters;
        }

        internal static Type GetReturnType(this MethodBase mi)
        {
            if (!mi.IsConstructor)
            {
                return ((MethodInfo)mi).ReturnType;
            }
            return mi.DeclaringType;
        }

        internal static T[] AddFirst<T>(this IList<T> list, T item)
        {
            T[] array = new T[list.Count + 1];
            array[0] = item;
            list.CopyTo(array, 1);
            return array;
        }

        internal static T First<T>(this IEnumerable<T> source)
        {
            IList<T> list = source as IList<T>;
            if (list != null)
            {
                return list[0];
            }
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
            throw new InvalidOperationException();
        }
    }
}
