using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IronSmalltalk.ExpressionCompiler.Visiting;
namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public interface IDynamicCallStrategy
    {
        Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext);
        Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, Expression argument);
        Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments);
        Expression CompileGetClass(VisitingContext context, Expression receiver, Expression executionContext);
    }
}
