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
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public class InitializerVisitor : RootEncoderVisitor<Expression<Func<object, ExecutionContext, object>>, InitializerNode>
    {
        public string InitializerName { get; private set; }

        public InitializerVisitor(RootCompilationContext context, string initializerName)
            : base(context)
        {
            this.InitializerName = initializerName;
        }

        protected override void DefineArguments(InitializerNode node)
        {
            // Initializers have no arguments
        }

        protected override List<Expression> GenerateExpressions(InitializerNode node, out StatementVisitor visitor)
        {
            List<Expression> expressions = base.GenerateExpressions(node, out visitor);

            if (expressions.Count == 0)
            {
                NameBinding binding = this.Context.GetBinding(SemanticConstants.Self);
                if (binding.IsErrorBinding)
                    binding = this.Context.GetBinding(SemanticConstants.Nil);
                expressions.Add(binding.GenerateReadExpression(this));
            }

            return expressions;
        }


        public override Expression<Func<object, ExecutionContext, object>> VisitInitializer(InitializerNode node)
        {
            Expression body = this.InternalVisitFunction(node);

            return Expression.Lambda<Func<object, ExecutionContext, object>>(body, this.InitializerName,
                new ParameterExpression[] { (ParameterExpression)this.Context.Self, (ParameterExpression)this.Context.ExecutionContextArgument });
        }
    }
}
