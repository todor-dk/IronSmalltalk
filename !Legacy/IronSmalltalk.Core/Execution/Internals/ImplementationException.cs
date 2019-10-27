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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Execution.Internals
{
    /// <summary>
    /// An exception that is thrown due to a bug in our implementation.
    /// </summary>
    [Serializable]
    public class ImplementationException : Exception
    {
        public ImplementationException()
        {
        }
        public ImplementationException(string message)
            : base(message)
        {
        }
        public ImplementationException(string message, Exception inner)
            : base(message, inner)
        {
        }
        protected ImplementationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
