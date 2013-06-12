using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class GlobalConstantGenerator : GlobalGenerator<GlobalConstantBinding>
    {
        internal GlobalConstantGenerator(NativeCompiler compiler, GlobalConstantBinding binding)
            : base(compiler, binding)
        {
        }

        protected override string SubNamespace
        {
            get { return "Constants"; }
        }

        protected override string AddBindingMethodName
        {
            get { return "AddGlobalConstantBinding"; }
        }

    }
}
