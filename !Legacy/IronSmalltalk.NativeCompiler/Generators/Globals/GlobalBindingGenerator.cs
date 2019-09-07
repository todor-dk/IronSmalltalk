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
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.NativeCompiler.Internals;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Generators.Globals
{
    /// <summary>
    /// Class for generating global bindings like classes, pools and globals (constants/variables).
    /// </summary>
    internal abstract class GlobalBindingGenerator : GeneratorBase
    {
        protected GlobalBindingGenerator(NativeCompiler compiler)
            : base(compiler)
        {
        }

        protected abstract string AddBindingMethodName { get; }

        internal abstract string BindingName { get; }

        internal void GenerateGlobal(NameScopeGenerator scopeGenerator, ParameterExpression runtime, ParameterExpression scope, List<ParameterExpression> variables, List<Expression> createBindings, List<Expression> initializeBindings)
        {
            // The method that will add the global binding, e.g. ... NativeLoadHelper.AddClassBinding(...);
            MethodInfo addBindingMethod = this.GetAddBindingMethod();
            // Create a temp var for each global binding object.
            ParameterExpression variable = Expression.Parameter(addBindingMethod.ReturnType, this.BindingName);
            variables.Add(variable);
            // Add a statement to call that method and assign it to the temp var .... ClassBinding binding27 = NativeLoadHelper.AddClassBinding(runtime, scope, "...");
            createBindings.Add(Expression.Assign(variable, Expression.Call(addBindingMethod, runtime, scope, Expression.Constant(this.BindingName, typeof(String)))));

            // Add a statement that will create the global object. This is relevant for clases and pools.
            // Example ...  NativeLoadHelper.CreateClass(runtime, scope, binding27, "...", SmalltalkClass.InstanceStateEnum.Native, ...); 
            IEnumerable<Expression> expression = this.GenerateCreateGlobalObjectExpression(runtime, scopeGenerator, scope, variable);
            if (expression != null)
                initializeBindings.AddRange(expression);
            // Add optional annotations ... NativeLoadHelper.AnnotateObject(binding27, "...", "...");
            this.GenerateAnnotations(initializeBindings, variable);
        }

        private MethodInfo GetAddBindingMethod()
        {
            return TypeUtilities.Method(typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), this.AddBindingMethodName,
                typeof(SmalltalkRuntime), typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope), typeof(string));
        }

        /// <summary>
        /// Generate a statement that will create the global object. This is relevant for clases and pools.
        /// </summary>
        /// <param name="runtime">Parameter representing the SmalltalkRuntime.</param>
        /// <param name="scopeGenerator">Generator for the current name scope.</param>
        /// <param name="scope">Parameter representing the current name scope.</param>
        /// <param name="binding">Parameter representing the global binding.</param>
        /// <returns>A collection of expressions or null.</returns>
        protected virtual IEnumerable<Expression> GenerateCreateGlobalObjectExpression(ParameterExpression runtime, NameScopeGenerator scopeGenerator, ParameterExpression scope, ParameterExpression binding)
        {
            return null;
        }

        protected abstract void GenerateAnnotations(List<Expression> expressions, ParameterExpression binding);
    }

    internal abstract class GlobalBindingGenerator<TBinding> : GlobalBindingGenerator
        where TBinding : IDiscreteGlobalBinding
    {
        internal TBinding Binding { get; private set; }

        protected GlobalBindingGenerator(NativeCompiler compiler, TBinding binding)
            : base(compiler)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");
            this.Binding = binding;
        }

        internal override string BindingName
        {
            get { return this.Binding.Name; }
        }

        protected static readonly MethodInfo AnnotateObjectMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "AnnotateObject",
            typeof(IDiscreteBinding), typeof(string), typeof(string));

        protected override void GenerateAnnotations(List<Expression> expressions, ParameterExpression binding)
        {
            foreach (KeyValuePair<string, string> pair in this.Binding.Annotations)
            {
                expressions.Add(Expression.Call(AnnotateObjectMethod,
                    binding,
                    Expression.Constant(pair.Key, typeof(string)),
                    Expression.Constant(pair.Value, typeof(string))));
            }
        }
    }
}
