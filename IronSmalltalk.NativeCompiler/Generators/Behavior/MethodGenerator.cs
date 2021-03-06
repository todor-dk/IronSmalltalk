﻿/*
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
using IronSmalltalk.NativeCompiler.Generators.Globals;
using IronSmalltalk.NativeCompiler.Generators;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.NativeCompiler.CompilationStrategies;
using IronSmalltalk.NativeCompiler.Internals;

namespace IronSmalltalk.NativeCompiler.Generators.Behavior
{
    internal abstract class MethodGenerator : GeneratorBase, INativeStrategyClient
    {
        private readonly MethodDictionary Methods;
        internal readonly SmalltalkClass Class;
        protected readonly NativeLiteralEncodingStrategy LiteralEncodingStrategy;
        protected readonly NativeDynamicCallStrategy DynamicCallStrategy;
        protected readonly NativeDiscreteBindingEncodingStrategy DiscreteBindingEncodingStrategy;

        protected MethodGenerator(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods)
            : base(compiler)
        {
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (methods == null)
                throw new ArgumentNullException("methods");
            this.Class = cls;
            this.Methods = methods;
            this.LiteralEncodingStrategy = new NativeLiteralEncodingStrategy(this);
            this.DynamicCallStrategy = new NativeDynamicCallStrategy(this);
            this.DiscreteBindingEncodingStrategy = new NativeDiscreteBindingEncodingStrategy(this);
        }

        protected abstract MethodCompiler GetMethodCompiler(MethodInformation method);

        private TypeBuilder _TypeBuilder;
        internal TypeBuilder TypeBuilder
        {
            get
            {
                if (this._TypeBuilder == null)
                {
                    this._TypeBuilder = this.Compiler.NativeGenerator.DefineType(
                        this.Compiler.GetTypeName("Classes", this.TypeName),
                        typeof(Object),
                        TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);
                }
                return this._TypeBuilder;
            }
        }

        protected abstract string TypeName { get; }


        private List<MethodInformation> MethodsInfo;

        protected void PrepareGenerator()
        {
            this.MethodsInfo = this.GetMethodNameMap();
            foreach (MethodInformation method in this.MethodsInfo)
            {
                this.PrepareMethod(method);
            }
        }

        private void PrepareMethod(MethodInformation method)
        {
            method.LambdaExpression = this.GenerateMethodLambda(method);
        }

        /// <summary>
        /// Generate the types that are needed for the behavior of the class.
        /// An instance of MethodGenerator generates either the class or the instance side of the class behavior,
        /// i.e. a ClassMethodGenerator and an InstanceMethodGenerator are needed to generate the full class behavior.
        /// </summary>
        /// <remarks>
        /// Generated:
        /// ".Classes.ClassName[ class]"                    - Contains method implementations
        /// ".Classes.ClassName[ class].$Literals"          - Static variables with literals that cannot be inlined in IL code
        /// ".Classes.ClassName[ class].$LiteralCallSites"  - Static variables with CallSites and Binders for "tricky" literals
        /// ".Classes.ClassName[ class].$CallSites"         - Static variables with CallSites and Binders for polymorphic calls (every ST method call)
        /// </remarks>
        /// </summary>
        internal void GenerateBehaviors()
        {
            // ".Classes.ClassName[ class].$Literals"          - Static variables with literals that cannot be inlined in IL code
            // ".Classes.ClassName[ class].$LiteralCallSites"  - Static variables with CallSites and Binders for "tricky" literals
            this.LiteralEncodingStrategy.GenerateTypes();
            // ".Classes.ClassName[ class].$CallSites"         - Static variables with CallSites and Binders for polymorphic calls (every ST method call)
            this.DynamicCallStrategy.GenerateTypes();
            // ".Classes.ClassName[ class].$BindingCallSites"  - Static variables with CallSites and Binders for discrete bindings
            this.DiscreteBindingEncodingStrategy.GenerateTypes();

            // Generates a method in ".Classes.ClassName[ class]" for each defined IronSmalltalk method.
            foreach (MethodInformation method in this.MethodsInfo)
                this.GenerateMethod(method);
        }

        /// <summary>
        /// Generates a method in ".Classes.ClassName[ class]" for each defined IronSmalltalk method.
        /// </summary>
        /// <param name="method">The newly created native method.</param>
        private void GenerateMethod(MethodInformation method)
        {
            MethodBuilder methodBuilder = this.TypeBuilder.DefineMethod(method.MethodName, MethodAttributes.Public | MethodAttributes.Static);
            // The lambda expression has been generated by the Expression Compiler. We just compile it into a native method.
            if (this.Compiler.NativeGenerator.DebugInfoGenerator == null)
                method.LambdaExpression.CompileToMethod(methodBuilder);
            else
                method.LambdaExpression.CompileToMethod(methodBuilder, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

        private LambdaExpression GenerateMethodLambda(MethodInformation method)
        {
            return this.GetMethodCompiler(method).CompileMethodLambda(method.Method.ParseTree, this.Class, method.MethodName);
        }

        /// <summary>
        /// Generates a map of unique method names and the corresponding compiled-methods.
        /// </summary>
        /// <returns></returns>
        private List<MethodInformation> GetMethodNameMap()
        {
            Dictionary<string, MethodInformation> map = new Dictionary<string, MethodInformation>();

            foreach (CompiledMethod method in this.Methods.Values)
            {
                RuntimeCompiledMethod runtimeMethod = method as RuntimeCompiledMethod;
                if (runtimeMethod == null)
                    throw new Exception("Expected to see a RuntimeCompiledMethod"); // ... since we have compiled everything ourself.
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

        protected class MethodInformation
        {
            public static MethodInformation CreateMethodInformation(NativeCompiler compiler, string name, RuntimeCompiledMethod method)
            {
                return new MethodInformation(compiler, name, method);
            }

            public readonly NativeCompiler Compiler;
            public readonly RuntimeCompiledMethod Method;
            public readonly string MethodName;
            public LambdaExpression LambdaExpression; 

            private MethodInformation(NativeCompiler compiler, string name, RuntimeCompiledMethod method)
            {
                this.Compiler = compiler;
                this.Method = method;
                this.MethodName = name;
            }


        }

        internal MethodInfo InitMethodDictionariesMethod { get; private set; }

        /// <summary>
        /// Generate initializer methods to initialize the contents of the method dictionaries.
        /// Those are contained in a class named ".Initializers.ScopeName_MethodInitializers".
        /// </summary>
        internal void GenerateMethodDictionaryInitializer(TypeBuilder type)
        {
            string name = type.GetUniqueMemberName(this.InitMethodDictionariesMethodName);
            MethodBuilder method = type.DefineMethod(name, MethodAttributes.Assembly | MethodAttributes.Static);

            var lambda = this.GenerateInitMethodDictionaryLambda(name);
            if (this.Compiler.NativeGenerator.DebugInfoGenerator == null)
                lambda.CompileToMethod(method);
            else
                lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);

            this.InitMethodDictionariesMethod = method;
        }

        protected abstract string InitMethodDictionariesMethodName { get; }

        private static readonly ConstructorInfo DictionarySymbolCompiledMethodCtor = TypeUtilities.Constructor(typeof(Dictionary<Symbol, CompiledMethod>), typeof(int));

        private Expression<Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>>> GenerateInitMethodDictionaryLambda(string name)
        {
            ParameterExpression cls = Expression.Parameter(typeof(SmalltalkClass), "cls");
            ParameterExpression dictionary = Expression.Parameter(typeof(Dictionary<Symbol, CompiledMethod>), "dictionary");
            ParameterExpression containingType = Expression.Parameter(typeof(Type), "containingType");
            ParameterExpression method = Expression.Parameter(typeof(CompiledMethod), "method");

            List<Expression> expressions = new List<Expression>();

            expressions.Add(Expression.Assign(
                dictionary,
                Expression.New(
                    MethodGenerator.DictionarySymbolCompiledMethodCtor,
                    Expression.Constant(this.MethodsInfo.Count, typeof(int)))));

            if (this.MethodsInfo.Count != 0)
            {
                expressions.Add(Expression.Assign(
                    containingType,
                    Expression.Constant(this.TypeBuilder, typeof(Type))));

                foreach (MethodInformation info in this.MethodsInfo)
                {
                    expressions.Add(Expression.Assign(method,
                        Expression.Call(this.AddMethodMethod,
                            dictionary,
                            cls,
                            Expression.Constant(info.Method.Selector.Value, typeof(string)),
                            containingType,
                            Expression.Constant(info.MethodName, typeof(string)))));
                    foreach (KeyValuePair<string, string> pair in info.Method.Annotations)
                    {
                        expressions.Add(Expression.Call(MethodGenerator.AnnotateObjectMethod,
                            method,
                            Expression.Constant(pair.Key, typeof(string)),
                            Expression.Constant(pair.Value, typeof(string))));
                    }
                }
            }

            expressions.Add(dictionary);

            return Expression.Lambda<Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>>>(
                Expression.Block(new ParameterExpression[] { dictionary, containingType, method }, expressions), name, new ParameterExpression[] { cls });
        }

        private static readonly MethodInfo AnnotateObjectMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AnnotateObject",
            typeof(CompiledCode), typeof(string), typeof(string));

        protected abstract MethodInfo AddMethodMethod { get; }

        /// <summary>
        /// Generate a delegate that can initialize the method dictionary for this generator.
        /// </summary>
        /// <param name="scopeGenerator"></param>
        /// <returns>Returns a delegate of type: cls => ScopeName_MethodInitializers.Init_ClassName_ClassMethods(cls)</returns>
        internal Expression<Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>>> GetMethodDictionaryInitializerDelegate(NameScopeGenerator scopeGenerator)
        {
            // IMPROVE: Why can't we use this.InitMethodDictionariesMethod directly and need to do the extra lookup?
            MethodInfo initializer = TypeUtilities.Method(scopeGenerator.MethodsInitializerType, this.InitMethodDictionariesMethod.Name, BindingFlags.Static | BindingFlags.NonPublic, typeof(SmalltalkClass));

            // NB: This will create helper methods, but too much work to get around this ...
            ParameterExpression cls = Expression.Parameter(typeof(SmalltalkClass), "cls");
            return Expression.Lambda<Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>>>(Expression.Call(initializer, cls), cls);
        }

        TypeBuilder INativeStrategyClient.ContainingType
        {
            get { return this.TypeBuilder; }
        }

        NativeCompiler INativeStrategyClient.Compiler
        {
            get { return this.Compiler; }
        }
    }
}
