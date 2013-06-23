using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Installer;
using IronSmalltalk.Runtime.Installer.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    public interface IInitializerFactory
    {
        CompiledInitializer CreateInitializer(InitializerDefinition definition, IInstallerContext installer);
    }

    public interface IGlobalInitializerFactory : IInitializerFactory
    {
        bool ValidateClassInitializer(GlobalInitializer definition, SmalltalkClass cls, IInstallerContext installer, ICodeValidationErrorSink errorSink);

        bool ValidateGlobalInitializer(GlobalInitializer definition, IInstallerContext installer, ICodeValidationErrorSink errorSink);
    }

    public interface IProgramInitializerFactory : IInitializerFactory
    {
        bool ValidateProgramInitializer(ProgramInitializer definition, IInstallerContext installer, ICodeValidationErrorSink errorSink);
    }

    public interface IPoolVariableInitializerFactory : IInitializerFactory
    {
        bool ValidatePoolVariableInitializer(PoolVariableInitializer definition, Pool pool, IInstallerContext installer, ICodeValidationErrorSink errorSink);
    }
}
