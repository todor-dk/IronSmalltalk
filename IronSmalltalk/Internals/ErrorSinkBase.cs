using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Interchange;
using IronSmalltalk.Runtime.Installer;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;

namespace IronSmalltalk.Internals
{
    public abstract class ErrorSinkBase : IParseErrorSink, IInterchangeErrorSink, IInstallErrorSink
    {
        void IParseErrorSink.AddParserError(SourceLocation startPosition, SourceLocation stopPosition, string parseErrorMessage, IronSmalltalk.Compiler.LexicalTokens.IToken offendingToken)
        {
            this.ReportError(parseErrorMessage, startPosition, stopPosition, ErrorType.Parse, offendingToken);
        }

        void IParseErrorSink.AddParserError(IParseNode node, SourceLocation startPosition, SourceLocation stopPosition, string parseErrorMessage, IronSmalltalk.Compiler.LexicalTokens.IToken offendingToken)
        {
            this.ReportError(parseErrorMessage, startPosition, stopPosition, ErrorType.Parse, node, offendingToken);
        }

        void IronSmalltalk.Compiler.LexicalAnalysis.IScanErrorSink.AddScanError(IronSmalltalk.Compiler.LexicalTokens.IToken token, SourceLocation startPosition, SourceLocation stopPosition, string scanErrorMessage)
        {
            this.ReportError(scanErrorMessage, startPosition, stopPosition, ErrorType.Scan, token);
        }

        void IInterchangeErrorSink.AddInterchangeError(SourceLocation startPosition, SourceLocation stopPosition, string errorMessage)
        {
            this.ReportError(errorMessage, startPosition, stopPosition, ErrorType.Interchange);
        }

        void IInstallErrorSink.AddInstallError(string installErrorMessage, ISourceReference sourceReference)
        {
            this.ReportError(installErrorMessage, sourceReference.StartPosition, sourceReference.StopPosition, ErrorType.Install);
        }

        protected abstract void ReportError(string message, SourceLocation start, SourceLocation end, ErrorType type, params object[] offenders);

        protected enum ErrorType
        {
            Scan,
            Parse,
            Install,
            Interchange
        }
    }
}
