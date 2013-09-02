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
        public ImplementationException() { }
        public ImplementationException(string message) : base(message) { }
        public ImplementationException(string message, Exception inner) : base(message, inner) { }
        protected ImplementationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
