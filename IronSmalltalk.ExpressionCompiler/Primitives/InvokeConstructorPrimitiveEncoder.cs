using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Primitives.Exceptions;
using IronSmalltalk.ExpressionCompiler.Visiting;

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    public sealed class InvokeConstructorPrimitiveEncoder : MemberPrimitiveEncoder
    {
        private InvokeConstructorPrimitiveEncoder(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType)
            : base(visitor, parameters, definingType)
        {
        }

        public static Expression GeneratePrimitive(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType)
        {
            return (new InvokeConstructorPrimitiveEncoder(visitor, parameters, definingType)).GenerateExpression();
        }

        /// <summary>
        /// Get the expression needed to perform a constructor call.
        /// </summary>
        /// <returns>Expression for the call</returns>
        private Expression GenerateExpression()
        {
            Type[] argumentTypes = this.GetArgumentTypes(); // Types of arguments that we are to pass to the method.

            // Lookup the constructor ... matching argument types.
            ConstructorInfo ctor = this.DefiningType.GetConstructor(
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
                null, argumentTypes, null);
            // If no constructor found, throw an exception.
            if (ctor == null)
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingConstructor, this.DefiningType.Name));
            return Expression.New(ctor, this.GetArguments(argumentTypes, Conversion.Checked));
        }
    }
}
