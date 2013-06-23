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
using IronSmalltalk.Common;
using IronSmalltalk.Runtime.Behavior;
using System.Linq;
using IronSmalltalk.DefinitionInstaller.Definitions;

namespace IronSmalltalk.Runtime.Installer.Definitions
{
    public abstract class CodeBasedDefinition<TFactory, TCompiledCode> : DefinitionBase
        where TCompiledCode : CompiledCode
    {
        /// <summary>
        /// Factory object that will be used to compiled the executable code.
        /// </summary>
        public TFactory Factory { get; private set; }

        protected TCompiledCode CompiledCode { get; set; }

        /// <summary>
        /// Source code service for translating source code positions.
        /// </summary>
        public ISourceCodeReferenceService SourceCodeService { get; private set; }

        /// <summary>
        /// Source code service for translating source code positions in the method code.
        /// </summary>
        public ISourceCodeReferenceService MethodSourceCodeService { get; private set; }

        public CodeBasedDefinition(ISourceCodeReferenceService sourceCodeService, ISourceCodeReferenceService methodSourceCodeService, TFactory factory)
        {
            if (sourceCodeService == null)
                throw new ArgumentNullException("sourceCodeService");
            if (methodSourceCodeService == null)
                throw new ArgumentNullException("methodSourceCodeService");
            if (factory == null)
                throw new ArgumentNullException("factory");
            this.SourceCodeService = sourceCodeService;
            this.MethodSourceCodeService = methodSourceCodeService;
            this.Factory = factory;
        }

        /// <summary>
        /// Add annotations the the object being created.
        /// </summary>
        /// <param name="installer">Context which is performing the installation.</param>
        /// <returns>Returns true if successful, otherwise false.</returns>
        protected internal override bool AnnotateObject(IInstallerContext installer)
        {
            if (installer == null)
                throw new ArgumentNullException();

            if (!this.Annotations.Any())
                return true;

            if (this.CompiledCode == null)
                return false;

            installer.AnnotateObject(this.CompiledCode, this.Annotations);

            return true;
        }

        protected class IntermediateCodeValidationErrorSink : ICodeValidationErrorSink
        {
            private ISourceCodeReferenceService SourceCodeService;
            private IInstallerContext Installer;

            public IntermediateCodeValidationErrorSink(ISourceCodeReferenceService sourceCodeService, IInstallerContext installer)
            {
                if (installer == null)
                    throw new ArgumentNullException();
                if (sourceCodeService == null)
                    throw new ArgumentNullException();
                this.Installer = installer;
                this.SourceCodeService = sourceCodeService;
            }

            public void ReportError(string errorMessage, SourceLocation start, SourceLocation stop)
            {
                this.Installer.ReportError(
                    new SourceReference(start, stop, this.SourceCodeService),
                    errorMessage);
            }
        }
    }
}
