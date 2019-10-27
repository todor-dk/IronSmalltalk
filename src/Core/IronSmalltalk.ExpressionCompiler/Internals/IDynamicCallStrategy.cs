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
using System.Linq.Expressions;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.ExpressionCompiler.Primitives;
namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public interface IDynamicCallStrategy
    {
        Expression CompileDynamicCall(CompilationContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext);
        Expression CompileDynamicCall(CompilationContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, Expression argument);
        Expression CompileDynamicCall(CompilationContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments);
        Expression CompileGetClass(CompilationContext context, Expression receiver, Expression executionContext);
        Expression CompileDynamicConvert(CompilationContext context, Expression parameter, Type type, Conversion conversion);
    }
}
