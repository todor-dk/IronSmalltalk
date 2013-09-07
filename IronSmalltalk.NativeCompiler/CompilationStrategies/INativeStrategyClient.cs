using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.CompilationStrategies
{
    internal interface INativeStrategyClient
    {
        NativeCompiler Compiler { get; }
        TypeBuilder ContainingType { get; }
    }
}
