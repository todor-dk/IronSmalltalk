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
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.ExpressionCompiler.Runtime
{
    public sealed class RuntimeCompiledMethod : CompiledMethod
    {
        public MethodNode ParseTree { get; private set; }

        public IDebugInfoService DebugInfoService { get; private set; }

        public RuntimeCompiledMethod(SmalltalkClass cls, Symbol selector, MethodType methodType, MethodNode parseTree, IDebugInfoService debugInfoService)
            : base(cls, selector, methodType)
        {
            if (parseTree == null)
                throw new ArgumentNullException();
            this.ParseTree = parseTree;
            this.DebugInfoService = debugInfoService;
        }

        public override Expression GetExpression(Expression self, Expression executionContext, IEnumerable<Expression> arguments)
        {
            return this.GetExpression(self, executionContext, arguments, this.Class.Runtime.GlobalScope);
        }

        private Expression GetExpression(Expression self, Expression executionContext, IEnumerable<Expression> arguments, SmalltalkNameScope globalNameScope)
        {
            Symbol superScope = (this.Class.Superclass == null) ? null : this.Class.Superclass.Name;
            if (this.Type == MethodType.Class)
                return this.CompileClassMethod(self, executionContext, arguments.ToArray(), superScope, globalNameScope);
            else
                return this.CompileInstanceMethod(self, executionContext, arguments.ToArray(), superScope, globalNameScope);
        }

        private Expression CompileClassMethod(Expression self, Expression executionContext, Expression[] arguments, Symbol superScope, SmalltalkNameScope globalNameScope)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (executionContext == null)
                throw new ArgumentNullException("executionContext");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = this.GetDebugInfoService();
            options.GlobalNameScope = globalNameScope;

            ClassMethodCompiler compiler = new ClassMethodCompiler(this.Class.Runtime, options);

            return compiler.CompileMethod(this.ParseTree, this.Class, self, executionContext, arguments);
        }

        private Expression CompileInstanceMethod(Expression self, Expression executionContext, Expression[] arguments, Symbol superScope, SmalltalkNameScope globalNameScope)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (executionContext == null)
                throw new ArgumentNullException("executionContext");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = this.GetDebugInfoService();
            options.GlobalNameScope = globalNameScope;

            InstanceMethodCompiler compiler = new InstanceMethodCompiler(this.Class.Runtime, options);
            return compiler.CompileMethod(this.ParseTree, this.Class, self, executionContext, arguments);
        }

        public IDebugInfoService GetDebugInfoService()
        {
            if (this.DebugInfoService == null)
                return null;
            if (this.DebugInfoService.SymbolDocument == null)
                return null;
            return this.DebugInfoService;
        }

        public bool Validate(SmalltalkNameScope globalNameScope, IRuntimeCodeValidationErrorSink errorSink)
        {
            return RuntimeCompiledMethod.Validate(this.ParseTree, errorSink, () =>
            {
                Expression self = Expression.Parameter(typeof(object), "self");
                Expression executionContext = Expression.Parameter(typeof(ExecutionContext), "$executionContext");
                Expression[] args = new Expression[this.ParseTree.Arguments.Count];
                for (int i = 0; i < args.Length; i++)
                    args[i] = Expression.Parameter(typeof(object), String.Format("arg{0}", i+1));

                return this.GetExpression(self, executionContext, args, globalNameScope);
            });
        }

        internal static bool Validate<TResult>(SemanticNode rootNode, IRuntimeCodeValidationErrorSink errorSink, Func<TResult> compileFunc)
        {
            if (rootNode == null)
                throw new ArgumentNullException("rootNode");
            if (compileFunc == null)
                throw new ArgumentNullException("compileFunc");

            try
            {
                TResult result = compileFunc();
                if (result == null)
                    return RuntimeCompiledMethod.ReportError(errorSink, rootNode, "Could not compile method");
                return true;
            }
            catch (IronSmalltalk.ExpressionCompiler.Primitives.Exceptions.PrimitiveInvalidTypeException ex)
            {
                return RuntimeCompiledMethod.ReportError(errorSink, ex.GetNode(), ex.Message);
            }
            catch (SemanticCodeGenerationException ex)
            {
                return RuntimeCompiledMethod.ReportError(errorSink, ex.GetNode(), ex.Message);
            }
            catch (IronSmalltalk.Runtime.Internal.SmalltalkDefinitionException ex)
            {
                return RuntimeCompiledMethod.ReportError(errorSink, rootNode, ex.Message);
            }
            catch (IronSmalltalk.Runtime.Internal.SmalltalkRuntimeException ex)
            {
                return RuntimeCompiledMethod.ReportError(errorSink, rootNode, ex.Message);
            }
            //catch (Exception ex)
            //{
            //    return RuntimeCompiledMethod.ReportError(errorSink, rootNode, ex.Message);
            //}
        }

        private static bool ReportError(IRuntimeCodeValidationErrorSink errorSink, SemanticNode node, string errorMessage)
        {
            if (errorSink != null)
            {
                SourceLocation start = SourceLocation.Invalid;
                SourceLocation end = SourceLocation.Invalid;
                if (node != null)
                {
                    var tokens = node.GetTokens();
                    if ((tokens != null) && tokens.Any())
                    {
                        start = tokens.Min(t => t.StartPosition);
                        end = tokens.Max(t => t.StopPosition);
                    }
                    foreach (var sn in node.GetChildNodes())
                    {
                        tokens = sn.GetTokens();
                        if ((tokens != null) && tokens.Any())
                        {
                            start = start.Min(tokens.Min(t => t.StartPosition));
                            end = end.Max(tokens.Max(t => t.StopPosition));
                        }
                    }
                }

                errorSink.ReportError(errorMessage, start, end);
            }
            return false;
        }

    }
}
