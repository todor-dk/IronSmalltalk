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
using IronSmalltalk.NativeCompiler.Generators;
using IronSmalltalk.NativeCompiler.Internals;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.NativeCompiler.Generators
{
    /// <summary>
    /// This class generates the helper class to create the new Smalltalk runtime.
    /// </summary>
    internal sealed class RuntimeGenerator : GeneratorBase
    {
        private readonly MethodInfo ExtensionScopeInitializer;
        private readonly MethodInfo GlobalScopeInitializer;

        internal RuntimeGenerator(NativeCompiler compiler, MethodInfo extensionScopeInitializer, MethodInfo globalScopeInitializer)
            : base(compiler)
        {
            this.ExtensionScopeInitializer = extensionScopeInitializer;
            this.GlobalScopeInitializer = globalScopeInitializer;
        }

        private TypeBuilder Type;

        internal void GenerateCreateRuntimeMethods()
        {
            this.Type = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Smalltalk"),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            this.GenerateCreateRuntime(false);
            this.GenerateCreateRuntime(true);
        }

        private void GenerateCreateRuntime(bool hasInitializeParameter)
        {
            ConstructorInfo initializerCtor = typeof(Action<SmalltalkRuntime, SmalltalkNameScope>).GetConstructor(
                new Type[] { typeof(object), typeof(IntPtr) });
            Type helperType = typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper);
            Type[] argTypes = new Type[] { typeof(bool), typeof(Action<SmalltalkRuntime, SmalltalkNameScope>), typeof(Action<SmalltalkRuntime, SmalltalkNameScope>) };
            MethodInfo createRuntimeMethod = helperType.GetMethod("CreateRuntime", BindingFlags.Static | BindingFlags.Public, null, argTypes, null);
            if (createRuntimeMethod == null)
                throw new Exception(String.Format("Could not find static method CreateRuntime in class {0}.", helperType.FullName));

            argTypes = hasInitializeParameter ? new Type[] { typeof(bool) } : new Type[] {};
            MethodBuilder method = this.Type.DefineMethod("CreateRuntime", MethodAttributes.Public | MethodAttributes.Static, 
                CallingConventions.Standard, typeof(SmalltalkRuntime), argTypes);

            ILGenerator ilGen = method.GetILGenerator();

            if (hasInitializeParameter)
                ilGen.Emit(OpCodes.Ldarg_0);                        // arg 0
            else
                ilGen.Emit(OpCodes.Ldc_I4_1);                       // true

            // Create delegate for the extension scope initializer
            ilGen.Emit(OpCodes.Ldnull);                             // instance (this) ... it's a static method.
            ilGen.Emit(OpCodes.Ldftn, this.ExtensionScopeInitializer);   // pointer to the initializer method
            ilGen.Emit(OpCodes.Newobj, initializerCtor);            // create the delegate object

            // Create delegate for the global scope initializer
            ilGen.Emit(OpCodes.Ldnull);                             // instance (this) ... it's a static method.
            ilGen.Emit(OpCodes.Ldftn, this.GlobalScopeInitializer);      // pointer to the initializer method
            ilGen.Emit(OpCodes.Newobj, initializerCtor);            // create the delegate object

            // Call the method (on the stack: bool, Action<SmalltalkRuntime, SmalltalkNameScope>, Action<SmalltalkRuntime, SmalltalkNameScope>
            ilGen.Emit(OpCodes.Call, createRuntimeMethod);          // Call NativeLoadHelper.CreateRuntime();
            ilGen.Emit(OpCodes.Ret);                                // Return the result
        }
    }
}
