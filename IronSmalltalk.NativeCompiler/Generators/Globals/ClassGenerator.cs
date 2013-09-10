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
using IronSmalltalk.Common.Internal;
using IronSmalltalk.NativeCompiler.Generators.Behavior;
using IronSmalltalk.NativeCompiler.Internals;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Generators.Globals
{
    internal sealed class ClassGenerator : GlobalBindingGenerator<ClassBinding>
    {
        internal ClassGenerator(NativeCompiler compiler, ClassBinding binding)
            : base(compiler, binding)
        {
            this.InstanceMethodGenerator = InstanceMethodGenerator.CreateAndPrepareGenerator(this);
            this.ClassMethodGenerator = ClassMethodGenerator.CreateAndPrepareGenerator(this);
        }

        internal readonly ClassMethodGenerator ClassMethodGenerator;
        internal readonly InstanceMethodGenerator InstanceMethodGenerator;


        #region Class Types and Binding Creation

        /// <summary>
        /// Generate the types that are needed for the behavior of the class.
        /// Those include:
        /// ".Classes.ClassName"                            - Contains instance method implementations
        /// ".Classes.ClassName.$Literals"                  - Static variables with literals that cannot be inilined in IL code
        /// ".Classes.ClassName.$LiteralCallSites"          - Static variables with CallSites and Binders for "tricky" literals
        /// ".Classes.ClassName.$CallSites"                 - Static variables with CallSites and Binders for polymorphic calls (every ST method call)
        /// ".Classes.ClassName class"                      - Same as above, but the class side ...
        /// ".Classes.ClassName class.$Literals"
        /// ".Classes.ClassName class.$LiteralCallSites"
        /// ".Classes.ClassName class.$CallSites"
        /// </summary>
        internal void GenerateBehaviorTypes()
        {
            this.InstanceMethodGenerator.GenerateBehaviors();
            this.ClassMethodGenerator.GenerateBehaviors();
        }

        protected override string AddBindingMethodName
        {
            get { return "AddClassBinding"; }
        }

        private static readonly MethodInfo CreateClassMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "CreateClass",
            typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(ClassBinding), typeof(string),
            typeof(SmalltalkClass.InstanceStateEnum), typeof(string[]), typeof(string[]), typeof(string[]), typeof(string[]),
            typeof(Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>>), typeof(Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>>));

        protected override IEnumerable<Expression> GenerateCreateGlobalObjectExpression(ParameterExpression runtime, NameScopeGenerator scopeGenerator, ParameterExpression scope, ParameterExpression binding)
        {
            return new Expression[] 
            { 
                Expression.Call(ClassGenerator.CreateClassMethod, runtime, scope, binding,
                    Expression.Constant(((this.Binding.Value.SuperclassBinding == null) ? null : this.Binding.Value.SuperclassBinding.Name.Value), typeof(string)),
                    Expression.Constant(this.Binding.Value.InstanceState, typeof(IronSmalltalk.Runtime.SmalltalkClass.InstanceStateEnum)),
                    this.CreateExpressionArray(this.Binding.Value.ClassVariableBindings),
                    this.CreateExpressionArray(this.Binding.Value.InstanceVariableBindings),
                    this.CreateExpressionArray(this.Binding.Value.ClassInstanceVariableBindings),
                    this.CreateExpressionArray(this.Binding.Value.ImportedPoolBindings),
                    this.ClassMethodGenerator.GetMethodDictionaryInitializerDelegate(scopeGenerator),
                    this.InstanceMethodGenerator.GetMethodDictionaryInitializerDelegate(scopeGenerator))
            };
        }

        #endregion

        #region Method Initializers

        private Expression CreateExpressionArray<TItem>(BindingDictionary<TItem> items)
            where TItem : IBinding
        {
            if (items.Count == 0)
                return Expression.Constant(null, typeof(string[]));

            var strings = ((IEnumerable<KeyValuePair<Symbol, TItem>>)items).Select(pair => Expression.Constant(pair.Key.Value, typeof(string)));
            return Expression.NewArrayInit(typeof(string), strings);
        }


        /// <summary>
        /// Generate initializer methods to initialize the contents of the method dictionaries.
        /// Those are contained in a class named ".Initializers.ScopeName_MethodInitializers".
        /// </summary>
        internal void GenerateMethodDictionariesInitializer(TypeBuilder type)
        {
            this.ClassMethodGenerator.GenerateMethodDictionaryInitializer(type);
            this.InstanceMethodGenerator.GenerateMethodDictionaryInitializer(type);
        }

        #endregion

    }
}
