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

using System.Linq.Expressions;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public class LiteralVisitorExpressionValue : NestedEncoderVisitor<Expression>
    {
        public LiteralVisitorExpressionValue(EncoderVisitor parentVisitor)
            : base(parentVisitor)
        {
        }

        public override Expression VisitArrayLiteral(ArrayLiteralNode node)
        {
            return this.Context.Compiler.LiteralEncodingStrategy.Array(this, node.Elements);
        }

        public override Expression VisitCharacterLiteral(CharacterLiteralNode node)
        {
            return this.Context.Compiler.LiteralEncodingStrategy.Character(this, node.Token.Value);
        }

        public override Expression VisitFloatDLiteral(FloatDLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncodingStrategy.FloatD(this, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncodingStrategy.FloatD(this, -node.Token.Value);
        }

        public override Expression VisitFloatELiteral(FloatELiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncodingStrategy.FloatE(this, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncodingStrategy.FloatE(this, -node.Token.Value);
        }

        public override Expression VisitIdentifierLiteral(IdentifierLiteralNode node)
        {
            // Only nested inside arrays
            if (node.Token.Value == SemanticConstants.True)
                return this.Context.Compiler.LiteralEncodingStrategy.True(this);
            if (node.Token.Value == SemanticConstants.False)
                return this.Context.Compiler.LiteralEncodingStrategy.False(this);
            if (node.Token.Value == SemanticConstants.Nil)
                return this.Context.Compiler.LiteralEncodingStrategy.Nil(this);
            return this.Context.Compiler.LiteralEncodingStrategy.Symbol(this, node.Token.Value);
        }

        public override Expression VisitLargeIntegerLiteral(LargeIntegerLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncodingStrategy.LargeInteger(this, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncodingStrategy.LargeInteger(this, -node.Token.Value);
        }

        public override Expression VisitScaledDecimalLiteral(ScaledDecimalLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncodingStrategy.ScaledDecimal(this, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncodingStrategy.ScaledDecimal(this, -node.Token.Value);
        }

        public override Expression VisitSelectorLiteral(SelectorLiteralNode node)
        {
            // #asUppercase or #with:with: 
            return this.Context.Compiler.LiteralEncodingStrategy.Symbol(this, node.Token.Value);
        }

        public override Expression VisitSmallIntegerLiteral(SmallIntegerLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncodingStrategy.SmallInteger(this, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncodingStrategy.SmallInteger(this, -node.Token.Value);
        }

        public override Expression VisitStringLiteral(StringLiteralNode node)
        {
            return this.Context.Compiler.LiteralEncodingStrategy.String(this, node.Token.Value);
        }

        public override Expression VisitSymbolLiteral(SymbolLiteralNode node)
        {
            // #'asUppercase' or #'this is a test'
            return this.Context.Compiler.LiteralEncodingStrategy.Symbol(this, node.Token.Value);
        }

    }
}
