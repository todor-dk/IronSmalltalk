using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class RuntimeGenerator : GeneratorBase
    {
        internal RuntimeGenerator(NativeCompiler compiler)
            : base(compiler)
        {
        }

        private TypeBuilder Type;

        internal void Generate()
        {
            this.Type = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Smalltalk"),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);
        }

    }
}
