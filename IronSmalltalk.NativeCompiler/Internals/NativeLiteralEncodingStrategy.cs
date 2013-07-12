using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.NativeCompiler.Internals
{
    public class NativeLiteralEncodingStrategy : ILiteralEncodingStrategy
    {
        public Expression Array(EncoderVisitor visitor, IList<LiteralNode> elements)
        {
            return Expression.Constant(null, typeof(object));
        }

        public Expression Array(LiteralVisitorExpressionValue visitor, IList<LiteralNode> elements)
        {
            return Expression.Constant(null, typeof(object));
        }

        public Expression Character(EncoderVisitor visitor, char value)
        {
            return PreboxedConstants.GetConstant(value) ?? Expression.Constant(null, typeof(object));
        }

        public Expression False(EncoderVisitor visitor)
        {
            return PreboxedConstants.False_Expression;
        }

        public Expression FloatD(EncoderVisitor visitor, double value)
        {
            return PreboxedConstants.GetConstant(value) ?? Expression.Constant(null, typeof(object));
        }

        public Expression FloatE(EncoderVisitor visitor, float value)
        {
            return PreboxedConstants.GetConstant(value) ?? Expression.Constant(null, typeof(object));
        }

        public Expression LargeInteger(EncoderVisitor visitor, BigInteger value)
        {
            return PreboxedConstants.GetConstant(value) ?? Expression.Constant(null, typeof(object));
        }

        public Expression Nil(EncoderVisitor visitor)
        {
            return PreboxedConstants.Nil_Expression;
        }

        public Expression ScaledDecimal(EncoderVisitor visitor, BigDecimal value)
        {
            return PreboxedConstants.GetConstant(value) ?? Expression.Constant(null, typeof(object));
        }

        public Expression SmallInteger(EncoderVisitor visitor, int value)
        {
            return PreboxedConstants.GetConstant(value) ?? Expression.Constant(null, typeof(object));
        }

        public Expression String(EncoderVisitor visitor, string value)
        {
            return Expression.Convert(Expression.Constant(value, typeof(string)), typeof(object));
        }

        public Expression Symbol(EncoderVisitor visitor, string value)
        {
            return Expression.Constant(null, typeof(object));
        }

        public Expression True(EncoderVisitor visitor)
        {
            return PreboxedConstants.True_Expression;
        }
    }
}
