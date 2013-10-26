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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.ExpressionCompiler
{
    public abstract class MethodCompiler : ExpressionCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="compilerOptions">Options that control the workings of the compiler.</param>
        protected MethodCompiler(SmalltalkRuntime runtime, CompilerOptions compilerOptions)
            : base(runtime, compilerOptions)
        {
        }

        public Expression CompileMethod(MethodNode parseTree, SmalltalkClass cls, Expression self, Expression executionContext, Expression[] arguments)
        {
            if (parseTree == null)
                throw new ArgumentNullException("parseTree");
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (self == null)
                throw new ArgumentNullException("self");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            RootCompilationContext context = this.GetCompilationContext(parseTree, cls, self, executionContext, arguments);
            MethodVisitor visitor = new MethodVisitor(context);
            Expression code = parseTree.Accept(visitor);
            return code;
        }

        protected abstract RootCompilationContext GetCompilationContext(MethodNode parseTree, SmalltalkClass cls, Expression self, Expression executionContext, Expression[] arguments);

        public LambdaExpression CompileMethodLambda(MethodNode parseTree, SmalltalkClass cls, string methodName)
        {
            if (parseTree == null)
                throw new ArgumentNullException("parseTree");

            // Declare parameter expression for each argument ....
            ParameterExpression[] arguments = MethodCompiler.CreateParametersForMethod(parseTree);

            // Compile the method !!!
            Expression body = this.CompileMethod(parseTree, cls, arguments[0], arguments[1], arguments.Skip(2).Cast<Expression>().ToArray());

            // Finally, create a lambda expression out of it.
            return Expression.Lambda(
                body,
                methodName,
                true, // always compile the rules with tail call optimization,
                arguments);
        }

        private static ParameterExpression[] CreateParametersForMethod(MethodNode parseTree)
        {
            ParameterExpression[] args = new ParameterExpression[parseTree.Arguments.Count + 2];
            Dictionary<string, ParameterExpression> argsMap = new Dictionary<string, ParameterExpression>();

            string name = MethodCompiler.GetUniqueName(argsMap, "self");
            ParameterExpression param = Expression.Parameter(typeof(object), name); // All our args are Object
            argsMap.Add(name, param);
            args[0] = param;

            for (int i = 0; i < parseTree.Arguments.Count; i++)
            {
                MethodArgumentNode arg = parseTree.Arguments[i];
                name = MethodCompiler.GetUniqueName(argsMap, arg.Token.Value);
                param = Expression.Parameter(typeof(object), name); // All our args are Object
                argsMap.Add(name, param);
                args[i + 2] = param;
            }

            // Those are not used by code, and we define them last, just in case there are naming conflicts - the name of those is unimportant.
            name = MethodCompiler.GetUniqueName(argsMap, "$executionContext");
            param = Expression.Parameter(typeof(ExecutionContext), name);
            argsMap.Add(name, param);
            args[1] = param;

            return args;
        }

        private static string GetUniqueName<TItem>(IDictionary<string, TItem> map, string name)
        {
            string suggestion = name;
            int idx = 1;
            while (map.ContainsKey(suggestion))
            {
                suggestion = String.Format(CultureInfo.InvariantCulture, "{0}{1}", suggestion, idx);
                idx++;
            }
            return suggestion;
        }
    }
}
