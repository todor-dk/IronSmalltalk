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
