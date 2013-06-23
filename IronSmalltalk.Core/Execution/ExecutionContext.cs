using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Execution
{
    public class ExecutionContext
    {
        public SmalltalkRuntime Runtime { get; private set; }

        public ExecutionContext(SmalltalkRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            this.Runtime = runtime;
        }
    }
}
