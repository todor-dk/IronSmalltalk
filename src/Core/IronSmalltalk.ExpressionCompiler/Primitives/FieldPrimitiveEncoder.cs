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
using System.Globalization;
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
    public abstract class FieldPrimitiveEncoder : NamedMemberPrimitiveEncoder
    {
        protected FieldPrimitiveEncoder(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(visitor, parameters, definingType, memberName)
        {
        }

        /// <summary>
        /// Get the expression needed to perform a field get or set operation.
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns>Expression for the call</returns>
        protected Expression GenerateInvokeField(BindingFlags bindingFlags)
        {
            // Get/Set Field do not have any type parameters!
            if (this.Parameters.Any())
                throw new PrimitiveSemanticException(CodeGenerationErrors.WrongNumberOfParameters);

            // Lookup the field ... matching argument types.
            FieldInfo field = this.DefiningType.GetField(this.MemberName,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | bindingFlags);
            // If no field found, throw an exception.
            if (field == null)
                throw new PrimitiveInvalidMemberException(String.Format(CultureInfo.InvariantCulture, CodeGenerationErrors.MissingField, this.DefiningType.Name, this.MemberName));

            if ((bindingFlags & BindingFlags.GetField) == BindingFlags.GetField)
            {
                if (field.IsStatic)
                {
                    // If the field is "const" or "static readonly" and it's a value type, the literal strategy may want to pre-box it.
                    if ((field.IsLiteral || field.IsInitOnly) && field.FieldType.IsValueType)
                        return this.Compiler.LiteralEncodingStrategy.GenericLiteral(this.Visitor, String.Format(CultureInfo.InvariantCulture, "{0}.{1}", field.FieldType.Name, field.Name), Expression.Field(null, field));

                    return Expression.Field(null, field);
                }
                IList<Expression> args = this.GetArguments(new Type[] { this.DefiningType }, Conversion.Checked);
                return Expression.Field(args[0], field);
            }
            else
            {
                if (field.IsStatic)
                {
                    IList<Expression> args = this.GetArguments(new Type[] { typeof(object), field.FieldType }, Conversion.Checked);
                    return Expression.Assign(Expression.Field(null, field), args[1]);
                }
                else
                {
                    IList<Expression> args = this.GetArguments(new Type[] { this.DefiningType, field.FieldType }, Conversion.Checked);
                    return Expression.Assign(Expression.Field(args[0], field), args[1]);
                }
            }
        }
    }

    public sealed class GetFieldPrimitiveEncoder : FieldPrimitiveEncoder
    {
        private GetFieldPrimitiveEncoder(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(visitor, parameters, definingType, memberName)
        {
        }

        private Expression GenerateExpression()
        {
            return this.GenerateInvokeField(BindingFlags.GetField);
        }

        public static Expression GeneratePrimitive(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new GetFieldPrimitiveEncoder(visitor, parameters, definingType, memberName)).GenerateExpression();
        }
    }

    public sealed class SetFieldPrimitiveEncoder : FieldPrimitiveEncoder
    {
        private SetFieldPrimitiveEncoder(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(visitor, parameters, definingType, memberName)
        {
        }

        private Expression GenerateExpression()
        {
            return this.GenerateInvokeField(BindingFlags.SetField);
        }

        public static Expression GeneratePrimitive(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new SetFieldPrimitiveEncoder(visitor, parameters, definingType, memberName)).GenerateExpression();
        }
    }
}
