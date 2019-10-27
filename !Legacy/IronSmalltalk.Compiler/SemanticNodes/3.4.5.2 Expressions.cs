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
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.LexicalTokens;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.LexicalAnalysis;

namespace IronSmalltalk.Compiler.SemanticNodes
{
    /// <summary>
    /// The ExpressionNode parse node models expressions as defined in X3J20 "3.4.5.2 Expressions".
    /// Two types of expression are defined: Assignment and Basic Expression.
    /// </summary>
    public abstract class ExpressionNode : SemanticNode, IPrimaryParentNode
    {
        /// <summary>
        /// The parent node that defines this expression.
        /// </summary>
        public SemanticNode Parent { get; private set; }

        /// <summary>
        /// Create a new expression node.
        /// </summary>
        /// <param name="parent">Parent node that defines this expression.</param>
        protected ExpressionNode(SemanticNode parent)
        {
			Contract.RequiresNotNull(parent, nameof(parent));

			this.Parent = parent;
        }
    }

    /// <summary>
    /// The assignment models an assignment expression as defined in X3J20 "3.4.5.2 Expressions".
    /// It is defined as: assignment ::= assignment_target assignmentOperator expression.
    /// </summary>
    public partial class AssignmentNode : ExpressionNode
    {
        /// <summary>
        /// Assignment target (variable) for the assignment operation.
        /// Normally, the target is set to a valid node. Only in case
        /// of illegal source code will target be null.
        /// </summary>
        public AssignmentTargetNode Target { get; private set; }

        /// <summary>
        /// Token representing the assignment operator.
        /// </summary>
        public AssignmentOperatorToken AssignmentOperator { get; private set; }

        /// <summary>
        /// Expression to be assigned to the assignment target.
        /// Under normal circumstances, the expression is always set. 
        /// Only if the source code is illegal can expression be null.
        /// </summary>
        public ExpressionNode Expression { get; private set; }

        /// <summary>
        /// Create and initialize a new assignment expression.
        /// </summary>
        /// <param name="parent">Parent node that defines this expression.</param>
        /// <param name="identifier">Identifier token of the assignment target.</param>
        /// <param name="token">Token representing the assignment operator.</param>
        protected internal AssignmentNode(SemanticNode parent, IdentifierToken identifier, AssignmentOperatorToken token)
            : base(parent)
        {
            Contract.RequiresNotNull(identifier, nameof(identifier));
            Contract.RequiresNotNull(token, nameof(token));

            this.Target = new AssignmentTargetNode(this, identifier);
            this.AssignmentOperator = token;
        }

        /// <summary>
        /// Initializes the node after being parsed by the parser.
        /// </summary>
        /// <param name="expression"></param>
        protected internal void SetContents(ExpressionNode expression)
        {
            Contract.RequiresNotNull(expression, nameof(expression));

            this.Expression = expression;
        }

        /// <summary>
        /// Get the child nodes directly defines in this node.
        /// </summary>
        /// <returns>An enumerable collection with the child nodes directly defines in this node.</returns>
        public override IEnumerable<IParseNode> GetChildNodes()
        {
            List<IParseNode> result = new List<IParseNode>();
            if (this.Target != null)
                result.Add(this.Target);
            if (this.Expression != null)
                result.Add(this.Expression);
            return result;
        }

        /// <summary>
        /// Get all non-whitespace tokens that directly define the parse node.
        /// </summary>
        /// <returns>An enumerable collection with the tokens directly defining this node.</returns>
        public override IEnumerable<IToken> GetTokens()
        {
            List<IToken> result = new List<IToken>();
            if (this.AssignmentOperator != null)
                result.Add(this.AssignmentOperator);
            return result;
        }

        /// <summary>
        /// String representation of the parse node for diagnostic purposes.
        /// </summary>
        public override string PrintString()
        {
            string str;
            if (this.Target == null)
                str = "?target?";
            else
                str = this.Target.PrintString();

            str = str + " " + LexicalConstants.AssignmentOperator + " ";
            if (this.Expression == null)
                str = str + "?expression?";
            else
                str = str + this.Expression.PrintString();

            return str;
        }
    }

    /// <summary>
    /// The parenthesized expression is a parse node representing an expression enclosed in parentheses.
    /// It is defined in X3J20 as: '(' expression ')'.
    /// </summary>
    public partial class ParenthesizedExpressionNode : SemanticNode, IPrimaryNode
    {
        /// <summary>
        /// Left opening parenthesis of the parenthesized expression.
        /// </summary>
        public SpecialCharacterToken LeftParenthesis { get; private set; }

