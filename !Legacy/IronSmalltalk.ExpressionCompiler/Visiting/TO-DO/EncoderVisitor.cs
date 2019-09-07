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
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Compiler.Visiting;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.Internals;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public abstract class EncoderVisitor : IBindingClient
    {
        public abstract CompilationContext Context { get; }

        Expression IBindingClient.SelfExpression
        {
            get { return this.Context.Self; }
        }
        Expression IBindingClient.TrueExpression
        {
            get { return this.Context.Compiler.LiteralEncodingStrategy.True(this); }
        }

        Expression IBindingClient.FalseExpression
        {
            get { return this.Context.Compiler.LiteralEncodingStrategy.False(this); }
        }

        IDiscreteBindingEncodingStrategy IBindingClient.DiscreteBindingEncodingStrategy
        {
            get { return this.Context.Compiler.DiscreteBindingEncodingStrategy; }
        }

        Expression IBindingClient.ExecutionContextExpression
        {
            get { return this.Context.ExecutionContextArgument; }
        }

        public abstract EncoderVisitor ParentVisitor { get; }
    }

    public abstract class EncoderVisitor<TResult> : EncoderVisitor, IParseTreeVisitor<TResult>
    {
        #region IParseTreeVisitor interface implementation 

        /// <summary>
        /// This tells is that there is some inconsistency in our model,
        /// for example a parse tree node was found where it was not expected.
        /// This exception will only be thrown if we have a bug in our (.Net) code,
        /// and not if the parsed Smalltalk code is buggy.
        /// </summary>
        /// <returns>Not used</returns>
        protected TResult InvalidOperation()
        {
            throw new NotImplementedException();
        }

        public virtual TResult VisitSemanticNode(SemanticNode node)
        {
            // This is the default visitor, in case there is undefined node type in the tree,
            // but currently all node types should be explicitely handled, so we should never get here.
            throw new NotImplementedException(String.Format(
                "Did not expect to see parse tree node: {0}", node));
        }

        public virtual TResult VisitTemporaryVariable(TemporaryVariableNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitMethod(MethodNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitMethodArgument(MethodArgumentNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitInitializer(InitializerNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitBlock(BlockNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitBlockArgument(BlockArgumentNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitStatementSequence(StatementSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitReturnStatement(ReturnStatementNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitAssignment(AssignmentNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitBasicExpression(BasicExpressionNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitCascadeMessageSequence(CascadeMessageSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitParenthesizedExpression(ParenthesizedExpressionNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitVariableReferencele(VariableReferenceleNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitAssignmentTarget(AssignmentTargetNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitUnaryMessageSequence(UnaryMessageSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitUnaryBinaryMessageSequence(UnaryBinaryMessageSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitUnaryBinaryKeywordMessageSequence(UnaryBinaryKeywordMessageSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitBinaryMessageSequence(BinaryMessageSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitBinaryKeywordMessageSequence(BinaryKeywordMessageSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitKeywordMessageSequence(KeywordMessageSequenceNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitUnaryMessage(UnaryMessageNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitBinaryMessage(BinaryMessageNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitKeywordMessage(KeywordMessageNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitBinaryArgument(BinaryArgumentNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitKeywordArgument(KeywordArgumentNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitSmallIntegerLiteral(SmallIntegerLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitLargeIntegerLiteral(LargeIntegerLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitFloatELiteral(FloatELiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitFloatDLiteral(FloatDLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitScaledDecimalLiteral(ScaledDecimalLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitCharacterLiteral(CharacterLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitIdentifierLiteral(IdentifierLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitSelectorLiteral(SelectorLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitStringLiteral(StringLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitSymbolLiteral(SymbolLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitArrayLiteral(ArrayLiteralNode node)
        {
            return this.InvalidOperation();
        }

        public virtual TResult VisitPrimitiveCall(PrimitiveCallNode node)
        {
            return this.InvalidOperation();
        }

        #endregion
    }

    public abstract class NestedEncoderVisitor<TResult> : EncoderVisitor<TResult>
    {
        private readonly EncoderVisitor _ParentVisitor;

        public override EncoderVisitor ParentVisitor 
        {
            get { return this._ParentVisitor; }
        }

        protected NestedEncoderVisitor(EncoderVisitor parentVisitor)
        {
            if (parentVisitor == null)
                throw new ArgumentNullException();
            this._ParentVisitor = parentVisitor;
        }

        public override CompilationContext Context
        {
            get { return this.ParentVisitor.Context; }
        }
    }
}
