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
using System.Numerics;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Visiting;
namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public interface ILiteralEncodingStrategy
    {
        Expression Array(EncoderVisitor visitor, IList<LiteralNode> elements);
        Expression Character(EncoderVisitor visitor, char value);
        Expression False(EncoderVisitor visitor);
        Expression FloatD(EncoderVisitor visitor, double value);
        Expression FloatE(EncoderVisitor visitor, float value);
        Expression LargeInteger(EncoderVisitor visitor, BigInteger value);
        Expression Nil(EncoderVisitor visitor);
        Expression ScaledDecimal(EncoderVisitor visitor, BigDecimal value);
        Expression SmallInteger(EncoderVisitor visitor, int value);
        Expression String(EncoderVisitor visitor, string value);
        Expression Symbol(EncoderVisitor visitor, string value);
        Expression True(EncoderVisitor visitor);
        Expression GetZero(Type type);
        Expression GetOne(Type type);
        Expression GenericLiteral(EncoderVisitor visitor, string name, Expression value);
    }
}
