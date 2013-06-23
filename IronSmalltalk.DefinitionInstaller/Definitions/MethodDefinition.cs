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
using System.Linq;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.DefinitionInstaller.Definitions;

namespace IronSmalltalk.Runtime.Installer.Definitions
{
    public abstract class MethodDefinition : CodeBasedDefinition<IMethodFactory, CompiledMethod>
    {
        /// <summary>
        /// Name of the class that defines the method.
        /// </summary>
        public SourceReference<string> ClassName { get; private set; }

        /// <summary>
        /// Selector of the method.
        /// </summary>
        public SourceReference<string> Selector { get; private set; }

        public MethodDefinition(SourceReference<string> className, SourceReference<string> selector, ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, IMethodFactory code)
            : base(sourceCodeService, methodSourceCodeService, code)
        {
            if (className == null)
                throw new ArgumentNullException("className");
            if (selector == null)
                throw new ArgumentNullException("selector");
            this.ClassName = className;
            this.Selector = selector;
        }

        protected internal bool CreateMethod(IInstallerContext installer)
        {
            if (installer == null)
                throw new ArgumentNullException();
            // 1. Check if the selector is not complete garbage.
            if(String.IsNullOrWhiteSpace(this.Selector.Value))
                return installer.ReportError(this.Selector, InstallerErrors.MethodInvalidSelector);
            // 2. Get the class.
            ClassBinding classBinding = installer.GetClassBinding(this.ClassName.Value);
            // 3. Check that such a binding exists
            if (classBinding == null)
                return installer.ReportError(this.ClassName, InstallerErrors.MethodInvalidClassName);
            if (classBinding.Value == null)
                throw new InvalidOperationException("Should have been set in ClassDefinition.CreataGlobalObject().");

            // 3. Create the binding ... We allow duplicates and overwriting existing methods
            return this.InternalAddMethod(installer, classBinding.Value);
        }

        protected internal bool ValidateMethod(IInstallerContext installer)
        {
            if (installer == null)
                throw new ArgumentNullException();
            // 1. Check if the selector is not complete garbage.
            if (String.IsNullOrWhiteSpace(this.Selector.Value))
                return installer.ReportError(this.Selector, InstallerErrors.MethodInvalidSelector);
            // 2. Get the class.
            ClassBinding classBinding = installer.GetClassBinding(this.ClassName.Value);
            // 3. Check that such a binding exists
            if (classBinding == null)
                return installer.ReportError(this.ClassName, InstallerErrors.MethodInvalidClassName);
            if (classBinding.Value == null)
                throw new InvalidOperationException("Should have been set in ClassDefinition.CreataGlobalObject().");

            // 3. Create the binding ... We allow duplicates and overwriting existing methods
            return this.InternalValidateMethod(installer, classBinding.Value, 
                new IntermediateCodeValidationErrorSink(this.MethodSourceCodeService, installer));
        }

        protected abstract bool InternalAddMethod(IInstallerContext installer, SmalltalkClass cls);

        protected abstract bool InternalValidateMethod(IInstallerContext installer, SmalltalkClass cls, ICodeValidationErrorSink errorSink);
    }
}
