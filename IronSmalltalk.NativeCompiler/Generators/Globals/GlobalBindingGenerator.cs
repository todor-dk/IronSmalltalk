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
using IronSmalltalk.NativeCompiler.Internals;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Generators.Globals
{
    /// <summary>
    /// Class for generating global bindings like classes, pools and globals (constants/variables).
    /// </summary>
    internal abstract class GlobalBindingGenerator : GeneratorBase
    {
        internal GlobalBindingGenerator(NativeCompiler compiler)
            : base(compiler)
        {
        }

        protected abstract string AddBindingMethodName { get; }

        internal abstract string BindingName { get; }

        private static readonly Type[] AddBindingMethodParameterTypes = new Type[]
        {
            typeof(SmalltalkRuntime), typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope), typeof(string)
        };

        internal MethodInfo GetAddBindingMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod(this.AddBindingMethodName, BindingFlags.Static | BindingFlags.Public, null, GlobalBindingGenerator.AddBindingMethodParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method {0} in class {1}.", this.AddBindingMethodName, helperType.FullName));
            return method;
        }

        internal virtual IEnumerable<Expression> GenerateCreateObject(ParameterExpression runtime, NameScopeGenerator scopeGenerator, ParameterExpression scope, ParameterExpression binding)
        {
            return null;
        }

        internal abstract void GenerateAnnotations(List<Expression> expressions, ParameterExpression binding);
    }

    internal abstract class GlobalBindingGenerator<TBinding> : GlobalBindingGenerator
        where TBinding : IDiscreteGlobalBinding
    {
        internal TBinding Binding { get; private set; }

        internal GlobalBindingGenerator(NativeCompiler compiler, TBinding binding)
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

        private static readonly Type[] AnnotateObjectParameterTypes = new Type[]
        {
            typeof(IDiscreteBinding), typeof(string), typeof(string)
        };

        protected MethodInfo GetAnnotateObjectMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("AnnotateObject", BindingFlags.Static | BindingFlags.Public, null, AnnotateObjectParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method AnnotateObject in class {0}.", helperType.FullName));
            return method;
        }

        internal override void GenerateAnnotations(List<Expression> expressions, ParameterExpression binding)
        {
            MethodInfo method = this.GetAnnotateObjectMethod();
            foreach (KeyValuePair<string, string> pair in this.Binding.Annotations)
            {
                expressions.Add(Expression.Call(method,
                    binding,
                    Expression.Constant(pair.Key, typeof(string)),
                    Expression.Constant(pair.Value, typeof(string))));
            }
        }
    }
}
