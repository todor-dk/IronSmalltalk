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

using System.IO;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.InterchangeInstaller.Compiler.ParseNodes;
using IronSmalltalk.Runtime;

namespace IronSmalltalk.InterchangeInstaller.Compiler
{
    public class InterchangeFormatParserIST10 : InterchangeFormatParser
    {
        public InterchangeFormatParserIST10(TextReader reader)
            : base(reader)
        {
        }

        protected override ClassDefinitionNode CreateClassDefinitionNode()
        {
            return new ClassDefinitionNodeIST10();
        }
    }

    public class ClassDefinitionNodeIST10 : ClassDefinitionNode
    {
        protected override SmalltalkClass.InstanceStateEnum? GetInstanceState(InterchangeFormatProcessor processor, IParseErrorSink parseErrorSink, ISourceCodeReferenceService sourceCodeService)
        {
            // TODO : Move constants out of code into a the InterchangeFormatConstants class
            // TODO : Move error messages out of code into a the InterchangeFormatErrors class

            if (this.IndexedInstanceVariables.Value == "native")
                return SmalltalkClass.InstanceStateEnum.Native;
            return base.GetInstanceState(processor, parseErrorSink, sourceCodeService);
        }
    }
}
