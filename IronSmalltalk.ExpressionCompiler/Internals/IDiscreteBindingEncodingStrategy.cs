using IronSmalltalk.ExpressionCompiler.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public interface IDiscreteBindingEncodingStrategy
    {
        Expression GetReadExpression<TVariableBinding, TBinding>(IBindingClient client, TVariableBinding variableBinding)
            where TVariableBinding : DiscreteBinding<TBinding>
            where TBinding : IronSmalltalk.Runtime.Bindings.IDiscreteBinding;

        Expression GetBindingExpression<TVariableBinding, TBinding>(IBindingClient client, TVariableBinding variableBinding)
            where TVariableBinding : DiscreteBinding<TBinding>
            where TBinding : IronSmalltalk.Runtime.Bindings.IDiscreteBinding;
    }
}
