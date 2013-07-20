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
        public static MessageSendCallSiteBinder CreateCallSiteBinder(string selector, string nativeName, int argumentCount)
        {
            return new MessageSendCallSiteBinder(selector, nativeName, argumentCount);
        }

        public static ConstantSendCallSiteBinder CreateConstantCallSiteBinder(string selector, string nativeName, int argumentCount)
        {
            return new ConstantSendCallSiteBinder(selector, nativeName, argumentCount);
        }

        public static SuperSendCallSiteBinder CreateSuperSendCallSiteBinder(string selector, string superLookupScope, int argumentCount)
        {
            return new SuperSendCallSiteBinder(selector, superLookupScope);
        }
    }
}
