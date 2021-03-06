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

namespace IronSmalltalk.Runtime.Bindings
{
    /// <summary>
    /// This is the base class for all bindings. A binding is simply a
    /// mapping between an identifier (some name) and some element of the
    /// Smalltalk program. In most cases, the binding is a discrete bindings 
    /// between an identifier and some value, for example a global variable.
    /// </summary>
    /// <remarks>
    /// See DiscreteBinding for the global, class and pool variables and constants.
    /// </remarks>
    /// <typeparam name="TValue">Type if the value of the binding.</typeparam>
    public abstract class Binding<TValue> : IBinding
    {
        /// <summary>
        /// Internal variable holding the name of the binding.
        /// </summary>
        protected readonly Symbol _Name;
        /// <summary>
        /// Internal variable holding the value of the binding.
        /// </summary>
        protected TValue _Value;

        /// <summary>
        /// Tells if the value has actually been set.
        /// If not set, then the binding is an error binding.
        /// </summary>
        public bool HasBeenSet { get; protected set; }

        /// <summary>
        /// Creates a new binding with the given name.
        /// </summary>
        /// <param name="name">Name of the binding.</param>
        protected Binding(Symbol name)
        {
            if (name == null)
                throw new ArgumentNullException();
            this._Name = name;
            this.HasBeenSet = false;
        }

        /// <summary>
        /// Determines if the binding is a constant binding, i.e. a read-only binding.
        /// </summary>
        public bool IsConstantBinding {
            get { return !(this is IWritableBinding); }
        }

        /// <summary>
        /// The name of the binding.
        /// </summary>
        public Symbol Name
        {
            get { return this._Name; }
        }

        /// <summary>
        /// The value of the binding.
        /// </summary>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public TValue Value
        {
            get { return this._Value; }
        }

        object IBinding.Value
        {
            get { return this._Value; }
        }

        /// <summary>
        /// Returns a string that represents the current binding.
        /// </summary>
        /// <returns>A string that represents the current binding.</returns>
        public override string ToString()
        {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} ==> {1}", this.Name, this.Value);
        }
    }

    /// <summary>
    /// Interface that describes a bindings. A binding is simply a
    /// mapping between an identifier (some name) and some element of the
    /// Smalltalk program. In most cases, the binding is a discrete bindings 
    /// between an identifier and some value, for example a global variable.
    /// </summary>
    public interface IBinding
    {
        /// <summary>
        /// The name of the binding.
        /// </summary>
        Symbol Name { get; }

        /// <summary>
        /// The value of the binding.
        /// </summary>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        object Value { get; }

        /// <summary>
        /// Determines if the binding is a constant binding, i.e. a read-only binding.
        /// </summary>
        bool IsConstantBinding { get; }
    }

    /// <summary>
    /// A binding that is not a constant binding and allows the value to be modified.
    /// </summary>
    public interface IWritableBinding : IBinding
    {
        /// <summary>
        /// Sets the value of the binding.
        /// </summary>
        /// <remarks>
        /// The "new" keywords is needed due to technical reasons. 
        /// The Value.get method is semantically identical to the base class' method.
        /// </remarks>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        new object Value { get; set; }
    }
}
