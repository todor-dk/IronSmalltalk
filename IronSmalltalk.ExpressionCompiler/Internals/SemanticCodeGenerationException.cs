﻿/*
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
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    /// <summary>
    /// Exception that indicates that something is wrong with the Smalltalk code that
    /// was passed to us, but something that the Smalltalk compiler cannot (easily)
    /// detect and report to the developer.
    /// </summary>
    /// <example>
    /// Primitive call was given wrong number of API parameters that is needed for 
    /// the given primitive calling convention. The Smalltalk compiler does not know 
    /// how to interpret those and only stores the information in the parse node.
    /// 
    /// Some code reference a class variable that was not defined. Code tries to
    /// modify a constant or similar.
    /// 
    /// Things that do not fall in this category are runtime errors, 
    /// for example message not understood, illegal index accessors
    /// or similar "more dynamic" errors. For example, the member name
    /// or the a .Net type name given to a primitive API to resolve is 
    /// not considered a semantic error but a runtime error.
    /// </example>
    [Serializable]
    public class SemanticCodeGenerationException : CodeGenerationException
    {
        public SemanticCodeGenerationException()
        {
        }
        public SemanticCodeGenerationException(string message)
            : base(message)
        {
        }
        public SemanticCodeGenerationException(string message, Exception inner)
            : base(message, inner)
        {
        }
#if !SILVERLIGHT
        protected SemanticCodeGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
