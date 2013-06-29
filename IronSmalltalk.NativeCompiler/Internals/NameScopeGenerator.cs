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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class NameScopeGenerator : ISmalltalkNameScopeVisitor
    {
        #region Visiting

        internal readonly List<string> ProtectedNames = new List<string>();

        internal readonly List<GlobalBindingGenerator> Generators = new List<GlobalBindingGenerator>();
        internal readonly List<InitializerGenerator> Initializers = new List<InitializerGenerator>();

        internal readonly NativeCompiler Compiler;

        internal readonly string ScopeName;

        private bool IgnoreSmalltalk;

        internal NameScopeGenerator(NativeCompiler compiler, string name, bool ignoreSmalltalk)
        {
            this.Compiler = compiler;
            this.ScopeName = name;
            this.IgnoreSmalltalk = ignoreSmalltalk;
        }

        void ISmalltalkNameScopeVisitor.Visit(Runtime.Symbol protectedName)
        {
            this.ProtectedNames.Add(protectedName);
        }

        void ISmalltalkNameScopeVisitor.Visit(ClassBinding binding)
        {
            this.Generators.Add(new ClassGenerator(this.Compiler, binding));
        }

        void ISmalltalkNameScopeVisitor.Visit(PoolBinding binding)
        {
            this.Generators.Add(new PoolGenerator(this.Compiler, binding));
        }

        void ISmalltalkNameScopeVisitor.Visit(GlobalVariableBinding binding)
        {
            this.Generators.Add(new GlobalVariableGenerator(this.Compiler, binding));
        }

        void ISmalltalkNameScopeVisitor.Visit(GlobalConstantBinding binding)
        {
            if (this.IgnoreSmalltalk && (binding.Name.Value == "Smalltalk"))
                return;
            this.Generators.Add(new GlobalConstantGenerator(this.Compiler, binding));
        }

        void ISmalltalkNameScopeVisitor.Visit(CompiledInitializer initializer)
        {
            this.Initializers.Add(new InitializerGenerator(this.Compiler, initializer));
        }

        #endregion

        private TypeBuilder InitializersTypeBuilder;
        private TypeBuilder MethodsInitializerTypeBuilder;
        private TypeBuilder PoolsInitializerTypeBuilder;
        private TypeBuilder ScopeInitializerTypeBuilder;
        private Type _InitializersType;
        private Type _MethodsInitializerType;
        private Type _PoolsInitializerType;
        private Type _ScopeInitializerType;
        internal Type InitializersType
        {
            get
            {
                if (this._InitializersType == null)
                    this._InitializersType = this.InitializersTypeBuilder.CreateType();
                return this._InitializersType;
            }
        }
        internal Type MethodsInitializerType
        {
            get
            {
                if (this._MethodsInitializerType == null)
                    this._MethodsInitializerType = this.MethodsInitializerTypeBuilder.CreateType();
                return this._MethodsInitializerType;
            }
        }
        internal Type PoolsInitializerType
        {
            get
            {
                if (this._PoolsInitializerType == null)
                    this._PoolsInitializerType = this.PoolsInitializerTypeBuilder.CreateType();
                return this._PoolsInitializerType;
            }
        }
        internal Type ScopeInitializerType
        {
            get
            {
                if (this._ScopeInitializerType == null)
                    this._ScopeInitializerType = this.ScopeInitializerTypeBuilder.CreateType();
                return this._ScopeInitializerType;
            }
        }

        internal void Generate()
        {
            this.GenerateItemTypes();
            this.GenerateInitializers();
            this.GeneratePoolInitializers();
            this.GenerateMethodDictionaryInitializers();
            this.GenerateNameScopeInitializer();

            /* Order should be:
             * 1. Create Global Bindings
             * 2. Create Objects (for global bindings)
             * 3. Create Pool Variable Bindings
             * 
             * 
             *             
             this.CreateTemporaryNameSpace();
                
                if (!this.CreateGlobalBindings())
                    return false;
                if (!this.CreateGlobalObjects())
                    return false;
                if (!this.ValidateGlobalObjects())
                    return false;
                if (!this.CreatePoolVariableBindings())
                    return false;
            if (!this.ValidateMethods())
                return false;
            if (!this.ValidateInitializers())
                return false;
            if (!this.CreateMethods())
                return false;
            if (!this.AddAnnotation())      ** Methods + Initializers
                return false;

            this.ReplaceSmalltalkContextNameSpace();
            return this.RecompileClasses(); // Must be after ReplaceSmalltalkContextNameSpace(), otherwise class cannot find subclasses.
           */
        }

        private void GenerateItemTypes()
        {
            foreach (GlobalBindingGenerator generator in this.Generators)
                generator.GenerateTypes();
        }

        private void GeneratePoolInitializers()
        {
            this.PoolsInitializerTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_PoolInitializers", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);

            foreach (GlobalBindingGenerator generator in this.Generators)
            {
                PoolGenerator poolGenerator = generator as PoolGenerator;
                if (poolGenerator != null)
                    poolGenerator.GeneratePoolInitializer(this.PoolsInitializerTypeBuilder);
            }
        }

        private void GenerateMethodDictionaryInitializers()
        {
            this.MethodsInitializerTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_MethodInitializers", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);

            foreach (GlobalBindingGenerator generator in this.Generators)
            {
                ClassGenerator classGenerator = generator as ClassGenerator;
                if (classGenerator != null)
                    classGenerator.GenerateInitMethodDictionaries(this.MethodsInitializerTypeBuilder);
            }
        }

        private void GenerateInitializers()
        {
            this.InitializersTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_Initializers", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);

            HashSet<string> names = new HashSet<string>();
            foreach (InitializerGenerator generator in this.Initializers)
                generator.GenerateInitializerMethod(this.InitializersTypeBuilder, names);
        }

        internal const string InitializerMethodName = "InitializeScope";

        private void GenerateNameScopeInitializer()
        {
            this.ScopeInitializerTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_ScopeInitializer", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);

            MethodBuilder method = this.ScopeInitializerTypeBuilder.DefineMethod(NameScopeGenerator.InitializerMethodName, MethodAttributes.Assembly | MethodAttributes.Static);

            var lambda = this.GenerateLambda(NameScopeGenerator.InitializerMethodName);
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

        private Expression<Action<SmalltalkRuntime, IronSmalltalk.Runtime.Bindings.SmalltalkNameScope>> GenerateLambda(string name)
        {
            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            ParameterExpression scope = Expression.Parameter(typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope), "scope");

            List<ParameterExpression> variables = new List<ParameterExpression>();
            List<Expression> expressions = new List<Expression>();
            List<Expression> createObjects = new List<Expression>();

            foreach (string pn in this.ProtectedNames)
                expressions.Add(this.GenerateAddProtectedName(pn, runtime, scope));

            foreach (GlobalBindingGenerator gbg in this.Generators)
            {
                ParameterExpression variable = Expression.Parameter(gbg.GetAddBindingMethod().ReturnType, gbg.BindingName);
                variables.Add(variable);
                expressions.Add(Expression.Assign(variable, gbg.GenerateAddBinding(runtime, scope)));
                IEnumerable<Expression> expression = gbg.GenerateCreateObject(runtime, this, scope, variable);
                if (expression != null)
                    createObjects.AddRange(expression);
                gbg.GenerateAnnotations(createObjects, variable);
            }

            expressions.AddRange(createObjects);

            ParameterExpression initializersType = Expression.Parameter(typeof(Type), "initializersType");
            variables.Add(initializersType);
            ParameterExpression initializer = Expression.Parameter(typeof(CompiledInitializer), "initializer");
            variables.Add(initializer);
            expressions.Add(Expression.Assign(initializersType, Expression.Constant(this.InitializersTypeBuilder, typeof(Type))));
            foreach (InitializerGenerator generator in this.Initializers)
            {
                IEnumerable<Expression> expression = generator.GenerateCreateInitializer(initializer, runtime, scope, initializersType);
                if (expression != null)
                    expressions.AddRange(expression);
            }

            if (expressions.Count == 0)
                expressions.Add(Expression.Constant(null, typeof(object)));

            return Expression.Lambda<Action<SmalltalkRuntime, IronSmalltalk.Runtime.Bindings.SmalltalkNameScope>>(
                Expression.Block(variables, expressions), name, new ParameterExpression[] { runtime, scope });
        }

        private static readonly Type[] AddProtectedNameTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope), typeof(string)
        };

        private MethodCallExpression GenerateAddProtectedName(string name, ParameterExpression runtime, ParameterExpression scope)
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AddProtectedName", BindingFlags.Static | BindingFlags.Public, null, NameScopeGenerator.AddProtectedNameTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AddProtectedName in class {0}.", helperType.FullName));
            return Expression.Call(method, runtime, scope, Expression.Constant(name, typeof(String)));
        }

        internal Expression<Action<SmalltalkRuntime, SmalltalkNameScope>> GetInitializerDelegate()
        {
            Type initializerType = this.ScopeInitializerType;
            MethodInfo initializer = initializerType.GetMethod(NameScopeGenerator.InitializerMethodName, 
                BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(SmalltalkRuntime), typeof(SmalltalkNameScope) }, null);

            // NB: This will create helper methods, but too much work to get around this ...
            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            ParameterExpression scope = Expression.Parameter(typeof(SmalltalkNameScope), "scope");
            return Expression.Lambda<Action<SmalltalkRuntime, SmalltalkNameScope>>(Expression.Call(initializer, runtime, scope), runtime, scope);
        }

    }
}
