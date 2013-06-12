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

using System;
using System.Runtime.Serialization;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Runtime.CodeGeneration.Bindings;

namespace IronSmalltalk.ExpressionCompiler.Internals
{
    [Serializable]
    public class BindingCodeGeneraionException : IronSmalltalk.Runtime.Execution.Internals.SemanticCodeGenerationException
    {
        public BindingCodeGeneraionException() { }
        public BindingCodeGeneraionException(string message) : base(message) { }
        public BindingCodeGeneraionException(string message, Exception inner) : base(message, inner) { }
#if !SILVERLIGHT
        protected BindingCodeGeneraionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
        public BindingCodeGeneraionException(string message, SemanticNode node)
            : this(message)
        {
            this.Node = node;
        }
        public BindingCodeGeneraionException(NameBinding binding, SemanticNode node)
            : this((binding is IErrorBinding) ? ((IErrorBinding)binding).ErrorDescription : CodeGenerationErrors.UndefinedBinding, node)
        {
        }

        [NonSerialized]
        public SemanticNode Node;
    }
}