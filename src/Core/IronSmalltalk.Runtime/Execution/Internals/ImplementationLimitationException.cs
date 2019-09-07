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
