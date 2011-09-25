using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronSmalltalk.Compiler.SemanticNodes;

namespace IronSmalltalk.AstJitCompiler.Internals
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
                return null; ;
            exception.Data[ExceptionHelper.NodeKey] = node;
            return exception;
        }


    }
}
