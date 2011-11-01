using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
{
    /// <summary>
    /// This helper class contains helper function for each (all if possible, otherwise as may as possible)
    /// of the primitives defined in BuiltInPrimitivesEnum. The helper functions are the actual implementation
    /// of the primitive and therefore contain the logic for that primitive.
    /// </summary>
    /// <remarks>
    /// If there is a bug in built-in primitive, this is the place we should look.
    /// Some primitive delegate the logic for some operations to BuiltInPrimitiveHelper.
    /// </remarks>
    public static class BuiltInPrimitives
    {
        #region **** Very Common ****

        /// <summary>
        /// Generates an equality test expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression Equals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates an identity (reference equals) test expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For System.Boolean values, the test is equality, because Smalltalk assumes that true and false are singletons.</remarks>
        public static Expression IdentityEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => BuiltInPrimitiveHelper.EncodeReferenceEquals(arg1, arg2, Expression.Constant(true), Expression.Constant(false)));
        }

        /// <summary>
        /// Generates an expression that returns the SmalltalkClass instance that an object/value belongs to.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 0 elements.</param>
        /// <param name="binder">Required. An ObjectClassCallSiteBinder that performs the dynamic operation binding.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 0 elements.</returns>
        public static Expression Class(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters, ObjectClassCallSiteBinder binder)
        {
            if (binder == null)
                throw new ArgumentNullException("binder");

            if (parameters == null)
                return null;
            if (parameters.Count != 0)
                return null;
            if (restrictions == null)
                restrictions = BindingRestrictions.Empty;
            if (self.Restrictions != null)
                restrictions = restrictions.Merge(self.Restrictions);
            return Expression.Dynamic(binder, typeof(Object), self.Expression);
        }

        /// <summary>
        /// Generates an expression that converts the type (cast) with overflow check.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 1 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>If overflow occurs, the expression throws an OverflowException.</remarks>
        public static Expression ConvertChecked(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters, true, true, (arg, na) => arg);
        }

        /// <summary>
        /// Generates an expression that converts the type (cast) without overflow check.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 1 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 1 elements.</returns>
        public static Expression ConvertUnchecked(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters, true, false, (arg, na) => arg);
        }
        #endregion

        #region **** Numeric Operations ****
        #region ISO/IEC 10967 Integer Operations
        /// <summary>
        /// Generates an Interger Equals (ISO/IEC 10967 integer operation eqI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same integer type. When comparing integers and non-integers, 
        /// the expression throws InvalidCastException tells Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        public static Expression IntegerEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Less-Than (ISO/IEC 10967 integer operation lssI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerLessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Less-Than-Or-Equal (ISO/IEC 10967 integer operation leqI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerLessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Greather-Than (ISO/IEC 10967 integer operation gtrI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerGreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Greather-Than-Or-Equal (ISO/IEC 10967 integer operation geqI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerGreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Addition (ISO/IEC 10967 integer operation addI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerAdd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Subtraction (ISO/IEC 10967 integer operation subI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerSubtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Multiplication (ISO/IEC 10967 integer operation mulI) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerMultiply(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Division (ISO/IEC 10967 integer operation divI-t) with truncation towards zero expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerDivideTruncate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.DivideIntT(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Division (ISO/IEC 10967 integer operation divI-f) with flooring truncation towards negative infinity expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerDivideFloor(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.DivideIntF(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Remainder (ISO/IEC 10967 operation remI-t) with truncation towards zero expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerRemainderTruncate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.ReminderIntT(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Remainder (ISO/IEC 10967 operation remI-f) with flooring truncation towards negative infinity expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerRemainderFloor(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.ReminderIntF(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Negation (ISO/IEC 10967 operation negI) operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 1 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerNegate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region Integer operations .... not part of ISO/IEC 10967

        /// <summary>
        /// Generates an Interger Bit-Shift operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// For bounded values, the expression throws an OverflowException on overflow.
        /// 
        /// The current implementation <see cref="BuiltInPrimitiveHelper.Shift"/> only supports the following types:
        ///     Value:
        ///         - System.Numerics.BigInteger
        ///         - System.Int64
        ///         - System.Int32
        ///         - System.Int16
        ///         - System.SByte
        ///         - System.UInt64
        ///         - System.UInt32
        ///         - System.UInt16
        ///         - System.Byte
        ///     Shift:
        ///         - System.Int32
        /// </remarks>
        public static Expression IntegerBitShift(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.Shift(self, arguments, ref restrictions, parameters);
        }

        /// <summary>
        /// Generates an Interger Bit-And operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerBitAnd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.And(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Bit-Or operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerBitOr(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Or(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Bit-Xor operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerBitXor(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.ExclusiveOr(arg1, arg2));
        }
        #endregion

        #region ISO/IEC 10967 Float Operations

        /// <summary>
        /// Generates a Float Equals (ISO/IEC 10967 integer operation eqF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same float type. When comparing floats and non-floats, 
        /// the expression throws InvalidCastException telling Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        public static Expression FloatEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Less-Than (ISO/IEC 10967 float operation lssF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatLessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Less-Than-Or-Equal (ISO/IEC 10967 float operation leqF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatLessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Greather-Than (ISO/IEC 10967 float operation gtrF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatGreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Greather-Than-Or-Equal (ISO/IEC 10967 float operation geqF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatGreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Addition (ISO/IEC 10967 float operation addF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        public static Expression FloatAdd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Subtraction (ISO/IEC 10967 float operation subF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        public static Expression FloatSubtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Multiplication (ISO/IEC 10967 float operation mulF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        public static Expression FloatMultiply(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Division (ISO/IEC 10967 float operation divF) expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression FloatDivide(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            // BUGBUG: Do we throw an OverflowException?
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Divide(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Negation (ISO/IEC 10967 float negF) operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 1 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 1 elements.</returns>
        public static Expression FloatNegate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.

        /// <summary>
        /// Generates a generic number Equals operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same numeric type. When comparing numeric and non-numeric, 
        /// the expression throws InvalidCastException telling Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        public static Expression NumberEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Less-Than comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberLessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Less-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberLessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Greather-Than comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberGreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Greather-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberGreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Addition operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression NumberAdd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Subtraction operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression NumberSubtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Negation operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression NumberNegate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region Decimal operations .... not part of ISO/IEC 10967 but currently we assume same semantics

        /// <summary>
        /// Generates a Decimal Multiplication operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression DecimalMultiply(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Decimal Division operation with truncation towards zero expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression DecimalDivide(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Divide(arg1, arg2));
        }
        #endregion
        #endregion

        #region **** Generic Operations ****
        //  NB: Don't use those for numeric types. It's not a technical error but difficult to maintain if we find a bug or need to change something.

        /// <summary>
        /// Generates a generic (non-numeric) object Less-Than comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression LessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Less-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression LessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Greather-Than comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression GreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Greather-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        public static Expression GreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Addition operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression Add(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Subtraction operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 2 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression Subtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Negation operation expression.
        /// </summary>
        /// <param name="self">Required. DynamicMetaObject representing the 'self' (the first) argument for the operation.</param>
        /// <param name="arguments">Optional. Array of DynamicMetaObject representing additional arguments for the operation.</param>
        /// <param name="restrictions">Optional. Set of binding restriction under which the operation is valid. On return, new binding restriction may be added.</param>
        /// <param name="parameters">Required. Type-names describing the arguments of the operation. This must contain exactly 1 elements.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression Negate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.NegateChecked(arg));
        }
        #endregion
    }
}
