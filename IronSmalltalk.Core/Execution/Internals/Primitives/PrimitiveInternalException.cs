using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
{
    /// <summary>
    /// Exception that indicates that something wrong happened during generation of a primitive code.
    /// </summary>
    /// <remarks>
    /// This is last-defense exception, i.e. the caller should have validated input data etc.,
    /// but they didn't. This is most probably due to semantic error in the Smalltalk source code,
    /// but we cannot be 100% sure this is the case, and we also have no idea which Smalltalk 
    /// code is the offending code, so we can't throw a semantic error and instead throw
    /// this exception. This means, the client of our code didn't do the validatio they were 
    /// supposed to. Example:
    ///     Add (+) expects two (2) arguments, but 4 (four) were passed in the 
    ///     primitive declaration, so we fail!
    /// </remarks>
    public class PrimitiveInternalException : InternalCodeGenerationException
    {
        public PrimitiveInternalException() { }
        public PrimitiveInternalException(string message) : base(message) { }
        public PrimitiveInternalException(string message, Exception inner) : base(message, inner) { }
#if !SILVERLIGHT
        protected PrimitiveInternalException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
