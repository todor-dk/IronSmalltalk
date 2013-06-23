using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.AstJitCompiler.Runtime;
using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.Runtime.Installer;

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
