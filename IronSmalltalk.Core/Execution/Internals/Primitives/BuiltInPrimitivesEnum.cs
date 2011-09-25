using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronSmalltalk.Runtime.Execution.Internals.Primitives
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
    ///     <primitive: IntegerAdd 'System.Int32' 'System.Int32'>
    ///     
    ///     self error: 'Addition failed'.
    /// </example>
    public enum BuiltInPrimitivesEnum
    {
        // **** Very Common ****
        Equals,
        IdentityEquals,
        ObjectClass,
        ConvertChecked,
        ConvertUnchecked,
        // **** Numeric Operations ****
        // ISO/IEC 10967 Integer Operations
        IntegerEquals,
        IntegerLessThan,
        IntegerLessThanOrEqual,
        IntegerGreatherThan,
        IntegerGreatherThanOrEqual,
        IntegerAdd,
        IntegerSubtract,
        IntegerMultiply,
        IntegerDivideTruncate,
        IntegerDivideFloor,
        IntegerRemainderTruncate,
        IntegerRemainderFloor,
        IntegerNegate,
        // Integer operations .... not part of ISO/IEC 10967
        IntegerBitShift,
        IntegerBitAnd,
        IntegerBitOr,
        IntegerBitXor,
        // ISO/IEC 10967 Float Operations
        FloatEquals,
        FloatLessThan,
        FloatLessThanOrEqual,
        FloatGreatherThan,
        FloatGreatherThanOrEqual,
        FloatAdd,
        FloatSubtract,
        FloatMultiply,
        FloatDivide,
        FloatNegate,
        // ISO/IEC 10967 Generic Operations ... for numbers that are not Integers or Floats.
        NumberEquals,
        NumberLessThan,
        NumberLessThanOrEqual,
        NumberGreatherThan,
        NumberGreatherThanOrEqual,
        NumberAdd,
        NumberSubtract,
        NumberNegate,
        // Decimal operations .... not part of ISO/IEC 10967, but currently we assume same semantics
        DecimalMultiply,
        DecimalDivide,
        // **** Generic Operations ****
        //  NB: Don't use those for numeric types. It's not a technical error, but difficult to maintain if we find a bug or need to change something.
        LessThan,
        LessThanOrEqual,
        GreatherThan,
        GreatherThanOrEqual,
        Add,
        Subtract,
        Negate,


    }
}
