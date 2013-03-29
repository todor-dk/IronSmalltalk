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


namespace IronSmalltalk.Runtime.Execution.Internals
{
    // TO-DO ... rename to better name and refactor
    public class RuntimeCodeGenerationErrors
    {
        public const string UndefinedBinding = "Undefined.";
        public const string PoolVariableNotUnique = "Duplicate pool variable or pool constant name found.";
        public const string DoesNotUnderstandMissing = "Could not find the #_doesNotUnderstand:arguments: method.";
        public const string WrongNumberOfParameters = "Wrong number of primitive call parameters.";
        public const string WrongShiftTypeName = "Shift primitive does not support type named '{0}'.";
        public const string MissingMethod = "Type '{0}' does not contain method named {1}.";
        public const string MissingProperty = "Type '{0}' does not contain property named {1}.";
        public const string MissingField = "Type '{0}' does not contain field named {1}.";
        public const string MissingConstructor = "Type '{0}' does not contain constructor with the given types.";
        public const string WrongTypeName = "Could not resolve type named '{0}'.";
        public const string WrongPrimitive = "Unrecognized primitive call '{0}'.";
    }
}
