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


        public Expression GetZero(Type type)
        {
            if (type == typeof(System.Numerics.BigInteger))
                return Expression.Property(null, typeof(System.Numerics.BigInteger), "Zero");
            if (type == typeof(Int64))
                return Expression.Constant((Int64)0, type);
            if (type == typeof(Int32))
                return Expression.Constant((Int32)0, type);
            if (type == typeof(Int16))
                return Expression.Constant((Int16)0, type);
            if (type == typeof(SByte))
                return Expression.Constant((SByte)0, type);
            if (type == typeof(UInt64))
                return Expression.Constant((UInt64)0, type);
            if (type == typeof(UInt32))
                return Expression.Constant((UInt32)0, type);
            if (type == typeof(UInt16))
                return Expression.Constant((UInt16)0, type);
            if (type == typeof(Byte))
                return Expression.Constant((Byte)0, type);
            throw new NotImplementedException();
        }

        public Expression GetOne(Type type)
        {
            if (type == typeof(System.Numerics.BigInteger))
                return Expression.Property(null, typeof(System.Numerics.BigInteger), "One");
            if (type == typeof(Int64))
                return Expression.Constant((Int64)1, type);
            if (type == typeof(Int32))
                return Expression.Constant((Int32)1, type);
            if (type == typeof(Int16))
                return Expression.Constant((Int16)1, type);
            if (type == typeof(SByte))
                return Expression.Constant((SByte)1, type);
            if (type == typeof(UInt64))
                return Expression.Constant((UInt64)1, type);
            if (type == typeof(UInt32))
                return Expression.Constant((UInt32)1, type);
            if (type == typeof(UInt16))
                return Expression.Constant((UInt16)1, type);
            if (type == typeof(Byte))
                return Expression.Constant((Byte)1, type);
            throw new NotImplementedException();
        }
    }
}
