using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler
{
    public class CompilerOptions
    {
        /// <summary>
        /// Service providing information necessary to emit 
        /// debugging symbol information for a source file.
        /// 
        /// If this property is set, the generator will emit debug(able) code.
        /// </summary>
        public IDebugInfoService DebugInfoService { get; set; }

        /// <summary>
        /// Strategy for encoding literal objects.
        /// </summary>
        public ILiteralEncodingStrategy LiteralEncodingStrategy { get; set; }

        /// <summary>
        /// Strategy for encoding dynamic calls.
        /// </summary>
        public IDynamicCallStrategy DynamicCallStrategy { get; set; }

        /// <summary>
        /// Strategy for encoding reading and writing of discrete bindings.
        /// </summary>
        public IDiscreteBindingEncodingStrategy DiscreteBindingEncodingStrategy { get; set; }

        /// <summary>
        /// Name scope for resolution of globals.
        /// </summary>
        /// <remarks>
        /// This is only used by the method compilers, not by the initializer compilers.
        /// 
        /// If it is set to null, the default GlobalScope of the SmalltalkRuntime is used.
        /// </remarks>
        public SmalltalkNameScope GlobalNameScope { get; set; }

        /// <summary>
        /// Set this to true if multiple runtimes should be supported.
        /// </summary>
        public bool CheckRuntimeInstance { get; set; }
    }
}
