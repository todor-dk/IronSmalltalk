using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
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

        private readonly MethodGenerator MethodGenerator;

        internal NativeLiteralEncodingStrategy(MethodGenerator methodGenerator)
        {
            this.MethodGenerator = methodGenerator;
        }

        private TypeBuilder _LiteralsType = null;
        private TypeBuilder LiteralsType
        {
            get
            {
                if (this._LiteralsType == null)
                    this._LiteralsType = this.GetLiteralsType();
                return this._LiteralsType;
            }
        }

        private TypeBuilder GetLiteralsType()
        {
            string name = string.Format("{0}.{1}", this.MethodGenerator.TypeBuilder.FullName, "Literals");
            name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalTypeName(name);
            return this.MethodGenerator.TypeBuilder.DefineNestedType(
                name,
                TypeAttributes.Class | TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Abstract,
                typeof(object));
        }

        private Type LiteralTypeType;

        internal void GenerateLiteralType()
        {
            if (this._LiteralsType == null)
                return;
            this.LiteralTypeType = this._LiteralsType.CreateType();
        }

        private int LiteralCounter = 1;

        private Expression DefineLiteral(string prefix, Expression initializer)
        {
            string name = string.Format("{0}_{1}", prefix, this.LiteralCounter++);
            name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalMethodName(name);

            FieldBuilder field = this.LiteralsType.DefineField(name, typeof(object), FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Assembly);
            return initializer;
        }

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
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(char)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("Char", initializer);
        }

        public Expression False(EncoderVisitor visitor)
        {
            return PreboxedConstants.False_Expression;
        }

        public Expression FloatD(EncoderVisitor visitor, double value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(double)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("FloatD", initializer);
        }

        public Expression FloatE(EncoderVisitor visitor, float value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(float)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("FloatE", initializer);
        }

        public Expression LargeInteger(EncoderVisitor visitor, BigInteger value)
        {
            Expression bytes = Expression.NewArrayInit(typeof(byte[]),
                value.ToByteArray().Select(b => Expression.Constant(b, typeof(byte))));
            ConstructorInfo ctor = typeof(BigInteger).GetConstructor(new Type[] { typeof(byte[]) });
            Expression initializer = Expression.New(ctor, bytes);
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("BigInteger", initializer);
        }

        public Expression Nil(EncoderVisitor visitor)
        {
            return PreboxedConstants.Nil_Expression;
        }

        public Expression ScaledDecimal(EncoderVisitor visitor, BigDecimal value)
        {
            Expression bytes;
            ConstructorInfo ctor = typeof(BigInteger).GetConstructor(new Type[] { typeof(byte[]) });
            bytes = Expression.NewArrayInit(typeof(byte[]),
                value.Numerator.ToByteArray().Select(b => Expression.Constant(b, typeof(byte))));
            Expression numerator = Expression.New(ctor, bytes);
            bytes = Expression.NewArrayInit(typeof(byte[]),
                value.Denominator.ToByteArray().Select(b => Expression.Constant(b, typeof(byte))));
            Expression denominator = Expression.New(ctor, bytes);
            Expression scale = Expression.Constant(value.Scale, typeof(int));
            ctor = typeof(BigDecimal).GetConstructor(new Type[] { typeof(BigInteger), typeof(BigInteger), typeof(int) });
            Expression initializer = Expression.New(ctor, numerator, denominator, scale);
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("BigDecimal", initializer);
        }

        public Expression SmallInteger(EncoderVisitor visitor, int value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(int)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("Int", initializer);
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
