using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Primitives.Exceptions;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    /// <summary>
    /// This class contains the encodings for each (all if possible, otherwise as many as possible)
    /// of the primitives defined in BuiltInPrimitivesEnum. The functions are the actual implementation
    /// of the primitive and therefore contain the logic for that primitive.
    /// </summary>
    /// <remarks>
    /// If there is a bug in built-in primitive, this is the place we should look.
    /// Some primitive delegate the logic for some operations to the base class.
    /// </remarks>

    public sealed class BuiltInPrimitiveEncoder : PrimitiveEncoder
    {
        public string PrimitiveName { get; private set; }

        private BuiltInPrimitiveEncoder(VisitingContext context, IEnumerable<string> parameters, string primitiveName)
            : base(context, parameters)
        {
            if (String.IsNullOrWhiteSpace(primitiveName))
                throw new ArgumentNullException("primitiveName");
            this.PrimitiveName = primitiveName;
        }

        public static Expression GeneratePrimitive(VisitingContext context, IEnumerable<string> parameters, string primitiveName)
        {
            return (new BuiltInPrimitiveEncoder(context, parameters, primitiveName)).GenerateExpression();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This function MUST BE updated if changes are made to the BuiltInPrimitivesEnum enumeration!
        /// </remarks>
        private Expression GenerateExpression()
        {
            // For built-in primitives, the defining type is not used!
            BuiltInPrimitiveEnum primitive;
            if (!Enum.TryParse(this.PrimitiveName, out primitive))
                throw new PrimitiveSemanticException(String.Format(CodeGenerationErrors.WrongPrimitive, this.PrimitiveName));

            Expression exp;
            switch (primitive)
            {
                // **** Very Common ****
                case BuiltInPrimitiveEnum.Equals:
                    exp = this.Equals();
                    break;
                case BuiltInPrimitiveEnum.IdentityEquals:
                    exp = this.IdentityEquals();
                    break;
                case BuiltInPrimitiveEnum.ObjectClass:
                    exp = this.Class();
                    break;
                case BuiltInPrimitiveEnum.ConvertChecked:
                    exp = this.ConvertChecked();
                    break;
                case BuiltInPrimitiveEnum.ConvertUnchecked:
                    exp = this.ConvertUnchecked();
                    break;
                // **** Numeric Operations ****
                // ISO/IEC 10967 Integer Operations
                case BuiltInPrimitiveEnum.IntegerEquals:
                    exp = this.IntegerEquals();
                    break;
                case BuiltInPrimitiveEnum.IntegerLessThan:
                    exp = this.IntegerLessThan();
                    break;
                case BuiltInPrimitiveEnum.IntegerLessThanOrEqual:
                    exp = this.IntegerLessThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.IntegerGreatherThan:
                    exp = this.IntegerGreatherThan();
                    break;
                case BuiltInPrimitiveEnum.IntegerGreatherThanOrEqual:
                    exp = this.IntegerGreatherThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.IntegerAdd:
                    exp = this.IntegerAdd();
                    break;
                case BuiltInPrimitiveEnum.IntegerSubtract:
                    exp = this.IntegerSubtract();
                    break;
                case BuiltInPrimitiveEnum.IntegerMultiply:
                    exp = this.IntegerMultiply();
                    break;
                case BuiltInPrimitiveEnum.IntegerDivideTruncate:
                    exp = this.IntegerDivideTruncate();
                    break;
                case BuiltInPrimitiveEnum.IntegerDivideFloor:
                    exp = this.IntegerDivideFloor();
                    break;
                case BuiltInPrimitiveEnum.IntegerRemainderTruncate:
                    exp = this.IntegerRemainderTruncate();
                    break;
                case BuiltInPrimitiveEnum.IntegerRemainderFloor:
                    exp = this.IntegerRemainderFloor();
                    break;
                case BuiltInPrimitiveEnum.IntegerNegate:
                    exp = this.IntegerNegate();
                    break;
                // Integer operations .... not part of ISO/IEC 10967
                case BuiltInPrimitiveEnum.IntegerBitShift:
                    exp = this.IntegerBitShift();
                    break;
                case BuiltInPrimitiveEnum.IntegerBitAnd:
                    exp = this.IntegerBitAnd();
                    break;
                case BuiltInPrimitiveEnum.IntegerBitOr:
                    exp = this.IntegerBitOr();
                    break;
                case BuiltInPrimitiveEnum.IntegerBitXor:
                    exp = this.IntegerBitXor();
                    break;
                // ISO/IEC 10967 Float Operations
                case BuiltInPrimitiveEnum.FloatEquals:
                    exp = this.FloatEquals();
                    break;
                case BuiltInPrimitiveEnum.FloatLessThan:
                    exp = this.FloatLessThan();
                    break;
                case BuiltInPrimitiveEnum.FloatLessThanOrEqual:
                    exp = this.FloatLessThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.FloatGreatherThan:
                    exp = this.FloatGreatherThan();
                    break;
                case BuiltInPrimitiveEnum.FloatGreatherThanOrEqual:
                    exp = this.FloatGreatherThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.FloatAdd:
                    exp = this.FloatAdd();
                    break;
                case BuiltInPrimitiveEnum.FloatSubtract:
                    exp = this.FloatSubtract();
                    break;
                case BuiltInPrimitiveEnum.FloatMultiply:
                    exp = this.FloatMultiply();
                    break;
                case BuiltInPrimitiveEnum.FloatDivide:
                    exp = this.FloatDivide();
                    break;
                case BuiltInPrimitiveEnum.FloatNegate:
                    exp = this.FloatNegate();
                    break;
                // ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.
                case BuiltInPrimitiveEnum.NumberEquals:
                    exp = this.NumberEquals();
                    break;
                case BuiltInPrimitiveEnum.NumberLessThan:
                    exp = this.NumberLessThan();
                    break;
                case BuiltInPrimitiveEnum.NumberLessThanOrEqual:
                    exp = this.NumberLessThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.NumberGreatherThan:
                    exp = this.NumberGreatherThan();
                    break;
                case BuiltInPrimitiveEnum.NumberGreatherThanOrEqual:
                    exp = this.NumberGreatherThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.NumberAdd:
                    exp = this.NumberAdd();
                    break;
                case BuiltInPrimitiveEnum.NumberSubtract:
                    exp = this.NumberSubtract();
                    break;
                case BuiltInPrimitiveEnum.NumberNegate:
                    exp = this.NumberNegate();
                    break;
                // Decimal operations .... not part of ISO/IEC 10967 but currently we assume same semantics
                case BuiltInPrimitiveEnum.DecimalMultiply:
                    exp = this.DecimalMultiply();
                    break;
                case BuiltInPrimitiveEnum.DecimalDivide:
                    exp = this.DecimalDivide();
                    break;
                // **** Generic Operations ****
                //  NB: Don't use those for numeric types. It's not a technical error but difficult to maintain if we find a bug or need to change something.
                case BuiltInPrimitiveEnum.LessThan:
                    exp = this.LessThan();
                    break;
                case BuiltInPrimitiveEnum.LessThanOrEqual:
                    exp = this.LessThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.GreatherThan:
                    exp = this.GreatherThan();
                    break;
                case BuiltInPrimitiveEnum.GreatherThanOrEqual:
                    exp = this.GreatherThanOrEqual();
                    break;
                case BuiltInPrimitiveEnum.Add:
                    exp = this.Add();
                    break;
                case BuiltInPrimitiveEnum.Subtract:
                    exp = this.Subtract();
                    break;
                case BuiltInPrimitiveEnum.Negate:
                    exp = this.Negate();
                    break;
                default:
                    throw new NotImplementedException(String.Format("Primitive {0} is not implemented. This is a bug!", primitive));
            }
            return exp;
        }

        #region **** Very Common ****

        /// <summary>
        /// Generates an equality test expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression Equals()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates an identity (reference equals) test expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For System.Boolean values, the test is equality, because Smalltalk assumes that true and false are singletons.</remarks>
        private Expression IdentityEquals()
        {
            return this.BinaryOperation((arg1, arg2) => PrimitiveEncoder.EncodeReferenceEquals(arg1, arg2));
        }

        /// <summary>
        /// Generates an expression that returns the SmalltalkClass instance that an object/value belongs to.
        /// </summary>
        /// <param name="binder">Required. An ObjectClassCallSiteBinder that performs the dynamic operation binding.</param>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 0 elements.</returns>
        private Expression Class()
        {
            if (this.Parameters == null)
                return null;
            if (this.Parameters.Count != 0)
                return null;

            return this.Context.CompileGetClass(this.Context.Self);
        }

        /// <summary>
        /// Generates an expression that converts the type (cast) with overflow check.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>If overflow occurs, the expression throws an OverflowException.</remarks>
        private Expression ConvertChecked()
        {
            return this.UnaryOperation(Conversion.Explicit | Conversion.Checked, (arg, na) => arg);
        }

        /// <summary>
        /// Generates an expression that converts the type (cast) without overflow check.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 1 elements.</returns>
        private Expression ConvertUnchecked()
        {
            return this.UnaryOperation(Conversion.Explicit, (arg, na) => arg);
        }
        #endregion

        #region **** Numeric Operations ****
        #region ISO/IEC 10967 Integer Operations
        /// <summary>
        /// Generates an Interger Equals (ISO/IEC 10967 integer operation eqI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same integer type. When comparing integers and non-integers, 
        /// the expression throws InvalidCastException tells Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        private Expression IntegerEquals()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Less-Than (ISO/IEC 10967 integer operation lssI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression IntegerLessThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Less-Than-Or-Equal (ISO/IEC 10967 integer operation leqI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression IntegerLessThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Greather-Than (ISO/IEC 10967 integer operation gtrI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression IntegerGreatherThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Greather-Than-Or-Equal (ISO/IEC 10967 integer operation geqI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression IntegerGreatherThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Addition (ISO/IEC 10967 integer operation addI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression IntegerAdd()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Subtraction (ISO/IEC 10967 integer operation subI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression IntegerSubtract()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Multiplication (ISO/IEC 10967 integer operation mulI) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression IntegerMultiply()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Division (ISO/IEC 10967 integer operation divI-t) with truncation towards zero expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        private Expression IntegerDivideTruncate()
        {
            return this.BinaryOperation((arg1, arg2, type1, type2) => this.DivideIntT(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Division (ISO/IEC 10967 integer operation divI-f) with flooring truncation towards negative infinity expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        private Expression IntegerDivideFloor()
        {
            return this.BinaryOperation((arg1, arg2, type1, type2) => this.DivideIntF(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Remainder (ISO/IEC 10967 operation remI-t) with truncation towards zero expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        private Expression IntegerRemainderTruncate()
        {
            return this.BinaryOperation((arg1, arg2, type1, type2) => this.ReminderIntT(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Remainder (ISO/IEC 10967 operation remI-f) with flooring truncation towards negative infinity expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        private Expression IntegerRemainderFloor()
        {
            return this.BinaryOperation((arg1, arg2, type1, type2) => this.ReminderIntF(arg1, arg2, type1, type2));
        }

        /// <summary>
        /// Generates an Interger Negation (ISO/IEC 10967 operation negI) operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression IntegerNegate()
        {
            return this.UnaryOperation(arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region Integer operations .... not part of ISO/IEC 10967

        /// <summary>
        /// Generates an Interger Bit-Shift operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// For bounded values, the expression throws an OverflowException on overflow.
        /// 
        /// The current implementation <see cref="BuiltInPrimitiveEncoder.Shift"/> only supports the following types:
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
        private Expression IntegerBitShift()
        {
            return this.Shift();
        }

        /// <summary>
        /// Generates an Interger Bit-And operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression IntegerBitAnd()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.And(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Bit-Or operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression IntegerBitOr()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.Or(arg1, arg2));
        }

        /// <summary>
        /// Generates an Interger Bit-Xor operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression IntegerBitXor()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.ExclusiveOr(arg1, arg2));
        }
        #endregion

        #region ISO/IEC 10967 Float Operations

        /// <summary>
        /// Generates a Float Equals (ISO/IEC 10967 integer operation eqF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same float type. When comparing floats and non-floats, 
        /// the expression throws InvalidCastException telling Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        private Expression FloatEquals()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Less-Than (ISO/IEC 10967 float operation lssF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression FloatLessThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Less-Than-Or-Equal (ISO/IEC 10967 float operation leqF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression FloatLessThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Greather-Than (ISO/IEC 10967 float operation gtrF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression FloatGreatherThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Greather-Than-Or-Equal (ISO/IEC 10967 float operation geqF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression FloatGreatherThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Addition (ISO/IEC 10967 float operation addF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        private Expression FloatAdd()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Subtraction (ISO/IEC 10967 float operation subF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        private Expression FloatSubtract()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Multiplication (ISO/IEC 10967 float operation mulF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow.</remarks>
        private Expression FloatMultiply()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Division (ISO/IEC 10967 float operation divF) expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>The expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        private Expression FloatDivide()
        {
            // BUGBUG: Do we throw an OverflowException?
            return this.BinaryOperation((arg1, arg2) => Expression.Divide(arg1, arg2));
        }

        /// <summary>
        /// Generates a Float Negation (ISO/IEC 10967 float negF) operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 1 elements.</returns>
        private Expression FloatNegate()
        {
            return this.UnaryOperation(arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.

        /// <summary>
        /// Generates a generic number Equals operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>
        /// Both values being compared must be of same numeric type. When comparing numeric and non-numeric, 
        /// the expression throws InvalidCastException telling Smalltalk to use the fall-back code to do the equality comparison.
        /// </remarks>
        private Expression NumberEquals()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.Equal(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Less-Than comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression NumberLessThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Less-Than-Or-Equal comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression NumberLessThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Greather-Than comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression NumberGreatherThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Greather-Than-Or-Equal comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression NumberGreatherThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Addition operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression NumberAdd()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Subtraction operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression NumberSubtract()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic number Negation operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression NumberNegate()
        {
            return this.UnaryOperation(arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region Decimal operations .... not part of ISO/IEC 10967 but currently we assume same semantics

        /// <summary>
        /// Generates a Decimal Multiplication operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression DecimalMultiply()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.MultiplyChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a Decimal Division operation with truncation towards zero expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow. When dividing by zero, the expression throws a DivideByZeroException.</remarks>
        private Expression DecimalDivide()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.Divide(arg1, arg2));
        }
        #endregion
        #endregion

        #region **** Generic Operations ****
        //  NB: Don't use those for numeric types. It's not a technical error but difficult to maintain if we find a bug or need to change something.

        /// <summary>
        /// Generates a generic (non-numeric) object Less-Than comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression LessThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Less-Than-Or-Equal comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression LessThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.LessThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Greather-Than comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression GreatherThan()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThan(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Greather-Than-Or-Equal comparison expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        private Expression GreatherThanOrEqual()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.GreaterThanOrEqual(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Addition operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression Add()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.AddChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Subtraction operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 2 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression Subtract()
        {
            return this.BinaryOperation((arg1, arg2) => Expression.SubtractChecked(arg1, arg2));
        }

        /// <summary>
        /// Generates a generic (non-numeric) object Negation operation expression.
        /// </summary>
        /// <returns>An Expression with the logic necessary to perform the operation. Null if 'this.Parameters' does not contain exactly 1 elements.</returns>
        /// <remarks>For bounded values, the expression throws an OverflowException on overflow.</remarks>
        private Expression Negate()
        {
            return this.UnaryOperation(arg => Expression.NegateChecked(arg));
        }
        #endregion

        #region Helper Functions

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
        private Expression DivideIntT(Expression arg1, Expression arg2, Type type1, Type type2)
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
        private Expression DivideIntF(Expression arg1, Expression arg2, Type type1, Type type2)
        {
            // BUGBUG: Do we throw an OverflowException?
            if (BuiltInPrimitiveEncoder.IsUnsignedType(type1) && BuiltInPrimitiveEncoder.IsUnsignedType(type2))
                return Expression.Divide(arg1, arg2);

            // C# pseudocode:
            //  if ((arg1 >= 0) ^ (arg2 >= 0))
            //      if ((arg1 % arg2) == 0)
            //          return arg1 / arg2;
            //      else
            //          return (arg1 / arg2) - 1;
            //  else
            //      return arg1 / arg2;
            Expression zero = this.GetZero(type1);
            Expression one = this.GetOne(type1);
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
        private Expression DivideFloat(Expression arg1, Expression arg2, Type type1, Type type2)
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
        private Expression ReminderIntT(Expression arg1, Expression arg2, Type type1, Type type2)
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
        private Expression ReminderIntF(Expression arg1, Expression arg2, Type type1, Type type2)
        {
            if (BuiltInPrimitiveEncoder.IsUnsignedType(type1) && BuiltInPrimitiveEncoder.IsUnsignedType(type2))
                return Expression.Modulo(arg1, arg2);

            Expression zero1 = this.GetZero(type1);
            Expression zero2 = this.GetZero(type2);

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

        private Expression Shift()
        {
            if (this.Parameters == null)
                return null;
            if (this.Parameters.Count != 2)
                return null;
            // We don't support if the shift parameter is other than int
            if (NativeTypeClassMap.GetType(this.Parameters[1]) != typeof(int))
                throw new PrimitiveInvalidTypeException(String.Format(CodeGenerationErrors.WrongShiftTypeName, this.Parameters[1]));
            Type type = NativeTypeClassMap.GetType(this.Parameters[0]);

            int bits;
            if (type == typeof(System.Numerics.BigInteger))
                bits = 0; 
            else if (type == typeof(Int64))
                bits = -63; 
            else if (type == typeof(Int32))
                bits = -31; 
            else if (type == typeof(Int16))
                bits = -15; 
            else if (type == typeof(SByte))
                bits = -7; 
            else if (type == typeof(UInt64))
                bits = 64; 
            else if (type == typeof(UInt32))
                bits = 32; 
            else if (type == typeof(UInt16))
                bits = 16; 
            else if (type == typeof(Byte))
                bits = 8; 
            else
                throw new PrimitiveInvalidTypeException(String.Format(CodeGenerationErrors.WrongShiftTypeName, this.Parameters[0]));

            IList<Expression> args = this.GetArguments(new Type[] { type, typeof(int) }, Conversion.Checked);
            Expression value = args[0];
            Expression shift = args[1];
            Expression zero = this.GetZero(type);

            return this.Shift(value, shift, type, bits, zero);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Value to be shifted. It must have the same type as the "type" parameter.</param>
        /// <param name="shift">Shift value. Positive = left, netagive = right. It must be of type "System.Int32".</param>
        /// <param name="type">Type if the value to be shifted and of the zero value.</param>
        /// <param name="bits">Number of significant bits in the integer type. Negative if type is signed.</param>
        /// <param name="zero">Value of 0 (zero). It must have the same type as the "type" parameter.</param>
        /// <returns></returns>
        private Expression Shift(Expression value, Expression shift, Type type, int bits, Expression zero)
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
                Expression overflow = Expression.New(typeof(OverflowException));
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

        private Expression GetZero(Type type)
        {
            return this.Compiler.LiteralEncoding.GetZero(type);
        }

        private Expression GetOne(Type type)
        {
            return this.Compiler.LiteralEncoding.GetOne(type);
        }

        #endregion
    }
}
