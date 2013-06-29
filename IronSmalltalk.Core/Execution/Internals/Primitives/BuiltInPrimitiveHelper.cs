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
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
{
    internal static class BuiltInPrimitiveHelper
    {
        public static Expression UnaryOperation(PrimitiveBuilder builder, Func<Expression, Expression> func)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder, (arg, type) => func(arg));
        }

        public static Expression UnaryOperation(PrimitiveBuilder builder, Func<Expression, Type, Expression> func)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(builder, Conversion.Checked, func);
        }

        public static Expression UnaryOperation(PrimitiveBuilder builder, Conversion conversion, Func<Expression, Type, Expression> func)
        {
            if (builder == null)
                throw new ArgumentNullException();

            if (builder.Parameters == null)
                return null;
            if (builder.Parameters.Count != 1)
                return null;
            Type type = (builder.Parameters[0] == "self") ? null : NativeTypeClassMap.GetType(builder.Parameters[0]);
            Type[] types = new Type[] { type };
            IList<Expression> args = PrimitiveHelper.GetArguments(builder, types, conversion);
            return func(args[0], type);
        }

        public static Expression BinaryOperation(PrimitiveBuilder builder, Func<Expression, Expression, Expression> func)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder, (arg1, arg2, type1, type2) => func(arg1, arg2));
        }

        public static Expression BinaryOperation(PrimitiveBuilder builder, Func<Expression, Expression, Type, Type, Expression> func)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(builder, Conversion.Checked, func);
        }

        public static Expression BinaryOperation(PrimitiveBuilder builder, Conversion conversion, Func<Expression, Expression, Type, Type, Expression> func)
        {
            if (builder == null)
                throw new ArgumentNullException();

            if (builder.Parameters == null)
                return null;
            if (builder.Parameters.Count != 2)
                return null;
            Type type0 = (builder.Parameters[0] == "self") ? null : NativeTypeClassMap.GetType(builder.Parameters[0]);
            Type type1 = (builder.Parameters[1] == "self") ? null : NativeTypeClassMap.GetType(builder.Parameters[1]);
            Type[] types = new Type[] { type0, type1 };
            IList<Expression> args = PrimitiveHelper.GetArguments(builder, types, conversion);
            return func(args[0], args[1], type0, type1);
        }

        //private Expression ReferenceEqual(Expression a, Expression b)
        //{
        //    NameBinding trueBinding = this.GetBinding(SemanticConstants.True);
        //    NameBinding falseBinding = this.GetBinding(SemanticConstants.False);
        //    return BuiltInPrimitiveHelper.EncodeReferenceEquals(a, b, trueBinding.GenerateReadExpression(this), falseBinding.GenerateReadExpression(this));
        //}

        /// <summary>
        /// Performs an ISO/IEC 10967 integer operation divI-t, i.e. division with truncation towards zero.
        /// </summary>
        /// <remarks>
        /// See ISO/IEC 10967:
        ///     4.1 Symbols (... about truncation)
        ///     5.1.3 Axioms
        /// Defined as:
        ///     divI-t(x,y): tr(x/y)
        ///     tr(x):      [x]     if (x ˃= 0)
        ///                 -[-x]   if (x ˂ 0)
        /// Example:
        ///     divI-t( -3, 2) =˃ -1
        ///     divI-t( -2, 2) =˃ -1
        ///     divI-t( -1, 2) =˃ 0
        ///     divI-t(  0, 2) =˃ 0
        ///     divI-t(  1, 2) =˃ 0
        ///     divI-t(  2, 2) =˃ 1
        ///     divI-t(  3, 2) =˃ 1
        /// </remarks>
        public static Expression DivideIntT(Expression arg1, Expression arg2, Type type1, Type type2)
        {
            // BUGBUG: Do we throw an OverflowException?
            return Expression.Divide(arg1, arg2);
        }

        /// <summary>
        /// Performs an ISO/IEC 10967 integer operation divI-f, i.e. division with flooring truncation towards negative infinity.
        /// </summary>
        /// <remarks>
        /// See ISO/IEC 10967:
        ///     4.1 Symbols (... about truncation)
        ///     5.1.3 Axioms
        /// Defined as:
        ///     divI-f(x,y): [x/y]
        ///     [x]:    Largest integer where:  x-1 ˂ [x] ˂= x
        /// Example:
        ///     divI-f( -3, 2) =˃ -2
        ///     divI-f( -2, 2) =˃ -1
        ///     divI-f( -1, 2) =˃ -1
        ///     divI-f(  0, 2) =˃ 0
        ///     divI-f(  1, 2) =˃ 0
        ///     divI-f(  2, 2) =˃ 1
        ///     divI-f(  3, 2) =˃ 1
        /// </remarks>
        public static Expression DivideIntF(Expression arg1, Expression arg2, Type type1, Type type2)
        {
            // BUGBUG: Do we throw an OverflowException?
            if (BuiltInPrimitiveHelper.IsUnsignedType(type1) && BuiltInPrimitiveHelper.IsUnsignedType(type2))
                return Expression.Divide(arg1, arg2);

            // C# pseudocode:
            //  if ((arg1 >= 0) ^ (arg2 >= 0))
            //      if ((arg1 % arg2) == 0)
            //          return arg1 / arg2;
            //      else
            //          return (arg1 / arg2) - 1;
            //  else
            //      return arg1 / arg2;
            Expression zero = Expression.Constant(BuiltInPrimitiveHelper.GetZero(type1), type1);
            Expression one = Expression.Constant(BuiltInPrimitiveHelper.GetOne(type1), type1);
            Expression division = Expression.Divide(arg1, arg2);
            Expression modulo = Expression.Modulo(arg1, arg2);
            return Expression.Condition(
                Expression.ExclusiveOr(
                    Expression.GreaterThanOrEqual(arg1, zero),
                    Expression.GreaterThanOrEqual(arg2, zero)),
                Expression.Condition(
                    Expression.Equal(modulo, zero),
                    division,
                    Expression.Subtract(division, one)),
                division);
        }

        /// <summary>
        /// Performs an ISO/IEC 10967 float operation divF, i.e. division with truncation towards zero.
        /// </summary>
        /// <remarks>
        /// See ISO/IEC 10967:
        ///     4.1 Symbols (... about truncation)
        ///     5.2.2 Operations
        /// </remarks>
        public static Expression DivideFloat(Expression arg1, Expression arg2, Type type1, Type type2)
        {
            return Expression.Divide(arg1, arg2);
        }

        /// <summary>
        /// Performs an ISO/IEC 10967 operation rem-t, i.e. reminder with truncation towards zero.
        /// </summary>
        /// <remarks>
        /// See ISO/IEC 10967:
        ///     4.1 Symbols (... about truncation)
        ///     5.1.3 Axioms
        /// Defined as:
        ///     remI-t(x,y):     x - ( div-t(x, y) * y )
        /// </remarks>
        public static Expression ReminderIntT(Expression arg1, Expression arg2, Type type1, Type type2)
        {
            return Expression.Modulo(arg1, arg2);
        }

        /// <summary>
        /// Performs an ISO/IEC 10967 operation rem-f, i.e. reminder with flooring truncation towards negative infinity.
        /// </summary>
        /// <remarks>
        /// See ISO/IEC 10967:
        ///     4.1 Symbols (... about truncation)
        ///     5.1.3 Axioms
        /// Defined as:
        ///     divI-f(x,y):     x - ( div-f(x, y) * y )
        /// </remarks>
        public static Expression ReminderIntF(Expression arg1, Expression arg2, Type type1, Type type2)
        {
            if (BuiltInPrimitiveHelper.IsUnsignedType(type1) && BuiltInPrimitiveHelper.IsUnsignedType(type2))
                return Expression.Modulo(arg1, arg2);

            Expression zero1 = Expression.Constant(BuiltInPrimitiveHelper.GetZero(type1), type1);
            Expression zero2 = Expression.Constant(BuiltInPrimitiveHelper.GetZero(type2), type2);

            // C# pseudocode:
            //  if (arg1 >= 0)
            //      if (arg2 => 0)
            //          return arg1 % arg2;
            //      else 
            //          return -(arg1 % -arg2);
            //  else
            //      if (arg2 => 0)
            //          return -arg1 % arg2;
            //      else 
            //          return arg1 % arg2;  // same as: -(-arg1 % -arg2)
            Expression modulo = Expression.Modulo(
                Expression.Condition(
                    Expression.GreaterThanOrEqual(arg1, zero1),
                    arg1,
                    Expression.Negate(arg1)),
                arg2);
            return Expression.Condition(
                Expression.GreaterThanOrEqual(arg2, zero2),
                    modulo,
                    Expression.Negate(modulo));
        }


        private static bool IsUnsignedType(Type type)
        {
            if (type == typeof(ulong))
                return true;
            else if (type == typeof(uint))
                return true;
            else if (type == typeof(ushort))
                return true;
            else if (type == typeof(byte))
                return true;
            return false;
        }

        private static object GetZero(Type type)
        {
            return BuiltInPrimitiveHelper.GetValue(type, 0);
        }

        private static object GetOne(Type type)
        {
            return BuiltInPrimitiveHelper.GetValue(type, 1);
        }

        private static object GetValue(Type type, object value)
        {
            Expression expression = Expression.Convert(Expression.Constant(value, value.GetType()), type);
            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object)));
            var method = lambda.Compile();
            return method();
        }

        public static Expression EncodeReferenceEquals(Expression a, Expression b, Expression trueValue, Expression falseValue)
        {
            return Expression.Condition(
                Expression.AndAlso(Expression.TypeIs(a, typeof(bool)), Expression.TypeIs(b, typeof(bool))),
                Expression.Condition(
                    Expression.Equal(a, b),
                    trueValue,
                    falseValue),
                Expression.Condition(
                    Expression.ReferenceEqual(a, b),
                    trueValue,
                    falseValue));
        }

        public static Expression Shift(PrimitiveBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException();

            if (builder.Parameters == null)
                return null;
            if (builder.Parameters.Count != 2)
                return null;
            // We don't support if the shift parameter is other than int
            if (NativeTypeClassMap.GetType(builder.Parameters[1]) != typeof(int))
                throw new PrimitiveInvalidTypeException(String.Format(RuntimeCodeGenerationErrors.WrongShiftTypeName, builder.Parameters[1]));
            Type type = NativeTypeClassMap.GetType(builder.Parameters[0]);

            int bits;
            object zeroValue;
            if (type == typeof(System.Numerics.BigInteger))
            { bits = 0; zeroValue = System.Numerics.BigInteger.Zero; }
            else if (type == typeof(long))
            { bits = -63; zeroValue = (long)0; }
            else if (type == typeof(int))
            { bits = -31; zeroValue = (int)0; }
            else if (type == typeof(short))
            { bits = -15; zeroValue = (short)0; }
            else if (type == typeof(sbyte))
            { bits = -7; zeroValue = (sbyte)0; }
            else if (type == typeof(ulong))
            { bits = 64; zeroValue = (ulong)0; }
            else if (type == typeof(uint))
            { bits = 32; zeroValue = (uint)0; }
            else if (type == typeof(ushort))
            { bits = 16; zeroValue = (ushort)0; }
            else if (type == typeof(byte))
            { bits = 8; zeroValue = (byte)0; }
            else
                throw new PrimitiveInvalidTypeException(String.Format(RuntimeCodeGenerationErrors.WrongShiftTypeName, builder.Parameters[0]));


            IList<Expression> args = PrimitiveHelper.GetArguments(builder, new Type[] { type, typeof(int) }, Conversion.Checked);
            Expression value = args[0];
            Expression shift = args[1];

            return BuiltInPrimitiveHelper.Shift(value, shift, type, bits, zeroValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Value to be shifted. It must have the same type as the "type" parameter.</param>
        /// <param name="shift">Shift value. Positive = left, netagive = right. It must be of type "System.Int32".</param>
        /// <param name="type">Type if the value to be shifted and of the zero value.</param>
        /// <param name="bits">Number of significant bits in the integer type. Negative if type is signed.</param>
        /// <param name="zeroValue">Value of 0 (zero). It must have the same type as the "type" parameter.</param>
        /// <returns></returns>
        public static Expression Shift(Expression value, Expression shift, Type type, int bits, object zeroValue)
        {
            Expression leftShift;
            if (bits == 0)
            {
                // BigInteger never overflows .... so just shift away
                leftShift = Expression.LeftShift(value, shift);
            }
            else
            {
                // Some integer with limited precision ... must handle delicately with self-made overflow checks. 
                // NB: Logic here executes only if shift is greather than zero.
                // C# overflow check semantics ... example for int (32 bit):
                //  if (value < 0)
                //      throw constant OverflowException();         // Handle negatives in Smalltalk (X3J20 says "undefined" for this)
                //  if (shift >= 31)
                //      if (value == 0)                             // Zero is save to shift forever
                //          return 0;
                //      else
                //          throw constant OverflowException();     // Will definitely overflow
                //  if ((value >> (31 - shift)) == 0)               // Do test if there are significant bits that will overflow
                //      return value << shift;
                //  else
                //      throw constant OverflowException();         // Throw and let Smalltalk code to the work

                bool signed = bits < 0;
                bits = Math.Abs(bits);
                Expression zero = Expression.Constant(zeroValue, type);
                Expression overflow = Expression.Constant(new OverflowException(), typeof(OverflowException));
                Expression bitCount = Expression.Constant(bits, typeof(int));

                leftShift = Expression.Condition(
                    Expression.GreaterThanOrEqual(shift, bitCount),
                        Expression.Condition(
                            Expression.Equal(value, zero),
                                zero,
                                Expression.Throw(overflow, type)),
                        Expression.Condition(
                            Expression.Equal(Expression.RightShift(value, Expression.Subtract(bitCount, shift)), zero),
                                Expression.LeftShift(value, shift),
                                Expression.Throw(overflow, type)));
                if (signed)
                    leftShift = Expression.Condition(
                        Expression.LessThan(value, zero),
                            Expression.Throw(overflow, type),
                            leftShift);
            }

            // C# semantics:
            //  if (shift > 0)
            //      return checked(value << shift);     ... this can overflow, so see above for overflow checking
            //  else
            //      return value >> -shift;              ... this can never overflow ... so just do it
            return Expression.Condition(
                Expression.GreaterThan(shift, Expression.Constant(0, typeof(int))),
                leftShift,
                Expression.RightShift(value, Expression.Negate(shift)));
        }
    }
}
