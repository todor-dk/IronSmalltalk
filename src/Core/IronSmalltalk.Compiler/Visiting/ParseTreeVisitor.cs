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

using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;

namespace IronSmalltalk.Compiler.Visiting
{
	/// <summary>
	/// Base class that implements the protocol for visiting
	/// the parse tree nodes defined in X3J20 section 3.4,
	/// i.e. the parse tree nodes for methods and initializers.
	/// </summary>
	public abstract class ParseTreeVisitor<TResult> : IParseTreeVisitor<TResult>
	{
		/// <summary>
		/// Visits the Semantic Node. This is the default visit, in case no other implementation.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitSemanticNode(SemanticNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            // Naive brute force implementation
            foreach (IParseNode child in node.GetChildNodes())
			{
				if (child is SemanticNode)
					((SemanticNode)child).Accept(this);
			}

            return default(TResult); // The default naive implementation
		}

		#region 3.4.1 Functions

		/// <summary>
		/// Visits the Temporary Variable node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitTemporaryVariable(TemporaryVariableNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		#endregion
		
		#region 3.4.2 Methods

		/// <summary>
		/// Visits the Method node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitMethod(MethodNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            foreach (MethodArgumentNode arg in node.Arguments)
				arg.Accept(this);

            foreach (TemporaryVariableNode tmp in node.Temporaries)
				tmp.Accept(this);

            if (node.Primitive != null)
				node.Primitive.Accept(this);

            if (node.Statements != null)
				node.Statements.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Method Argument node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitMethodArgument(MethodArgumentNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		#endregion

		#region 3.4.3 Initializers (Expressions)

		/// <summary>
		/// Visits the Initializer (Expression) node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitInitializer(InitializerNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            foreach (TemporaryVariableNode tmp in node.Temporaries)
				tmp.Accept(this);

            if (node.Statements != null)
				node.Statements.Accept(this);

            return default(TResult); // The default naive implementation
		}

		#endregion
		
		#region 3.4.4 Blocks

		/// <summary>
		/// Visits the Block node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitBlock(BlockNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            foreach (BlockArgumentNode arg in node.Arguments)
				arg.Accept(this);

            foreach (TemporaryVariableNode tmp in node.Temporaries)
				tmp.Accept(this);

            if (node.Statements != null)
				node.Statements.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Block Argument node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitBlockArgument(BlockArgumentNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		#endregion

		#region 3.4.5 Statements

		/// <summary>
		/// Visits the Statement Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitStatementSequence(StatementSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Expression != null)
				node.Expression.Accept(this);
            if (node.NextStatement != null)
				node.NextStatement.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Return Statement node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitReturnStatement(ReturnStatementNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Expression != null)
				node.Expression.Accept(this);

            return default(TResult); // The default naive implementation
		}

		#endregion

		#region 3.4.5.2 Expressions

		/// <summary>
		/// Visits the Assignment node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitAssignment(AssignmentNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Target != null)
				node.Target.Accept(this);

            if (node.Expression != null)
				node.Expression.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Basic Expression node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitBasicExpression(BasicExpressionNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Primary != null)
				node.Primary.Accept(this);

            if (node.Messages != null)
				node.Messages.Accept(this);

            if (node.CascadeMessages != null)
				node.CascadeMessages.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Cascade Message Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitCascadeMessageSequence(CascadeMessageSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Messages != null)
				node.Messages.Accept(this);

            if (node.NextCascade != null)
				node.NextCascade.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Parenthesized Expression node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitParenthesizedExpression(ParenthesizedExpressionNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Expression != null)
				node.Expression.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Variable Reference (as opposite to declaration) node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitVariableReferencele(VariableReferenceleNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Assignment Target node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitAssignmentTarget(AssignmentTargetNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		#endregion

		#region 3.4.5.3 Messages Sequences

		/// <summary>
		/// Visits the Unary Message Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitUnaryMessageSequence(UnaryMessageSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Message != null)
				node.Message.Accept(this);

            if (node.NextMessage != null)
				node.NextMessage.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Unary-Binary Message Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitUnaryBinaryMessageSequence(UnaryBinaryMessageSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Message != null)
				node.Message.Accept(this);

            if (node.NextMessage != null)
				node.NextMessage.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Unary-Binary-Keyword Message Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitUnaryBinaryKeywordMessageSequence(UnaryBinaryKeywordMessageSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Message != null)
				node.Message.Accept(this);

            if (node.NextMessage != null)
				node.NextMessage.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Binary Message Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitBinaryMessageSequence(BinaryMessageSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Message != null)
				node.Message.Accept(this);

            if (node.NextMessage != null)
				node.NextMessage.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Binary-Keyword Message Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitBinaryKeywordMessageSequence(BinaryKeywordMessageSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Message != null)
				node.Message.Accept(this);

            if (node.NextMessage != null)
				node.NextMessage.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Keyword Message Sequence node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitKeywordMessageSequence(KeywordMessageSequenceNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Message != null)
				node.Message.Accept(this);

            return default(TResult); // The default naive implementation
		}

		#endregion

		#region 3.4.5.3 Messages

		/// <summary>
		/// Visits the Unary Message node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitUnaryMessage(UnaryMessageNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Binary Message node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitBinaryMessage(BinaryMessageNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Argument != null)
				node.Argument.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Keyword Message node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitKeywordMessage(KeywordMessageNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            foreach (KeywordArgumentNode arg in node.Arguments)
				arg.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Binary Message Argument node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitBinaryArgument(BinaryArgumentNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Primary != null)
				node.Primary.Accept(this);

            if (node.Messages != null)
				node.Messages.Accept(this);

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Keyword Message Argument node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitKeywordArgument(KeywordArgumentNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            if (node.Primary != null)
				node.Primary.Accept(this);

            if (node.Messages != null)
				node.Messages.Accept(this);

            return default(TResult); // The default naive implementation
		}

		#endregion

		#region 3.4.6 Literals 

		/// <summary>
		/// Visits the Small Integer Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitSmallIntegerLiteral(SmallIntegerLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Large Integer Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitLargeIntegerLiteral(LargeIntegerLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the FloatD Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitFloatDLiteral(FloatDLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the FloatE Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitFloatELiteral(FloatELiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Scaled Decimal Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitScaledDecimalLiteral(ScaledDecimalLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Character Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitCharacterLiteral(CharacterLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Identifier Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitIdentifierLiteral(IdentifierLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Selector Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitSelectorLiteral(SelectorLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the String Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitStringLiteral(StringLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Symbol Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitSymbolLiteral(SymbolLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}

		/// <summary>
		/// Visits the Array Literal node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitArrayLiteral(ArrayLiteralNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            foreach (LiteralNode item in node.Elements)
				item.Accept(this);

            return default(TResult); // The default naive implementation
		}

		#endregion

		/// <summary>
		/// Visits the Primitive Call node.
		/// </summary>
		/// <param name="node">The node to visit.</param>
		public virtual TResult VisitPrimitiveCall(PrimitiveCallNode node)
		{
            Contract.RequiresNotNull(node, nameof(node));

            return default(TResult); // The default naive implementation
		}
	}
}
