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
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.NativeCompiler.Internals;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.Internals;
using IronSmalltalk.ExpressionCompiler.Primitives;
using System.Dynamic;

namespace IronSmalltalk.NativeCompiler.CompilationStrategies
{
    internal sealed class NativeDynamicCallStrategy : IDynamicCallStrategy
    {
        private readonly CallSiteGenerator CallSiteGenerator;

        private readonly INativeStrategyClient Client;

        internal NativeDynamicCallStrategy(INativeStrategyClient client)
        {
            this.CallSiteGenerator = new CallSiteGenerator(client, "$CallSites");
            this.Client = client;
        }

        internal void GenerateTypes()
        {
            this.CallSiteGenerator.GenerateCallSitesType();
        }

        Expression IDynamicCallStrategy.CompileDynamicCall(CompilationContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext)
        {
            Expression callSite = this.CreateCallSite(0, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { });
        }

        Expression IDynamicCallStrategy.CompileDynamicCall(CompilationContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, Expression argument)
        {
            Expression callSite = this.CreateCallSite(1, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { argument });
        }

        Expression IDynamicCallStrategy.CompileDynamicCall(CompilationContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            Expression callSite = this.CreateCallSite(argumentCount, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, arguments);
        }

        Expression IDynamicCallStrategy.CompileGetClass(CompilationContext context, Expression receiver, Expression executionContext)
        {
            IBinderDefinition binder = new ClassBinderDefinition();

            Type delegateType = typeof(Func<CallSite, object, ExecutionContext, object>);
            Type siteType = typeof(CallSite<Func<CallSite, object, ExecutionContext, object>>);

            Expression callSite = this.CallSiteGenerator.CreateCallSite(binder, delegateType, siteType, "class");
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { });            
        }

        private Expression CompileDynamicCall(Expression callSite, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            Type delegateType = NativeDynamicCallStrategy.GetCallSiteType(arguments.Count());
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);

