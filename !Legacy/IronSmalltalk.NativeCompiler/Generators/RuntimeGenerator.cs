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

        /// <summary>
        /// Generates a type that's the "entry point" into this IronSmalltalk compilation
        /// as well as methods named "CreateRuntime" (two variants), which are used to
        /// initialize a new instance of a SmalltalkRuntime.
        /// </summary>
        internal void GenerateCreateRuntimeMethods()
        {
            this.Type = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Smalltalk"),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            if (this.Compiler.Parameters.AssemblyType == NativeCompilerParameters.AssemblyTypeEnum.Dll)
            {
                // DLLs have two public overloaded methods called CreateRuntime().
                this.GenerateCreateRuntime(false);
                this.GenerateCreateRuntime(true);
            }
            else
            {
                // For EXEs, create a main method that calls the CreateRuntime
                MethodBuilder createRuntime = this.GenerateCreateRuntime(false);

                MethodBuilder main = this.Type.DefineMethod("Main",
                    MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig,
                    CallingConventions.Standard,
                    typeof(void),
                    new Type[] { });

                // Add [STAThread]
                CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(
						TypeUtilities.Constructor(typeof(STAThreadAttribute)),
						new object[] { });
                main.SetCustomAttribute(caBuilder);

                ILGenerator ilGen = main.GetILGenerator();
                ilGen.Emit(OpCodes.Call, createRuntime);
                ilGen.Emit(OpCodes.Pop); // Discard the result form CreateRuntime()
                ilGen.Emit(OpCodes.Ret);    

                this.Compiler.NativeGenerator.AssemblyBuilder.SetEntryPoint(main);
            }
        }

        private static readonly ConstructorInfo ScopeInitializerDelegateCtor = TypeUtilities.Constructor(
            typeof(Action<SmalltalkRuntime, SmalltalkNameScope>), typeof(object), typeof(IntPtr));

        private static readonly MethodInfo CreateRuntimeMethod = TypeUtilities.Method(typeof(IronSmalltalk.Runtime.Internal.NativeLoadHelper),
            "CreateRuntime", typeof(bool), typeof(Action<SmalltalkRuntime, SmalltalkNameScope>), typeof(Action<SmalltalkRuntime, SmalltalkNameScope>));

        private MethodBuilder GenerateCreateRuntime(bool hasInitializeParameter)
        {
            Type[] argTypes = hasInitializeParameter ? new Type[] { typeof(bool) } : new Type[] { };
            MethodBuilder method = this.Type.DefineMethod("CreateRuntime", MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard, typeof(SmalltalkRuntime), argTypes);

            if (hasInitializeParameter)
                method.DefineParameter(1, ParameterAttributes.None, "initialize");

            ILGenerator ilGen = method.GetILGenerator();

            if (hasInitializeParameter)
                ilGen.Emit(OpCodes.Ldarg_0);                                            // arg 0
            else
                ilGen.Emit(OpCodes.Ldc_I4_1);                                           // true

            // Create delegate for the extension scope initializer
            ilGen.Emit(OpCodes.Ldnull);                                                 // instance (this) ... it's a static method.
            ilGen.Emit(OpCodes.Ldftn, this.ExtensionScopeInitializer);                  // pointer to the initializer method
            ilGen.Emit(OpCodes.Newobj, RuntimeGenerator.ScopeInitializerDelegateCtor);  // create the delegate object

            // Create delegate for the global scope initializer
            ilGen.Emit(OpCodes.Ldnull);                                                 // instance (this) ... it's a static method.
            ilGen.Emit(OpCodes.Ldftn, this.GlobalScopeInitializer);                     // pointer to the initializer method
            ilGen.Emit(OpCodes.Newobj, RuntimeGenerator.ScopeInitializerDelegateCtor);  // create the delegate object

            // Call the method (on the stack: bool, Action<SmalltalkRuntime, SmalltalkNameScope>, Action<SmalltalkRuntime, SmalltalkNameScope>
            ilGen.Emit(OpCodes.Call, RuntimeGenerator.CreateRuntimeMethod);             // Call NativeLoadHelper.CreateRuntime();
            ilGen.Emit(OpCodes.Ret);                                                    // Return the result

            return method;
        }
    }
}
