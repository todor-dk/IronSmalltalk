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

namespace IronSmalltalk.Runtime.Execution
{
    public class ExecutionContext
    {
        public SmalltalkRuntime Runtime { get; private set; }

        public ExecutionContext(SmalltalkRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            this.Runtime = runtime;
        }
    }
}
