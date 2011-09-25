using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
{
    public static class BuiltInPrimitives
    {
        // **** Very Common ****
        public static Expression Equals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        public static Expression IdentityEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => BuiltInPrimitiveHelper.EncodeReferenceEquals(arg1, arg2, Expression.Constant(true), Expression.Constant(false)));
        }

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

        public static Expression ConvertChecked(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters, true, true, (arg, na) => arg);
        }

        public static Expression ConvertUnchecked(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters, true, false, (arg, na) => arg);
        }

        // **** Numeric Operations ****
        // ISO/IEC 10967 Integer Operations
        public static Expression IntegerEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        public static Expression IntegerLessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        public static Expression IntegerLessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        public static Expression IntegerGreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        public static Expression IntegerGreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        public static Expression IntegerAdd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        public static Expression IntegerSubtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        public static Expression IntegerMultiply(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        public static Expression IntegerDivideTruncate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.DivideIntT(arg1, arg2, type1, type2));
        }

        public static Expression IntegerDivideFloor(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.DivideIntF(arg1, arg2, type1, type2));
        }

        public static Expression IntegerRemainderTruncate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.ReminderIntT(arg1, arg2, type1, type2));
        }

        public static Expression IntegerRemainderFloor(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2, type1, type2) => BuiltInPrimitiveHelper.ReminderIntF(arg1, arg2, type1, type2));
        }

        public static Expression IntegerNegate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.Negate(arg));
        }

        // Integer operations .... not part of ISO/IEC 10967
        public static Expression IntegerBitShift(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.Shift(self, arguments, ref restrictions, parameters);
        }

        public static Expression IntegerBitAnd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.And(arg1, arg2));
        }

        public static Expression IntegerBitOr(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Or(arg1, arg2));
        }

        public static Expression IntegerBitXor(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.ExclusiveOr(arg1, arg2));
        }

        // ISO/IEC 10967 Float Operations
        public static Expression FloatEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        public static Expression FloatLessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        public static Expression FloatLessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        public static Expression FloatGreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        public static Expression FloatGreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        public static Expression FloatAdd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        public static Expression FloatSubtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        public static Expression FloatMultiply(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        public static Expression FloatDivide(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Divide(arg1, arg2));
        }

        public static Expression FloatNegate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.Negate(arg));
        }

        // ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.
        public static Expression NumberEquals(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        public static Expression NumberLessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        public static Expression NumberLessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        public static Expression NumberGreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        public static Expression NumberGreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        public static Expression NumberAdd(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        public static Expression NumberSubtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        public static Expression NumberNegate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.Negate(arg));
        }

        // Decimal operations .... not part of ISO/IEC 10967 but currently we assume same semantics
        public static Expression DecimalMultiply(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        public static Expression DecimalDivide(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.Divide(arg1, arg2));
        }

        // **** Generic Operations ****
        //  NB: Don't use those for numeric types. It's not a technical error but difficult to maintain if we find a bug or need to change something.
        public static Expression LessThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        public static Expression LessThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        public static Expression GreatherThan(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        public static Expression GreatherThanOrEqual(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        public static Expression Add(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        public static Expression Subtract(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.BinaryOperation(self, arguments, ref restrictions, parameters,
                (arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        public static Expression Negate(DynamicMetaObject self, DynamicMetaObject[] arguments, ref BindingRestrictions restrictions, IList<string> parameters)
        {
            return BuiltInPrimitiveHelper.UnaryOperation(self, arguments, ref restrictions, parameters,
                arg => Expression.Negate(arg));
        }

    }
}
