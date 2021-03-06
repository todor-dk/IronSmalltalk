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
using System.Linq;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    /// <summary>
    /// Definition description of a pool constant.
    /// </summary>
    public class PoolConstantDefinition : PoolValueDefinition
    {
        /// <summary>
        /// Creates a definition description of a pool constant.
        /// </summary>
        /// <param name="poolName">Name of the Pool that owns the pool constant.</param>
        /// <param name="variableName">Name of the pool constant.</param>
        public PoolConstantDefinition(SourceReference<string> poolName, SourceReference<string> variableName)
            : base(poolName, variableName)
        {
        }

        /// <summary>
        /// Returns a System.String that represents the pool constant definition object.
        /// </summary>
        /// <returns>A System.String that represents the pool constant definition object.</returns>
        public override string ToString()
        {
            return String.Format("{0} constant: '{1}'", this.PoolName.Value, this.VariableName.Value);
        }

        /// <summary>
        /// Create a binding object (association) for the pool variable or pool constant in the pool that owns it.
        /// </summary>
        /// <param name="installer">Context within which the binding is to be created.</param>
        /// <returns>Returns true if successful, otherwise false.</returns>
        protected internal override bool CreatePoolVariableBinding(IDefinitionInstallerContext installer)
        {
            if (installer == null)
                throw new ArgumentNullException();
            // 1. Check if the name is not complete garbage.
            if (!IronSmalltalk.Common.Utilities.ValidateIdentifier(this.VariableName.Value))
                return installer.ReportError(this.VariableName, InstallerErrors.PoolConstInvalidName);
            // 2. Get the pool dictionary.
            PoolBinding poolBinding = installer.GetPoolBinding(this.PoolName.Value);
            // 3. Check that such a binding exists
            if (poolBinding == null)
                return installer.ReportError(this.PoolName, InstallerErrors.PoolInvalidPoolName);
            if (poolBinding.Value == null)
                throw new InvalidOperationException("Should have been set in PoolDefinition.CreataGlobalObject().");
            // 4. Check for reserved keywords
            if (IronSmalltalk.Common.GlobalConstants.ReservedIdentifiers.Contains(this.VariableName.Value))
                return installer.ReportError(this.VariableName, InstallerErrors.PoolConstReservedName);
            // 5. Check that no duplicate exists.
            Symbol varName = installer.Runtime.GetSymbol(this.VariableName.Value);
            PoolVariableOrConstantBinding existing;
            poolBinding.Value.TryGetValue(varName, out existing);
            if (existing != null)
                return installer.ReportError(this.VariableName, InstallerErrors.PoolItemNameNotUnique);
            // 6. Create the binding
            poolBinding.Value.Add(new PoolConstantBinding(varName));
            return true;
        }
    }
}
