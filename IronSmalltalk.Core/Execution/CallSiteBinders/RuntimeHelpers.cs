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
