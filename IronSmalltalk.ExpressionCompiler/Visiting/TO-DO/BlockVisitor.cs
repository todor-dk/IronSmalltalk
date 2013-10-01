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
        protected BindingScope LocalScope { get; private set; }
        protected readonly List<ArgumentBinding> Arguments = new List<ArgumentBinding>();
        protected readonly List<TemporaryBinding> Temporaries = new List<TemporaryBinding>();

        public BlockVisitorBase(EncoderVisitor enclosingVisitor)
            : base(enclosingVisitor)
        {
            this.LocalScope = new BindingScope();
        }

        protected void DefineTemporary(string name)
        {
            TemporaryBinding temporary = new TemporaryBinding(name);
            this.Temporaries.Add(temporary);
            this.LocalScope.DefineBinding(temporary);
        }

        protected virtual void DefineArgument(string name)
        {
            this.DefineArgument(new ArgumentBinding(name));
        }

        protected virtual void DefineArgument(ArgumentBinding argument)
        {
            if (argument == null)
                throw new ArgumentNullException();
            this.Arguments.Add(argument);
            this.LocalScope.DefineBinding(argument);
        }

        protected internal override NameBinding GetBinding(string name)
        {
            NameBinding result;
            result = this.Context.ReservedScope.GetBinding(name);
            if (result != null)
                return result;

            result = this.LocalScope.GetBinding(name);
            if (result != null)
                return result;

            return base.GetBinding(name);
        }

        public override Expression VisitBlock(Compiler.SemanticNodes.BlockNode node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
                this.DefineArgument(node.Arguments[i].Token.Value);

            foreach (TemporaryVariableNode tmp in node.Temporaries)
                this.DefineTemporary(tmp.Token.Value);

            List<Expression> expressions = new List<Expression>();

            NameBinding nilBinding = this.GetBinding(SemanticConstants.Nil);

            // On each execution init all temp-vars with nil
            foreach (TemporaryBinding tmp in this.Temporaries)
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

            Expression result;
            if ((this.Temporaries.Count == 0) && (expressions.Count == 1))
                result = expressions[0];
            else
                result = Expression.Block(this.Temporaries.Select(binding => binding.Expression), expressions);

            return result;
        }
    }

    public class BlockVisitor : BlockVisitorBase
    {
        public BlockVisitor(EncoderVisitor enclosingVisitor)
            : base(enclosingVisitor)
        {
        }

        public override Expression VisitBlock(BlockNode node)
        {
            Expression result = base.VisitBlock(node);

            // Somebody requested to return
            if (this._ReturnLabel != null)
                result = Expression.Label(this._ReturnLabel, result);

            if (this._TempValue != null)
                result = Expression.Block(new ParameterExpression[] { this._TempValue }, result);
            string lambdaName = this.Context.GetLambdaName(this, node);
            LambdaExpression lambda = Expression.Lambda(result, lambdaName, true, this.Arguments.Select(binding => (ParameterExpression)binding.Expression));
            return Expression.Convert(lambda, typeof(object));
        }

        protected internal override Expression Return(Expression value)
        {
            if (this.Context.Compiler.CompilerOptions.LightweightExceptions)
                return this.ReturnLocal(Expression.New(BlockResult.ConstructorInfo, this.Context.HomeContext, value));
            else
                return Expression.Throw(Expression.New(BlockResult.ConstructorInfo, this.Context.HomeContext, value), typeof(object));
        }

        protected internal override Expression ReturnLocal(Expression value)
        {
            return Expression.Return(this.ReturnLabel, value, typeof(object));
        }

        private ParameterExpression _TempValue;
        internal override Expression TempValue
        {
            get
            {
                if (this._TempValue == null)
                    this._TempValue = Expression.Variable(typeof(object), "_TempValue");
                return this._TempValue;
            }
        }

        private LabelTarget _ReturnLabel;
        public LabelTarget ReturnLabel
        {
            get
            {
                if (this._ReturnLabel == null)
                    this._ReturnLabel = Expression.Label(typeof(object), "return");
                return this._ReturnLabel;
            }
        }
    }

    public class InlineBlockVisitor : BlockVisitorBase
    {
        public InlineBlockVisitor(EncoderVisitor enclosingVisitor)
            : base(enclosingVisitor)
        {
        }

        protected override void DefineArgument(ArgumentBinding argument)
        {
            if (argument == null)
                throw new ArgumentNullException();
            // If already defined, do not define it twice ... because it's given externally.
            if (this.Arguments.Any(b => b.Name == argument.Name))
                return;
            base.DefineArgument(argument);
        }

        /// <summary>
        /// This allows us to define argument for inline blocks, which in reality 
        /// are not arguments but outer-scope variables.
        /// </summary>
        /// <param name="name">Name of the block argument.</param>
        /// <param name="expression">Expression that binds to the argument.</param>
        public void DefineExternalArgument(string name, Expression expression)
        {
            this.DefineArgument(new ArgumentBinding(name, expression));
        }
    }
}
