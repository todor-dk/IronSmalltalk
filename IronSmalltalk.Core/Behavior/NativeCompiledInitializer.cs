using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.Runtime.Behavior
{
    public sealed class NativeCompiledInitializer : CompiledInitializer
    {
        public Func<object, ExecutionContext, object> Delegate { get; private set; }

        public NativeCompiledInitializer(InitializerType type, IDiscreteBinding binding, Func<object, ExecutionContext, object> functionDelegate)
            : base(type, binding)
        {
            if (functionDelegate == null)
                throw new ArgumentNullException("functionDelegate");
            this.Delegate = functionDelegate;
        }

        public override object Execute(object self, Execution.ExecutionContext executionContext)
        {
            return this.Delegate(self, executionContext);
        }
    }
}
