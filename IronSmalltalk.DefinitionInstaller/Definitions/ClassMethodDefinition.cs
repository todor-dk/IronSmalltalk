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
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    public class ClassMethodDefinition : MethodDefinition
    {
        public ClassMethodDefinition(SourceReference<string> className, SourceReference<string> selector, ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, IMethodFactory factory)
            : base(className, selector, sourceCodeService, methodSourceCodeService, factory)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} classMethod", this.ClassName.Value);
        }

        protected override bool InternalAddMethod(IDefinitionInstallerContext installer, SmalltalkClass cls)
        {
            CompiledMethod method = this.Factory.CreateMethod(this, installer, cls);
            if (method == null)
                return false;
            System.Diagnostics.Debug.Assert(this.Selector.Value == method.Selector.Value);
            cls.ClassBehavior[method.Selector] = method;
            this.CompiledCode = method;
            return true;
        }

        protected override bool InternalValidateMethod(IDefinitionInstallerContext installer, SmalltalkClass cls, ICodeValidationErrorSink errorSink)
        {
            return this.Factory.ValidateClassMethod(this, installer, cls, errorSink);
        }
    }
}
