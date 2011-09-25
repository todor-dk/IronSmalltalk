using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
{
    public enum PrimitivesEnum
    {
        BuiltInPrimitive,
        InvokeStaticMethod,
        InvokeInstanceMethod,
        InvokeConstructor,
        InvokeGetProperty,
        InvokeSetProperty,
        GetFieldValue,
        SetFieldValue
        // TO-DO: Event
    }
}
