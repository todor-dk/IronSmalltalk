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

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    public static class RuntimeHelpers
    {
        public static MessageSendCallSiteBinder CreateCallSiteBinder(SmalltalkRuntime runtime, string selector, string nativeName, int argumentCount)
        {
            return new MessageSendCallSiteBinder(runtime,
                runtime.GetSymbol(selector),
                nativeName,
                argumentCount);
        }

        public static ConstantSendCallSiteBinder CreateConstantCallSiteBinder(SmalltalkRuntime runtime, string selector, string nativeName, int argumentCount)
        {
            return new ConstantSendCallSiteBinder(runtime,
                runtime.GetSymbol(selector),
                nativeName,
                argumentCount);
        }

        public static SuperSendCallSiteBinder CreateSuperSendCallSiteBinder(SmalltalkRuntime runtime, string selector, string superLookupScope, int argumentCount)
        {
            return new SuperSendCallSiteBinder(runtime,
                runtime.GetSymbol(selector),
                runtime.GetSymbol(superLookupScope));
        }
    }
}
