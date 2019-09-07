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
using System.Globalization;
using System.Linq.Expressions;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.ExpressionCompiler.Runtime
{
    public abstract class RuntimeCompiledInitializer : CompiledInitializer
    {
        public InitializerNode ParseTree { get; private set; }

        public IDebugInfoService DebugInfoService { get; private set; }

        protected RuntimeCompiledInitializer(InitializerType type, IDiscreteBinding binding, InitializerNode parseTree, IDebugInfoService debugInfoService)
            : base(type, binding)
        {
            if (parseTree == null)
                throw new ArgumentNullException();
            this.ParseTree = parseTree;
            this.DebugInfoService = debugInfoService;
        }

        public Expression<Func<object, ExecutionContext, object>> Compile(SmalltalkRuntime runtime)
        {
            return this.Compile(runtime, runtime.GlobalScope);
        }

        protected abstract Expression<Func<object, ExecutionContext, object>> Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope);

        protected Expression<Func<object, ExecutionContext, object>> Compile(SmalltalkRuntime runtime, BindingScope globalScope, BindingScope reservedScope, string initializerName)
        {
            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = this.GetDebugInfoService();

            InitializerCompiler compiler = new InitializerCompiler(runtime, options, globalScope, reservedScope);
            return compiler.CompileInitializer(this.ParseTree, initializerName);
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
            return RuntimeCompiledMethod.Validate(this.ParseTree, errorSink, () => this.Compile(globalNameScope.Runtime, globalNameScope));
        }

        private volatile Func<object, ExecutionContext, object> _Delegate = null;

        public override object Execute(object self, ExecutionContext executionContext)
        {
            if (this._Delegate == null)
                System.Threading.Interlocked.CompareExchange(ref this._Delegate, this.NativeCompile(executionContext.Runtime), null);
            return this._Delegate(self, executionContext);
        }

        private Func<object, ExecutionContext, object> NativeCompile(SmalltalkRuntime runtime)
        {
            return this.Compile(runtime).Compile();
        }
    }

    public sealed class RuntimeProgramInitializer : RuntimeCompiledInitializer
    {
        public RuntimeProgramInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService)
            : base(InitializerType.ProgramInitializer, null, parseTree, debugInfoService)
        {
        }

        protected override Expression<Func<object, ExecutionContext, object>> Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
        {
            return this.Compile(runtime,
                BindingScope.ForProgramInitializer(globalScope),
                ReservedScope.ForProgramInitializer(),
                "Global initializer");
        }
    }

    public sealed class RuntimeGlobalInitializer : RuntimeCompiledInitializer
    {

        public RuntimeGlobalInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, GlobalVariableOrConstantBinding binding)
            : base(InitializerType.GlobalInitializer, binding, parseTree, debugInfoService)
        {
        }


        public RuntimeGlobalInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, ClassBinding binding)
            : base(InitializerType.ClassInitializer, binding, parseTree, debugInfoService)
        {
        }

        protected override Expression<Func<object, ExecutionContext, object>> Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
        {
            if (this.Type == InitializerType.ClassInitializer)
                return this.CompileClassInitializer(runtime, globalScope, (SmalltalkClass) this.Binding.Value);
            else
                return this.CompileGlobalInitializer(runtime, globalScope);
        }

        private Expression<Func<object, ExecutionContext, object>> CompileGlobalInitializer(SmalltalkRuntime runtime, SmalltalkNameScope globalNameScope)
        {
            return this.Compile(runtime,
                BindingScope.ForGlobalInitializer(globalNameScope),
                ReservedScope.ForGlobalInitializer(),
                String.Format("{0} initializer", this.Binding.Name.Value));
        }

        private Expression<Func<object, ExecutionContext, object>> CompileClassInitializer(SmalltalkRuntime runtime, SmalltalkNameScope globalNameScope, SmalltalkClass cls)
        {
            return this.Compile(runtime,
                BindingScope.ForClassInitializer(cls, globalNameScope),
                ReservedScope.ForClassInitializer(),
                String.Format("{0} initializer", this.Binding.Name.Value));
        }
    }

    public sealed class RuntimePoolItemInitializer : RuntimeCompiledInitializer
    {
        public string PoolName { get; private set; }

        public RuntimePoolItemInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, PoolVariableOrConstantBinding binding, string poolName)
            : base(InitializerType.PoolVariableInitializer, binding, parseTree, debugInfoService)
        {
            if (String.IsNullOrWhiteSpace(poolName))
                throw new ArgumentNullException("poolName");
            this.PoolName = poolName;
        }

        protected override Expression<Func<object, ExecutionContext, object>> Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
        {
            PoolBinding poolBinding = globalScope.GetPoolBinding(this.PoolName);
            if ((poolBinding == null) || (poolBinding.Value == null))
                throw new RuntimeCodeGenerationException(String.Format("Cannot find pool named {0}", this.PoolName)); // May be better exception type

            return this.Compile(runtime,
                BindingScope.ForPoolInitializer(poolBinding.Value, globalScope),
                ReservedScope.ForPoolInitializer(),
                String.Format("{0} initializerFor: {1}", this.PoolName, this.Binding.Name.Value));
        }
    }
}
