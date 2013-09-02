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

        internal void GenerateTypes()
        {
            this.InstanceMethodGenerator.GenerateMethods();
            this.ClassMethodGenerator.GenerateMethods();
        }

        protected override string AddBindingMethodName
        {
            get { return "AddClassBinding"; }
        }

        private MethodInfo GetCreateObjectMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            Type[] argTypes = new Type[]
            {
                typeof(SmalltalkRuntime), typeof(SmalltalkNameScope), typeof(ClassBinding), typeof(string), 
                typeof(SmalltalkClass.InstanceStateEnum), typeof(string[]), typeof(string[]), typeof(string[]), typeof(string[]),
                typeof(Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>), typeof(Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>>)
            };
            MethodInfo method = helperType.GetMethod("CreateClass", BindingFlags.Static | BindingFlags.Public, null, argTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method CreateClass in class {0}.", helperType.FullName));
            return method;
        }

        internal override IEnumerable<Expression> GenerateCreateObject(ParameterExpression runtime, NameScopeGenerator scopeGenerator, ParameterExpression scope, ParameterExpression binding)
        {
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
                    this.ClassMethodGenerator.GetInitMethodsDelegate(scopeGenerator),
                    this.InstanceMethodGenerator.GetInitMethodsDelegate(scopeGenerator))
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


        internal void GenerateInitMethodDictionaries(TypeBuilder type)
        {
            this.ClassMethodGenerator.GenerateInitMethodDictionaries(type);
            this.InstanceMethodGenerator.GenerateInitMethodDictionaries(type);
        }

        #endregion

    }
}
