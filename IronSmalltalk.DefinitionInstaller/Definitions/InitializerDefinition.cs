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

using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.Runtime.Installer.Definitions
{
    public abstract class InitializerDefinition : CodeBasedDefinition<CompiledInitializer>
    {
        public InitializerDefinition(ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, CompiledInitializer code)
            : base(sourceCodeService, methodSourceCodeService, code)
        {
        }

        protected internal abstract bool ValidateInitializer(IInstallerContext installer);

        protected internal abstract void Execute(IInstallerContext installer);
    }
}