        /// <summary>
        /// Right closing parenthesis of the parenthesized expression.
        /// </summary>
        public SpecialCharacterToken RightParenthesis { get; private set; }

        /// <summary>
        /// Expression that's being enclosed by the parentheses.
        /// Under normal circumstances this is always present. 
        /// It is null only if illegal source code was encountered.
        /// </summary>
        public ExpressionNode Expression { get; private set; }

        /// <summary>
        /// Parent node that defines this expression.
        /// </summary>
        public IPrimaryParentNode Parent { get; private set; }

        /// <summary>
        /// Create a new parenthesized expression node.
        /// </summary>
        /// <param name="parent">Parent node that defines this expression.</param>
        /// <param name="token">Left opening parenthesis of the parenthesized expression.</param>
        protected internal ParenthesizedExpressionNode(IPrimaryParentNode parent, SpecialCharacterToken token)
        {
            Contract.RequiresNotNull(parent, nameof(parent));
#if DEBUG
            if (!Parser.IsOpeningParenthesis(token))
                // We do expect the caller to this method to have ensured that we are actually parsing a parenthesis.
                throw new InvalidParserOperationException("Expected opening parenthesis token ... '('");
#endif
            this.Parent = parent;
            this.LeftParenthesis = token;
        }

        /// <summary>
        /// Initializes the node after being parsed by the parser.
        /// </summary>
        /// <param name="expression">Expression that's being enclosed by the parentheses.</param>
        /// <param name="rightParenthesis">Right closing parenthesis of the parenthesized expression.</param>
        protected internal void SetContents(ExpressionNode expression, SpecialCharacterToken rightParenthesis)
        {
            Contract.RequiresNotNull(expression, nameof(expression));

            if ((rightParenthesis != null) && !Parser.IsClosingParenthesis(rightParenthesis)) // Must allow for null
                throw new ArgumentException("Not a closing parenthesis.", nameof(rightParenthesis));
            this.Expression = expression;
            this.RightParenthesis = rightParenthesis;
        }

        /// <summary>
        /// Get the child nodes directly defines in this node.
        /// </summary>
        /// <returns>An enumerable collection with the child nodes directly defines in this node.</returns>
        public override IEnumerable<IParseNode> GetChildNodes()
        {
            List<IParseNode> result = new List<IParseNode>();
            if (this.Expression != null)
                result.Add(this.Expression);
            return result;
        }

        /// <summary>
        /// Get all non-whitespace tokens that directly define the parse node.
        /// </summary>
        /// <returns>An enumerable collection with the tokens directly defining this node.</returns>
        public override IEnumerable<IToken> GetTokens()
        {
            List<IToken> result = new List<IToken>();
            if (this.LeftParenthesis != null)
                result.Add(this.LeftParenthesis);
            if (this.RightParenthesis != null)
                result.Add(this.RightParenthesis);
            return result;
        }

        /// <summary>
        /// String representation of the parse node for diagnostic purposes.
        /// </summary>
        public override string PrintString()
        {
            if (this.Expression == null)
                return "(?expression?)";
            else
                return SemanticConstants.OpeningParenthesis + this.Expression.PrintString() + SemanticConstants.ClosingParenthesis;
        }
    }

    /// <summary>
    /// Interface that is used to tag parse nodes that can be 'primary' for an expression.
    /// Those are: Literals, Block closures, Parenthesized expressions and Variable references.
    /// X3J20 definition: primary ::= identifier | literal | block_constructor | ( '(' expression ')' ).
    /// Primary parse nodes can serve as a receiver of a message send in an expression.
    /// </summary>
    public partial interface IPrimaryNode : IParseNode
    {
    }

    /// <summary>
    /// Interface that is used to tag parse nodes that can parent a node implementing IPrimaryNode.
    /// </summary>
    public interface IPrimaryParentNode : ILiteralNodeParent // also inherits IParseNode
    {
    }

    /// <summary>
    /// The BasicExpressionNode represents the basic building block of a Smalltalk expression.
    /// If is defined in X3J20 as: basic_expression ::= primary [messages cascaded_messages].
    /// </summary>
    public partial class BasicExpressionNode : ExpressionNode, IMessageSequenceParentNode, ICascadeMessageSequenceParentNode
    {
        /// <summary>
        /// The primary, or the receiver of the basic expression.
        /// Under normal circumstances the primary is always present.
        /// Only in case of illegal source code is the primary set to null.
        /// </summary>
        public IPrimaryNode Primary { get; private set; }

