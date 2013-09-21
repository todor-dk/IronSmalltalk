using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Behavior
{
    public sealed class NativeCompiledMethod : CompiledMethod
    {
        public MethodInfo NativeMethod { get; private set; }
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public NativeCompiledMethod(SmalltalkClass cls, Symbol selector, MethodType methodType, MethodInfo nativeMethod)
            : base(cls, selector, methodType)
        {
            if (nativeMethod == null)
                throw new ArgumentNullException("nativeMethod");
            this.NativeMethod = nativeMethod;
        }

        public override Expression GetExpression(Expression self, Expression executionContext, IEnumerable<Expression> arguments)
        {
            List<Expression> args = new List<Expression>();
            args.Add(self);
            args.Add(executionContext);
            args.AddRange(arguments);
            return Expression.Call(this.NativeMethod, args);
        }
    }
}
