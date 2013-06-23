using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Installer;
using IronSmalltalk.Runtime.Installer.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    public interface IMethodFactory
    {
        CompiledMethod CreateMethod(MethodDefinition definition, IInstallerContext installer, SmalltalkClass cls);

        bool ValidateClassMethod(ClassMethodDefinition definition, IInstallerContext installer, SmalltalkClass cls, ICodeValidationErrorSink errorSink);

        bool ValidateInstanceMethod(InstanceMethodDefinition definition, IInstallerContext installer, SmalltalkClass cls, ICodeValidationErrorSink errorSink);
    }

    /// <summary>
    /// Error sink for reporting errors while validating intermediate code.
    /// </summary>
    public interface ICodeValidationErrorSink
    {
        /// <summary>
        /// Report an intermediate code validation error.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="start">Start location in the source code where the error occured.</param>
        /// <param name="stop">Stop location in the source code where the error occured.</param>
        // <returns></returns>
        void ReportError(string errorMessage, SourceLocation start, SourceLocation stop);
    }
}
