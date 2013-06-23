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
using IronSmalltalk.Runtime.Installer;
using IronSmalltalk.Runtime.Installer.Definitions;

namespace IronSmalltalk.InterchangeInstaller.Compiler.DefinitionInstaller
{
    public class RuntimeCompiledMethodFactory : RuntimeCodeFactory<MethodNode>, IMethodFactory
    {

        public RuntimeCompiledMethodFactory(MethodNode parseTree, ISourceCodeReferenceService sourceCodeService)
            : base(parseTree, sourceCodeService)
        {
        }

        public CompiledMethod CreateMethod(MethodDefinition definition, IInstallerContext installer, SmalltalkClass cls)
        {
            Symbol selector = installer.Runtime.GetSymbol(this.ParseTree.Selector);
            return new RuntimeCompiledMethod(selector, this.ParseTree, new DebugInfoService(this.SourceCodeService));
        }

        public bool ValidateClassMethod(ClassMethodDefinition definition, IInstallerContext installer, SmalltalkClass cls, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeCompiledMethod)this.CreateMethod(definition, installer, cls)).ValidateClassMethod(cls, installer.NameScope, this.GetErrorSink(errorSink));
        }

        public bool ValidateInstanceMethod(InstanceMethodDefinition definition, IInstallerContext installer, SmalltalkClass cls, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeCompiledMethod)this.CreateMethod(definition, installer, cls)).ValidateInstanceMethod(cls, installer.NameScope, this.GetErrorSink(errorSink));
        }
    }
}
