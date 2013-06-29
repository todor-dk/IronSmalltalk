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

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public class LiteralEncodingStrategy
    {
        public Expression Character(EncoderVisitor visitor, char value)
        {
            return Expression.Constant(value, typeof(object));
        }

        public Expression FloatE(EncoderVisitor visitor, float value)
        {
            return Expression.Constant(value, typeof(object));
        }

        public Expression FloatD(EncoderVisitor visitor, double value)
        {
            return Expression.Constant(value, typeof(object));
        }

        public Expression LargeInteger(EncoderVisitor visitor, BigInteger value)
        {
            return Expression.Constant(value, typeof(object));
        }

        public Expression ScaledDecimal(EncoderVisitor visitor, BigDecimal value)
        {
            return Expression.Constant(value, typeof(object));
        }


        public Expression SmallInteger (EncoderVisitor visitor, int value)
        {
            return Expression.Constant(value, typeof(object));
        }

        public Expression String(EncoderVisitor visitor, string value)
        {
            return Expression.Constant(value, typeof(object));
        }

        public Expression Symbol(EncoderVisitor visitor, string value)
        {
            // #'asUppercase' or #'this is a test'
            return Expression.Constant(visitor.Context.Compiler.GetSymbol(value), typeof(object));
        }

        public Expression Nil(EncoderVisitor visitor)
        {
            return Expression.Constant(null, typeof(object));
        }

        public Expression True(EncoderVisitor visitor)
        {
            return Expression.Constant(true, typeof(object));
        }

        public Expression False(EncoderVisitor visitor)
        {
            return Expression.Constant(false, typeof(object));
        }

        public Expression Array(EncoderVisitor visitor, IList<LiteralNode> elements)
        {
            LiteralVisitorConstantValue arrayVisitor = new LiteralVisitorConstantValue(visitor);
            object[] result = new object[elements.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = elements[i].Accept(arrayVisitor);
            return Expression.Constant(result, typeof(object));
        }

        public Expression Array(LiteralVisitorExpressionValue visitor, IList<LiteralNode> elements)
        {
            // BUG BUG
            LiteralVisitorExpressionValue arrayVisitor = new LiteralVisitorExpressionValue(visitor);
            Expression[] result = new Expression[elements.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = elements[i].Accept(arrayVisitor);
            return Expression.Constant(result, typeof(object));
        }
    }
}
