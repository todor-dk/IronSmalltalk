﻿using System;
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
    public abstract class FieldPrimitiveEncoder : NamedMemberPrimitiveEncoder
    {
        protected FieldPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
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
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingField, this.DefiningType.Name, this.MemberName));

            if ((bindingFlags & BindingFlags.GetField) == BindingFlags.GetField)
            {
                if (field.IsStatic)
                    return Expression.Field(null, field);
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
        private GetFieldPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }

        private Expression GenerateExpression()
        {
            return this.GenerateInvokeField(BindingFlags.GetField);
        }

        public static Expression GeneratePrimitive(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new GetFieldPrimitiveEncoder(context, parameters, definingType, memberName)).GenerateExpression();
        }
    }

    public sealed class SetFieldPrimitiveEncoder : FieldPrimitiveEncoder
    {
        private SetFieldPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }

        private Expression GenerateExpression()
        {
            return this.GenerateInvokeField(BindingFlags.SetField);
        }

        public static Expression GeneratePrimitive(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new SetFieldPrimitiveEncoder(context, parameters, definingType, memberName)).GenerateExpression();
        }
    }
}