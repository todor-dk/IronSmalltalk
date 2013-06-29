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

using System.Collections.Generic;
using System.Linq.Expressions;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Internals;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public class StatementVisitor : NestedEncoderVisitor<List<Expression>>
    {
        public StatementVisitor(EncoderVisitor enclosingVisitor)
            : base(enclosingVisitor)
        {
            this.HasReturned = false;
        }

        public bool HasReturned { get; protected set; }
        protected readonly List<Expression> Expressions = new List<Expression>();

        public override List<Expression> VisitStatementSequence(StatementSequenceNode node)
        {
            if (this.HasReturned)
                throw (new IronSmalltalk.Runtime.Execution.Internals.SemanticCodeGenerationException(CodeGenerationErrors.CodeAfterReturnStatement)).SetNode(node);

            Expression statementCode = node.Expression.Accept(new ExpressionVisitor(this));
            statementCode = this.Context.Compiler.AddDebugInfo(statementCode, node.Expression);

            this.Expressions.Add(statementCode);

            if (node.NextStatement != null)
                node.NextStatement.Accept(this);

            return this.Expressions;
        }

        public override List<Expression> VisitReturnStatement(ReturnStatementNode node)
        {
            if (this.HasReturned)
                throw (new IronSmalltalk.Runtime.Execution.Internals.SemanticCodeGenerationException(CodeGenerationErrors.CodeAfterReturnStatement)).SetNode(node);
            this.HasReturned = true;

            this.Expressions.Add(this.Return(node.Expression.Accept(new ExpressionVisitor(this))));

            return this.Expressions;
        }
    }
}
