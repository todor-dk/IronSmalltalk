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
using IronSmalltalk.NativeCompiler.Internals;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Generators.Globals
{
    internal sealed class PoolGenerator : GlobalBindingGenerator<PoolBinding>
    {
        internal PoolGenerator(NativeCompiler compiler, PoolBinding binding)
            : base(compiler, binding)
        {
        }

        protected override string AddBindingMethodName
        {
            get { return "AddPoolBinding"; }
        }
        
        private static readonly MethodInfo CreatePoolMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "CreatePool",
            typeof(SmalltalkRuntime), typeof(PoolBinding));

        private MethodInfo InitializerMethod;

        private void GeneratePoolInitializer(TypeBuilder type, string name)
        {
            MethodBuilder method = type.DefineMethod(name, MethodAttributes.Assembly | MethodAttributes.Static);

            var lambda = this.GeneratePoolInitializerLambda(name);
            if (this.Compiler.NativeGenerator.DebugInfoGenerator == null)
                lambda.CompileToMethod(method);
            else
                lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);

            this.InitializerMethod = method;
        }

        protected override IEnumerable<Expression> GenerateCreateGlobalObjectExpression(ParameterExpression runtime, NameScopeGenerator scopeGenerator, ParameterExpression scope, ParameterExpression binding)
        {
            // IMPROVE: Why can't we use this.InitializerMethod directly and need to do the extra lookup?
            Type initializerType = scopeGenerator.PoolsInitializerType;
            MethodInfo initializer = TypeUtilities.Method(initializerType, this.InitializerMethod.Name, BindingFlags.Static | BindingFlags.NonPublic, typeof(SmalltalkRuntime), typeof(PoolBinding));

            return new Expression[] 
            {
                Expression.Call(PoolGenerator.CreatePoolMethod, runtime, binding),
                Expression.Call(initializer, runtime, binding)
            };
        }

        /// <summary>
        /// Generate initializer methods to initialize the contents of the pool dictionary.
        /// Those are contained in a class named ".Initializers.ScopeName_PoolInitializers".
        /// </summary>
        internal string GeneratePoolInitializer(TypeBuilder type)
        {
            string name = type.GetUniqueMemberName(String.Format("Init_{0}", this.Binding.Name.Value));
            this.GeneratePoolInitializer(type, name);
            return name;
        }

        private static readonly MethodInfo CreatePoolConstantBindingMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "CreatePoolConstantBinding",
            typeof(SmalltalkRuntime), typeof(PoolBinding), typeof(string));

        private static readonly MethodInfo CreatePoolVariableBindingMethod = TypeUtilities.Method(
            typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper), "CreatePoolVariableBinding",
            typeof(SmalltalkRuntime), typeof(PoolBinding), typeof(string));

        private Expression<Action<SmalltalkRuntime, PoolBinding>> GeneratePoolInitializerLambda(string name)
        {
            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            ParameterExpression binding = Expression.Parameter(typeof(PoolBinding), "poolBinding");
            ParameterExpression tempBinding = Expression.Parameter(typeof(PoolVariableOrConstantBinding), "tempBinding");
            List<Expression> expressions = new List<Expression>();

            foreach (PoolVariableOrConstantBinding varBinding in this.Binding.Value.Values)
            {
                MethodInfo method = varBinding.IsConstantBinding ? PoolGenerator.CreatePoolConstantBindingMethod : PoolGenerator.CreatePoolVariableBindingMethod;
                expressions.Add(Expression.Assign(
                    tempBinding,
                    Expression.Convert(
                        Expression.Call(method, runtime, binding, Expression.Constant(varBinding.Name.Value, typeof(string))),
                        typeof(PoolVariableOrConstantBinding))));

                foreach (KeyValuePair<string, string> pair in varBinding.Annotations)
                {
                    expressions.Add(Expression.Call(PoolGenerator.AnnotateObjectMethod,
                        tempBinding,
                        Expression.Constant(pair.Key, typeof(string)),
                        Expression.Constant(pair.Value, typeof(string))));
                }
            }

            return Expression.Lambda<Action<SmalltalkRuntime, PoolBinding>>(
                Expression.Block(new ParameterExpression[] { tempBinding }, expressions), name, new ParameterExpression[] { runtime, binding });
        }
    }
}
