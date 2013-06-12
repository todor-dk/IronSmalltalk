using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.CodeGeneration.BindingScopes;

namespace IronSmalltalk.Runtime.Behavior
{
    public abstract class CompiledInitializer : CompiledCode
    {
        public InitializerCompilationResult Compile(SmalltalkRuntime runtime)
        {
            return this.Compile(runtime, runtime.GlobalScope);
        }

        protected abstract InitializerCompilationResult Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope);

        public abstract bool Validate(SmalltalkNameScope globalNameScope, IIntermediateCodeValidationErrorSink errorSink);

        public abstract object Execute(SmalltalkRuntime runtime, object self);
    }

    /// <summary>
    /// Error sink for reporting errors while validating intermediate code.
    /// </summary>
    public interface IIntermediateCodeValidationErrorSink
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

    public interface IDebugInfoService
    {
        /// <summary>
        /// Translate the locations of source references from relative to absolute positions.
        /// </summary>
        /// <param name="position">Relative source location.</param>
        /// <returns>Absolute source location.</returns>
        SourceLocation TranslateSourcePosition(SourceLocation position);

        /// <summary>
        /// Get or set the document containing the debug symbols.
        /// </summary>
        SymbolDocumentInfo SymbolDocument { get; }
    }
}
