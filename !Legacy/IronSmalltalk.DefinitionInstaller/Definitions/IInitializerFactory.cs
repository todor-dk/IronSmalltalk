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
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    public interface IInitializerFactory
    {
        CompiledInitializer CreateInitializer(InitializerDefinition definition, IDefinitionInstallerContext installer);
    }

    public interface IGlobalInitializerFactory : IInitializerFactory
    {
        bool ValidateClassInitializer(GlobalInitializer definition, SmalltalkClass cls, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink);

        bool ValidateGlobalInitializer(GlobalInitializer definition, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink);
    }

    public interface IProgramInitializerFactory : IInitializerFactory
    {
        bool ValidateProgramInitializer(ProgramInitializer definition, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink);
    }

    public interface IPoolVariableInitializerFactory : IInitializerFactory
    {
        bool ValidatePoolVariableInitializer(PoolVariableInitializer definition, Pool pool, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink);
    }
}
