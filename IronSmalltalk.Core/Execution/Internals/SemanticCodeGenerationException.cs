﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace IronSmalltalk.Runtime.Execution.Internals
{
    /// <summary>
    /// Exception that inidicates that somethig is wrong with the Smalltalk code that
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
    /// Things that do not fall in this catogory are runtime errors, 
    /// for example message not understoods, illegal index accessors
    /// or similar "more dynamic" errors. For example, the member name
    /// or the a .Net type name given to a primitive API to resolve is 
    /// not concidered a semantic error but a runtime error.
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