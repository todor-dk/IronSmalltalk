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
    public abstract class PropertyPrimitiveEncoder : NamedMemberPrimitiveEncoder
    {
        protected PropertyPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }

        protected Expression GenerateInvokeProperty(BindingFlags bindingFlags)
        {
            // Get/Set property must have at least ONE type parameter (the return type)
            if (!this.Parameters.Any())
                throw new PrimitiveSemanticException(CodeGenerationErrors.WrongNumberOfParameters);
            Type returnType = this.GetArgumentTypes(new string[] { this.Parameters.Last() })[0];
            Type[] argumentTypes = this.GetArgumentTypes(this.Parameters.Take(this.Parameters.Count() - 1));
            return this.GenerateInvokeProperty(returnType, argumentTypes, bindingFlags);
        }

        /// <summary>
        /// Get the expression needed to perform a property get or set operation.
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="argumentTypes">Types of arguments that we are to pass to the method.</param>
        /// <param name="bindingFlags"></param>
        /// <returns>Expression for the call</returns>
        private Expression GenerateInvokeProperty(Type returnType, Type[] argumentTypes, BindingFlags bindingFlags)
        {
            // First, resolve the name of default properties
            string propertyName = this.MemberName;
            if (propertyName == "this")
                propertyName = PropertyPrimitiveEncoder.GetDefaultMemberName(this.DefiningType, typeof(PropertyInfo)) ?? propertyName;

            // Lookup the property ... matching argument types.
            PropertyInfo property = this.DefiningType.GetProperty(propertyName,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | bindingFlags,
                null, returnType, argumentTypes, null);
            // If no property found, throw an exception.
            if (property == null)
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingProperty, this.DefiningType.Name, propertyName));

            bool isStatic = property.GetAccessors()[0].IsStatic;

            if ((bindingFlags & BindingFlags.GetProperty) == BindingFlags.GetProperty)
            {
                if ((argumentTypes == null) || (argumentTypes.Length == 0))
                {
                    if (isStatic)
                        return Expression.Property(null, property);
                    IList<Expression> args = this.GetArguments(new Type[] { this.DefiningType }, Conversion.Checked);
                    return Expression.Property(args[0], property);
                }
                else
                {
                    List<Type> types = new List<Type>();
                    if (isStatic)
                        types.Add(typeof(object));
                    else
                        types.Add(this.DefiningType);
                    types.AddRange(argumentTypes);
                    IList<Expression> args = this.GetArguments(types.ToArray(), Conversion.Checked);
                    Expression instance = args[0];
                    args.RemoveAt(0);
                    if (isStatic)
                        instance = null;
                    return Expression.Property(instance, property, args);
                }
            }
            else
            {
                if ((argumentTypes == null) || (argumentTypes.Length == 0))
                {
                    if (isStatic)
                    {
                        IList<Expression> args = this.GetArguments(new Type[] { typeof(object), property.PropertyType }, Conversion.Checked);
                        return Expression.Assign(Expression.Property(null, property), args[1]);
                    }
                    else
                    {
                        IList<Expression> args = this.GetArguments(new Type[] { this.DefiningType, property.PropertyType }, Conversion.Checked);
                        return Expression.Assign(Expression.Property(args[0], property), args[1]);
                    }
                }
                else
                {
                    List<Type> types = new List<Type>();
                    if (isStatic)
                        types.Add(typeof(object));
                    else
                        types.Add(this.DefiningType);
                    types.AddRange(argumentTypes);
                    types.Add(property.PropertyType);
                    IList<Expression> args = this.GetArguments(types.ToArray(), Conversion.Checked);
                    Expression instance = args[0];
                    args.RemoveAt(0);
                    Expression value = args[args.Count - 1];
                    args.RemoveAt(args.Count - 1);
                    if (isStatic)
                        instance = null;
                    return Expression.Assign(Expression.Property(instance, property, args), value);
                }
            }
        }

        private static string GetDefaultMemberName(Type type, Type memberType)
        {
            foreach (var member in type.GetDefaultMembers())
            {
                if (memberType.IsInstanceOfType(member))
                    return member.Name;
            }
            return null;
        }
    }

    public sealed class GetPropertyPrimitiveEncoder : PropertyPrimitiveEncoder
    {
        private GetPropertyPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }

        private Expression GenerateExpression()
        {
            return this.GenerateInvokeProperty(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static);
        }

        public static Expression GeneratePrimitive(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new GetPropertyPrimitiveEncoder(context, parameters, definingType, memberName)).GenerateExpression();
        }
    }

    public sealed class SetPropertyPrimitiveEncoder : PropertyPrimitiveEncoder
    {
        private SetPropertyPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(context, parameters, definingType, memberName)
        {
        }

        private Expression GenerateExpression()
        {
            return this.GenerateInvokeProperty(BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Static);
        }

        public static Expression GeneratePrimitive(VisitingContext context, IEnumerable<string> parameters, Type definingType, string memberName)
        {
            return (new SetPropertyPrimitiveEncoder(context, parameters, definingType, memberName)).GenerateExpression();
        }
    }
}
