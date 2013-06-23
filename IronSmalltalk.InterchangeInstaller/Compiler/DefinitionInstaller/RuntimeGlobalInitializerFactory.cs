using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.InterchangeInstaller.Runtime;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Installer;
using IronSmalltalk.Runtime.Installer.Definitions;

namespace IronSmalltalk.InterchangeInstaller.Compiler.DefinitionInstaller
{
    public class RuntimeGlobalInitializerFactory : RuntimeCodeFactory<InitializerNode>, IGlobalInitializerFactory
    {
        public string GlobalName { get; private set; }

        public RuntimeGlobalInitializerFactory(InitializerNode parseTree, ISourceCodeReferenceService sourceCodeService, string globalName)
            : base(parseTree, sourceCodeService)
        {
            if (String.IsNullOrWhiteSpace(globalName))
                throw new ArgumentNullException("globalName");

            this.GlobalName = globalName;
        }

        public bool ValidateClassInitializer(GlobalInitializer definition, SmalltalkClass cls, IInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeGlobalInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public bool ValidateGlobalInitializer(GlobalInitializer definition, IInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeGlobalInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public CompiledInitializer CreateInitializer(InitializerDefinition definition, IInstallerContext installer)
        {
            GlobalVariableOrConstantBinding globalBinding = installer.GetGlobalVariableOrConstantBinding(this.GlobalName);
            ClassBinding classBinding = installer.GetClassBinding(this.GlobalName);

            // 3. Check that such a binding exists ... but not both
            if (!((globalBinding == null) ^ (classBinding == null)))
                return null;

            if (classBinding != null)
                return new RuntimeGlobalInitializer(this.ParseTree, new DebugInfoService(this.SourceCodeService), classBinding);
            if (globalBinding != null)
                return new RuntimeGlobalInitializer(this.ParseTree, new DebugInfoService(this.SourceCodeService), globalBinding);
            return null;
        }
    }
}
