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
    public class RuntimeErrors
    {
        public const string DoesNotUnderstandMissing = "Could not find the #_doesNotUnderstand:arguments: method.";
        public const string TooManyMethodArguments = "IronSmalltalk does not support methods with {0} argument. Maximum argument number is {1}.";
    }
}
