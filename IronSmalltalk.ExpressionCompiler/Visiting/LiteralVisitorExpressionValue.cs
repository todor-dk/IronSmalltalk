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
        public LiteralVisitorExpressionValue(EncoderVisitor enclosingVisitor)
            : base(enclosingVisitor)
        {
        }

        public override Expression VisitArrayLiteral(ArrayLiteralNode node)
        {
            return this.Context.Compiler.LiteralEncoding.Array(this, node.Elements);
        }

        public override Expression VisitCharacterLiteral(CharacterLiteralNode node)
        {
            return this.Context.Compiler.LiteralEncoding.Character(this.Context, node.Token.Value);
        }

        public override Expression VisitFloatDLiteral(FloatDLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncoding.FloatD(this.Context, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncoding.FloatD(this.Context, -node.Token.Value);
        }

        public override Expression VisitFloatELiteral(FloatELiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncoding.FloatE(this.Context, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncoding.FloatE(this.Context, -node.Token.Value);
        }

        public override Expression VisitIdentifierLiteral(IdentifierLiteralNode node)
        {
            // Only nested inside arrays
            if (node.Token.Value == SemanticConstants.True)
                return this.Context.Compiler.LiteralEncoding.True(this.Context);
            if (node.Token.Value == SemanticConstants.False)
                return this.Context.Compiler.LiteralEncoding.False(this.Context);
            if (node.Token.Value == SemanticConstants.Nil)
                return this.Context.Compiler.LiteralEncoding.Nil(this.Context);
            return this.Context.Compiler.LiteralEncoding.Symbol(this.Context, node.Token.Value);
        }

        public override Expression VisitLargeIntegerLiteral(LargeIntegerLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncoding.LargeInteger(this.Context, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncoding.LargeInteger(this.Context, -node.Token.Value);
        }

        public override Expression VisitScaledDecimalLiteral(ScaledDecimalLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncoding.ScaledDecimal(this.Context, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncoding.ScaledDecimal(this.Context, -node.Token.Value);
        }

        public override Expression VisitSelectorLiteral(SelectorLiteralNode node)
        {
            // #asUppercase or #with:with: 
            return this.Context.Compiler.LiteralEncoding.Symbol(this.Context, node.Token.Value);
        }

        public override Expression VisitSmallIntegerLiteral(SmallIntegerLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return this.Context.Compiler.LiteralEncoding.SmallInteger(this.Context, node.Token.Value);
            else
                return this.Context.Compiler.LiteralEncoding.SmallInteger(this.Context, -node.Token.Value);
        }

        public override Expression VisitStringLiteral(StringLiteralNode node)
        {
            return this.Context.Compiler.LiteralEncoding.String(this.Context, node.Token.Value);
        }

        public override Expression VisitSymbolLiteral(SymbolLiteralNode node)
        {
            // #'asUppercase' or #'this is a test'
            return this.Context.Compiler.LiteralEncoding.Symbol(this.Context, node.Token.Value);
        }

    }
}
