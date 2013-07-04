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

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class MethodGenerator : GeneratorBase
    {
        internal static MethodGenerator GenerateMethods(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods, TypeBuilder typeBuilder)
        {
            MethodGenerator generator = new MethodGenerator(compiler, cls, methods, typeBuilder);
            generator.GenerateMethods();
            return generator;
        }

        private readonly MethodDictionary Methods;
        private readonly SmalltalkClass Class;
        private readonly TypeBuilder TypeBuilder;

        private MethodGenerator(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods, TypeBuilder typeBuilder)
            : base(compiler)
        {
            this.Class = cls;
            this.Methods = methods;
            this.TypeBuilder = typeBuilder;
        }

        private void GenerateMethods()
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
            lambda.CompileToMethod(methodBuilder, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

        private LambdaExpression GenerateMethodLambda(MethodInformation method)
        {
            Expression body = Expression.Constant(null, typeof(object));

            return Expression.Lambda(
                body,
                method.MethodName,
                true, // always compile the rules with tail call optimization,
                method.Arguments);
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
                ParameterExpression[] args = MethodInformation.CreateParametersForMethod(compiler, method);
                return new MethodInformation(compiler, name, method, args);
            }

            private static ParameterExpression[] CreateParametersForMethod(NativeCompiler compiler, RuntimeCompiledMethod method)
            {
                System.Diagnostics.Debug.Assert(method.ParseTree.Arguments.Count == method.NumberOfArguments);

                ParameterExpression[] args = new ParameterExpression[method.NumberOfArguments + 3];
                Dictionary<string, ParameterExpression> argsMap = new Dictionary<string, ParameterExpression>();

                string name = MethodGenerator.GetUniqueName(argsMap, "self");
                ParameterExpression param = Expression.Parameter(typeof(object), name); // All our args are Object
                argsMap.Add(name, param);
                args[1] = param;

                for (int i = 0; i < method.ParseTree.Arguments.Count; i++)
                {
                    MethodArgumentNode arg = method.ParseTree.Arguments[i];
                    name = MethodGenerator.GetUniqueName(argsMap, compiler.NativeGenerator.AsLegalArgumentName(arg.Token.Value));
                    param = Expression.Parameter(typeof(object), name); // All our args are Object
                    argsMap.Add(name, param);
                    args[i + 3] = param;
                }

                // Those are not used by code, and we define them last, just in case there are naming conflicts - the name of those is unimportant.
                name = MethodGenerator.GetUniqueName(argsMap, "$site");
                param = Expression.Parameter(typeof(CallSite), name);
                argsMap.Add(name, param);
                args[0] = param;
                name = MethodGenerator.GetUniqueName(argsMap, "$executionContext");
                param = Expression.Parameter(typeof(ExecutionContext), name);
                argsMap.Add(name, param);
                args[2] = param;

                return args;
            }

            public readonly NativeCompiler Compiler;
            public readonly RuntimeCompiledMethod Method;
            public readonly string MethodName;
            public readonly ParameterExpression[] Arguments;

            private MethodInformation(NativeCompiler compiler, string name, RuntimeCompiledMethod method, ParameterExpression[] arguments)
            {
                this.Compiler = compiler;
                this.Method = method;
                this.MethodName = name;
                this.Arguments = arguments;
            }
        }
    }
}
