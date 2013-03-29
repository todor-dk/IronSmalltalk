using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
{
    public interface IPrimitiveClient
    {
        ObjectClassCallSiteBinder GetClassBinder();
    }

    public static class PrimitiveBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="primitive"></param>
        /// <param name="definingType"></param>
        /// <param name="memberName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Expression GeneratePrimitive(IPrimitiveClient client, PrimitivesEnum primitive, Type definingType, string memberName, IEnumerable<string> parameters, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if ((primitive != PrimitivesEnum.InvokeConstructor) && (memberName == null))
                throw new ArgumentNullException("memberName");
            if ((primitive != PrimitivesEnum.BuiltInPrimitive) && (definingType == null))
                throw new ArgumentNullException("definingType");

            Type[] argumentTypes;
            Type returnType;
            switch (primitive)
            {
                case PrimitivesEnum.BuiltInPrimitive:
                    // For built-in primitives, the defining type is not used!
                    if (definingType != null)
                        throw new ArgumentException("PrimitivesEnum.InvokeConstructor did not expect a value for <definingType>.", "definingType");
                    BuiltInPrimitivesEnum prim;
                    if (!Enum.TryParse(memberName, out prim))
                        throw new PrimitiveSemanticException(String.Format(RuntimeCodeGenerationErrors.WrongPrimitive, memberName));
                    return PrimitiveBuilder.GenerateBuiltInPrimitive(prim, client, self, arguments, ref restrictions, parameters.ToArray());
                case PrimitivesEnum.InvokeStaticMethod:
                    argumentTypes = PrimitiveHelper.GetArgumentTypes(parameters, definingType);
                    return PrimitiveBuilder.GenerateInvokeStaticMethod(definingType, 
                            memberName, 
                            argumentTypes, 
                            BindingFlags.InvokeMethod | BindingFlags.Static, 
                            self, 
                            arguments, 
                            ref restrictions);
                case PrimitivesEnum.InvokeInstanceMethod:
                    if (!parameters.Any())
                        throw new PrimitiveSemanticException(RuntimeCodeGenerationErrors.WrongNumberOfParameters);
                    argumentTypes = PrimitiveHelper.GetArgumentTypes(parameters, definingType);
                    return PrimitiveBuilder.GenerateInvokeInstanceMethod(definingType,
                            memberName, 
                            argumentTypes,
                            BindingFlags.InvokeMethod | BindingFlags.Instance, 
                            self, 
                            arguments, 
                            ref restrictions);
                case PrimitivesEnum.InvokeConstructor:
                    if (memberName != null)
                        throw new ArgumentException("PrimitivesEnum.InvokeConstructor did not expect a value for <memberName>.", "memberName");
                    argumentTypes = PrimitiveHelper.GetArgumentTypes(parameters, definingType);
                    return PrimitiveBuilder.GenerateInvokeConstructor(definingType,
                            argumentTypes,
                            self, 
                            arguments, 
                            ref restrictions);
                case PrimitivesEnum.InvokeGetProperty:
                    // Get/Set property must have at least ONE type parameter (the return type)
                    if (!parameters.Any())
                        throw new PrimitiveSemanticException(RuntimeCodeGenerationErrors.WrongNumberOfParameters);
                    returnType = PrimitiveHelper.GetArgumentTypes(new string[] { parameters.Last() }, definingType)[0];
                    argumentTypes = PrimitiveHelper.GetArgumentTypes(parameters.Take(parameters.Count() -1), definingType);
                    return PrimitiveBuilder.GenerateInvokeProperty(definingType,
                            memberName,
                            returnType,
                            argumentTypes,
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Static,
                            self, 
                            arguments, 
                            ref restrictions);
                case PrimitivesEnum.InvokeSetProperty:
                    // Get/Set property must have at least ONE type parameter (the return type)
                    if (!parameters.Any())
                        throw new PrimitiveSemanticException(RuntimeCodeGenerationErrors.WrongNumberOfParameters);
                    returnType = PrimitiveHelper.GetArgumentTypes(new string[] { parameters.Last() }, definingType)[0];
                    argumentTypes = PrimitiveHelper.GetArgumentTypes(parameters.Take(parameters.Count() -1), definingType);
                    return PrimitiveBuilder.GenerateInvokeProperty(definingType,
                            memberName,
                            returnType,
                            argumentTypes,
                            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Static,
                            self, 
                            arguments, 
                            ref restrictions);
                case PrimitivesEnum.GetFieldValue:
                    // Get/Set Field do not have any type parameters!
                    if (parameters.Any())
                        throw new PrimitiveSemanticException(RuntimeCodeGenerationErrors.WrongNumberOfParameters);
                    return PrimitiveBuilder.GenerateInvokeField(definingType,
                            memberName,
                            BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static,
                            self,
                            arguments,
                            ref restrictions);
                case PrimitivesEnum.SetFieldValue:
                    // Get/Set Field do not have any type parameters!
                    if (parameters.Any())
                        throw new PrimitiveSemanticException(RuntimeCodeGenerationErrors.WrongNumberOfParameters);
                    return PrimitiveBuilder.GenerateInvokeField(definingType,
                            memberName,
                            BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Static,
                            self,
                            arguments,
                            ref restrictions);
                default:
                    break;
            }

            return null;
        }


        /// <summary>
        /// Get the expression needed to perform a static method call.
        /// </summary>
        /// <param name="type">Type that is expected to define the method we are looking for.</param>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="argumentTypes">Types of arguments that we are to pass to the method.</param>
        /// <returns>Expression for the call</returns>
        public static Expression GenerateInvokeStaticMethod(Type type, string methodName, Type[] argumentTypes, BindingFlags bindingFlags, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            // Lookup the method ... matching argument types.
            MethodInfo method = type.GetMethod(methodName, BindingFlags.FlattenHierarchy | BindingFlags.Public | bindingFlags,
                null, argumentTypes.ToArray(), null);
            // If no method found, throw an exception.
            if (method == null)
                throw new PrimitiveInvalidMemberException(String.Format(RuntimeCodeGenerationErrors.MissingMethod, type.Name, methodName));
            return Expression.Call(method, PrimitiveHelper.GetArguments(self, arguments,argumentTypes, false, true, ref restrictions));
        }

        /// <summary>
        /// Get the expression needed to perform an instance method call.
        /// </summary>
        /// <param name="type">Type that is expected to define the method we are looking for.</param>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="argumentTypes">Types of arguments that we are to pass to the method.</param>
        /// <returns>Expression for the call</returns>
        public static Expression GenerateInvokeInstanceMethod(Type type, string methodName, Type[] argumentTypes, BindingFlags bindingFlags, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            Type[] matchTypes = new Type[argumentTypes.Length - 1];
            Array.Copy(argumentTypes, 1, matchTypes, 0, matchTypes.Length);
            // Lookup the method ... matching argument types.
            MethodInfo method = type.GetMethod(methodName, BindingFlags.FlattenHierarchy | BindingFlags.Public | bindingFlags,
                null, matchTypes, null);
            // If no method found, throw an exception.
            if (method == null)
                throw new PrimitiveInvalidMemberException(String.Format(RuntimeCodeGenerationErrors.MissingMethod, type.Name, methodName));
            IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, argumentTypes, false, true, ref restrictions);
            Expression instance = args[0];
            args.RemoveAt(0);
            return Expression.Call(instance, method, args);
        }

        /// <summary>
        /// Get the expression needed to perform a constructor call.
        /// </summary>
        /// <param name="type">Type that is expected to define the method we are looking for.</param>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="argumentTypes">Types of arguments that we are to pass to the method.</param>
        /// <returns>Expression for the call</returns>
        public static Expression GenerateInvokeConstructor(Type type, Type[] argumentTypes, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            // Lookup the constructor ... matching argument types.
            ConstructorInfo ctor = type.GetConstructor(
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
                null, argumentTypes, null);
            // If no constructor found, throw an exception.
            if (ctor == null)
                throw new PrimitiveInvalidMemberException(String.Format(RuntimeCodeGenerationErrors.MissingConstructor, type.Name));
            return Expression.New(ctor, PrimitiveHelper.GetArguments(self, arguments, argumentTypes, false, true, ref restrictions));
        }

        /// <summary>
        /// Get the expression needed to perform a property get or set operation.
        /// </summary>
        /// <param name="type">Type that is expected to define the method we are looking for.</param>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="argumentTypes">Types of arguments that we are to pass to the method.</param>
        /// <returns>Expression for the call</returns>
        public static Expression GenerateInvokeProperty(Type type, string propertyName, Type returnType, Type[] argumentTypes, BindingFlags bindingFlags, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            // First, resolve the name of default properties
            if (propertyName == "this")
                propertyName = PrimitiveHelper.GetDefaultMemberName(type, typeof(PropertyInfo)) ?? propertyName;
            // Lookup the property ... matching argument types.
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.FlattenHierarchy | BindingFlags.Public | bindingFlags,
                    null, returnType, argumentTypes, null);
            // If no property found, throw an exception.
            if (property == null)
                throw new PrimitiveInvalidMemberException(String.Format(RuntimeCodeGenerationErrors.MissingProperty, type.Name, propertyName));

            bool isStatic = property.GetAccessors()[0].IsStatic;

            if ((bindingFlags & BindingFlags.GetProperty) == BindingFlags.GetProperty)
            {
                if ((argumentTypes == null) || (argumentTypes.Length == 0))
                {
                    if (isStatic)
                        return Expression.Property(null, property);
                    IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, new Type[] { type }, false, true, ref restrictions);
                    return Expression.Property(args[0], property);
                }
                else
                {
                    List<Type> types = new List<Type>();
                    if (isStatic)
                        types.Add(typeof(object));
                    else
                        types.Add(type);
                    types.AddRange(argumentTypes);
                    IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, types.ToArray(), false, true, ref restrictions);
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
                        IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, new Type[] { typeof(object), property.PropertyType }, false, true, ref restrictions);
                        return Expression.Assign(Expression.Property(null, property), args[1]);
                    }
                    else
                    {
                        IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, new Type[] { type, property.PropertyType }, false, true, ref restrictions);
                        return Expression.Assign(Expression.Property(args[0], property), args[1]);
                    }
                }
                else
                {
                    List<Type> types = new List<Type>();
                    if (isStatic)
                        types.Add(typeof(object));
                    else
                        types.Add(type);
                    types.AddRange(argumentTypes);
                    types.Add(property.PropertyType);
                    IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, types.ToArray(), false, true, ref restrictions);
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
        /// <param name="type">Type that is expected to define the method we are looking for.</param>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="argumentTypes">Types of arguments that we are to pass to the method.</param>
        /// <returns>Expression for the call</returns>
        public static Expression GenerateInvokeField(Type type, string fieldName, BindingFlags bindingFlags, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions)
        {
            // Lookup the field ... matching argument types.
            FieldInfo field = type.GetField(fieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | bindingFlags);
            // If no field found, throw an exception.
            if (field == null)
                throw new PrimitiveInvalidMemberException(String.Format(RuntimeCodeGenerationErrors.MissingField, type.Name, fieldName));

            if ((bindingFlags & BindingFlags.GetField) == BindingFlags.GetField)
            {
                if (field.IsStatic)
                    return Expression.Field(null, field);
                IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, new Type[] { type }, false, true, ref restrictions);
                return Expression.Field(args[0], field);
            }
            else
            {
                if (field.IsStatic)
                {
                    IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, new Type[] { typeof(object), field.FieldType }, false, true, ref restrictions);
                    return Expression.Assign(Expression.Field(null, field), args[1]);
                }
                else
                {
                    IList<Expression> args = PrimitiveHelper.GetArguments(self, arguments, new Type[] { type, field.FieldType }, false, true, ref restrictions);
                    return Expression.Assign(Expression.Field(args[0], field), args[1]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="client"></param>
        /// <param name="self"></param>
        /// <param name="arguments"></param>
        /// <param name="restrictions"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function MUST BE updated if changes are made to the BuiltInPrimitivesEnum enumeration!
        /// </remarks>
        public static Expression GenerateBuiltInPrimitive(BuiltInPrimitivesEnum primitive, IPrimitiveClient client, DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (client == null)
                throw new ArgumentNullException("client");
            Expression exp;
            switch (primitive)
            {
                // **** Very Common ****
                case BuiltInPrimitivesEnum.Equals:
                    exp = BuiltInPrimitives.Equals(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IdentityEquals:
                    exp = BuiltInPrimitives.IdentityEquals(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.ObjectClass:
                    exp = BuiltInPrimitives.Class(self, arguments, ref restrictions, parameters, client.GetClassBinder());
                    break;
                case BuiltInPrimitivesEnum.ConvertChecked:
                    exp = BuiltInPrimitives.ConvertChecked(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.ConvertUnchecked:
                    exp = BuiltInPrimitives.ConvertUnchecked(self, arguments, ref restrictions, parameters);
                    break;
                // **** Numeric Operations ****
                // ISO/IEC 10967 Integer Operations
                case BuiltInPrimitivesEnum.IntegerEquals:
                    exp = BuiltInPrimitives.IntegerEquals(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerLessThan:
                    exp = BuiltInPrimitives.IntegerLessThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerLessThanOrEqual:
                    exp = BuiltInPrimitives.IntegerLessThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerGreatherThan:
                    exp = BuiltInPrimitives.IntegerGreatherThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerGreatherThanOrEqual:
                    exp = BuiltInPrimitives.IntegerGreatherThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerAdd:
                    exp = BuiltInPrimitives.IntegerAdd(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerSubtract:
                    exp = BuiltInPrimitives.IntegerSubtract(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerMultiply:
                    exp = BuiltInPrimitives.IntegerMultiply(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerDivideTruncate:
                    exp = BuiltInPrimitives.IntegerDivideTruncate(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerDivideFloor:
                    exp = BuiltInPrimitives.IntegerDivideFloor(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerRemainderTruncate:
                    exp = BuiltInPrimitives.IntegerRemainderTruncate(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerRemainderFloor:
                    exp = BuiltInPrimitives.IntegerRemainderFloor(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerNegate:
                    exp = BuiltInPrimitives.IntegerNegate(self, arguments, ref restrictions, parameters);
                    break;
                // Integer operations .... not part of ISO/IEC 10967
                case BuiltInPrimitivesEnum.IntegerBitShift:
                    exp = BuiltInPrimitives.IntegerBitShift(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerBitAnd:
                    exp = BuiltInPrimitives.IntegerBitAnd(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerBitOr:
                    exp = BuiltInPrimitives.IntegerBitOr(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.IntegerBitXor:
                    exp = BuiltInPrimitives.IntegerBitXor(self, arguments, ref restrictions, parameters);
                    break;
                // ISO/IEC 10967 Float Operations
                case BuiltInPrimitivesEnum.FloatEquals:
                    exp = BuiltInPrimitives.FloatEquals(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatLessThan:
                    exp = BuiltInPrimitives.FloatLessThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatLessThanOrEqual:
                    exp = BuiltInPrimitives.FloatLessThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatGreatherThan:
                    exp = BuiltInPrimitives.FloatGreatherThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatGreatherThanOrEqual:
                    exp = BuiltInPrimitives.FloatGreatherThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatAdd:
                    exp = BuiltInPrimitives.FloatAdd(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatSubtract:
                    exp = BuiltInPrimitives.FloatSubtract(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatMultiply:
                    exp = BuiltInPrimitives.FloatMultiply(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatDivide:
                    exp = BuiltInPrimitives.FloatDivide(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.FloatNegate:
                    exp = BuiltInPrimitives.FloatNegate(self, arguments, ref restrictions, parameters);
                    break;
                // ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.
                case BuiltInPrimitivesEnum.NumberEquals:
                    exp = BuiltInPrimitives.NumberEquals(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.NumberLessThan:
                    exp = BuiltInPrimitives.NumberLessThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.NumberLessThanOrEqual:
                    exp = BuiltInPrimitives.NumberLessThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.NumberGreatherThan:
                    exp = BuiltInPrimitives.NumberGreatherThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.NumberGreatherThanOrEqual:
                    exp = BuiltInPrimitives.NumberGreatherThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.NumberAdd:
                    exp = BuiltInPrimitives.NumberAdd(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.NumberSubtract:
                    exp = BuiltInPrimitives.NumberSubtract(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.NumberNegate:
                    exp = BuiltInPrimitives.NumberNegate(self, arguments, ref restrictions, parameters);
                    break;
                // Decimal operations .... not part of ISO/IEC 10967 but currently we assume same semantics
                case BuiltInPrimitivesEnum.DecimalMultiply:
                    exp = BuiltInPrimitives.DecimalMultiply(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.DecimalDivide:
                    exp = BuiltInPrimitives.DecimalDivide(self, arguments, ref restrictions, parameters);
                    break;
                // **** Generic Operations ****
                //  NB: Don't use those for numeric types. It's not a technical error but difficult to maintain if we find a bug or need to change something.
                case BuiltInPrimitivesEnum.LessThan:
                    exp = BuiltInPrimitives.LessThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.LessThanOrEqual:
                    exp = BuiltInPrimitives.LessThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.GreatherThan:
                    exp = BuiltInPrimitives.GreatherThan(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.GreatherThanOrEqual:
                    exp = BuiltInPrimitives.GreatherThanOrEqual(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.Add:
                    exp = BuiltInPrimitives.Add(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.Subtract:
                    exp = BuiltInPrimitives.Subtract(self, arguments, ref restrictions, parameters);
                    break;
                case BuiltInPrimitivesEnum.Negate:
                    exp = BuiltInPrimitives.Negate(self, arguments, ref restrictions, parameters);
                    break;
                default:
                    throw new NotImplementedException(String.Format("Primitive {0} is not implemented. This is a bug!", primitive));
            }
            return exp;
        }
    }
}
