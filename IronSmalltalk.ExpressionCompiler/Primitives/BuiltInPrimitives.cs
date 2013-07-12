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
using System.Linq.Expressions;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.ExpressionCompiler.Primitives
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
    internal static class BuiltInPrimitives
    {
        #region **** Very Common ****

        /// <summary>
        /// Generates an equality test expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression Equals(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder, 
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates an identity (reference equals) test expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For System.Boolean values, the test is equality, because Smalltalk assumes that true and false are singletons.</remarks>
        public static Expression IdentityEquals(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder, 
                (arg1, arg2) => BuiltInPrimitiveHelper.EncodeReferenceEquals(arg1, arg2, Expression.Constant(true), Expression.Constant(false)));
        }

        /// <summary>
        /// Generates an expression that returns the SmalltalkClass instance that an object/value belongs to.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <param name="binder">Required. An ObjectClassCallSiteBinder that performs the dynamic operation binding.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 0 elements.</returns>
        public static Expression Class(PrimitiveBuilder builder, ObjectClassCallSiteBinder binder)
        {
            if (binder == null)
                throw new ArgumentNullException("binder");

            if (builder.Parameters == null)
                return null;
            if (builder.Parameters.Count != 0)
                return null;

            if (builder.Self.Restrictions != null)
                builder.Restrictions = (builder.Restrictions ?? BindingRestrictions.Empty).Merge(builder.Self.Restrictions);
            return Expression.Dynamic(binder, typeof(Object), builder.Self.Expression);
        }

        /// <summary>
        /// Generates an expression that converts the type (cast) with overflow check.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>If overflow occurs, the expression throws an OverflowException.</remarks>
        public static Expression ConvertChecked(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder, Conversion.Explicit | Conversion.Checked, (arg, na) => arg);
        }

        /// <summary>
        /// Generates an expression that converts the type (cast) without overflow check.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 1 elements.</returns>
        public static Expression ConvertUnchecked(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder, Conversion.Explicit, (arg, na) => arg);
        }
        #endregion

        #region **** Numeric Operations ****
        #region ISO/IEC 10967 Integer Operations
        /// <summary>
        /// Generates an Interger Equals (ISO/IEC 10967 integer operation eqI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same integer type. When comparing integers and non-integers, 
        /// the expression throws InvalidCastException tells Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        public static Expression IntegerEquals(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Less-Than (ISO/IEC 10967 integer operation lssI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerLessThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Less-Than-Or-Equal (ISO/IEC 10967 integer operation leqI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerLessThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Greather-Than (ISO/IEC 10967 integer operation gtrI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerGreatherThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Greather-Than-Or-Equal (ISO/IEC 10967 integer operation geqI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerGreatherThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Addition (ISO/IEC 10967 integer operation addI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerAdd(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Subtraction (ISO/IEC 10967 integer operation subI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerSubtract(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Multiplication (ISO/IEC 10967 integer operation mulI) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerMultiply(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Division (ISO/IEC 10967 integer operation divI-t) with truncation towards zero expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerDivideTruncate(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.DivideIntT(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Division (ISO/IEC 10967 integer operation divI-f) with flooring truncation towards negative infinity expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerDivideFloor(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.DivideIntF(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Remainder (ISO/IEC 10967 operation remI-t) with truncation towards zero expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerRemainderTruncate(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.ReminderIntT(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Remainder (ISO/IEC 10967 operation remI-f) with flooring truncation towards negative infinity expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression IntegerRemainderFloor(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.ReminderIntF(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Negation (ISO/IEC 10967 operation negI) operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression IntegerNegate(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder,
                arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region Integer operations .... not part of ISO/IEC 10967

        /// <summary>
        /// Generates an Interger Bit-Shift operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
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
        public static Expression IntegerBitShift(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.Shift(builder);
        }

        /// <summary>
        /// Generates an Interger Bit-And operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerBitAnd(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.And(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Bit-Or operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerBitOr(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.Or(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Bit-Xor operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression IntegerBitXor(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.ExclusiveOr(arg1, arg2));
        }
        #endregion

        #region ISO/IEC 10967 Float Operations

        /// <summary>
        /// Generates a Float Equals (ISO/IEC 10967 integer operation eqF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same float type. When comparing floats and non-floats, 
        /// the expression throws InvalidCastException telling Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        public static Expression FloatEquals(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Less-Than (ISO/IEC 10967 float operation lssF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatLessThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Less-Than-Or-Equal (ISO/IEC 10967 float operation leqF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatLessThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Greather-Than (ISO/IEC 10967 float operation gtrF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatGreatherThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Greather-Than-Or-Equal (ISO/IEC 10967 float operation geqF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression FloatGreatherThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Addition (ISO/IEC 10967 float operation addF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        public static Expression FloatAdd(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Subtraction (ISO/IEC 10967 float operation subF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        public static Expression FloatSubtract(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Multiplication (ISO/IEC 10967 float operation mulF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        public static Expression FloatMultiply(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Division (ISO/IEC 10967 float operation divF) expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression FloatDivide(PrimitiveBuilder builder)
        {
            // BUGBUG: Do we throw an OverflowException?
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.Divide(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Negation (ISO/IEC 10967 float negF) operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 1 elements.</returns>
        public static Expression FloatNegate(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder,
                arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.

        /// <summary>
        /// Generates a generic number Equals operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same numeric type. When comparing numeric and non-numeric, 
        /// the expression throws InvalidCastException telling Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        public static Expression NumberEquals(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Less-Than comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberLessThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Less-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberLessThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Greather-Than comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberGreatherThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Greather-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression NumberGreatherThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Addition operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression NumberAdd(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Subtraction operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression NumberSubtract(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Negation operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression NumberNegate(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder,
                arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region Decimal operations .... not part of ISO/IEC 10967 but currently we assume same semantics

        /// <summary>
        /// Generates a Decimal Multiplication operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression DecimalMultiply(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Decimal Division operation with truncation towards zero expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        public static Expression DecimalDivide(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.Divide(arg1, arg2));
        }
        #endregion
        #endregion

        #region **** Generic Operations ****
        //  NB: Don't use those for numeric types. It's not a technical error but difficult to maintain if we find a bug or need to change something.

        /// <summary>
        /// Generates a generic (non-numeric) object Less-Than comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression LessThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Less-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression LessThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Greather-Than comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression GreatherThan(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Greather-Than-Or-Equal comparison expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        public static Expression GreatherThanOrEqual(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Addition operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression Add(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Subtraction operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>

        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression Subtract(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Negation operation expression.
        /// </summary>
        /// <param name="builder">Required. Primitive builder representing the context for this primitive operation.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'builder.Parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        public static Expression Negate(PrimitiveBuilder builder)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder,
                arg => Expression.NegateChecked(arg));
        }
        #endregion
    }
}
