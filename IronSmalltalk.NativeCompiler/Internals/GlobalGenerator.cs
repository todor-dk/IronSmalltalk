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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal abstract class GlobalGenerator<TBinding> : GlobalBindingGenerator<TBinding>
        where TBinding : IDiscreteGlobalBinding
    {
        internal GlobalGenerator(NativeCompiler compiler, TBinding binding)
            : base(compiler, binding)
        {
        }

        private TypeBuilder Type;

        internal override void GenerateTypes()
        {
            //this.Type = this.Compiler.NativeGenerator.DefineType(
            //    this.Compiler.GetTypeName(this.SubNamespace, this.Binding.Name),
            //    typeof(Object),
            //    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);
        }

        protected abstract string SubNamespace { get; }
    }
}
