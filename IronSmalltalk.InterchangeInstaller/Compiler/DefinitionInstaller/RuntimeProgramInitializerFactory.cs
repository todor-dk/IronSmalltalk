﻿/*
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
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.InterchangeInstaller.Compiler.DefinitionInstaller
{
    public class RuntimeProgramInitializerFactory : RuntimeCodeFactory<InitializerNode>, IProgramInitializerFactory
    {
        public RuntimeProgramInitializerFactory(InitializerNode parseTree, ISourceCodeReferenceService sourceCodeService)
            : base(parseTree, sourceCodeService)
        {
        }

        public bool ValidateProgramInitializer(ProgramInitializer definition, IDefinitionInstallerContext installer, ICodeValidationErrorSink errorSink)
        {
            return ((RuntimeProgramInitializer)this.CreateInitializer(definition, installer)).Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }

        public CompiledInitializer CreateInitializer(InitializerDefinition definition, IDefinitionInstallerContext installer)
        {
            return new RuntimeProgramInitializer(this.ParseTree, new DebugInfoService(this.SourceCodeService));
        }
    }
}
