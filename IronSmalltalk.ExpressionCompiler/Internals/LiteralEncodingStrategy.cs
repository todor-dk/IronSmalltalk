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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public class LiteralEncodingStrategy : ILiteralEncodingStrategy
    {
        Expression ILiteralEncodingStrategy.Character(EncoderVisitor visitor, char value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        Expression ILiteralEncodingStrategy.FloatE(EncoderVisitor visitor, float value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        Expression ILiteralEncodingStrategy.FloatD(EncoderVisitor visitor, double value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        Expression ILiteralEncodingStrategy.LargeInteger(EncoderVisitor visitor, BigInteger value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        Expression ILiteralEncodingStrategy.ScaledDecimal(EncoderVisitor visitor, BigDecimal value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        Expression ILiteralEncodingStrategy.SmallInteger(EncoderVisitor visitor, int value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        Expression ILiteralEncodingStrategy.String(EncoderVisitor visitor, string value)
        {
            return Expression.Constant(value, typeof(object));
        }

        Expression ILiteralEncodingStrategy.Symbol(EncoderVisitor visitor, string value)
        {
            // #'asUppercase' or #'this is a test'
            return Expression.Constant(visitor.Context.Compiler.GetSymbol(value), typeof(object));
        }

        Expression ILiteralEncodingStrategy.Nil(EncoderVisitor visitor)
        {
            return PreboxedConstants.Nil_Expression;
        }

        Expression ILiteralEncodingStrategy.True(EncoderVisitor visitor)
        {
            return Expression.Constant(PreboxedConstants.True, typeof(object));
        }

        Expression ILiteralEncodingStrategy.False(EncoderVisitor visitor)
        {
            return Expression.Constant(PreboxedConstants.False, typeof(object));
        }

        Expression ILiteralEncodingStrategy.Array(EncoderVisitor visitor, IList<LiteralNode> elements)
        {
            LiteralVisitorConstantValue arrayVisitor = new LiteralVisitorConstantValue(visitor);
            object[] result = new object[elements.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = elements[i].Accept(arrayVisitor);
            return Expression.Constant(result, typeof(object));
        }

        Expression ILiteralEncodingStrategy.GetZero(Type type)
        {
            return Expression.Constant(LiteralEncodingStrategy.GetValue(type, 0));
        }

        Expression ILiteralEncodingStrategy.GetOne(Type type)
        {
            return Expression.Constant(LiteralEncodingStrategy.GetValue(type, 1));
        }

        private static object GetValue(Type type, object value)
        {
            Expression expression = Expression.Convert(Expression.Constant(value, value.GetType()), type);
            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object)));
            var method = lambda.Compile();
            return method();
        }


        Expression ILiteralEncodingStrategy.GenericLiteral(EncoderVisitor visitor, string name, Expression value)
        {
            return value;
        }


        
    }
}
