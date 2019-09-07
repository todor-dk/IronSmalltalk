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

        public bool ValidateClassInitializer(GlobalInitializer definition, SmalltalkClass cls, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeGlobalInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public bool ValidateGlobalInitializer(GlobalInitializer definition, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeGlobalInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public CompiledInitializer CreateInitializer(InitializerDefinition definition, IDefinitionInstallerContext installer)
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
