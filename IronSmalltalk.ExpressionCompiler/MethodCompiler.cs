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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.ExpressionCompiler
{
    public abstract class MethodCompiler : ExpressionCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="globalScope">Binding lookup scope with global identifiers, e.g. globals, class variables, instance variables etc.</param>
        /// <param name="reservedScope">Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.</param>
        /// <param name="debugInfoService">Optional debug info service if the generator is to emit debug symbols.</param>
        protected MethodCompiler(SmalltalkRuntime runtime, BindingScope globalScope, BindingScope reservedScope, IDebugInfoService debugInfoService)
            : base(runtime, globalScope, reservedScope, debugInfoService)
        {
        }

        public MethodCompilationResult CompileMethod(MethodNode parseTree, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments)
        {
            if (parseTree == null)
                throw new ArgumentNullException("parseTree");
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (self == null)
                throw new ArgumentNullException("self");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            VisitingContext context = new VisitingContext(this, (ParameterExpression)self.Expression, (ParameterExpression)arguments[0].Expression, cls.Name);
            MethodVisitor visitor = new MethodVisitor(context, self, arguments);
            Expression code = parseTree.Accept(visitor);
            return new MethodCompilationResult(code, context.BindingRestrictions);
        }
    }

    public sealed class InstanceMethodCompiler : MethodCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="globalScope">Binding lookup scope with global identifiers, e.g. globals, class variables, instance variables etc.</param>
        /// <param name="reservedScope">Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.</param>
        /// <param name="debugInfoService">Optional debug info service if the generator is to emit debug symbols.</param>
        public InstanceMethodCompiler(SmalltalkRuntime runtime, BindingScope globalScope, BindingScope reservedScope, IDebugInfoService debugInfoService)
            : base(runtime, globalScope, reservedScope, debugInfoService)
        {
        
        }
    }

    public sealed class ClassMethodCompiler : MethodCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="globalScope">Binding lookup scope with global identifiers, e.g. globals, class variables, instance variables etc.</param>
        /// <param name="reservedScope">Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.</param>
        /// <param name="debugInfoService">Optional debug info service if the generator is to emit debug symbols.</param>
        public ClassMethodCompiler(SmalltalkRuntime runtime, BindingScope globalScope, BindingScope reservedScope, IDebugInfoService debugInfoService)
            : base(runtime, globalScope, reservedScope, debugInfoService)
        {
        
        }
    }
}
