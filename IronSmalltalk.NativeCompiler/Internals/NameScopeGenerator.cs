using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class NameScopeGenerator : ISmalltalkNameScopeVisitor
    {
        #region Visiting

        internal readonly List<string> ProtectedNames = new List<string>();

        internal readonly List<GlobalBindingGenerator> Generators = new List<GlobalBindingGenerator>();

        internal readonly NativeCompiler Compiler;

        internal readonly string ScopeName;

        internal NameScopeGenerator(NativeCompiler compiler, string name)
        {
            this.Compiler = compiler;
            this.ScopeName = name;
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
            this.Generators.Add(new GlobalConstantGenerator(this.Compiler, binding));
        }

        #endregion

        private TypeBuilder MethodsInitializerTypeBuilder;
        private TypeBuilder PoolsInitializerTypeBuilder;
        private TypeBuilder InitializerTypeBuilder;
        private Type _MethodsInitializerType;
        private Type _PoolsInitializerType;
        private Type _InitializerType;
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
        internal Type InitializerType
        {
            get
            {
                if (this._InitializerType == null)
                    this._InitializerType = this.InitializerTypeBuilder.CreateType();
                return this._InitializerType;
            }
        }

        internal void Generate()
        {
            this.GenerateItemTypes();
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
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

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
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            foreach (GlobalBindingGenerator generator in this.Generators)
            {
                ClassGenerator classGenerator = generator as ClassGenerator;
                if (classGenerator != null)
                    classGenerator.GenerateInitMethodDictionaries(this.MethodsInitializerTypeBuilder);
            }
        }

        internal const string InitializerMethodName = "InitializeScope";

        private void GenerateNameScopeInitializer()
        {
            this.InitializerTypeBuilder = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Initializers", String.Format("{0}_Initializers", this.ScopeName)),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            MethodBuilder method = this.InitializerTypeBuilder.DefineMethod(NameScopeGenerator.InitializerMethodName, MethodAttributes.Assembly | MethodAttributes.Static);

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
    }
}
