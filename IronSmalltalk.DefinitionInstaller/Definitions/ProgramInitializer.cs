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

namespace IronSmalltalk.Runtime.Installer.Definitions
{
    public class ProgramInitializer : InitializerDefinition
    {
        public ProgramInitializer(ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, CompiledInitializer code)
            : base(sourceCodeService, methodSourceCodeService, code)
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

            return this.Code.Validate(installer.NameScope,
                new IntermediateCodeValidationErrorSink(this.MethodSourceCodeService, installer));
        }

        protected internal override void Execute(IInstallerContext installer)
        {
            this.Code.Execute(installer.Runtime, null);
        }
    }
}
