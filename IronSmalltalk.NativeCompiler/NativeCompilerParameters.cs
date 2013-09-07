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
        public string FileVersion { get; set; }
        public string Product { get; set; }
        public string AssemblyVersion { get; set; }
        public string ProductVersion { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public string Company { get; set; }
        public string Copyright { get; set; }
        public string Trademark { get; set; }

        internal NativeCompilerParameters Copy()
        {
            return (NativeCompilerParameters) this.MemberwiseClone();
        }

    }
}
