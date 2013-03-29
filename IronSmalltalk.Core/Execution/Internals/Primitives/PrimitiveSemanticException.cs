using System;
using System.Runtime.Serialization;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
{
    public class PrimitiveSemanticException : SemanticCodeGenerationException
    {
        public PrimitiveSemanticException() { }
        public PrimitiveSemanticException(string message) : base(message) { }
        public PrimitiveSemanticException(string message, Exception inner) : base(message, inner) { }
#if !SILVERLIGHT
        protected PrimitiveSemanticException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
