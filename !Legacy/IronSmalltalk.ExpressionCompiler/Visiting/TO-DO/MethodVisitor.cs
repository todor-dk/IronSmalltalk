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
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public class MethodVisitor : RootEncoderVisitor<Expression, MethodNode>
    {
        public MethodVisitor(RootCompilationContext context)
            : base(context)
        {
        }

        protected override void DefineArguments(MethodNode node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
                this.Context.DefineArgument(node.Arguments[i].Token.Value, this._Context.MethodArguments[i]);
        }

        protected override List<Expression> GenerateExpressions(MethodNode node, out StatementVisitor visitor)
        {
            List<Expression> expressions = new List<Expression>();
            if (node.Primitive != null)
                expressions.AddRange(node.Primitive.Accept(new PrimitiveCallVisitor(this, (node.Statements != null))));

            expressions.AddRange(base.GenerateExpressions(node, out visitor));

            if ((node.Primitive == null) && ((expressions.Count == 0) || !visitor.HasReturned))
            {
                // If no explicit return, a method must return self. 
                NameBinding binding = this.Context.GetBinding(SemanticConstants.Self);
                expressions.Add(binding.GenerateReadExpression(this));
            }

            return expressions;
        }

        public override Expression VisitMethod(MethodNode node)
        {
            //IEnumerable<ParameterExpression> parameters = this.PassedArguments.Select(arg => (ParameterExpression)arg.Expression);
            Expression code = this.InternalVisitFunction(node);
            //return Expression.Invoke(Expression.Lambda(code, "My Method", parameters), parameters);
            return code;
        }

        internal NameBinding GetLocalVariable(string name)
        {
            return this.Context.GetLocalVariable(name);
        }
    }
}
