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
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class ClassGenerator : GlobalBindingGenerator<ClassBinding>
    {
        internal ClassGenerator(NativeCompiler compiler, ClassBinding binding)
            : base(compiler, binding)
        {
        }

        internal TypeBuilder InstanceMethodsType { get; private set; }
        internal TypeBuilder ClassMethodsType { get; private set; }

        #region Class Types and Binding Creation

        internal override void GenerateTypes()
        {
            this.InstanceMethodsType = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Classes", this.Binding.Name),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            this.ClassMethodsType = this.InstanceMethodsType.DefineNestedType(
                "class",
                TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Abstract,
                typeof(Object));
            this.Compiler.NativeGenerator.DefinedTypes.Add(this.ClassMethodsType);
        }

        protected override string AddBindingMethodName
        {
            get { return "AddClassBinding"; }
        }

        private static readonly Type[] CreateObjectMethodParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(ClassBinding), typeof(string), 
            typeof(SmalltalkClass.InstanceStateEnum), typeof(string[]), typeof(string[]), typeof(string[]), typeof(string[]),
            typeof(Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>), typeof(Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>)
        };

        private MethodInfo GetCreateObjectMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("CreateClass", BindingFlags.Static | BindingFlags.Public, null, ClassGenerator.CreateObjectMethodParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method CreateClass in class {0}.", helperType.FullName));
            return method;
        }

        internal override IEnumerable<Expression> GenerateCreateObject(ParameterExpression runtime, NameScopeGenerator scopeGenerator, ParameterExpression scope, ParameterExpression binding)
        {
            string name = String.Format("Init_{0}_ClassMethods", this.Binding.Name.Value);
            Expression<Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>> classMethodsDelegate = this.GetInitMethodsDelegate(scopeGenerator, name);
            name = String.Format("Init_{0}_InstanceMethods", this.Binding.Name.Value);
            Expression<Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>> instanceMethodsDelegate = this.GetInitMethodsDelegate(scopeGenerator, name);

            MethodInfo method = this.GetCreateObjectMethod();
            return new Expression[] 
            { 
                Expression.Call(method, runtime, scope, binding,
                    Expression.Constant(((this.Binding.Value.SuperclassBinding == null) ? null : this.Binding.Value.SuperclassBinding.Name.Value), typeof(string)),
                    Expression.Constant(this.Binding.Value.InstanceState, typeof(IronSmalltalk.Runtime.SmalltalkClass.InstanceStateEnum)),
                    this.CreateExpressionArray(this.Binding.Value.ClassVariableBindings),
                    this.CreateExpressionArray(this.Binding.Value.InstanceVariableBindings),
                    this.CreateExpressionArray(this.Binding.Value.ClassInstanceVariableBindings),
                    this.CreateExpressionArray(this.Binding.Value.ImportedPoolBindings),
                    classMethodsDelegate,
                    instanceMethodsDelegate)
            };
        }

        #endregion

        #region Method Initializers

        private Expression<Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>> GetInitMethodsDelegate(NameScopeGenerator scopeGenerator, string name)
        {
            Type initializerType = scopeGenerator.MethodsInitializerType;
            MethodInfo initializer = initializerType.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(SmalltalkRuntime) }, null);

            // NB: This will create helper methods, but too much work to get around this ...
            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            return Expression.Lambda<Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>>(Expression.Call(initializer, runtime), runtime);
        }

        private Expression CreateExpressionArray<TItem>(BindingDictionary<TItem> items)
            where TItem : IBinding
        {
            if (items.Count == 0)
                return Expression.Constant(null, typeof(string[]));

            var strings = ((IEnumerable<KeyValuePair<Symbol, TItem>>)items).Select(pair => Expression.Constant(pair.Key.Value, typeof(string)));
            return Expression.NewArrayInit(typeof(string), strings);
        }


        internal void GenerateInitMethodDictionaries(TypeBuilder type)
        {
            string name = String.Format("Init_{0}_ClassMethods", this.Binding.Name.Value);
            this.GenerateInitMethodDictionary(type, name, this.Binding.Value.ClassBehavior);
            name = String.Format("Init_{0}_InstanceMethods", this.Binding.Name.Value);
            this.GenerateInitMethodDictionary(type, name, this.Binding.Value.InstanceBehavior);
        }

        private void GenerateInitMethodDictionary(TypeBuilder type, string name, MethodDictionary methodDictionary)
        {
            MethodBuilder method = type.DefineMethod(name, MethodAttributes.Assembly | MethodAttributes.Static);

            var lambda = this.GenerateInitMethodDictionaryLambda(name, methodDictionary);
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

        private Expression<Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>> GenerateInitMethodDictionaryLambda(string name, MethodDictionary methodDictionary)
        {
            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            ParameterExpression dictionary = Expression.Parameter(typeof(Dictionary<Symbol, CompiledMethod>), "dictionary");

            List<Expression> expressions = new List<Expression>();

            ConstructorInfo ctor = typeof(Dictionary<Symbol, CompiledMethod>).GetConstructor(new Type[] { typeof(int) });
            if (ctor == null)
                throw new Exception("Could not find the constructor for Dictionary<Symbol, CompiledMethod> with type (int).");
            expressions.Add(Expression.Assign(
                dictionary,
                Expression.New(
                    ctor, 
                    Expression.Constant(methodDictionary.Count, typeof(int)))));

            return Expression.Lambda<Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>>(
                Expression.Block(new ParameterExpression[] { dictionary } , expressions), name, new ParameterExpression[] { runtime });
        }

        #endregion

        private MethodGenerator InstanceMethodGenerator;
        private MethodGenerator ClassMethodGenerator;

        internal void GenerateMethods()
        {
            this.InstanceMethodGenerator = Internals.InstanceMethodGenerator.GenerateMethods(this);
            this.ClassMethodGenerator = Internals.ClassMethodGenerator.GenerateMethods(this);
        }

    }
}
