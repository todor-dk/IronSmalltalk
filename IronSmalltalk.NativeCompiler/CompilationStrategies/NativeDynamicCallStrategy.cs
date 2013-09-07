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

namespace IronSmalltalk.NativeCompiler.CompilationStrategies
{
    internal sealed class NativeDynamicCallStrategy : IDynamicCallStrategy
    {
        internal string CurrentMethodName;

        private readonly CallSiteGenerator CallSiteGenerator;

        internal NativeDynamicCallStrategy(INativeStrategyClient client)
        {
            this.CallSiteGenerator = new CallSiteGenerator(client, "$CallSites");
        }

        internal void GenerateTypes()
        {
            this.CallSiteGenerator.GenerateCallSitesType();
        }

        Expression IDynamicCallStrategy.CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext)
        {
            Expression callSite = this.CreateCallSite(0, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { });
        }

        Expression IDynamicCallStrategy.CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, Expression argument)
        {
            Expression callSite = this.CreateCallSite(1, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { argument });
        }

        Expression IDynamicCallStrategy.CompileDynamicCall(VisitingContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            Expression callSite = this.CreateCallSite(argumentCount, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, arguments);
        }

        Expression IDynamicCallStrategy.CompileGetClass(VisitingContext context, Expression receiver, Expression executionContext)
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

    }
}
