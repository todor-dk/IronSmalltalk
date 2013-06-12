using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.CodeGeneration.BindingScopes;
using IronSmalltalk.Runtime.CodeGeneration.Visiting;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.InterchangeInstaller.Runtime
{
    public abstract class RuntimeCompiledInitializer : CompiledInitializer
    {
        public InitializerNode ParseTree { get; private set; }

        public IDebugInfoService DebugInfoService { get; private set; }

        protected RuntimeCompiledInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService)
            : base()
        {
            if (parseTree == null)
                throw new ArgumentNullException();
            this.ParseTree = parseTree;
            this.DebugInfoService = debugInfoService;
        }

        protected InitializerCompilationResult Compile(SmalltalkRuntime runtime, BindingScope globalScope, BindingScope reservedScope, string initializerName)
        {
            InitializerVisitor visitor = new InitializerVisitor(runtime, globalScope, reservedScope, initializerName, this.DebugInfoService);
            var code = this.ParseTree.Accept(visitor);
            return new InitializerCompilationResult(code, visitor.BindingRestrictions);
        }

        public override bool Validate(SmalltalkNameScope globalNameScope, IIntermediateCodeValidationErrorSink errorSink)
        {
            return IronSmalltalk.InterchangeInstaller.Runtime.RuntimeCompiledMethod.Validate(this.ParseTree, errorSink, () =>
            {
                return this.Compile(globalNameScope.Runtime, globalNameScope);
            });
        }

        private volatile Func<SmalltalkRuntime, object, object> _Delegate = null;

        public override object Execute(SmalltalkRuntime runtime, object self)
        {
            if (this._Delegate == null)
                System.Threading.Interlocked.CompareExchange(ref this._Delegate, this.NativeCompile(runtime), null);
            return this._Delegate(runtime, self);
        }

        private Func<SmalltalkRuntime, object, object> NativeCompile(SmalltalkRuntime runtime)
        {
            return this.Compile(runtime).ExecutableCode.Compile();
        }
    }

    public class RuntimeProgramInitializer : RuntimeCompiledInitializer
    {
        public RuntimeProgramInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService)
            : base(parseTree, debugInfoService)
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
        public string GlobalName { get; private set; }

        public RuntimeGlobalInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, string globalName)
            : base(parseTree, debugInfoService)
        {
            if (String.IsNullOrWhiteSpace(globalName))
                throw new ArgumentNullException("globalName");
            this.GlobalName = globalName;
        }

        protected override InitializerCompilationResult Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
        {
            ClassBinding cls = globalScope.GetClassBinding(this.GlobalName);
            if ((cls != null) && (cls.Value != null))
                return this.CompileClassInitializer(runtime, globalScope, cls.Value);
            else
                return this.CompileGlobalInitializer(runtime, globalScope);
        }

        private InitializerCompilationResult CompileGlobalInitializer(SmalltalkRuntime runtime, SmalltalkNameScope globalNameScope)
        {
            return this.Compile(runtime,
                BindingScope.ForGlobalInitializer(globalNameScope),
                ReservedScope.ForGlobalInitializer(),
                String.Format("{0} initializer", this.GlobalName));
        }

        private InitializerCompilationResult CompileClassInitializer(SmalltalkRuntime runtime, SmalltalkNameScope globalNameScope, SmalltalkClass cls)
        {
            return this.Compile(runtime,
                BindingScope.ForClassInitializer(cls, globalNameScope),
                ReservedScope.ForClassInitializer(),
                String.Format("{0} initializer", this.GlobalName));
        }
    }

    public class RuntimePoolItemInitializer : RuntimeCompiledInitializer
    {
        public string PoolName { get; private set; }
        public string PoolItemName { get; private set; }

        public RuntimePoolItemInitializer(InitializerNode parseTree, IDebugInfoService debugInfoService, string poolName, string poolItemName)
            : base(parseTree, debugInfoService)
        {
            if (String.IsNullOrWhiteSpace(poolName))
                throw new ArgumentNullException("poolName");
            if (String.IsNullOrWhiteSpace(poolItemName))
                throw new ArgumentNullException("poolItemName");
            this.PoolName = poolName;
            this.PoolItemName = poolItemName;
        }

        protected override InitializerCompilationResult Compile(SmalltalkRuntime runtime, SmalltalkNameScope globalScope)
        {
            PoolBinding poolBinding = globalScope.GetPoolBinding(this.PoolName);
            if ((poolBinding == null) || (poolBinding.Value == null))
                throw new RuntimeCodeGenerationException(String.Format("Cannot find pool named {0}", this.PoolName)); // May be better exception type

            return this.Compile(runtime,
                BindingScope.ForPoolInitializer(poolBinding.Value, globalScope),
                ReservedScope.ForPoolInitializer(),
                String.Format("{0} initializerFor: {1}", this.PoolName, this.PoolItemName));
        }
    }
}
