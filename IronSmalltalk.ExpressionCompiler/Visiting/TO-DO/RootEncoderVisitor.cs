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
        /// <summary>
        /// Binding lookup scope with locally defined identifiers, e.g. arguments and temporary variables.
        /// </summary>
        protected BindingScope LocalScope { get; private set; }

        /// <summary>
        /// Collection of temporary variables bindings. We need this to define the vars in the AST block.
        /// </summary>
        protected readonly List<TemporaryBinding> Temporaries = new List<TemporaryBinding>();




        private VisitingContext _Context;

        public override VisitingContext Context
        {
            get { return this._Context; }
        }

        /// <summary>
        /// Create a new root-function (method or initializer) visitor.
        /// </summary>
        protected RootEncoderVisitor(VisitingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this._Context = context;
            this.LocalScope = new BindingScope();
        }

        protected Expression InternalVisitFunction(TNode node)
        {
            if (node == null)
                throw new ArgumentNullException();
            if (!node.Accept(ParseTreeValidatingVisitor.Current))
                throw (new IronSmalltalk.Runtime.Execution.Internals.SemanticCodeGenerationException(CodeGenerationErrors.InvalidCode)).SetNode(node);

            this.DefineArguments(node);
            this.DefineTemporaries(node);

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

            Expression result;
            if ((this.Temporaries.Count == 0) && (expressions.Count == 1))
                result = expressions[0];
            else
                result = Expression.Block(this.Temporaries.Select(binding => binding.Expression), expressions);

            return this.Context.GeneratePrologAndEpilogue(result);
        }

        protected abstract void DefineArguments(TNode node);

        protected void DefineTemporaries(FunctionNode node)
        {
            foreach (TemporaryVariableNode tmp in node.Temporaries)
                this.DefineTemporary(tmp.Token.Value);
        }

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

        protected internal override NameBinding GetBinding(string name)
        {
            NameBinding result = this.Context.Compiler.ReservedScope.GetBinding(name);
            if (result != null)
                return result;

            result = this.LocalScope.GetBinding(name);
            if (result != null)
                return result;

            result = this.Context.Compiler.GlobalScope.GetBinding(name);
            if (result != null)
                return result;

            return new ErrorBinding(name);
        }

        protected internal override Expression Return(Expression value)
        {
            return Expression.Return(this.Context.ReturnLabel, value, typeof(object));
        }



        #region Helpers 

        protected void DefineTemporary(string name)
        {
            TemporaryBinding temporary = new TemporaryBinding(name);
            this.Temporaries.Add(temporary);
            this.LocalScope.DefineBinding(temporary);
        }

        protected void DefineArgument(string name)
        {
            this.DefineArgument(new ArgumentBinding(name));
        }

        protected void DefineArgument(string name, Expression expression)
        {
            this.DefineArgument(new ArgumentBinding(name, expression));
        }

        protected void DefineArgument(ArgumentBinding argument)
        {
            if (argument == null)
                throw new ArgumentNullException();
            this.LocalScope.DefineBinding(argument);
        }

        #endregion
    }
}
