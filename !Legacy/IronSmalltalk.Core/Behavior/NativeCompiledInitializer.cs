/*
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
                throw new ArgumentNullException(nameof(functionDelegate));
            this.Delegate = functionDelegate;
        }

        public override object Execute(object self, Execution.ExecutionContext executionContext)
        {
            return this.Delegate(self, executionContext);
        }
    }
}
