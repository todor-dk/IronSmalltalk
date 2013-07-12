using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.NativeCompiler.Internals
{
    public class NativeDynamicCallStrategy : IDynamicCallStrategy
    {

        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext)
        {
            Expression callSite = this.CreateCallSite(0, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { });
        }

        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, Expression argument)
        {
            Expression callSite = this.CreateCallSite(1, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { argument });
        }

        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            Expression callSite = this.CreateCallSite(argumentCount, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, arguments);
        }

        private Expression CompileDynamicCall(Expression callSite, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            Type delegateType = MethodSignatures.GetMethodType(arguments.Count());
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);

            ParameterExpression site = Expression.Variable(siteType, "$site");
            List<Expression> args = new List<Expression>();
            args.Add(site);
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);

            // ($site = siteExpr).Target.Invoke($site, *args)
            return Expression.Block(
                new[] { site },
                Expression.Call(
                    Expression.Field(Expression.Assign(site, callSite), siteType.GetField("Target", BindingFlags.Instance | BindingFlags.Public)),
                    delegateType.GetMethod("Invoke"),
                    args));
        }

        private Expression CreateCallSite(int argumentCount, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
        {
            Type delegateType = MethodSignatures.GetMethodType(argumentCount);
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);
            MethodInfo create = siteType.GetMethod("Create");
            MethodInfo getCallSite = typeof(NativeDynamicCallStrategy).GetMethod("GetCallSite");

            Expression binder = Expression.Call(null, getCallSite,
                Expression.Constant(selector, typeof(string)),
                Expression.Constant(nativeName, typeof(string)),
                Expression.Constant(isSuperSend, typeof(bool)),
                Expression.Constant(isConstantReceiver, typeof(bool)),
                Expression.Constant(superLookupScope, typeof(string)));
            return Expression.Call(null, create, binder);
        }

        public static CallSiteBinder GetCallSite(string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
        {
            return null;
        }
    }
}
