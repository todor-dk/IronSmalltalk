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

using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Interchange;
using IronSmalltalk.Runtime.Installer;

namespace IronSmalltalk.Internals
{
    public abstract class ErrorSinkBase : IParseErrorSink, IInterchangeErrorSink, IInstallErrorSink, IFileInErrorSink
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

        void IFileInErrorSink.AddInstallError(SourceLocation startPosition, SourceLocation stopPosition, string errorMessage)
        {
            this.ReportError(errorMessage, startPosition, stopPosition, ErrorType.Install);
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
