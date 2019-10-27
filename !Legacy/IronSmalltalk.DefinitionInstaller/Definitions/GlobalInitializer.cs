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
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This class handles initialization for both classes and global variables / constants
    /// </remarks>
    public class GlobalInitializer : InitializerDefinition<IGlobalInitializerFactory>
    {
        public SourceReference<string> GlobalName { get; private set; }

        public GlobalInitializer(SourceReference<string> globalName, ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, IGlobalInitializerFactory factory)
            : base(sourceCodeService, methodSourceCodeService, factory)
        {
            if (globalName == null)
                throw new ArgumentNullException(nameof(globalName));
            this.GlobalName = globalName;
        }

        public override string ToString()
        {
            return $"{this.GlobalName.Value} initializer";
        }

        protected internal override bool ValidateInitializer(IDefinitionInstallerContext installer)
        {
            if (installer == null)
                throw new ArgumentNullException(nameof(installer));
            // 2. Get the global.
            GlobalVariableOrConstantBinding globalBinding = installer.GetGlobalVariableOrConstantBinding(this.GlobalName.Value);
            ClassBinding classBinding = installer.GetClassBinding(this.GlobalName.Value);

            // 3. Check that such a binding exists ... but not both
            if (!((globalBinding == null) ^ (classBinding == null)))
                return installer.ReportError(this.GlobalName, InstallerErrors.GlobalInvalidName);
            if ((classBinding != null) && (classBinding.Value == null))
                throw new InvalidOperationException("Should have been set in ClassDefinition.CreataGlobalObject().");

            if (classBinding != null)
            {
                return this.Factory.ValidateClassInitializer(this, classBinding.Value, installer,
                    new IntermediateCodeValidationErrorSink(this.MethodSourceCodeService, installer));
            }

            if (globalBinding.IsConstantBinding && globalBinding.HasBeenSet)
                return installer.ReportError(this.GlobalName, InstallerErrors.GlobalIsConstant);

            return this.Factory.ValidateGlobalInitializer(this, installer,
                    new IntermediateCodeValidationErrorSink(this.MethodSourceCodeService, installer));
        }
    }
}
