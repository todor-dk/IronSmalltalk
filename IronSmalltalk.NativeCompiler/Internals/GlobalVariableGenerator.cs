using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class GlobalVariableGenerator : GlobalGenerator<GlobalVariableBinding>
    {
        internal GlobalVariableGenerator(NativeCompiler compiler, GlobalVariableBinding binding)
            : base(compiler, binding)
        {
        }

        protected override string SubNamespace
        {
            get { return "Globals"; }
        }

        protected override string AddBindingMethodName
        {
            get { return "AddGlobalVariableBinding"; }
        }
    }
}
