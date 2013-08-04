using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public class DynamicCallStrategy : IronSmalltalk.ExpressionCompiler.Internals.IDynamicCallStrategy
    {
        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext)
        {
            CallSiteBinder binder = context.Compiler.GetBinder(selector, nativeName, 0, isSuperSend, isConstantReceiver, superLookupScope);
            return Expression.Dynamic(binder, typeof(Object), receiver, executionContext);
        }

        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext, Expression argument)
        {
            CallSiteBinder binder = context.Compiler.GetBinder(selector, nativeName, 1, isSuperSend, isConstantReceiver, superLookupScope);
            return Expression.Dynamic(binder, typeof(Object), receiver, executionContext, argument);
        }

        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope,
            Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            CallSiteBinder binder = context.Compiler.GetBinder(selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, superLookupScope);
            List<Expression> args = new List<Expression>();
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);
            return Expression.Dynamic(binder, typeof(Object), args);
        }


        public Expression CompileGetClass(VisitingContext context, Expression receiver, Expression executionContext)
        {
            ObjectClassCallSiteBinder binder = context.Compiler.GetClassBinder();
            return Expression.Dynamic(binder, typeof(Object), receiver);
        }
    }
}
