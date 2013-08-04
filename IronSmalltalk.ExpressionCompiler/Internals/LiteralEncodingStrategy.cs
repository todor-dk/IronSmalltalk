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
        public Expression Character(VisitingContext context, char value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        public Expression FloatE(VisitingContext context, float value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        public Expression FloatD(VisitingContext context, double value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        public Expression LargeInteger(VisitingContext context, BigInteger value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        public Expression ScaledDecimal(VisitingContext context, BigDecimal value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        public Expression SmallInteger(VisitingContext context, int value)
        {
            return Expression.Constant(PreboxedConstants.GetValue(value) ?? value, typeof(object));
        }

        public Expression String(VisitingContext context, string value)
        {
            return Expression.Constant(value, typeof(object));
        }

        public Expression Symbol(VisitingContext context, string value)
        {
            // #'asUppercase' or #'this is a test'
            return Expression.Constant(context.Compiler.GetSymbol(value), typeof(object));
        }

        public Expression Nil(VisitingContext context)
        {
            return PreboxedConstants.Nil_Expression;
        }

        public Expression True(VisitingContext context)
        {
            return Expression.Constant(PreboxedConstants.True, typeof(object));
        }

        public Expression False(VisitingContext context)
        {
            return Expression.Constant(PreboxedConstants.False, typeof(object));
        }

        // Used when the array is primary
        public Expression Array(EncoderVisitor visitor, IList<LiteralNode> elements)
        {
            LiteralVisitorConstantValue arrayVisitor = new LiteralVisitorConstantValue(visitor);
            object[] result = new object[elements.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = elements[i].Accept(arrayVisitor);
            return Expression.Constant(result, typeof(object));
        }

        // Used when the array is nested inside an array and we need to encode to expression
        public Expression Array(LiteralVisitorExpressionValue visitor, IList<LiteralNode> elements)
        {
            // BUG BUG ... review this code
            LiteralVisitorExpressionValue arrayVisitor = new LiteralVisitorExpressionValue(visitor);
            Expression[] result = new Expression[elements.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = elements[i].Accept(arrayVisitor);
            return Expression.Constant(result, typeof(object));
        }


        public Expression GetZero(Type type)
        {
            return Expression.Constant(LiteralEncodingStrategy.GetValue(type, 0));
        }

        public Expression GetOne(Type type)
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


        public Expression GenericLiteral(VisitingContext context, string name, Expression value)
        {
            return value;
        }


        
    }
}
