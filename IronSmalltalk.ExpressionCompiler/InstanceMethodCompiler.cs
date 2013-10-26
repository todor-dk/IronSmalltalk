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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler
{
    public sealed class InstanceMethodCompiler : MethodCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="compilerOptions">Options that control the workings of the compiler.</param>
        public InstanceMethodCompiler(SmalltalkRuntime runtime, CompilerOptions compilerOptions)
            : base(runtime, compilerOptions)
        {
        }

        protected override RootCompilationContext GetCompilationContext(MethodNode parseTree, SmalltalkClass cls, Expression self, Expression executionContext, Expression[] arguments)
        {
            SmalltalkNameScope globalNameScope = this.CompilerOptions.GlobalNameScope ?? this.Runtime.GlobalScope;

            BindingScope globalScope = BindingScope.ForInstanceMethod(cls, globalNameScope);
            BindingScope reservedScope = (cls.Superclass == null) ?
                ReservedScope.ForRootClassInstanceMethod() :
                ReservedScope.ForInstanceMethod();

            return new RootCompilationContext(this, globalScope, reservedScope, self, executionContext, arguments, cls.Name, parseTree.Selector);
        }
    }
}
