using IronSmalltalk.Common.Internal;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.NativeCompiler.CompilationStrategies;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.Generators.Initializers
{
    internal sealed class ClassInitializerGenerator : InitializerGenerator<RuntimeGlobalInitializer>
    {
        public ClassInitializerGenerator(NativeCompiler compiler, RuntimeGlobalInitializer initializer)
            : base(compiler, initializer)
        {
        }

        protected override string GetInitializerNameSuggestion()
        {
            return String.Format("{0}_Class_Initializer", this.Initializer.Binding.Name.Value);
        }

        protected override InitializerCompiler GetInitializerCompiler(NativeLiteralEncodingStrategy nativeLiteralEncodingStrategy, NativeDynamicCallStrategy nativeDynamicCallStrategy)
        {
            BindingScope globalScope = BindingScope.ForClassInitializer((SmalltalkClass)this.Initializer.Binding.Value, this.Compiler.Parameters.Runtime.GlobalScope);
            BindingScope reservedScope = ReservedScope.ForClassInitializer();
            return this.GetInitializerCompiler(globalScope, reservedScope, nativeLiteralEncodingStrategy, nativeDynamicCallStrategy);
        }

        protected override MethodCallExpression GenerateInitializerCall(ParameterExpression runtime, ParameterExpression scope, ParameterExpression initializersType)
        {
            return Expression.Call(ClassInitializerGenerator.AddClassInitializerMethod,
                runtime,
                scope,
                initializersType,
                Expression.Constant(this.MethodName, typeof(string)),
                Expression.Constant(this.Initializer.Binding.Name.Value, typeof(string)));
        }

        private static readonly MethodInfo AddClassInitializerMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AddClassInitializer",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(Type), typeof(string), typeof(string));
    }
}
