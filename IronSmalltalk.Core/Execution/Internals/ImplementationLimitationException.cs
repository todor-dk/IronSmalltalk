using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Execution.Internals
{
    /// <summary>
    /// Exception that occurs when we've reached some implementation limit.
    /// </summary>
    /// <remarks>
    /// This exception should normally be thrown by the compilers and/or definition installer.
    /// </remarks>
    [Serializable]
    public class ImplementationLimitationException : CodeGenerationException
    {
        public ImplementationLimitationException() { }
        public ImplementationLimitationException(string message) : base(message) { }
        public ImplementationLimitationException(string message, Exception inner) : base(message, inner) { }
        protected ImplementationLimitationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
