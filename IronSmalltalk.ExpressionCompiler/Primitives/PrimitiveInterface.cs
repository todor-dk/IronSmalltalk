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
using System.Dynamic;
using System.Linq.Expressions;

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    public static class PrimitiveInterface
    {
        public static Expression EncodeReferenceEquals(Expression a, Expression b, Expression trueValue, Expression falseValue)
        {
            return BuiltInPrimitiveHelper.EncodeReferenceEquals(a, b, trueValue, falseValue);
        }

        public static Expression GeneratePrimitive(IPrimitiveClient client, PrimitivesEnum primitive, Type definingType, string memberName, IEnumerable<string> parameters, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            return PrimitiveBuilder.GeneratePrimitive(client, primitive, definingType, memberName, parameters, self, arguments, ref restrictions);
        }
    }
}
