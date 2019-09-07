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
using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.ExpressionCompiler.Runtime;

namespace IronSmalltalk.InterchangeInstaller.Compiler.DefinitionInstaller
{
    public abstract class RuntimeCodeFactory<TParseTree>
    {
        public TParseTree ParseTree { get; private set; }

        public ISourceCodeReferenceService SourceCodeService { get; private set; }

        public RuntimeCodeFactory(TParseTree parseTree, ISourceCodeReferenceService sourceCodeService)
        {
            if (parseTree == null)
                throw new ArgumentNullException("parseTree");
            if (sourceCodeService == null)
                throw new ArgumentNullException("sourceCodeService");

            this.ParseTree = parseTree;
            this.SourceCodeService = sourceCodeService;
        }

        protected IRuntimeCodeValidationErrorSink GetErrorSink(ICodeValidationErrorSink errorSink)
        {
            if (errorSink == null)
                return null;
            return new RuntimeCodeValidationErrorSink(errorSink);
        }

        protected class RuntimeCodeValidationErrorSink : IRuntimeCodeValidationErrorSink
        {
            private readonly ICodeValidationErrorSink ErrorSink;

            internal RuntimeCodeValidationErrorSink(ICodeValidationErrorSink errorSink)
            {
                this.ErrorSink = errorSink;
            }

            public void ReportError(string errorMessage, Common.SourceLocation start, Common.SourceLocation stop)
            {
                this.ErrorSink.ReportError(errorMessage, start, stop);
            }
        }

    }
}
