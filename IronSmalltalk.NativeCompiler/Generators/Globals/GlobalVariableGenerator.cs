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
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Generators.Globals
{
    internal sealed class GlobalVariableGenerator : GlobalGenerator<GlobalVariableBinding>
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
