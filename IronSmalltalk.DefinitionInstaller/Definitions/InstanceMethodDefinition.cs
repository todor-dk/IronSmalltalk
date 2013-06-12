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
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.Runtime.Installer.Definitions
{
    public class InstanceMethodDefinition : MethodDefinition
    {
        public InstanceMethodDefinition(SourceReference<string> className, SourceReference<string> selector, ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, CompiledMethod code)
            : base(className, selector, sourceCodeService, methodSourceCodeService, code)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} method", this.ClassName.Value);
        }

        protected override bool InternalAddMethod(IInstallerContext installer, SmalltalkClass cls)
        {
            System.Diagnostics.Debug.Assert(this.Selector.Value == this.Code.Selector.Value);
            cls.InstanceBehavior[this.Code.Selector] = this.Code;
            return true;
        }

        protected override bool InternalValidateMethod(IInstallerContext installer, SmalltalkClass cls, IIntermediateCodeValidationErrorSink errorSink)
        {
            return this.Code.ValidateInstanceMethod(cls, installer.NameScope, errorSink);
        }
    }
}
