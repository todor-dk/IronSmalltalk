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
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using System.Dynamic;
using System.Collections.Concurrent;
using IronSmalltalk.ExpressionCompiler.Primitives;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public class DynamicCallStrategy : IronSmalltalk.ExpressionCompiler.Internals.IDynamicCallStrategy
    {
        public Expression CompileDynamicCall(CompilationContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext)
        {
            CallSiteBinder binder = CallSiteBinderCache.GetMessageBinder(selector, nativeName, 0, isSuperSend, isConstantReceiver, superLookupScope);
            return Expression.Dynamic(binder, typeof(Object), receiver, executionContext);
        }

        public Expression CompileDynamicCall(CompilationContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext, Expression argument)
        {
            CallSiteBinder binder = CallSiteBinderCache.GetMessageBinder(selector, nativeName, 1, isSuperSend, isConstantReceiver, superLookupScope);
            return Expression.Dynamic(binder, typeof(Object), receiver, executionContext, argument);
        }

        public Expression CompileDynamicCall(CompilationContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            CallSiteBinder binder = CallSiteBinderCache.GetMessageBinder(selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, superLookupScope);
            List<Expression> args = new List<Expression>();
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);
            return Expression.Dynamic(binder, typeof(Object), args);
        }


        public Expression CompileGetClass(CompilationContext context, Expression receiver, Expression executionContext)
        {
            ObjectClassCallSiteBinder binder = CallSiteBinderCache.Current.ObjectClassCallSiteBinder;
            return Expression.Dynamic(binder, typeof(Object), receiver);
        }

        private static readonly ConcurrentDictionary<Tuple<Type, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags>, ConvertBinder> ConvertBinders = new ConcurrentDictionary<Tuple<Type,Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags>,ConvertBinder>();

        public Expression CompileDynamicConvert(CompilationContext context, Expression parameter, Type type, Primitives.Conversion conversion)
        {
            // This is the tricky part .... 
            //
            // Example that will NOT WORK:
            //      Int16 i16 = 123;
            //      Object o16 = i16;
            //      Int32 i32 = (Int32) o16     // *** FAILS *** ... even if object currently has an Int16, it's an object and no cast to Int32!
            //  
            // Example that works:
            //      Int16 i16 = 123;
            //      Object o16 = i16;
            //      Int32 i32 = (Int16) o16     // OK! First cast Object=>Int16 then IMPLICIT cast Int16=>Int32
            //
            // VERY IMPORTANT!!!!
            //      This should ONLY do implicit conversion AND NO EXPLICIT conversions.
            //      If we do explicit conversion, we are screwed because the value 
            //      will loose precision - and this is critical show stopper!
            //
            // C# Pseudocode Example ... cast to Int32:
            //      if (obj is Int32)
            //          return (Int32) obj;
            //      else
            //          return (Int32) ((dynamic) obj);     // This uses a CallSite and a C# CallSiteBinder to do the cast.
            //
            // The DLR does not provide an easy helper function. So we have two options:
            //  1. Write the logic for what converts implicitly to what (incl. dynamic cast). Too much work currently.
            //  2. Use the C# binder to do the work. We actually WANT the same semantics as C#, so that's OK. But we depend on them :-/

            Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags = Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None;
            if ((conversion & Conversion.Checked) == Conversion.Checked)
                flags = flags | Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.CheckedContext;
            if ((conversion & Conversion.Explicit) == Conversion.Explicit)
                flags = flags | Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.ConvertExplicit;
            // Create a C# convert binder. Currently, this is not cached, but we could do this in the future.
            Tuple<Type, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags> key = new Tuple<Type,Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags>(type, flags);
            ConvertBinder binder = DynamicCallStrategy.ConvertBinders.GetOrAdd(key, k => (ConvertBinder)Microsoft.CSharp.RuntimeBinder.Binder.Convert(k.Item2, k.Item1, typeof(DynamicCallStrategy)));
            // Create a call site
            Type delegateType = typeof(Func<,,>).MakeGenericType(typeof(CallSite), parameter.Type, type);
            return Expression.MakeDynamic(delegateType, binder, parameter);
        }
    }
}
