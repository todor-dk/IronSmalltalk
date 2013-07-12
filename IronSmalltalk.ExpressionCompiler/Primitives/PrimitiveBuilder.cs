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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    public class PrimitiveBuilder
    {
        public static Expression GeneratePrimitive(IPrimitiveClient client, PrimitivesEnum primitive, Type definingType, string memberName, IEnumerable<string> parameters, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            PrimitiveBuilder builder = new PrimitiveBuilder(client, primitive, definingType, memberName, parameters, self, arguments, restrictions);
            Expression result = builder.GeneratePrimitive();
            restrictions = builder.Restrictions;
            return result;
        }

        public IPrimitiveClient Client { get; private set; }
        public PrimitivesEnum Primitive { get; private set; }
        public Type DefiningType { get; private set; }
        public string MemberName { get; private set; }
        public IReadOnlyList<string> Parameters { get; private set; }
        public DynamicMetaObject Self { get; private set; }
        public DynamicMetaObject[] Arguments { get; private set; }
        public BindingRestrictions Restrictions { get; internal set; }

        public PrimitiveBuilder(IPrimitiveClient client, PrimitivesEnum primitive, Type definingType, string memberName,
            IEnumerable<string> parameters, DynamicMetaObject self, DynamicMetaObject[] arguments, BindingRestrictions restrictions)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if ((primitive != PrimitivesEnum.InvokeConstructor) && (memberName == null))
                throw new ArgumentNullException("memberName");
            // Constructors do not use the member name.
            if ((primitive == PrimitivesEnum.InvokeConstructor) && (memberName != null))
                throw new ArgumentException("PrimitivesEnum.InvokeConstructor did not expect a value for <memberName>.", "memberName");
            if ((primitive != PrimitivesEnum.BuiltInPrimitive) && (definingType == null))
                throw new ArgumentNullException("definingType");
            // For built-in primitives, the defining type is not used!
            if ((primitive == PrimitivesEnum.BuiltInPrimitive) && (definingType != null))
                throw new ArgumentException("PrimitivesEnum.BuiltInPrimitive did not expect a value for <definingType>.", "definingType");

            this.Client = client;
            this.Primitive = primitive;
            this.DefiningType = definingType;
            this.MemberName = memberName;
            this.Parameters = parameters.ToList().AsReadOnly();
            this.Self = self;
            this.Arguments = arguments;
            this.Restrictions = restrictions;
        }

        private Expression GeneratePrimitive()
        {
            switch (this.Primitive)
            {
                case PrimitivesEnum.BuiltInPrimitive:
                    return this.GenerateBuiltInPrimitive();
                case PrimitivesEnum.InvokeStaticMethod:
                    return this.GenerateInvokeStaticMethod();
                case PrimitivesEnum.InvokeInstanceMethod:
                    return this.GenerateInvokeInstanceMethod();
                case PrimitivesEnum.InvokeConstructor:
                    return this.GenerateInvokeConstructor();
                case PrimitivesEnum.InvokeGetProperty:
                    return this.GenerateInvokePropertyGet();
                case PrimitivesEnum.InvokeSetProperty:
                    return this.GenerateInvokePropertySet();
                case PrimitivesEnum.GetFieldValue:
                    return this.GenerateInvokeField(BindingFlags.GetField);
                case PrimitivesEnum.SetFieldValue:
                    return this.GenerateInvokeField(BindingFlags.SetField);
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This function MUST BE updated if changes are made to the BuiltInPrimitivesEnum enumeration!
        /// </remarks>
        private Expression GenerateBuiltInPrimitive()
        {
            // For built-in primitives, the defining type is not used!
            BuiltInPrimitivesEnum primitive;
            if (!Enum.TryParse(this.MemberName, out primitive))
                throw new PrimitiveSemanticException(String.Format(CodeGenerationErrors.WrongPrimitive, this.MemberName));

            Expression exp;
            switch (primitive)
            {
                // **** Very Common ****
                case BuiltInPrimitivesEnum.Equals:
                    exp = BuiltInPrimitives.Equals(this);
                    break;
                case BuiltInPrimitivesEnum.IdentityEquals:
                    exp = BuiltInPrimitives.IdentityEquals(this);
                    break;
                case BuiltInPrimitivesEnum.ObjectClass:
                    exp = BuiltInPrimitives.Class(this, this.Client.GetClassBinder());
                    break;
                case BuiltInPrimitivesEnum.ConvertChecked:
                    exp = BuiltInPrimitives.ConvertChecked(this);
                    break;
                case BuiltInPrimitivesEnum.ConvertUnchecked:
                    exp = BuiltInPrimitives.ConvertUnchecked(this);
                    break;
                // **** Numeric Operations ****
                // ISO/IEC 10967 Integer Operations
                case BuiltInPrimitivesEnum.IntegerEquals:
                    exp = BuiltInPrimitives.IntegerEquals(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerLessThan:
                    exp = BuiltInPrimitives.IntegerLessThan(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerLessThanOrEqual:
                    exp = BuiltInPrimitives.IntegerLessThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerGreatherThan:
                    exp = BuiltInPrimitives.IntegerGreatherThan(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerGreatherThanOrEqual:
                    exp = BuiltInPrimitives.IntegerGreatherThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerAdd:
                    exp = BuiltInPrimitives.IntegerAdd(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerSubtract:
                    exp = BuiltInPrimitives.IntegerSubtract(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerMultiply:
                    exp = BuiltInPrimitives.IntegerMultiply(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerDivideTruncate:
                    exp = BuiltInPrimitives.IntegerDivideTruncate(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerDivideFloor:
                    exp = BuiltInPrimitives.IntegerDivideFloor(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerRemainderTruncate:
                    exp = BuiltInPrimitives.IntegerRemainderTruncate(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerRemainderFloor:
                    exp = BuiltInPrimitives.IntegerRemainderFloor(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerNegate:
                    exp = BuiltInPrimitives.IntegerNegate(this);
                    break;
                // Integer operations .... not part of ISO/IEC 10967
                case BuiltInPrimitivesEnum.IntegerBitShift:
                    exp = BuiltInPrimitives.IntegerBitShift(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerBitAnd:
                    exp = BuiltInPrimitives.IntegerBitAnd(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerBitOr:
                    exp = BuiltInPrimitives.IntegerBitOr(this);
                    break;
                case BuiltInPrimitivesEnum.IntegerBitXor:
                    exp = BuiltInPrimitives.IntegerBitXor(this);
                    break;
                // ISO/IEC 10967 Float Operations
                case BuiltInPrimitivesEnum.FloatEquals:
                    exp = BuiltInPrimitives.FloatEquals(this);
                    break;
                case BuiltInPrimitivesEnum.FloatLessThan:
                    exp = BuiltInPrimitives.FloatLessThan(this);
                    break;
                case BuiltInPrimitivesEnum.FloatLessThanOrEqual:
                    exp = BuiltInPrimitives.FloatLessThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.FloatGreatherThan:
                    exp = BuiltInPrimitives.FloatGreatherThan(this);
                    break;
                case BuiltInPrimitivesEnum.FloatGreatherThanOrEqual:
                    exp = BuiltInPrimitives.FloatGreatherThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.FloatAdd:
                    exp = BuiltInPrimitives.FloatAdd(this);
                    break;
                case BuiltInPrimitivesEnum.FloatSubtract:
                    exp = BuiltInPrimitives.FloatSubtract(this);
                    break;
                case BuiltInPrimitivesEnum.FloatMultiply:
                    exp = BuiltInPrimitives.FloatMultiply(this);
                    break;
                case BuiltInPrimitivesEnum.FloatDivide:
                    exp = BuiltInPrimitives.FloatDivide(this);
                    break;
                case BuiltInPrimitivesEnum.FloatNegate:
                    exp = BuiltInPrimitives.FloatNegate(this);
                    break;
                // ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.
                case BuiltInPrimitivesEnum.NumberEquals:
                    exp = BuiltInPrimitives.NumberEquals(this);
                    break;
                case BuiltInPrimitivesEnum.NumberLessThan:
                    exp = BuiltInPrimitives.NumberLessThan(this);
                    break;
                case BuiltInPrimitivesEnum.NumberLessThanOrEqual:
                    exp = BuiltInPrimitives.NumberLessThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.NumberGreatherThan:
                    exp = BuiltInPrimitives.NumberGreatherThan(this);
                    break;
                case BuiltInPrimitivesEnum.NumberGreatherThanOrEqual:
                    exp = BuiltInPrimitives.NumberGreatherThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.NumberAdd:
                    exp = BuiltInPrimitives.NumberAdd(this);
                    break;
                case BuiltInPrimitivesEnum.NumberSubtract:
                    exp = BuiltInPrimitives.NumberSubtract(this);
                    break;
                case BuiltInPrimitivesEnum.NumberNegate:
                    exp = BuiltInPrimitives.NumberNegate(this);
                    break;
                // Decimal operations .... not part of ISO/IEC 10967 but currently we assume same semantics
                case BuiltInPrimitivesEnum.DecimalMultiply:
                    exp = BuiltInPrimitives.DecimalMultiply(this);
                    break;
                case BuiltInPrimitivesEnum.DecimalDivide:
                    exp = BuiltInPrimitives.DecimalDivide(this);
                    break;
                // **** Generic Operations ****
                //  NB: Don't use those for numeric types. It's not a technical error but difficult to maintain if we find a bug or need to change something.
                case BuiltInPrimitivesEnum.LessThan:
                    exp = BuiltInPrimitives.LessThan(this);
                    break;
                case BuiltInPrimitivesEnum.LessThanOrEqual:
                    exp = BuiltInPrimitives.LessThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.GreatherThan:
                    exp = BuiltInPrimitives.GreatherThan(this);
                    break;
                case BuiltInPrimitivesEnum.GreatherThanOrEqual:
                    exp = BuiltInPrimitives.GreatherThanOrEqual(this);
                    break;
                case BuiltInPrimitivesEnum.Add:
                    exp = BuiltInPrimitives.Add(this);
                    break;
                case BuiltInPrimitivesEnum.Subtract:
                    exp = BuiltInPrimitives.Subtract(this);
                    break;
                case BuiltInPrimitivesEnum.Negate:
                    exp = BuiltInPrimitives.Negate(this);
                    break;
                default:
                    throw new NotImplementedException(String.Format("Primitive {0} is not implemented. This is a bug!", primitive));
            }
            return exp;
        }

        /// <summary>
        /// Get the expression needed to perform a static method call.
        /// </summary>
        /// <returns>Expression for the call</returns>
        private Expression GenerateInvokeStaticMethod()
        {
            Type[] argumentTypes = PrimitiveHelper.GetArgumentTypes(this.Parameters, this.DefiningType); // Types of arguments that we are to pass to the method.

            // Lookup the method ... matching argument types.
            MethodInfo method = this.DefiningType.GetMethod(this.MemberName,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static,
                null, argumentTypes.ToArray(), null);
            // If no method found, throw an exception.
            if (method == null)
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingMethod, this.DefiningType.Name, this.MemberName));
            return Expression.Call(method, PrimitiveHelper.GetArguments(this, argumentTypes, Conversion.Checked));
        }

        /// <summary>
        /// Get the expression needed to perform an instance method call.
        /// </summary>
        /// <returns>Expression for the call</returns>
        private Expression GenerateInvokeInstanceMethod()
        {
            if (!this.Parameters.Any())
                throw new PrimitiveSemanticException(CodeGenerationErrors.WrongNumberOfParameters);
            Type[] argumentTypes = PrimitiveHelper.GetArgumentTypes(this.Parameters, this.DefiningType); // Types of arguments that we are to pass to the method.

            Type[] matchTypes = new Type[argumentTypes.Length - 1];
            Array.Copy(argumentTypes, 1, matchTypes, 0, matchTypes.Length);
            // Lookup the method ... matching argument types.
            MethodInfo method = this.DefiningType.GetMethod(this.MemberName,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                null, matchTypes, null);
            // If no method found, throw an exception.
            if (method == null)
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingMethod, this.DefiningType.Name, this.MemberName));
            IList<Expression> args = PrimitiveHelper.GetArguments(this, argumentTypes, Conversion.Checked);
            Expression instance = args[0];
            args.RemoveAt(0);
            return Expression.Call(instance, method, args);
        }

        /// <summary>
        /// Get the expression needed to perform a constructor call.
        /// </summary>
        /// <returns>Expression for the call</returns>
        private Expression GenerateInvokeConstructor()
        {
            Type[] argumentTypes = PrimitiveHelper.GetArgumentTypes(this.Parameters, this.DefiningType); // Types of arguments that we are to pass to the method.

            // Lookup the constructor ... matching argument types.
            ConstructorInfo ctor = this.DefiningType.GetConstructor(
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
                null, argumentTypes, null);
            // If no constructor found, throw an exception.
            if (ctor == null)
                throw new PrimitiveInvalidMemberException(String.Format(CodeGenerationErrors.MissingConstructor, this.DefiningType.Name));
            return Expression.New(ctor, PrimitiveHelper.GetArguments(this, argumentTypes, Conversion.Checked));
        }

        private Expression GenerateInvokePropertyGet()
        {
            // Get/Set property must have at least ONE type parameter (the return type)
            if (!this.Parameters.Any())
                throw new PrimitiveSemanticException(CodeGenerationErrors.WrongNumberOfParameters);
            Type returnType = PrimitiveHelper.GetArgumentTypes(new string[] { this.Parameters.Last() }, this.DefiningType)[0];
            Type[] argumentTypes = PrimitiveHelper.GetArgumentTypes(this.Parameters.Take(this.Parameters.Count() - 1), this.DefiningType);
            return this.GenerateInvokeProperty(returnType, argumentTypes, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static);
        }

        private Expression GenerateInvokePropertySet()
        {
            // Get/Set property must have at least ONE type parameter (the return type)
            if (!this.Parameters.Any())
                throw new PrimitiveSemanticException(CodeGenerationErrors.WrongNumberOfParameters);
            Type returnType = PrimitiveHelper.GetArgumentTypes(new string[] { this.Parameters.Last() }, this.DefiningType)[0];
            Type[] argumentTypes = PrimitiveHelper.GetArgumentTypes(this.Parameters.Take(this.Parameters.Count() - 1), this.DefiningType);
            return this.GenerateInvokeProperty(returnType, argumentTypes, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Static);
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
                propertyName = PrimitiveHelper.GetDefaultMemberName(this.DefiningType, typeof(PropertyInfo)) ?? propertyName;

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
                    IList<Expression> args = PrimitiveHelper.GetArguments(this, new Type[] { this.DefiningType }, Conversion.Checked);
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
                    IList<Expression> args = PrimitiveHelper.GetArguments(this, types.ToArray(), Conversion.Checked);
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
                        IList<Expression> args = PrimitiveHelper.GetArguments(this, new Type[] { typeof(object), property.PropertyType }, Conversion.Checked);
                        return Expression.Assign(Expression.Property(null, property), args[1]);
                    }
                    else
                    {
                        IList<Expression> args = PrimitiveHelper.GetArguments(this, new Type[] { this.DefiningType, property.PropertyType }, Conversion.Checked);
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
                    IList<Expression> args = PrimitiveHelper.GetArguments(this, types.ToArray(), Conversion.Checked);
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

        /// <summary>
        /// Get the expression needed to perform a field get or set operation.
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns>Expression for the call</returns>
        private Expression GenerateInvokeField(BindingFlags bindingFlags)
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
                IList<Expression> args = PrimitiveHelper.GetArguments(this, new Type[] { this.DefiningType }, Conversion.Checked);
                return Expression.Field(args[0], field);
            }
            else
            {
                if (field.IsStatic)
                {
                    IList<Expression> args = PrimitiveHelper.GetArguments(this, new Type[] { typeof(object), field.FieldType }, Conversion.Checked);
                    return Expression.Assign(Expression.Field(null, field), args[1]);
                }
                else
                {
                    IList<Expression> args = PrimitiveHelper.GetArguments(this, new Type[] { this.DefiningType, field.FieldType }, Conversion.Checked);
                    return Expression.Assign(Expression.Field(args[0], field), args[1]);
                }
            }
        }
    }
}
