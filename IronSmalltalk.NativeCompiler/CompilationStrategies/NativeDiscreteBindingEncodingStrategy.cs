using IronSmalltalk.Common.Internal;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.CompilationStrategies
{
    internal class NativeDiscreteBindingEncodingStrategy : IDiscreteBindingEncodingStrategy
    {
        private readonly CallSiteGenerator CallSiteGenerator;

        internal NativeDiscreteBindingEncodingStrategy(INativeStrategyClient client)
        {
            if (client == null)
                throw new ArgumentNullException();
            this.CallSiteGenerator = new CallSiteGenerator(client, "$BindingCallSites");
        }

        internal void GenerateTypes()
        {
            this.CallSiteGenerator.GenerateCallSitesType();
        }

        Expression IDiscreteBindingEncodingStrategy.GetReadExpression<TVariableBinding, TBinding>(IBindingClient client, TVariableBinding variableBinding)
        {
            DiscreteBindingValueBinderDefinition binder = new DiscreteBindingValueBinderDefinition(variableBinding.Moniker, variableBinding.IsConstantValueBinding);
            return this.GenerateBindingCallSite<object>(client.ExecutionContextExpression, binder, variableBinding.Moniker);
        }

        Expression IDiscreteBindingEncodingStrategy.GetBindingExpression<TVariableBinding, TBinding>(IBindingClient client, TVariableBinding variableBinding)
        {
            DiscreteBindingBinderDefinition binder = new DiscreteBindingBinderDefinition(variableBinding.Moniker);
            return this.GenerateBindingCallSite<TBinding>(client.ExecutionContextExpression, binder, variableBinding.Moniker);
        }

        private Expression GenerateBindingCallSite<TBinding>(Expression executioncontext, IBinderDefinition binder, string nameSuggestion)
        {
            Type delegateType = typeof(Func<CallSite, ExecutionContext, TBinding>);
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);
            Expression callSite = this.CallSiteGenerator.CreateCallSite(binder, delegateType, siteType, nameSuggestion);

            List<Expression> args = new List<Expression>();
            args.Add(callSite);
            args.Add(executioncontext);

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
            args.Add(context.ExecutionContext);
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

        private class DiscreteBindingBinderDefinition : IBinderDefinition
        {
            public readonly string Moniker;

            public DiscreteBindingBinderDefinition(string moniker)
            {
                if (moniker == null)
                    throw new ArgumentNullException("moniker");
                this.Moniker = moniker;
            }

            private static readonly MethodInfo GetDiscreteObjectBindingMethod = TypeUtilities.Method(
                typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache), "GetDiscreteObjectBinding");

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                ilgen.Emit(OpCodes.Ldstr, this.Moniker);
                ilgen.Emit(OpCodes.Call, DiscreteBindingBinderDefinition.GetDiscreteObjectBindingMethod);
            }
        }

        private class DiscreteBindingValueBinderDefinition : IBinderDefinition
        {
            public readonly string Moniker;
            public readonly bool IsContstant;

            public DiscreteBindingValueBinderDefinition(string moniker, bool isConstant)
            {
                if (moniker == null)
                    throw new ArgumentNullException("moniker");
                this.Moniker = moniker;
                this.IsContstant = isConstant;
            }

            private static readonly MethodInfo GetDiscreteConstantBindingMethod = TypeUtilities.Method(
                typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache), "GetDiscreteConstantBinding");

            private static readonly MethodInfo GetDiscreteVariableBindingMethod = TypeUtilities.Method(
                typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache), "GetDiscreteVariableBinding");

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                ilgen.Emit(OpCodes.Ldstr, this.Moniker);
                if (this.IsContstant)
                    ilgen.Emit(OpCodes.Call, DiscreteBindingValueBinderDefinition.GetDiscreteConstantBindingMethod);
                else
                    ilgen.Emit(OpCodes.Call, DiscreteBindingValueBinderDefinition.GetDiscreteVariableBindingMethod);
            }
        }
    }
}
