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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.AstJitCompiler.Runtime;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.CodeGeneration.BindingScopes;
using IronSmalltalk.Runtime.CodeGeneration.Visiting;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.InterchangeInstaller.Runtime
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

        public InitializerCompilationResult Compile(SmalltalkRuntime runtime)
        {
            return this.Compile(runtime, runtime.GlobalScope);
        }

        protected abstract InitializerCompilationResult Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope);
            
        protected InitializerCompilationResult Compile(SmalltalkRuntime runtime, BindingScope globalScope, BindingScope reservedScope, string initializerName)
        {
            InitializerVisitor visitor = new InitializerVisitor(runtime, globalScope, reservedScope, initializerName, this.DebugInfoService);
            var code = this.ParseTree.Accept(visitor);
            return new InitializerCompilationResult(code, visitor.BindingRestrictions);
        }

        public bool Validate(SmalltalkNameScope globalNameScope, IRuntimeCodeValidationErrorSink errorSink)
        {
            return IronSmalltalk.InterchangeInstaller.Runtime.RuntimeCompiledMethod.Validate(this.ParseTree, errorSink, () =>
            {
                return this.Compile(globalNameScope.Runtime, globalNameScope);
            });
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
            return this.Compile(runtime).ExecutableCode.Compile();
        }
    }

    public class RuntimeProgramInitializer : RuntimeCompiledInitializer
    {
        public RuntimeProgramInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService)
            : base(InitializerType.ProgramInitializer, null, parseTree, debugInfoService)
        {
        }

        protected override InitializerCompilationResult Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
        {
            return this.Compile(runtime,
                BindingScope.ForProgramInitializer(globalScope),
                ReservedScope.ForProgramInitializer(),
                "Global initializer");
        }
    }

    public class RuntimeGlobalInitializer : RuntimeCompiledInitializer
    {

        public RuntimeGlobalInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, GlobalVariableOrConstantBinding binding)
            : base(InitializerType.GlobalInitializer, binding, parseTree, debugInfoService)
        {
        }


        public RuntimeGlobalInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, ClassBinding binding)
            : base(InitializerType.ClassInitializer, binding, parseTree, debugInfoService)
        {
        }

        protected override InitializerCompilationResult Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
        {
            if (this.Type == InitializerType.ClassInitializer)
                return this.CompileClassInitializer(runtime, globalScope, (SmalltalkClass)this.Binding.Value);
            else
                return this.CompileGlobalInitializer(runtime, globalScope);
        }

        private InitializerCompilationResult CompileGlobalInitializer(SmalltalkRuntime runtime, SmalltalkNameScope globalNameScope)
        {
            return this.Compile(runtime,
                BindingScope.ForGlobalInitializer(globalNameScope),
                ReservedScope.ForGlobalInitializer(),
                String.Format("{0} initializer", this.Binding.Name.Value));
        }

        private InitializerCompilationResult CompileClassInitializer(SmalltalkRuntime runtime, SmalltalkNameScope globalNameScope, SmalltalkClass cls)
        {
            return this.Compile(runtime,
                BindingScope.ForClassInitializer(cls, globalNameScope),
                ReservedScope.ForClassInitializer(),
                String.Format("{0} initializer", this.Binding.Name.Value));
        }
    }

    public class RuntimePoolItemInitializer : RuntimeCompiledInitializer
    {
        public string PoolName { get; private set; }

        public RuntimePoolItemInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, PoolVariableOrConstantBinding binding, string poolName)
            : base(InitializerType.PoolVariableInitializer, binding, parseTree, debugInfoService)
        {
            if (String.IsNullOrWhiteSpace(poolName))
                throw new ArgumentNullException("poolName");
            this.PoolName = poolName;
        }

        protected override InitializerCompilationResult Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
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
