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

using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.Runtime.Installer.Definitions
{
    public abstract class InitializerDefinition: CodeBasedDefinition<IInitializerFactory, CompiledInitializer>
    {
        public InitializerDefinition(ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, IInitializerFactory factory)
            : base(sourceCodeService, methodSourceCodeService, factory)
        {
        }

        protected internal abstract bool ValidateInitializer(IInstallerContext installer);

        protected internal bool CreateInitializer(IInstallerContext installer)
        {
            CompiledInitializer initializer = this.Factory.CreateInitializer(this, installer);
            if (initializer == null)
                return false;
            installer.AddInitializer(initializer);
            this.CompiledCode = initializer;
            return true;
        }
    }

    public abstract class InitializerDefinition<TFactory> : InitializerDefinition
        where TFactory : IInitializerFactory
    {
        public InitializerDefinition(ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, TFactory factory)
            : base(sourceCodeService, methodSourceCodeService, factory)
        {
        }

        public new TFactory Factory
        {
            get { return (TFactory)base.Factory; }
        }
    }
}
