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

namespace IronSmalltalk.InterchangeInstaller.Compiler.DefinitionInstaller
{
    public class RuntimeCompiledMethodFactory : RuntimeCodeFactory<MethodNode>, IMethodFactory
    {

        public RuntimeCompiledMethodFactory(MethodNode parseTree, ISourceCodeReferenceService sourceCodeService)
            : base(parseTree, sourceCodeService)
        {
        }

        public CompiledMethod CreateMethod(MethodDefinition definition, IDefinitionInstallerContext installer, SmalltalkClass cls, CompiledMethod.MethodType methodType)
        {
            Symbol selector = installer.Runtime.GetSymbol(this.ParseTree.Selector);
            return new RuntimeCompiledMethod(cls, selector, methodType, this.ParseTree, new DebugInfoService(this.SourceCodeService));
        }

        public bool ValidateMethod(MethodDefinition definition, IDefinitionInstallerContext installer, SmalltalkClass cls, CompiledMethod.MethodType methodType, ICodeValidationErrorSink errorSink)
        {
            RuntimeCompiledMethod method = (RuntimeCompiledMethod)this.CreateMethod(definition, installer, cls, methodType);
            return method.Validate(installer.NameScope, this.GetErrorSink(errorSink));
        }
    }
}
