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
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    public class PoolVariableInitializer : InitializerDefinition<IPoolVariableInitializerFactory>
    {
        public SourceReference<string> PoolName { get; private set; }
        public SourceReference<string> VariableName { get; private set; }

        public PoolVariableInitializer(SourceReference<string> poolName, SourceReference<string> variableName, ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, IPoolVariableInitializerFactory factory)
            : base(sourceCodeService, methodSourceCodeService, factory)
        {
            if (poolName == null)
                throw new ArgumentNullException(nameof(poolName));
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            this.PoolName = poolName;
            this.VariableName = variableName;
        }

        public override string ToString()
        {
            return $"{this.PoolName.Value} initializerFor: '{this.VariableName.Value}'";
        }

        protected internal override bool ValidateInitializer(IDefinitionInstallerContext installer)
        {
            if (installer == null)
                throw new ArgumentNullException(nameof(installer));
            // 1. Check if the name is not complete garbage.
            if (!IronSmalltalk.Common.Utilities.ValidateIdentifier(this.VariableName.Value))
                return installer.ReportError(this.VariableName, InstallerErrors.PoolVarInvalidName);
            // 2. Get the pool dictionary.
            PoolBinding poolBinding = installer.GetPoolBinding(this.PoolName.Value);
            // 3. Check that such a binding exists
            if (poolBinding == null)
                return installer.ReportError(this.PoolName, InstallerErrors.PoolInvalidPoolName);
            if (poolBinding.Value == null)
                throw new InvalidOperationException("Should have been set in PoolDefinition.CreataGlobalObject().");

            Symbol varName = installer.Runtime.GetSymbol(this.VariableName.Value);
            PoolVariableOrConstantBinding poolItemBinding;
            poolBinding.Value.TryGetValue(varName, out poolItemBinding);
            if (poolItemBinding == null)
                return installer.ReportError(this.VariableName, InstallerErrors.PoolVarInvalidName);

            if (poolItemBinding.IsConstantBinding && poolItemBinding.HasBeenSet)
                return installer.ReportError(this.VariableName, InstallerErrors.PoolItemIsConstant);

            return this.Factory.ValidatePoolVariableInitializer(this, poolBinding.Value, installer,
                new IntermediateCodeValidationErrorSink(this.MethodSourceCodeService, installer));
        }
    }
}
