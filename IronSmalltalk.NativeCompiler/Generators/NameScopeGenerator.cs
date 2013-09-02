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
using IronSmalltalk.NativeCompiler.Generators.Globals;
using IronSmalltalk.NativeCompiler.Generators.Initializers;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Generators
{
    internal sealed class NameScopeGenerator : ISmalltalkNameScopeVisitor
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
        private Type _MethodsInitializerType;
        private Type _PoolsInitializerType;
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

        internal MethodInfo GenerateInitializerMethod()
        {
            this.GenerateClassTypes();
            this.GenerateMethodDictionaryInitializers();
            this.GeneratePoolInitializers();
            this.GenerateInitializers();
            return this.GenerateNameScopeInitializer();
        }

        /// <summary>
        /// Generate types for each class. This is the ".Classes.ClassName" and ".Classes.ClassName class" classes.
        /// Generate methods for each class. This includes the class and instance methods for each class,
        /// as well as nested classes containing literals, call sites and call site binders.
        /// </summary>
        private void GenerateClassTypes()
        {
            foreach (ClassGenerator generator in this.Generators.OfType<ClassGenerator>())
                generator.GenerateTypes();
        }

        /// <summary>
        /// Generate initializer methods to initialize the contents of the method dictionaries.
        /// Those are contained in a class named ".Initializers.ScopeName_MethodInitializers".
        /// </summary>
        private void GenerateMethodDictionaryInitializers()
        {
            this.MethodsInitializerTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_MethodInitializers", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);

            foreach (ClassGenerator generator in this.Generators.OfType<ClassGenerator>())
                generator.GenerateInitMethodDictionaries(this.MethodsInitializerTypeBuilder);
        }

        /// <summary>
        /// Generate initializer methods to initialize the contents of the pool dictionaries.
        /// Those are contained in a class named ".Initializers.ScopeName_PoolInitializers".
        /// </summary>
        private void GeneratePoolInitializers()
        {
            this.PoolsInitializerTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_PoolInitializers", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);

            foreach (PoolGenerator generator in this.Generators.OfType<PoolGenerator>())
                generator.GeneratePoolInitializer(this.PoolsInitializerTypeBuilder);
        }

        /// <summary>
        /// Generate a class named ".Initializers.ScopeName_Initializers" and in it,
        /// generate a method representing each initializer expression for the
        /// initializers (class, global (variable/constant), pool item (variable/constant) and program).
        /// </summary>
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

        /// <summary>
        /// Generate a type named ".Initializers.ScopeName_ScopeInitializer" that creates and initializes a new SmalltalkNameScope. 
        /// This includes initializing all global objects in the name scope (classes, globals, pools etc.),
        /// setting the contents of those objects (method dictionaries, pool items) as well as initializers.
        /// </summary>
        private MethodInfo GenerateNameScopeInitializer()
        {
            // Create the name scope initializer class. It's named ".Initializers.ScopeName_ScopeInitializer"
            this.ScopeInitializerTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_ScopeInitializer", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);

            // Create the initializer method ... internal static void InitializeScope(SmalltalkRuntime runtime, SmalltalkNameScope scope)
            MethodBuilder method = this.ScopeInitializerTypeBuilder.DefineMethod("InitializeScope", MethodAttributes.Assembly | MethodAttributes.Static);

            // Define parameters
            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            ParameterExpression scope = Expression.Parameter(typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope), "scope");

            List<ParameterExpression> variables = new List<ParameterExpression>();
            List<Expression> expressions = new List<Expression>();
            List<Expression> createObjects = new List<Expression>();

            // Adds statement ... NativeLoadHelper.AddProtectedName(runtime, scope, "...");
            foreach (string pn in this.ProtectedNames)
                expressions.Add(this.GenerateAddProtectedName(pn, runtime, scope));

            foreach (GlobalBindingGenerator gbg in this.Generators)
            {
                // The method that will add the global binding, e.g. ... NativeLoadHelper.AddClassBinding(...);
                MethodInfo addBindingMethod = gbg.GetAddBindingMethod();
                // Create a temp var for each global binding object.
                ParameterExpression variable = Expression.Parameter(addBindingMethod.ReturnType, gbg.BindingName);
                variables.Add(variable);
                // Add a statement to call that method and assign it to the temp var .... ClassBinding binding27 = NativeLoadHelper.AddClassBinding(runtime, scope, "...");
                expressions.Add(Expression.Assign(variable, Expression.Call(addBindingMethod, runtime, scope, Expression.Constant(gbg.BindingName, typeof(String)))));

                // Add a statement that will create the global object. This is relevant for clases and pools.
                // Example ...  NativeLoadHelper.CreateClass(runtime, scope, binding27, "...", SmalltalkClass.InstanceStateEnum.Native, ...); 
                IEnumerable<Expression> expression = gbg.GenerateCreateObject(runtime, this, scope, variable);
                if (expression != null)
                    createObjects.AddRange(expression);
                // Add optional annotations ... NativeLoadHelper.AnnotateObject(binding27, "...", "...");
                gbg.GenerateAnnotations(createObjects, variable);
            }

            // The statements for creating the global objects and adding annotations are added at the END after the create binding statements.
            expressions.AddRange(createObjects);

            // Now, create a temp var that holds the name type of the name scope initializers 
            ParameterExpression initializersType = Expression.Parameter(typeof(Type), "initializersType");
            variables.Add(initializersType);
            // ... Type initializersType = typeof(GlobalScope_Initializers);
            expressions.Add(Expression.Assign(initializersType, Expression.Constant(this.InitializersTypeBuilder, typeof(Type))));
            // Create a temp var to hold the initializer
            ParameterExpression initializer = Expression.Parameter(typeof(CompiledInitializer), "initializer");
            variables.Add(initializer);
            foreach (InitializerGenerator generator in this.Initializers)
            {
                // Call a helper method to add initializer statements ... CompiledInitializer initializer = NativeLoadHelper.AddClassInitializer(runtime, scope, delegateType, "BigDecimal_Class_Initializer", "BigDecimal");
                IEnumerable<Expression> expression = generator.GenerateCreateInitializerExpression(initializer, runtime, scope, initializersType);
                if (expression != null)
                    expressions.AddRange(expression);
            }

            // Create a lambda that executes all the above statements
            if (expressions.Count == 0)
                expressions.Add(Expression.Constant(null, typeof(object)));
            var lambda = Expression.Lambda<Action<SmalltalkRuntime, IronSmalltalk.Runtime.Bindings.SmalltalkNameScope>>(
                Expression.Block(variables, expressions), method.Name, new ParameterExpression[] { runtime, scope });

            // Compile the lambda into method. This is the name scope initializer method.
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);

            return method;
        }

        private MethodCallExpression GenerateAddProtectedName(string name, ParameterExpression runtime, ParameterExpression scope)
        {
            Type[] argTypes = new Type[] { typeof(SmalltalkRuntime), typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope), typeof(string) };
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AddProtectedName", BindingFlags.Static | BindingFlags.Public, null, argTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AddProtectedName in class {0}.", helperType.FullName));
            return Expression.Call(method, runtime, scope, Expression.Constant(name, typeof(String)));
        }
    }
}
