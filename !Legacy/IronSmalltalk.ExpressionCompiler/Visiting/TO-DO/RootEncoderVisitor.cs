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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Compiler.Visiting;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using IronSmalltalk.Runtime.Execution.Internals;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public abstract class RootEncoderVisitor<TResult, TNode> : EncoderVisitor<TResult>
        where TNode : FunctionNode
    {
        protected readonly RootCompilationContext _Context;

        public override CompilationContext Context
        {
            get { return this._Context; }
        }

        /// <summary>
        /// Create a new root-function (method or initializer) visitor.
        /// </summary>
        protected RootEncoderVisitor(RootCompilationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this._Context = context;
        }

        public override EncoderVisitor ParentVisitor
        {
            get { return null; }
        }

        protected Expression InternalVisitFunction(TNode node)
        {
            if (node == null)
                throw new ArgumentNullException();
            if (!node.Accept(ParseTreeValidatingVisitor.Current))
                throw (new SemanticCodeGenerationException(CodeGenerationErrors.InvalidCode)).SetErrorLocation(node);

            this.DefineArguments(node);

            foreach (TemporaryVariableNode tmp in node.Temporaries)
                this.Context.DefineTemporary(tmp.Token.Value);

            StatementVisitor na;
            List<Expression> expressions = this.GenerateExpressions(node, out na);

#if DEBUG
            if (expressions.Count == 0)
                throw new InternalCodeGenerationException("We did expect at least ONE expression");

            foreach (var expr in expressions)
            {
                if (expr.Type != typeof(object))
                    throw new InternalCodeGenerationException(String.Format("Expression does not return object! \n{0}", expr));
            }
#endif

            return this.Context.GeneratePrologAndEpilogue(expressions);
        }

        protected abstract void DefineArguments(TNode node);

        protected virtual List<Expression> GenerateExpressions(TNode node, out StatementVisitor visitor)
        {
            List<Expression> expressions = null;
            visitor = new StatementVisitor(this);

            if (node.Statements != null)
                expressions = node.Statements.Accept(visitor);

            if (expressions == null)
                expressions = new List<Expression>();
            return expressions;
        }
    }
}
