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

using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using System;
using System.Linq.Expressions;
using RTB = IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler.Bindings
{
    public sealed class PoolVariableBinding : DiscreteBinding<RTB.PoolVariableBinding>, IAssignableBinding
    {
        public RTB.PoolBinding PoolBinding { get; private set; }

        public PoolVariableBinding(string name, RTB.PoolBinding poolBinding, RTB.PoolVariableBinding binding)
            : base(name, binding)
        {
            if (poolBinding == null)
                throw new ArgumentNullException(nameof(poolBinding));
            this.PoolBinding = poolBinding;
        }

        public System.Linq.Expressions.Expression GenerateAssignExpression(System.Linq.Expressions.Expression value, IBindingClient client)
        {
            return Expression.Assign(
                Expression.Property(
                    client.DiscreteBindingEncodingStrategy.GetBindingExpression<PoolVariableBinding, RTB.PoolVariableBinding>(client, this),
                    PoolVariableBinding.SetPropertyInfo),
                value);
        }

        public override string Moniker
        {
            get { return DiscreteBindingCallSiteBinderBase.GetMoniker(this.PoolBinding, this.Binding); }
        }
    }

    public sealed class PoolConstantBinding : DiscreteBinding<RTB.PoolConstantBinding>
    {
        public RTB.PoolBinding PoolBinding { get; private set; }

        public PoolConstantBinding(string name, RTB.PoolBinding poolBinding, RTB.PoolConstantBinding binding)
            : base(name, binding)
        {
            if (poolBinding == null)
                throw new ArgumentNullException(nameof(poolBinding));
            this.PoolBinding = poolBinding;
        }

        /// <summary>
        /// This returns true if the value of the binding will always be the same. 
        /// Some read-only bindings (e.g. self, super) are NOT constant-value-bindings.
        /// </summary>
        public override bool IsConstantValueBinding
        {
            get { return true; }
        }

        public override string Moniker
        {
            get { return DiscreteBindingCallSiteBinderBase.GetMoniker(this.PoolBinding, this.Binding); }
        }
    }
}
