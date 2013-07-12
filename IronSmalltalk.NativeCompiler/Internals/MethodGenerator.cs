using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq.Expressions;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.Compiler.SemanticNodes;
using System.Runtime.CompilerServices;
using IronSmalltalk.Runtime.Execution;
using System.Reflection;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.ExpressionCompiler.BindingScopes;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal abstract class MethodGenerator : GeneratorBase
    {
        private readonly MethodDictionary Methods;
        private readonly SmalltalkClass Class;
        private readonly TypeBuilder TypeBuilder;

        protected MethodGenerator(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods, TypeBuilder typeBuilder)
            : base(compiler)
        {
            this.Class = cls;
            this.Methods = methods;
            this.TypeBuilder = typeBuilder;
        }

        private MethodCompiler _MethodCompiler;
        protected MethodCompiler MethodCompiler
        {
            get
            {
                if (this._MethodCompiler == null)
                    this._MethodCompiler = this.GetMethodCompiler();
                return this._MethodCompiler;
            }
        }

        protected abstract MethodCompiler GetMethodCompiler();

        protected void GenerateMethods()
        {
            List<MethodInformation> methods = this.GetMethodNameMap();
            foreach (MethodInformation method in methods)
            {
                this.GenerateMethod(method);
            }
        }

        private void GenerateMethod(MethodInformation method)
        {
            MethodBuilder methodBuilder = this.TypeBuilder.DefineMethod(method.MethodName, MethodAttributes.Public | MethodAttributes.Static);
            LambdaExpression lambda = this.GenerateMethodLambda(method);
            try
            {
                lambda.CompileToMethod(methodBuilder, this.Compiler.NativeGenerator.DebugInfoGenerator);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} {1}", lambda, ex);
            }
        }

        private LambdaExpression GenerateMethodLambda(MethodInformation method)
        {
            return this.MethodCompiler.CompileBindDelegate(method.Method.ParseTree, this.Class, method.MethodName);
        }

        /// <summary>
        /// Generates a map of unuque method names and the corresponding compiled-methods.
        /// </summary>
        /// <returns></returns>
        private List<MethodInformation> GetMethodNameMap()
        {
            Dictionary<string, MethodInformation> map = new Dictionary<string, MethodInformation>();

            foreach (CompiledMethod method in this.Methods.Values)
            {
                RuntimeCompiledMethod runtimeMethod = method as RuntimeCompiledMethod;
                if (runtimeMethod == null)
                    throw new Exception("Expected to see a RuntimeCompiledMethod"); // ... since we haved compiled everything ourself.
                string name = MethodGenerator.GetUniqueName(map, this.Compiler.NativeGenerator.AsLegalMethodName(method.Selector.Value));
                map.Add(name, MethodInformation.CreateMethodInformation(this.Compiler, name, runtimeMethod));
            }

            return map.Values.ToList();
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

        private class MethodInformation
        {
            public static MethodInformation CreateMethodInformation(NativeCompiler compiler, string name, RuntimeCompiledMethod method)
            {
                return new MethodInformation(compiler, name, method);
            }

            public readonly NativeCompiler Compiler;
            public readonly RuntimeCompiledMethod Method;
            public readonly string MethodName;

            private MethodInformation(NativeCompiler compiler, string name, RuntimeCompiledMethod method)
            {
                this.Compiler = compiler;
                this.Method = method;
                this.MethodName = name;
            }
        }
    }

    internal sealed class ClassMethodGenerator : MethodGenerator
    {
        internal static ClassMethodGenerator GenerateMethods(ClassGenerator classGenerator)
        {
            ClassMethodGenerator generator = new ClassMethodGenerator(
                classGenerator.Compiler,
                classGenerator.Binding.Value,
                classGenerator.Binding.Value.ClassBehavior,
                classGenerator.ClassMethodsType);
            generator.GenerateMethods();
            return generator;
        }

        private ClassMethodGenerator(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods, TypeBuilder typeBuilder)
            : base(compiler, cls, methods, typeBuilder)
        {
        }

        protected override MethodCompiler GetMethodCompiler()
        {
            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = null;    // BUG-BUG 
            options.LiteralEncodingStrategy = new NativeLiteralEncodingStrategy();
            options.DynamicCallStrategy = new NativeDynamicCallStrategy();

            return new ClassMethodCompiler(this.Compiler.Parameters.Runtime, options); 
        }
    }

    internal sealed class InstanceMethodGenerator : MethodGenerator
    {
        internal static InstanceMethodGenerator GenerateMethods(ClassGenerator classGenerator)
        {
            InstanceMethodGenerator generator = new InstanceMethodGenerator(
                classGenerator.Compiler, 
                classGenerator.Binding.Value, 
                classGenerator.Binding.Value.InstanceBehavior, 
                classGenerator.InstanceMethodsType);
            generator.GenerateMethods();
            return generator;
        }

        private InstanceMethodGenerator(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods, TypeBuilder typeBuilder)
            : base(compiler, cls, methods, typeBuilder)
        {
        }

        protected override MethodCompiler GetMethodCompiler()
        {
            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = null;    // BUG-BUG 
            options.LiteralEncodingStrategy = new NativeLiteralEncodingStrategy();
            options.DynamicCallStrategy = new NativeDynamicCallStrategy();

            return new InstanceMethodCompiler(this.Compiler.Parameters.Runtime, options);
        }
    }
}
