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
using System.Linq.Expressions;

namespace IronSmalltalk.ExpressionCompiler.Bindings
{
    public abstract class ExpressionBinding<TExpression> : NameBinding
        where TExpression : Expression
    {
        public TExpression Expression { get; protected set; }

        public ExpressionBinding(string name)
            : base(name)
        {
        }

        public override Expression GenerateReadExpression(IBindingClient client)
        {
            return this.Expression;
        }
    }
    
    public sealed class ArgumentBinding : ExpressionBinding<Expression>
    {
        public ArgumentBinding(string name)
            : base(name)
        {
            this.Expression = System.Linq.Expressions.Expression.Parameter(typeof(object), name);
        }

        public ArgumentBinding(string name, Expression expression)
            : base(name)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            this.Expression = expression;
        }
    }

    public sealed class TemporaryBinding : ExpressionBinding<ParameterExpression>, IAssignableBinding
    {
        public TemporaryBinding(string name)
            : base(name)
        {
            this.Expression = System.Linq.Expressions.Expression.Variable(typeof(object), name);
        }

        public Expression GenerateAssignExpression(Expression value, IBindingClient client)
        {
            return System.Linq.Expressions.Expression.Assign(this.Expression, value);
        }
    }
}
