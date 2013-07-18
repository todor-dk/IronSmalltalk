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
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.ExpressionCompiler
{
    public class InitializerCompiler : ExpressionCompiler
    {
        /// <summary>
        /// Binding lookup scope for identifiers of globals and similar, e.g. global variables, class or instance variables, pool variables etc.
        /// </summary>
        public BindingScope GlobalScope { get; private set; }

        /// <summary>
        /// Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.
        /// </summary>
        public BindingScope ReservedScope { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="compilerOptions">Options that control the workings of the compiler.</param>
        /// <param name="globalScope">Binding lookup scope with global identifiers, e.g. globals, class variables, instance variables etc.</param>
        /// <param name="reservedScope">Binding lookup scope with reserved identifiers, e.g. "true", "false", "nil" etc.</param>
        public InitializerCompiler(SmalltalkRuntime runtime, CompilerOptions compilerOptions, BindingScope globalScope, BindingScope reservedScope)
            : base(runtime, compilerOptions)
        {
            if (globalScope == null)
                throw new ArgumentNullException("globalScope");
            if (reservedScope == null)
                throw new ArgumentNullException("reservedScope");
            this.GlobalScope = globalScope;
            this.ReservedScope = reservedScope;        
        }

        public InitializerCompilationResult CompileInitializer(InitializerNode parseTree, string initializerName)
        {
            if (parseTree == null)
                throw new ArgumentNullException("parseTree");

            ParameterExpression self = Expression.Parameter(typeof(object), "self");
            ParameterExpression executionContext = Expression.Parameter(typeof(ExecutionContext), "executionContext");
            VisitingContext context = new VisitingContext(this, this.GlobalScope, this.ReservedScope, self, executionContext, new ParameterExpression[0], null);
            InitializerVisitor visitor = new InitializerVisitor(context, initializerName);
            Expression<Func<object, ExecutionContext, object>> code = parseTree.Accept(visitor);
            return new InitializerCompilationResult(code, null);   
        }
    }
}
