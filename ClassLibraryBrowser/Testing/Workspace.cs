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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Interchange;
using IronSmalltalk.InterchangeInstaller.Runtime;

namespace IronSmalltalk.Tools.ClassLibraryBrowser.Testing
{
    public class Workspace
    {
        public SmalltalkEnvironment Environment;
        public object LastResult;
        public IWorkspaceClient Client;

        public Workspace(IWorkspaceClient client)
        {
            if (client == null)
                throw new ArgumentNullException();
            this.Environment = new SmalltalkEnvironment();
            this.LastResult = null;
            this.Client = client;
        }

        public bool Install()
        {

            string txt = this.Client.InstallSourceCode;
            ErrorSink errorSink = new ErrorSink(this.Client);
            try
            {
                this.Environment.CompilerService.Install(new StringFileInInformation(txt, errorSink));
            }
            catch (IronSmalltalk.Runtime.Internal.SmalltalkDefinitionException ex)
            {
                if (this.Client != null)
                    this.Client.ReportError(ex.Message, SourceLocation.Invalid, SourceLocation.Invalid);
                return false;
            }
            catch (IronSmalltalk.Runtime.Internal.SmalltalkRuntimeException ex)
            {
                if (this.Client != null)
                    this.Client.ReportError(ex.Message, SourceLocation.Invalid, SourceLocation.Invalid);
                return false;
            }
            return !errorSink.HadErrors;
        }

        public bool Evaluate()
        {
            string txt = this.Client.EvaluateSourceCode;
            StringReader reader = new StringReader(txt);

            ErrorSink errorSink = new ErrorSink(this.Client);
            Parser parser = new Parser();
            parser.ErrorSink = errorSink;
            InitializerNode node = parser.ParseInitializer(reader);
            if (errorSink.HadErrors)
                return false;

            Expression<Func<SmalltalkRuntime, object, object>> lambda;
            try
            {
                RuntimeProgramInitializer code = new RuntimeProgramInitializer(node, null);
                var compilationResult = code.Compile(this.Environment.Runtime);
                if (compilationResult == null)
                    return false;
                lambda = compilationResult.ExecutableCode;
                if (lambda == null)
                    return false;
            }
            catch (IronSmalltalk.Runtime.Internal.SmalltalkDefinitionException ex)
            {
                if (this.Client != null)
                    this.Client.ReportError(ex.Message, SourceLocation.Invalid, SourceLocation.Invalid);
                return false;
            }
            catch (IronSmalltalk.Runtime.Internal.SmalltalkRuntimeException ex)
            {
                if (this.Client != null)
                    this.Client.ReportError(ex.Message, SourceLocation.Invalid, SourceLocation.Invalid);
                return false;
            }

            try
            {
                var function = lambda.Compile();
                this.LastResult = function(this.Environment.Runtime, null);
            }
            catch (Exception ex)
            {
                if (this.Client != null)
                    this.Client.ReportError(ex.Message, SourceLocation.Invalid, SourceLocation.Invalid);
                return false;
            }
            this.PrintResult(this.LastResult);


            //dynamic rt = this.Environment.Runtime;
            //string x = rt.GetTestString();
            //dynamic x = this.LastResult;
            //dynamic y = x.PrintString();
            //y = x.PrintString;
            //int z = x.Hash();

            return true;
        }

        #region Reporting

        private class ErrorSink : IronSmalltalk.Internals.ErrorSinkBase
        {
            public bool HadErrors;
            private IWorkspaceClient Client;

            public ErrorSink(IWorkspaceClient client)
            {
                this.Client = client;
            }

            protected override void ReportError(string message, SourceLocation start, SourceLocation end, IronSmalltalk.Internals.ErrorSinkBase.ErrorType type, params object[] offenders)
            {
                this.HadErrors = true;
                if (this.Client != null)
                    this.Client.ReportError(message, start, end);
            }
        }

        private void PrintResult(object val)
        {
            StringBuilder str = new StringBuilder();
            this.PrintResult(val, new HashSet<object>(), str, 0);
            this.Client.ReportResult(str.ToString());
        }

        private void PrintResult(object val, ISet<object> recursionSet, StringBuilder str, int indent)
        {
            for (int i = 0; i < indent; i++)
		        str.Append('\t');

            if (recursionSet.Contains(val))
            {
                str.Append("!RECURSION!");
                return;
            }
            recursionSet.Add(val);

            if (val == null)
            {
                str.Append("nil");
                return;
            }
            if (val is Array)
            {
                this.PrintResult((Array)val, recursionSet, str, indent);
                return;
            }

            str.Append(val.GetType().Name);
            str.Append(" ");
            str.Append(val.ToString());

        }
        private void PrintResult(Array val, ISet<object> recursionSet, StringBuilder str, int indent)
        {
            str.Append("Array ");
            str.Append(val.GetType().Name);
            str.Append(" Length: ");
            str.Append(val.Length.ToString());
            str.Append(" #(");
            foreach (object elem in val)
            {
                str.AppendLine();
                this.PrintResult(elem, recursionSet, str, indent + 1);
            }
            str.Append(" )");
        }
        private void PrintResult(IEnumerable val, ISet<object> recursionSet, StringBuilder str, int indent)
        {
            str.Append("Enumerable ");
            str.Append(val.GetType().Name);
            str.Append(" Length: ");
            str.Append(" #(");
            foreach (object elem in val)
            {
                str.AppendLine();
                this.PrintResult(elem, recursionSet, str, indent + 1);
            }
            str.Append(" )");
        }

        #endregion
    }

    public interface IWorkspaceClient
    {
        void ReportError(string message, SourceLocation start, SourceLocation end);
        void ReportResult(string message);
        string InstallSourceCode { get; }
        string EvaluateSourceCode { get; }
    }
}
