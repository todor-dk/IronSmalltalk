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


namespace IronSmalltalk.Runtime.Bindings
{
    /// <summary>
    /// Global binding to a pool variable or pool constant.
    /// </summary>
    public abstract class PoolVariableOrConstantBinding : DiscreteBinding<object>
    {
        /// <summary>
        /// Create a new pool variable or pool constant binding.
        /// </summary>
        /// <param name="name">Name of the binding.</param>
        protected PoolVariableOrConstantBinding(Symbol name)
            : base(name)
        {
        }
    }

    /// <summary>
    /// Global binding to a pool variable.
    /// </summary>
    public sealed class PoolVariableBinding : PoolVariableOrConstantBinding, IWritableBinding
    {
        /// <summary>
        /// Create a new pool variable binding.
        /// </summary>
        /// <param name="name">Name of the binding.</param>
        public PoolVariableBinding(Symbol name)
            : base(name)
        {
        }

        /// <summary>
        /// Sets the value of the binding.
        /// </summary>
        /// <remarks>
        /// The "new" keywords is needed due to technical reasons. 
        /// The Value.get method is semantically identical to the base class' method.
        /// </remarks>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public new object Value
        {
            get { return this._Value; }
            set { this.SetValue(value); }
        }
    }

    /// <summary>
    /// Global binding to a pool constant. This is a constant binding.
    /// </summary>
    public sealed class PoolConstantBinding : PoolVariableOrConstantBinding
    {
        /// <summary>
        /// Create a new pool constant binding.
        /// </summary>
        /// <param name="name">Name of the binding.</param>
        public PoolConstantBinding(Symbol name)
            : base(name)
        {
        }
    }
}