        /// <summary>
        /// Optional message or a sequence of messages sent to the primary of the expression.
        /// </summary>
        public MessageSequenceNode Messages { get; private set; }

        /// <summary>
        /// Optional cascade messages sent to the primary of the expression.
        /// </summary>
        public CascadeMessageSequenceNode CascadeMessages { get; private set; }

        /// <summary>
        /// Create a new basic expression parse node.
        /// </summary>
        /// <param name="parent">Parent node that defines this expression.</param>
        protected internal BasicExpressionNode(SemanticNode parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Initializes the node after being parsed by the parser.
        /// </summary>
        /// <param name="primary">The primary, or the receiver of the basic expression.</param>
        /// <param name="messages">Optional message or a sequence of messages sent to the primary of the expression.</param>
        /// <param name="cascadeMessages">Optional cascade messages sent to the primary of the expression.</param>
        protected internal void SetContents(IPrimaryNode primary, MessageSequenceNode messages, CascadeMessageSequenceNode cascadeMessages)
        {
            Contract.RequiresNotNull(primary, nameof(primary));

            this.Primary = primary;
            this.Messages = messages; // OK with null
            this.CascadeMessages = cascadeMessages; // OK with null
        }

        /// <summary>
        /// Get the child nodes directly defines in this node.
        /// </summary>
        /// <returns>An enumerable collection with the child nodes directly defines in this node.</returns>
        public override IEnumerable<IParseNode> GetChildNodes()
        {
            List<IParseNode> result = new List<IParseNode>();
            if (this.Primary != null)
                result.Add(this.Primary);
            if (this.Messages != null)
                result.Add(this.Messages);
            if (this.CascadeMessages != null)
                result.Add(this.CascadeMessages);
            return result;
        }

        /// <summary>
        /// Get all non-whitespace tokens that directly define the parse node.
        /// </summary>
        /// <returns>An enumerable collection with the tokens directly defining this node.</returns>
        public override IEnumerable<IToken> GetTokens()
        {
            return Array.Empty<IToken>();
        }

        /// <summary>
        /// String representation of the parse node for diagnostic purposes.
        /// </summary>
        public override string PrintString()
        {
            string str;
            if (this.Primary == null)
                str = "?primary?";
            else
                str = this.Primary.PrintString();

            if (this.Messages != null)
                str = str + " " + this.Messages.PrintString();

            return str;
        }
    }

    /// <summary>
    /// The VariabReferenceleNode parse node is a primary of a basic expression that references
    /// a variable, i.e. a temporary, argument or other variable usage.
    /// </summary>
    public partial class VariableReferenceleNode : VariableNode, IPrimaryNode
    {
        /// <summary>
        /// The parent node that defines this node.
        /// </summary>
        public IPrimaryParentNode Parent { get; private set; }


        /// <summary>
        /// Create a new variable reference node.
        /// </summary>
        /// <param name="parent">The parent node that defines this node.</param>
        /// <param name="token">Identifier token containing the name of the variable.</param>
        protected internal VariableReferenceleNode(IPrimaryParentNode parent, IdentifierToken token)
        {
            Contract.RequiresNotNull(parent, nameof(parent));
            Contract.RequiresNotNull(token, nameof(token));

            this.Parent = parent;
            this.Token = token;
        }
    }

    /// <summary>
    /// The AssignmentTargetNode parse node is target of an assignment statement, 
    /// i.e. assignment of a temporary, global or other variable.
    /// </summary>
    public partial class AssignmentTargetNode : VariableNode // , IPrimaryNode
    {
        /// <summary>
        /// The parent node that defines this node.
        /// </summary>
        public AssignmentNode Parent { get; private set; }

        /// <summary>
        /// Create a new assignment target (variable reference) node.
        /// </summary>
        /// <param name="parent">The parent node that defines this node.</param>
        /// <param name="token">Identifier token containing the name of the variable.</param>
        protected internal AssignmentTargetNode(AssignmentNode parent, IdentifierToken token)
        {
            Contract.RequiresNotNull(parent, nameof(parent));
            Contract.RequiresNotNull(token, nameof(token));

            this.Parent = parent;
            this.Token = token;
        }
    }
}