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

using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public class LiteralVisitorConstantValue : NestedEncoderVisitor<object>
    {
        public LiteralVisitorConstantValue(EncoderVisitor parentVisitor)
            : base(parentVisitor)
        {
        }

        public override object VisitArrayLiteral(ArrayLiteralNode node)
        {
            object[] result = new object[node.Elements.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = node.Elements[i].Accept(this);
            return result;
        }

        public override object VisitCharacterLiteral(CharacterLiteralNode node)
        {
            return node.Token.Value;
        }

        public override object VisitFloatDLiteral(FloatDLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return node.Token.Value;
            else
                return -node.Token.Value;
        }

        public override object VisitFloatELiteral(FloatELiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return node.Token.Value;
            else
                return -node.Token.Value;
        }

        public override object VisitIdentifierLiteral(IdentifierLiteralNode node)
        {
            // Only nested inside arrays
            if (node.Token.Value == SemanticConstants.True)
                return true;
            if (node.Token.Value == SemanticConstants.False)
                return false;
            if (node.Token.Value == SemanticConstants.Nil)
                return null;
            return this.Context.Compiler.GetSymbol(node.Token.Value);
        }

        public override object VisitLargeIntegerLiteral(LargeIntegerLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return node.Token.Value;
            else
                return -node.Token.Value;
        }

        public override object VisitScaledDecimalLiteral(ScaledDecimalLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return node.Token.Value;
            else
                return -node.Token.Value;
        }

        public override object VisitSelectorLiteral(SelectorLiteralNode node)
        {
            // #asUppercase or #with:with: 
            return this.Context.Compiler.GetSymbol(node.Token.Value);
        }

        public override object VisitSmallIntegerLiteral(SmallIntegerLiteralNode node)
        {
            if (node.NegativeSignToken == null)
                return node.Token.Value;
            else
                return -node.Token.Value;
        }

        public override object VisitStringLiteral(StringLiteralNode node)
        {
            return node.Token.Value;
        }

        public override object VisitSymbolLiteral(SymbolLiteralNode node)
        {
            // #'asUppercase' or #'this is a test'
            return this.Context.Compiler.GetSymbol(node.Token.Value);
        }

    }
}
