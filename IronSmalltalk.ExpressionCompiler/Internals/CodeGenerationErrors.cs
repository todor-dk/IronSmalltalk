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


namespace IronSmalltalk.ExpressionCompiler.Internals
{
    /// <summary>
    /// Error messages the code generator may report if it encounters illegal code.
    /// </summary>
    internal static class CodeGenerationErrors
    {
        public const string InternalError = "Internal error! Some code that shouldn't fail failed!";

        public const string InvalidCode = "The parsed method or initializer definition is invalid and cannot be JIT-compiled.";
        public const string SuperNotFollowedByMessage = "'super' must be followed by a message.";
        public const string CodeAfterReturnStatement = "Unexpected code following return \"^\" statement.";
        public const string AssigningToConstant = "Cannot assign to constant.";
        public const string UndefinedBinding = "Undefined.";
        public const string UnexpectedCallingconvention = "Unexpected calling convention.";

        public const string PoolVariableNotUnique = "Duplicate pool variable or pool constant name found.";
        public const string WrongNumberOfParameters = "Wrong number of primitive call parameters.";
        public const string WrongShiftTypeName = "Shift primitive does not support type named '{0}'.";
        public const string MissingMethod = "Type '{0}' does not contain method named {1}.";
        public const string VarArgsCallingConventionNotSupported = "The VarArgs calling convention used by method '{1}' in type '{0}' is not supported.";
        public const string MissingProperty = "Type '{0}' does not contain property named {1}.";
        public const string MissingField = "Type '{0}' does not contain field named {1}.";
        public const string MissingConstructor = "Type '{0}' does not contain constructor with the given types.";
        public const string WrongTypeName = "Could not resolve type named '{0}'.";
        public const string WrongPrimitive = "Unrecognized primitive call '{0}'.";
    }
}
