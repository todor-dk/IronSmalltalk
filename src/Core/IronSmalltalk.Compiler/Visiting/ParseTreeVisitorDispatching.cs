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
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.Visiting;

namespace IronSmalltalk.Compiler.SemanticNodes
{
    // ********************************************
    // *** File with partial classes that implement
    // *** the Parse-Tree-Visitor methods for
    // *** the Semantic Nodes (X3J20 3.4.x).
    // *** Moved here for logistical reasons.
    // ********************************************

    public partial interface IPrimaryNode
    {
        TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor);
    }

    public partial class SemanticNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public virtual TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitSemanticNode(this);
        }
    }

    #region 3.4.1 Functions

    public partial class TemporaryVariableNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitTemporaryVariable(this);
        }
    }

    #endregion

    #region 3.4.2 Methods

    public partial class MethodNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitMethod(this);
        }
    }

    public partial class MethodArgumentNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitMethodArgument(this);
        }
    }

    #endregion

    #region 3.4.3 Initializers (Expressions)

    public partial class InitializerNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitInitializer(this);
        }
    }

    #endregion

    #region 3.4.4 Blocks

    public partial class BlockNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitBlock(this);
        }
    }

    public partial class BlockArgumentNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitBlockArgument(this);
        }
    }

    #endregion

    #region 3.4.5 Statements

    public partial class ReturnStatementNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitReturnStatement(this);
        }
    }

    public partial class StatementSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitStatementSequence(this);
        }
    }

    #endregion

    #region 3.4.5.2 Expressions

    public partial class AssignmentNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitAssignment(this);
        }
    }

    public partial class ParenthesizedExpressionNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitParenthesizedExpression(this);
        }
    }

    public partial class BasicExpressionNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitBasicExpression(this);
        }
    }

    public partial class VariableReferenceleNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitVariableReferencele(this);
        }
    }

    public partial class AssignmentTargetNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitAssignmentTarget(this);
        }
    }

    public partial class CascadeMessageSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitCascadeMessageSequence(this);
        }
    }

    #endregion

    #region 3.4.5.3 Messages Sequences

    public partial class BinaryArgumentNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
            Contract.RequiresNotNull(visitor, nameof(visitor));

            return visitor.VisitBinaryArgument(this);
        }
    }

    public partial class UnaryMessageSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitUnaryMessageSequence(this);
        }
    }

    public partial class KeywordArgumentNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitKeywordArgument(this);
        }
    }

    public partial class UnaryBinaryMessageSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitUnaryBinaryMessageSequence(this);
        }
    }

    public partial class BinaryMessageSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitBinaryMessageSequence(this);
        }
    }

    public partial class UnaryBinaryKeywordMessageSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitUnaryBinaryKeywordMessageSequence(this);
        }
    }

    public partial class BinaryKeywordMessageSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitBinaryKeywordMessageSequence(this);
        }
    }

    public partial class KeywordMessageSequenceNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitKeywordMessageSequence(this);
        }
    }

    #endregion

    #region 3.4.5.3 Messages

    public partial class KeywordMessageNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitKeywordMessage(this);
        }
    }

    public partial class BinaryMessageNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitBinaryMessage(this);
        }
    }

    public partial class UnaryMessageNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitUnaryMessage(this);
        }
    }

    #endregion

    #region 3.4.6 Literals

    public partial class LargeIntegerLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitLargeIntegerLiteral(this);
        }
    }

    public partial class FloatELiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitFloatELiteral(this);
        }
    }

    public partial class FloatDLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitFloatDLiteral(this);
        }
    }

    public partial class ScaledDecimalLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitScaledDecimalLiteral(this);
        }
    }

    public partial class SmallIntegerLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitSmallIntegerLiteral(this);
        }
    }

    public partial class CharacterLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitCharacterLiteral(this);
        }
    }

    public partial class StringLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitStringLiteral(this);
        }
    }

    public partial class SymbolLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitSymbolLiteral(this);
        }
    }

    public partial class SelectorLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitSelectorLiteral(this);
        }
    }

    public partial class ArrayLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitArrayLiteral(this);
        }
    }

    public partial class IdentifierLiteralNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitIdentifierLiteral(this);
        }
    }

    #endregion

    public partial class PrimitiveCallNode
    {
        /// <summary>
        /// Implements the visitor pattern for parse nodes.
        /// Dispatches to the specific visit method for this node type. 
        /// For example, MethodNode calls the VisitMethod.
        /// </summary>
        /// <param name="visitor"></param>
        public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
        {
			Contract.RequiresNotNull(visitor, nameof(visitor));

			return visitor.VisitPrimitiveCall(this);
        }
    }
}
