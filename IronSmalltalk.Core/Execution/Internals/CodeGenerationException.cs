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
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Execution.Internals
{
    /// <summary>
    /// Exceptions that can be thrown during code generation (JIT compiling).
    /// </summary>
    [Serializable]
    public class CodeGenerationException : SmalltalkDefinitionException
    {
        public CodeGenerationException() { }
        public CodeGenerationException(string message) : base(message) { }
        public CodeGenerationException(string message, Exception inner) : base(message, inner) { }
#if !SILVERLIGHT
        public CodeGenerationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
