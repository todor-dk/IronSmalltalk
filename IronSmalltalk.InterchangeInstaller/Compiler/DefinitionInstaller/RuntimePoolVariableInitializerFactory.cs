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
    public class RuntimePoolVariableInitializerFactory : RuntimeCodeFactory<InitializerNode>, IPoolVariableInitializerFactory
    {
        public string PoolName { get; private set; }

        public string PoolItemName { get; private set; }

        public RuntimePoolVariableInitializerFactory(InitializerNode parseTree, ISourceCodeReferenceService sourceCodeService, string poolName, string poolItemName)
            : base(parseTree, sourceCodeService)
        {
            if (String.IsNullOrWhiteSpace(poolName))
                throw new ArgumentNullException("poolName");
            if (String.IsNullOrWhiteSpace(poolItemName))
                throw new ArgumentNullException("poolItemName");

            this.PoolName = poolName;
            this.PoolItemName = poolItemName;
        }


        public bool ValidatePoolVariableInitializer(PoolVariableInitializer definition, Pool pool, IInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimePoolItemInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public CompiledInitializer CreateInitializer(InitializerDefinition definition, IInstallerContext installer)
        {
            PoolBinding poolBinding = installer.GetPoolBinding(this.PoolName);
            if ((poolBinding == null) || (poolBinding.Value == null))
                return null;
            PoolVariableOrConstantBinding binding = poolBinding.Value[this.PoolItemName];
            if (binding == null)
                return null;
            return new RuntimePoolItemInitializer(this.ParseTree, new DebugInfoService(this.SourceCodeService), binding, this.PoolName);
        }
    }
}
