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

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    /// <summary>
    /// List of built-in primitives
    /// </summary>
    /// <remarks>
    /// Those are the primitives that can be references from code.
    /// The names of the enumerator members (values) are the 
    /// primitive names to be used from code. In other words,
    /// for integer addition, the primitive name is 'IntegerAdd'
    /// and it corresponds to BuiltInPrimitivesEnum.IntegerAdd.
    /// 
    /// If adding or removing a value, ensure to update the 
    /// PrimitiveBuilder.GenerateBuiltInPrimitive() method
    /// to reflect the change.
    /// </remarks>
    /// <example>
    /// Smalltalk code primitive usage:
    /// + operand
    ///     ˂primitive: IntegerAdd 'System.Int32' 'System.Int32'˃
    ///     
    ///     self error: 'Addition failed'.
    /// </example>
    internal enum BuiltInPrimitivesEnum
    {
        #region **** Very Common ****

        /// <summary>
        /// Performs an equality test.
        /// </summary>
        Equals,
        /// <summary>
        /// Performs an identity (reference equals) test.
        /// </summary>
        /// <remarks>
        /// For System.Boolean values, the test is equality, because Smalltalk assumes that true and false are singletons.
        /// </remarks>
        IdentityEquals,
        /// <summary>
        /// Returns the SmalltalkClass instance that the object/value belongs to.
        /// </summary>
        ObjectClass,
        /// <summary>
        /// Type conversion opearion (cast) with overflow check.
        /// If overflow occurs, OverflowException is signaled.
        /// </summary>
        ConvertChecked,
        /// <summary>
        /// Type conversion opearion (cast) without overflow check.
        /// </summary>
        ConvertUnchecked,

        #endregion

        #region **** Numeric Operations ****
        #region ISO/IEC 10967 Integer Operations

        /// <summary>
        /// Interger Equals (ISO/IEC 10967 integer operation eqI).
        /// </summary>
        /// <remarks>
        /// Both values being compared must be of same integer type. 
        /// When comparing integers and non-integers, the InvalidCastException tells Smalltalk 
        /// to use the fall-back code to do the equality comparison.
        /// </remarks>
        IntegerEquals,
        /// <summary>
        /// Interger Less-Than (ISO/IEC 10967 integer operation lssI).
        /// </summary>
        IntegerLessThan,
        /// <summary>
        /// Interger Less-Than-Or-Equal (ISO/IEC 10967 integer operation leqI).
        /// </summary>
        IntegerLessThanOrEqual,
        /// <summary>
        /// Interger Greather-Than (ISO/IEC 10967 integer operation gtrI).
        /// </summary>
        IntegerGreatherThan,
        /// <summary>
        /// Interger Greather-Than-Or-Equal (ISO/IEC 10967 integer operation geqI).
        /// </summary>
        IntegerGreatherThanOrEqual,
        /// <summary>
        /// Interger Addition (ISO/IEC 10967 integer operation addI).
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        IntegerAdd,
        /// <summary>
        /// Interger Subtraction (ISO/IEC 10967 integer operation subI).
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        IntegerSubtract,
        /// <summary>
        /// Interger Multiplication (ISO/IEC 10967 integer operation mulI).
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        IntegerMultiply,
        /// <summary>
        /// Interger Division (ISO/IEC 10967 integer operation divI-t) with truncation towards zero.
        /// For bounded values, OverflowException is signaled on overflow.
        /// For division with zero divider, DivideByZeroException is signaled.
        /// </summary>
        IntegerDivideTruncate,
        /// <summary>
        /// Interger Division (ISO/IEC 10967 integer operation divI-f) with flooring truncation towards negative infinity.
        /// For bounded values, OverflowException is signaled on overflow.
        /// For division with zero divider, DivideByZeroException is signaled.
        /// </summary>
        IntegerDivideFloor,
        /// <summary>
        /// Interger Remainder (ISO/IEC 10967 operation remI-t) with truncation towards zero.
        /// For bounded values, OverflowException is signaled on overflow.
        /// For division with zero divider, DivideByZeroException is signaled.
        /// </summary>
        IntegerRemainderTruncate,
        /// <summary>
        /// Interger Remainder (ISO/IEC 10967 operation remI-f) with flooring truncation towards negative infinity.
        /// For bounded values, OverflowException is signaled on overflow.
        /// For division with zero divider, DivideByZeroException is signaled.
        /// </summary>
        IntegerRemainderFloor,
        /// <summary>
        /// Interger Negation (ISO/IEC 10967 operation negI) operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        IntegerNegate,

        #endregion

        #region Integer operations .... not part of ISO/IEC 10967

        /// <summary>
        /// Interger Bit-Shift operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        /// <remarks>
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
        IntegerBitShift,
        /// <summary>
        /// Interger Bit-And operation.
        /// </summary>
        IntegerBitAnd,
        /// <summary>
        /// Interger Bit-Or operation.
        /// </summary>
        IntegerBitOr,
        /// <summary>
        /// Interger Bit-Xor operation.
        /// </summary>
        IntegerBitXor,

        #endregion

        #region ISO/IEC 10967 Float Operations

        /// <summary>
        /// Float Equals (ISO/IEC 10967 integer operation eqF).
        /// </summary>
        /// <remarks>
        /// Both values being compared must be of same float type. 
        /// When comparing floats and non-floats, the InvalidCastException tells Smalltalk 
        /// to use the fall-back code to do the equality comparison.
        /// </remarks>
        FloatEquals,
        /// <summary>
        /// Float Less-Than (ISO/IEC 10967 float operation lssF).
        /// </summary>
        FloatLessThan,
        /// <summary>
        /// Float Less-Than-Or-Equal (ISO/IEC 10967 float operation leqF).
        /// </summary>
        FloatLessThanOrEqual,
        /// <summary>
        /// Float Greather-Than (ISO/IEC 10967 float operation gtrF).
        /// </summary>
        FloatGreatherThan,
        /// <summary>
        /// Float Greather-Than-Or-Equal (ISO/IEC 10967 float operation geqF).
        /// </summary>
        FloatGreatherThanOrEqual,
        /// <summary>
        /// Float Addition (ISO/IEC 10967 float operation addF).
        /// OverflowException is signaled on overflow.
        /// </summary>
        FloatAdd,
        /// <summary>
        /// Float Subtraction (ISO/IEC 10967 float operation subF).
        /// OverflowException is signaled on overflow.
        /// </summary>
        FloatSubtract,
        /// <summary>
        /// Float Multiplication (ISO/IEC 10967 float operation mulF).
        /// OverflowException is signaled on overflow.
        /// </summary>
        FloatMultiply,
        /// <summary>
        /// Float Division (ISO/IEC 10967 float operation divF).
        /// OverflowException is signaled on overflow.
        /// For division with zero divider, DivideByZeroException is signaled.
        /// </summary>
        FloatDivide,
        /// <summary>
        /// Float Negation (ISO/IEC 10967 float negF) operation.
        /// OverflowException is signaled on overflow.
        /// </summary>
        FloatNegate,

        #endregion 

        #region ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.
        /// <summary>
        /// Generic number Equals operation.
        /// </summary>
        /// <remarks>
        /// Both values being compared must be of same numeric type. 
        /// When comparing numeric and non-numeric, the InvalidCastException tells Smalltalk 
        /// to use the fall-back code to do the equality comparison.
        /// </remarks>
        NumberEquals,
        /// <summary>
        /// Generic number Less-Than comparison.
        /// </summary>
        NumberLessThan,
        /// <summary>
        /// Generic number Less-Than-Or-Equal comparison.
        /// </summary>
        NumberLessThanOrEqual,
        /// <summary>
        /// Generic number Greather-Than comparison.
        /// </summary>
        NumberGreatherThan,
        /// <summary>
        /// Generic number Greather-Than-Or-Equal comparison.
        /// </summary>
        NumberGreatherThanOrEqual,
        /// <summary>
        /// Generic number Addition operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        NumberAdd,
        /// <summary>
        /// Generic number Subtraction operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        NumberSubtract,
        /// <summary>
        /// Generic number Negation operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        NumberNegate,

        #endregion

        #region Decimal operations .... not part of ISO/IEC 10967, but currently we assume same semantics

        /// <summary>
        /// Decimal Multiplication operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        DecimalMultiply,
        /// <summary>
        /// Decimal Division with truncation towards zero.
        /// For bounded values, OverflowException is signaled on overflow.
        /// For division with zero divider, DivideByZeroException is signaled.
        /// </summary>
        DecimalDivide,

        #endregion
        #endregion

        #region **** Generic Operations ****
        //  NB: Don't use those for numeric types. It's not a technical error, but difficult to maintain if we find a bug or need to change something.

        /// <summary>
        /// Generic (non-numeric) object Less-Than comparison.
        /// </summary>
        LessThan,
        /// <summary>
        /// Generic (non-numeric) object Less-Than-Or-Equal comparison.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Generic (non-numeric) object Greather-Than comparison.
        /// </summary>
        GreatherThan,
        /// <summary>
        /// Generic (non-numeric) object Greather-Than-Or-Equal comparison.
        /// </summary>
        GreatherThanOrEqual,
        /// <summary>
        /// Generic (non-numeric) object Addition operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        Add,
        /// <summary>
        /// Generic (non-numeric) object Subtraction operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        Subtract,
        /// <summary>
        /// Generic (non-numeric) object Negation operation.
        /// For bounded values, OverflowException is signaled on overflow.
        /// </summary>
        Negate,

        #endregion
    }
}
