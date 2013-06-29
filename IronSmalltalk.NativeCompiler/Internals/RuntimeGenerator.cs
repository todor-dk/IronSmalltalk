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
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class RuntimeGenerator : GeneratorBase
    {
        private readonly NameScopeGenerator ExtensionScope;
        private readonly NameScopeGenerator GlobalScope;

        internal RuntimeGenerator(NativeCompiler compiler, NameScopeGenerator extensionScope, NameScopeGenerator globalScope)
            : base(compiler)
        {
            this.ExtensionScope = extensionScope;
            this.GlobalScope = globalScope;
        }

        private TypeBuilder Type;

        internal void Generate()
        {
            this.Type = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Smalltalk"),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            MethodBuilder method = this.Type.DefineMethod("CreateRuntime", MethodAttributes.Public | MethodAttributes.Static);
            var lambda = this.GenerateCreateRuntimeLambda(false, method.Name);
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);
            method = this.Type.DefineMethod("CreateRuntime", MethodAttributes.Public | MethodAttributes.Static);
            lambda = this.GenerateCreateRuntimeLambda(true, method.Name);
            lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);
        }

        private LambdaExpression GenerateCreateRuntimeLambda(bool hasInitializeParameter, string name)
        {
            Expression<Action<SmalltalkRuntime, SmalltalkNameScope>> extensionScopeInitializer = this.ExtensionScope.GetInitializerDelegate();
            Expression<Action<SmalltalkRuntime, SmalltalkNameScope>> globalScopeInitializer = this.GlobalScope.GetInitializerDelegate();

            MethodInfo method = this.GetCreateRuntimeMethod();

            if (hasInitializeParameter)
            {
                ParameterExpression initialize = Expression.Parameter(typeof(bool), "initialize");
                MethodCallExpression expression = Expression.Call(method, 
                        initialize,
                        extensionScopeInitializer,
                        globalScopeInitializer);
                return Expression.Lambda<Func<bool, SmalltalkRuntime>>(expression, name, new ParameterExpression[] { initialize });
            }
            else
            {
                MethodCallExpression expression = Expression.Call(method, 
                        Expression.Constant(true, typeof(Boolean)),
                        extensionScopeInitializer,
                        globalScopeInitializer);
                return Expression.Lambda<Func<SmalltalkRuntime>>(expression, name, new ParameterExpression[0]);
            }
        }


        private static readonly Type[] CreateRuntimeParameterTypes = new Type[]
        {
            typeof(bool), typeof(Action<SmalltalkRuntime, SmalltalkNameScope>), typeof(Action<SmalltalkRuntime, SmalltalkNameScope>)
        };

        private MethodInfo GetCreateRuntimeMethod()
        {
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            MethodInfo method = helperType.GetMethod("CreateRuntime", BindingFlags.Static | BindingFlags.Public, null, RuntimeGenerator.CreateRuntimeParameterTypes, null);
            if (method == null)
                throw new Exception(String.Format("Could not find static method CreateRuntime in class {0}.", helperType.FullName));
            return method;
        }
    }
}
