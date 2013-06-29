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
