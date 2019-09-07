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

using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    public static class ExceptionHelper
    {
        private static readonly object ErrorLocationKey = new object();

        public static ErrorLocation GetErrorLocation(this IronSmalltalk.Runtime.Execution.Internals.CodeGenerationException exception)
        {
            if (exception == null)
                return null;
            return exception.Data[ExceptionHelper.ErrorLocationKey] as ErrorLocation;
        }

        public static TException SetErrorLocation<TException>(this TException exception, SemanticNode node)
            where TException : IronSmalltalk.Runtime.Execution.Internals.CodeGenerationException 
        {
            if (exception == null)
                return null;

            ErrorLocation errorLocation = null;
            if (node != null)
            {
                SourceLocation start = SourceLocation.Invalid;
                SourceLocation end = SourceLocation.Invalid;

                List<Compiler.LexicalTokens.IToken> allTokens = new List<Compiler.LexicalTokens.IToken>();
                var tokens = node.GetTokens();
                if (tokens != null)
                    allTokens.AddRange(tokens);
                foreach (var sn in node.GetChildNodes())
                {
                    tokens = sn.GetTokens();
                    if (tokens != null)
                        allTokens.AddRange(tokens);
                }


                if (allTokens.Count != 0)
                {
                    start = allTokens.Min(t => t.StartPosition);
                    end = allTokens.Max(t => t.StopPosition);
                }

                errorLocation = new ErrorLocation(start, end);
            }


            exception.Data[ExceptionHelper.ErrorLocationKey] = errorLocation;
            return exception;
        }
    }

    [Serializable]
    public class ErrorLocation
    {
        public SourceLocation Start { get; private set; }
        public SourceLocation End { get; private set; }

        public ErrorLocation(SourceLocation start, SourceLocation end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
