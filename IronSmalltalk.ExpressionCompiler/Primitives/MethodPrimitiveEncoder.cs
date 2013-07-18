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
    public abstract class MethodPrimitiveEncoder : NamedMemberPrimitiveEncoder
    {
        public MethodPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }
    }

    public sealed class InvokeStaticMethodPrimitiveEncoder : MethodPrimitiveEncoder
    {
        private InvokeStaticMethodPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }

        public static Expression GeneratePrimitive(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new InvokeStaticMethodPrimitiveEncoder(context, parameters, definingType, memberName)).GenerateExpression();
        }

        /// <summary>
        /// Get the expression needed to perform a static method call.
        /// </summary>
        /// <returns>Expression for the call</returns>
        private Expression GenerateExpression()
        {
            Type[] argumentTypes = this.GetArgumentTypes(); // Types of arguments that we are to pass to the method.

            // Lookup the method ... matching argument types.
            MethodInfo method = this.DefiningType.GetMethod(this.MemberName,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static,
                null, argumentTypes.ToArray(), null);
            // If no method found, throw an exception.
            if (method == null)
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingMethod, this.DefiningType.Name, this.MemberName));
            return Expression.Call(method, this.GetArguments(argumentTypes, Conversion.Checked));
        }
    }

    public sealed class InvokeInstanceMethodPrimitiveEncoder : MethodPrimitiveEncoder
    {
        private InvokeInstanceMethodPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }

        public static Expression GeneratePrimitive(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new InvokeInstanceMethodPrimitiveEncoder(context, parameters, definingType, memberName)).GenerateExpression();
        }

        /// <summary>
        /// Get the expression needed to perform an instance method call.
        /// </summary>
        /// <returns>Expression for the call</returns>
        private Expression GenerateExpression()
        {
            if (!this.Parameters.Any())
                throw new PrimitiveSemanticException(CodeGenerationErrors.WrongNumberOfParameters);
            Type[] argumentTypes = this.GetArgumentTypes(); // Types of arguments that we are to pass to the method.

            Type[] matchTypes = new Type[argumentTypes.Length - 1];
            Array.Copy(argumentTypes, 1, matchTypes, 0, matchTypes.Length);
            // Lookup the method ... matching argument types.
            MethodInfo method = this.DefiningType.GetMethod(this.MemberName,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                null, matchTypes, null);
            // If no method found, throw an exception.
            if (method == null)
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingMethod, this.DefiningType.Name, this.MemberName));
            IList<Expression> args = this.GetArguments(argumentTypes, Conversion.Checked);
            Expression instance = args[0];
            args.RemoveAt(0);
            return Expression.Call(instance, method, args);
        }
    }
}
