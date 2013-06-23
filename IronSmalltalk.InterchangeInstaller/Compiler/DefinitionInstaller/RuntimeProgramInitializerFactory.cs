using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.InterchangeInstaller.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Installer;
using IronSmalltalk.Runtime.Installer.Definitions;

namespace IronSmalltalk.InterchangeInstaller.Compiler.DefinitionInstaller
{
    public class RuntimeProgramInitializerFactory : RuntimeCodeFactory<InitializerNode>, IProgramInitializerFactory
    {
        public RuntimeProgramInitializerFactory(InitializerNode parseTree, ISourceCodeReferenceService sourceCodeService)
            : base(parseTree, sourceCodeService)
        {
        }

        public bool ValidateProgramInitializer(ProgramInitializer definition, IInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeProgramInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public CompiledInitializer CreateInitializer(InitializerDefinition definition, IInstallerContext installer)
        {
            return new RuntimeProgramInitializer(this.ParseTree, new DebugInfoService(this.SourceCodeService));
        }
    }
}
