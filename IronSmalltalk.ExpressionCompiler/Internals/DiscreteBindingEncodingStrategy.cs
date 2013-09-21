using IronSmalltalk.ExpressionCompiler.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public class DiscreteBindingEncodingStrategy : IDiscreteBindingEncodingStrategy
    {
        Expression IDiscreteBindingEncodingStrategy.GetReadExpression<TVariableBinding, TBinding>(IBindingClient client, TVariableBinding variableBinding)
        {
            if (variableBinding.IsConstantValueBinding)
                return Expression.Constant(variableBinding.Binding.Value, typeof(object));
            return Expression.Property(
                Expression.Constant(variableBinding.Binding, typeof(TBinding)),
                DiscreteBinding<TBinding>.GetPropertyInfo);
        }

        Expression IDiscreteBindingEncodingStrategy.GetBindingExpression<TVariableBinding, TBinding>(IBindingClient client, TVariableBinding variableBinding)
        {
            return Expression.Constant(variableBinding.Binding, typeof(TBinding));
        }
    }
}
