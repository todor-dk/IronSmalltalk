using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler
{
    public class NativeCompilerParameters
    {
        public SmalltalkRuntime Runtime { get; set; }
        public string RootNamespace { get; set; }
        public string OutputDirectory { get; set; }
        public string AssemblyName { get; set; }
        public string FileExtension { get; set; }
        public bool EmitDebugSymbols { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string Company { get; set; }
        public string Copyright { get; set; }
        public string Trademark { get; set; }

        internal NativeCompilerParameters Copy()
        {
            return (NativeCompilerParameters) this.MemberwiseClone();
        }

    }
}
