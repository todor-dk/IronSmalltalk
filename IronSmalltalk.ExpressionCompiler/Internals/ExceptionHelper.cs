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

using IronSmalltalk.Compiler.SemanticNodes;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public static class ExceptionHelper
    {
        private static readonly object NodeKey = new object();

        public static SemanticNode GetNode(this IronSmalltalk.Runtime.Execution.Internals.CodeGenerationException exception)
        {
            if (exception == null)
                return null;
            return exception.Data[ExceptionHelper.NodeKey] as SemanticNode;
        }

        public static TException SetNode<TException>(this TException exception, SemanticNode node)
            where TException : IronSmalltalk.Runtime.Execution.Internals.CodeGenerationException 
        {
            if (exception == null)
                return null;
            exception.Data[ExceptionHelper.NodeKey] = node;
            return exception;
        }


    }
}
