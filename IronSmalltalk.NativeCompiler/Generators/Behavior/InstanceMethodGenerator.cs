using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.NativeCompiler.Generators.Globals;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.NativeCompiler.Generators.Behavior
{
    internal sealed class InstanceMethodGenerator : MethodGenerator
    {
        internal static InstanceMethodGenerator CreateAndPrepareGenerator(ClassGenerator classGenerator)
        {
            InstanceMethodGenerator generator = new InstanceMethodGenerator(
                classGenerator.Compiler,
                classGenerator.Binding.Value,
                classGenerator.Binding.Value.InstanceBehavior);
            generator.PrepareGenerator();
            return generator;
        }

        private InstanceMethodGenerator(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods)
            : base(compiler, cls, methods)
        {
        }

        protected override MethodCompiler GetMethodCompiler(MethodInformation method)
        {
            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = method.Method.GetDebugInfoService();
            options.LiteralEncodingStrategy = this.LiteralEncodingStrategy;
            options.DynamicCallStrategy = this.DynamicCallStrategy;
            return new InstanceMethodCompiler(this.Compiler.Parameters.Runtime, options);
        }

        protected override string TypeName
        {
            get { return this.Class.Name.Value; }
        }

        protected override string InitMethodDictionariesMethodName
        {
            get { return String.Format("Init_{0}_InstanceMethods", this.Class.Name.Value); }
        }

        private static readonly MethodInfo AddInstanceMethodMethod = TypeUtilities.Method(typeof(NativeLoadHelper), "AddInstanceMethod",
            typeof(Dictionary<Symbol, CompiledMethod>), typeof(SmalltalkClass), typeof(string), typeof(Type), typeof(string));

        protected override System.Reflection.MethodInfo AddMethodMethod
        {
            get { return InstanceMethodGenerator.AddInstanceMethodMethod; }
        }
    }
}
