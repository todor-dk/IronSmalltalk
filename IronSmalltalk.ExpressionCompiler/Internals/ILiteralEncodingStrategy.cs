using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Visiting;
namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public interface ILiteralEncodingStrategy
    {
        Expression Array(EncoderVisitor visitor, IList<LiteralNode> elements);
        Expression Array(LiteralVisitorExpressionValue visitor, IList<LiteralNode> elements);
        Expression Character(VisitingContext context, char value);
        Expression False(VisitingContext context);
        Expression FloatD(VisitingContext context, double value);
        Expression FloatE(VisitingContext context, float value);
        Expression LargeInteger(VisitingContext context, BigInteger value);
        Expression Nil(VisitingContext context);
        Expression ScaledDecimal(VisitingContext context, BigDecimal value);
        Expression SmallInteger(VisitingContext context, int value);
        Expression String(VisitingContext context, string value);
        Expression Symbol(VisitingContext context, string value);
        Expression True(VisitingContext context);
        Expression GetZero(Type type);
        Expression GetOne(Type type);
        Expression GenericLiteral(VisitingContext context, string name, Expression value);
    }
}
