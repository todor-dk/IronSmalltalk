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

namespace IronSmalltalk.ExpressionCompiler.Runtime
{
    public sealed class RuntimeCompiledMethod : CompiledMethod
    {
        public MethodNode ParseTree { get; private set; }

        public IDebugInfoService DebugInfoService { get; private set; }

        public RuntimeCompiledMethod(Symbol selector, MethodNode parseTree, IDebugInfoService debugInfoService)
            : base(selector)
        {
            if (parseTree == null)
                throw new ArgumentNullException();
            this.ParseTree = parseTree;
            this.DebugInfoService = debugInfoService;
        }

        public override MethodCompilationResult CompileClassMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope)
        {
            return this.CompileClassMethod(runtime, cls, self, arguments, superScope, runtime.GlobalScope);
        }

        private MethodCompilationResult CompileClassMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope, SmalltalkNameScope globalNameScope)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (self == null)
                throw new ArgumentNullException("self");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = this.GetDebugInfoService();
            options.GlobalNameScope = globalNameScope;

            ClassMethodCompiler compiler = new ClassMethodCompiler(runtime, options);

            return compiler.CompileMethod(this.ParseTree, cls, self, arguments);


            //MethodVisitor visitor = new MethodVisitor(runtime,
            //    BindingScope.ForClassMethod(cls, globalNameScope),
            //    ReservedScope.ForClassMethod(self.Expression),
            //    self,
            //    arguments,
            //    cls.Name,
            //    this.GetDebugInfoService());
            ////visitor.SymbolDocument = Expression.SymbolDocument("<mod>", new Guid("{E1A254E3-FD2E-4D82-BAB3-D4E9B115154E}"), new Guid("{6A28E03C-E404-4190-A012-72B2CCE48DD5}"));
            //Expression code = this.ParseTree.Accept(visitor);
            //return new MethodCompilationResult(code, visitor.BindingRestrictions);
        }

        public IDebugInfoService GetDebugInfoService()
        {
            if (this.DebugInfoService == null)
                return null;
            if (this.DebugInfoService.SymbolDocument == null)
                return null;
            return this.DebugInfoService;
        }

        public override MethodCompilationResult CompileInstanceMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope)
        {
            return this.CompileInstanceMethod(runtime, cls, self, arguments, superScope, runtime.GlobalScope);
        }

        private MethodCompilationResult CompileInstanceMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope, SmalltalkNameScope globalNameScope)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (self == null)
                throw new ArgumentNullException("self");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = this.GetDebugInfoService();
            options.GlobalNameScope = globalNameScope;

            InstanceMethodCompiler compiler = new InstanceMethodCompiler(runtime, options);

            return compiler.CompileMethod(this.ParseTree, cls, self, arguments);
            //MethodVisitor visitor = new MethodVisitor(runtime,
            //    BindingScope.ForInstanceMethod(cls, globalNameScope),
            //    (cls.Superclass == null) ?
            //        ReservedScope.ForRootClassInstanceMethod(self.Expression) :
            //        ReservedScope.ForInstanceMethod(self.Expression),
            //    self,
            //    arguments,
            //    cls.Name,
            //    this.GetDebugInfoService());
            //Expression code = this.ParseTree.Accept(visitor);
            //return new MethodCompilationResult(code, visitor.BindingRestrictions);
        }


        public bool ValidateInstanceMethod(SmalltalkClass cls, SmalltalkNameScope globalNameScope, IRuntimeCodeValidationErrorSink errorSink)
        {
            return this.Validate(cls, errorSink, (r, c, s, a, u) => this.CompileInstanceMethod(r, c, s, a, u, globalNameScope));
        }

        public bool ValidateClassMethod(SmalltalkClass cls, SmalltalkNameScope globalNameScope, IRuntimeCodeValidationErrorSink errorSink)
        {
            return this.Validate(cls, errorSink, (r, c, s, a, u) => this.CompileClassMethod(r, c, s, a, u, globalNameScope));
        }

        private bool Validate(SmalltalkClass cls, IRuntimeCodeValidationErrorSink errorSink, Func<SmalltalkRuntime, SmalltalkClass, DynamicMetaObject, DynamicMetaObject[], Symbol, MethodCompilationResult> func)
        {
            return RuntimeCompiledMethod.Validate(this.ParseTree, errorSink, delegate()
            {
                DynamicMetaObject self = new DynamicMetaObject(Expression.Parameter(typeof(object), "self"), BindingRestrictions.Empty, null);
                DynamicMetaObject[] args = new DynamicMetaObject[this.ParseTree.Arguments.Count+1];
                for (int i = 0; i < args.Length; i++)
                    args[i] = new DynamicMetaObject(Expression.Parameter(typeof(object), String.Format("arg{0}", i)), BindingRestrictions.Empty, null);

                return func(cls.Runtime, cls, self, args, null);
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
            catch (IronSmalltalk.ExpressionCompiler.Primitives.PrimitiveInvalidTypeException ex)
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