            List<Expression> args = new List<Expression>();
            args.Add(callSite);
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);

            FieldInfo target = TypeUtilities.Field(siteType, "Target", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo invoke = TypeUtilities.Method(delegateType, "Invoke");
            ParameterInfo[] pis = invoke.GetParameters();

            // siteExpr.Target.Invoke(siteExpr, *args)
            return Expression.Call(
                Expression.Field(callSite, target),
                invoke,
                args);

            /* C# Style
            ParameterExpression site = Expression.Variable(siteType, "$site");
            List<Expression> args = new List<Expression>();
            args.Add(site);
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);

            FieldInfo target = TypeUtilities.Field(siteType, "Target", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo invoke = TypeUtilities.Method(delegateType, "Invoke");
            ParameterInfo[] pis = invoke.GetParameters();
            // ($site = siteExpr).Target.Invoke($site, *args)
            return Expression.Block(
                new[] { site },
                Expression.Call(
                    Expression.Field(Expression.Assign(site, callSite), target),
                    invoke,
                    args));
             */
        }

        private static Type GetCallSiteType(int argumentCount)
        {
            if (argumentCount == 0)
                return typeof(Func<CallSite, object, ExecutionContext, object>);
            if (argumentCount == 1)
                return typeof(Func<CallSite, object, ExecutionContext, object, object>);
            if (argumentCount == 2)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object>);
            if (argumentCount == 3)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object>);
            if (argumentCount == 4)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object>);
            if (argumentCount == 5)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object>);
            if (argumentCount == 6)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object>);
            if (argumentCount == 7)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object>);
            if (argumentCount == 8)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 9)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 10)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 11)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 12)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 13)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
            throw new NotImplementedException();
        }

        private Expression CreateCallSite(int argumentCount, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
        {
            MessageSendBinderDefinition binder = new MessageSendBinderDefinition(selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, superLookupScope);

            Type delegateType = NativeDynamicCallStrategy.GetCallSiteType(argumentCount);
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);

            return this.CallSiteGenerator.CreateCallSite(binder, delegateType, siteType, selector);
        }

        private class MessageSendBinderDefinition : IBinderDefinition
        {
            public readonly string Selector;
            public readonly string NativeName;
            public readonly int ArgumentCount;
            public readonly bool IsSuperSend;
            public readonly bool IsConstantReceiver;
            public readonly string SuperLookupScope;

            public MessageSendBinderDefinition(string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
            {
                if (selector == null)
                    throw new ArgumentNullException("selector");
                if (argumentCount < 0)
                    throw new ArgumentOutOfRangeException("argumentCount");
                this.Selector = selector;
                this.NativeName = nativeName;
                this.ArgumentCount = argumentCount;
                this.IsSuperSend = isSuperSend;
                this.IsConstantReceiver = isConstantReceiver;
                this.SuperLookupScope = superLookupScope;
            }

            private static readonly MethodInfo GetMessageBinderMethod = TypeUtilities.Method(
                typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache), "GetMessageBinder",
                typeof(string), typeof(string), typeof(int), typeof(bool), typeof(bool), typeof(string));

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                // CallSiteBinderCache.GetMessageBinder("Selector", "NativeName", ArgumentCount, IsSuperSend, IsConstantReceiver, "SuperLookupScope");
                ilgen.Emit(OpCodes.Ldstr, this.Selector);
                if (this.NativeName == null)
                    ilgen.Emit(OpCodes.Ldnull);
                else
                    ilgen.Emit(OpCodes.Ldstr, this.NativeName);
                ilgen.PushInt(this.ArgumentCount);
                if (this.IsSuperSend)
                    ilgen.Emit(OpCodes.Ldc_I4_1);
                else
                    ilgen.Emit(OpCodes.Ldc_I4_0);
                if (this.IsConstantReceiver)
                    ilgen.Emit(OpCodes.Ldc_I4_1);
                else
                    ilgen.Emit(OpCodes.Ldc_I4_0);
                if (this.SuperLookupScope == null)
                    ilgen.Emit(OpCodes.Ldnull);
                else
                    ilgen.Emit(OpCodes.Ldstr, this.SuperLookupScope);
                ilgen.Emit(OpCodes.Call, MessageSendBinderDefinition.GetMessageBinderMethod);
            }

        }

        private class ClassBinderDefinition : IBinderDefinition
        {
            private static readonly MethodInfo GetObjectClassBinderMethod = TypeUtilities.Method(
                typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache), "GetObjectClassBinder");


            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                // CallSiteBinderCache.GetObjectClassBinder();
                ilgen.Emit(OpCodes.Call, ClassBinderDefinition.GetObjectClassBinderMethod);
            }
        }



        public Expression CompileDynamicConvert(CompilationContext context, Expression parameter, Type type, ExpressionCompiler.Primitives.Conversion conversion)
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
            ConvertBinderDefinition binder = new ConvertBinderDefinition(this.Client.Compiler.GetDynamicConvertBinder(type, flags));
            // Create a call site
            Type delegateType = typeof(Func<,,>).MakeGenericType(typeof(CallSite), parameter.Type, type);
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);

            Expression callSite = this.CallSiteGenerator.CreateCallSite(binder, delegateType, siteType, String.Format("({0})", type.Name));

            FieldInfo target = TypeUtilities.Field(siteType, "Target", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo invoke = TypeUtilities.Method(delegateType, "Invoke");

            // siteExpr.Target.Invoke(siteExpr, parameter)
            return Expression.Call(
                Expression.Field(callSite, target),
                invoke,
                callSite, parameter);
        }

        private class ConvertBinderDefinition : IBinderDefinition
        {
            private readonly FieldInfo BinderField;

            public ConvertBinderDefinition(FieldInfo binderField)
            {
                this.BinderField = binderField;
            }

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                ilgen.Emit(OpCodes.Ldsfld, this.BinderField);
            }
        }
    }
}
