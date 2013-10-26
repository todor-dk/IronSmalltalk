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
using System.Linq;
using System.Linq.Expressions;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.Runtime.Execution.Internals;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{

    public abstract class BlockVisitorBase : NestedEncoderVisitor<Expression>
    {

        public BlockVisitorBase(EncoderVisitor parentVisitor)
            : base(parentVisitor)
        {
        }

        public override Expression VisitBlock(Compiler.SemanticNodes.BlockNode node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                string name = node.Arguments[i].Token.Value;
                // This is used by the inlined blocks.
                // If already defined, do not define it twice ... because it's given externally. 
                // See: DefineExternalArgument
                if (this.Context.LocalScope.GetBinding(name) != null)
                    continue;
                this.Context.DefineArgument(name);
            }

            foreach (TemporaryVariableNode tmp in node.Temporaries)
                this.Context.DefineTemporary(tmp.Token.Value);

            List<Expression> expressions = new List<Expression>();

            NameBinding nilBinding = this.Context.GetBinding(SemanticConstants.Nil);

            // On each execution init all temp-vars with nil
            foreach (TemporaryBinding tmp in this.Context.Temporaries)
                expressions.Add(tmp.GenerateAssignExpression(nilBinding.GenerateReadExpression(this), this));

            StatementVisitor visitor = new StatementVisitor(this);
            if (node.Statements != null)
                expressions.AddRange(node.Statements.Accept(visitor));

            if (expressions.Count == 0) 
                expressions.Add(nilBinding.GenerateReadExpression(this));

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

        protected virtual void DefineArguments(Compiler.SemanticNodes.BlockNode node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
                this.Context.DefineArgument(node.Arguments[i].Token.Value);
        }
    }

    public class BlockVisitor : BlockVisitorBase
    {
        public BlockVisitor(EncoderVisitor parentVisitor)
            : base(parentVisitor)
        {
            this._Context = new BlockCompilationContext(this.ParentVisitor.Context);
        }

        private readonly BlockCompilationContext _Context;

        public override CompilationContext Context
        {
            get { return this._Context; }
        }

        public override Expression VisitBlock(BlockNode node)
        {
            Expression result = base.VisitBlock(node);

            string lambdaName = this.Context.RootContext.GetLambdaName(this, node);
            LambdaExpression lambda = Expression.Lambda(result, lambdaName, true, this._Context.GetLambdaParameters());
            return Expression.Convert(lambda, typeof(object));
        }
    }

    public class InlineBlockVisitor : BlockVisitorBase
    {
        public InlineBlockVisitor(EncoderVisitor parentVisitor)
            : base(parentVisitor)
        {
            this._Context = new InlineBlockCompilationContext(this.ParentVisitor.Context);
        }

        private readonly InlineBlockCompilationContext _Context;

        public override CompilationContext Context
        {
            get { return this._Context; }
        }

        /// <summary>
        /// This allows us to define argument for inline blocks, which in reality 
        /// are not arguments but outer-scope variables.
        /// </summary>
        /// <param name="name">Name of the block argument.</param>
        /// <param name="expression">Expression that binds to the argument.</param>
        public void DefineExternalArgument(string name, Expression expression)
        {
            this._Context.DefineArgument(new ArgumentBinding(name, expression));
        }
    }
}
