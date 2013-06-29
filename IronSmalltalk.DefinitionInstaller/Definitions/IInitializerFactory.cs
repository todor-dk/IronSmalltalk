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
