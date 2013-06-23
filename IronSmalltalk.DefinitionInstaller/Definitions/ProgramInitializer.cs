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
using IronSmalltalk.Runtime.Behavior;
using Microsoft.Scripting.Generation;
using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.Runtime.Installer.Definitions
{
    public class ProgramInitializer : InitializerDefinition<IProgramInitializerFactory>
    {
        public ProgramInitializer(ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, IProgramInitializerFactory factory)
            : base(sourceCodeService, methodSourceCodeService, factory)
        {
        }

        public override string ToString()
        {
            return "Global initializer";
        }


        protected internal override bool ValidateInitializer(IInstallerContext installer)
        {
            if (installer == null)
                throw new ArgumentNullException();

            return this.Factory.ValidateProgramInitializer(this, installer,
                new IntermediateCodeValidationErrorSink(this.MethodSourceCodeService, installer));
        }
    }
}
