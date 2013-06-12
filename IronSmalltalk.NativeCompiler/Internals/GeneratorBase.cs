using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal abstract class GeneratorBase
    {
        internal NativeCompiler Compiler { get; private set; }

        internal GeneratorBase(NativeCompiler compiler)
        {
            if (compiler == null)
                throw new ArgumentNullException("compiler");
            this.Compiler = compiler;
        }
    }
}
