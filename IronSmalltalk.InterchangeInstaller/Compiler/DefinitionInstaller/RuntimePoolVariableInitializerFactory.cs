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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;

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


        public bool ValidatePoolVariableInitializer(PoolVariableInitializer definition, Pool pool, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimePoolItemInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public CompiledInitializer CreateInitializer(InitializerDefinition definition, IDefinitionInstallerContext installer)
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
