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

using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.DefinitionInstaller.Definitions;

namespace IronSmalltalk.InterchangeInstaller
{
    /// <summary>
    /// Installer context that handles the concurrent installation of a batch of sources.
    /// </summary>
    public class InterchangeInstallerContext : DefinitionInstallerContext, IInterchangeFileInProcessor
    {
        public InterchangeInstallerContext(IronSmalltalk.SmalltalkRuntime runtime)
            : base(runtime)
        {
        }

        bool IInterchangeFileInProcessor.FileInClass(ClassDefinition definition)
        {
            this.AddClass(definition);
            return true;
        }

        bool IInterchangeFileInProcessor.FileInGlobal(GlobalDefinition definition)
        {
            this.AddGlobal(definition);
            return true;
        }

        bool IInterchangeFileInProcessor.FileInGlobalInitializer(GlobalInitializer initializer)
        {
            this.AddGlobalInitializer(initializer);
            return true;
        }

        bool IInterchangeFileInProcessor.FileInMethod(MethodDefinition definition)
        {
            this.AddMethod(definition);
            return true;
        }

        bool IInterchangeFileInProcessor.FileInPool(PoolDefinition definition)
        {
            this.AddPool(definition);
            return true;
        }

        bool IInterchangeFileInProcessor.FileInPoolVariable(PoolValueDefinition definition)
        {
            this.AddPoolVariable(definition);
            return true;
        }

        bool IInterchangeFileInProcessor.FileInPoolVariableInitializer(PoolVariableInitializer initializer)
        {
            this.AddPoolVariableInitializer(initializer);
            return true;
        }

        bool IInterchangeFileInProcessor.FileInProgramInitializer(ProgramInitializer initializer)
        {
            this.AddProgramInitializer(initializer);
            return true;
        }
    }
}
