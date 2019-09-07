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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.NativeCompiler.Generators;
using IronSmalltalk.NativeCompiler.Internals;
using System.Reflection.Emit;
using System.Dynamic;
using IronSmalltalk.Common.Internal;

namespace IronSmalltalk.NativeCompiler
{
	/// <summary>
	/// The native compiler generates a .Net assembly given a Smalltalk runtime definition.
	/// </summary>
	/// <remarks>
	/// This class is the entry point to the native compiler.
	/// It is initializer from a NativeCompilerParameters object.
	/// </remarks>
	public class NativeCompiler
	{
		/// <summary>
		/// Generate a .Net native assembly for the given Smalltalk runtime definition.
		/// </summary>
		/// <param name="parameters">
		/// Parameter object that contains the information about the Smalltalk runtime
		/// as well as additional parameters that govern the native assembly generation.
		/// </param>
        /// <returns>The path to the newly generated assembly.</returns>
		public static string GenerateNativeAssembly(NativeCompilerParameters parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			if (parameters.Runtime == null)
				throw new ArgumentNullException("parameters.Runtime");
			if (String.IsNullOrWhiteSpace(parameters.RootNamespace))
				throw new ArgumentNullException("parameters.RootNamespace");
			if (String.IsNullOrWhiteSpace(parameters.OutputDirectory))
				throw new ArgumentNullException("parameters.OutputDirectory");
			if (String.IsNullOrWhiteSpace(parameters.AssemblyName))
				throw new ArgumentNullException("parameters.AssemblyName");
            if (parameters.IsBaseLibrary && (parameters.AssemblyType != NativeCompilerParameters.AssemblyTypeEnum.Dll))
                throw new ArgumentException("parameters.IsBaseLibrary requires parameters.AssemblyType = AssemblyTypeEnum.Dll");
            if (parameters.IsBaseLibrary && !parameters.Runtime.GlobalScope.IsEmpty)
                throw new ArgumentException("parameters.IsBaseLibrary requires parameters.Runtime.GlobalScope to be empty");

			NativeCompiler compiler = new NativeCompiler(parameters);
			return compiler.Generate();
		}


		internal readonly NativeCompilerParameters Parameters;
		internal readonly NativeGenerator NativeGenerator;

		private NativeCompiler(NativeCompilerParameters parameters)
		{
			this.Parameters = parameters.Copy();

			this.NativeGenerator = new NativeGenerator(parameters);
		}

		internal string GetTypeName(string partialName)
		{
			if (String.IsNullOrWhiteSpace(partialName))
				throw new ArgumentNullException();
			return this.NativeGenerator.AsLegalTypeName(String.Format("{0}.{1}", this.Parameters.RootNamespace, partialName));
		}

		internal string GetTypeName(string subNamespace, string typeName)
		{
			if (String.IsNullOrWhiteSpace(typeName))
				throw new ArgumentNullException();
			if (String.IsNullOrWhiteSpace(subNamespace))
				return this.GetTypeName(typeName);
			else
				return this.GetTypeName(String.Format("{0}.{1}", subNamespace, typeName));
		}

		internal string GetTypeName(params string[] names)
		{
			if (names == null)
				throw new ArgumentNullException();
			return this.GetTypeName(String.Join(".", names));
		}

        private TypeBuilder _ConvertBinderType;
        private TypeBuilder ConvertBinderType
        {
            get
            {
                if (this._ConvertBinderType == null)
                    this._ConvertBinderType = this.NativeGenerator.DefineType(
                        this.GetTypeName("ConvertBinders"),
                        typeof(Object),
                        TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract);
                return this._ConvertBinderType;
            }
        }

		/// <summary>
		/// The main method - responsible for generation of the assembly
		/// </summary>
		private string Generate()
		{
            if (this.Parameters.IsBaseLibrary)
                this.GenerateStanradLibrary();
            else
                this.GenerateCustomLibrary();

            this.GenerateDynamicConverterBinderInitializers();
			return this.NativeGenerator.SaveAssembly();
		}

        private void GenerateCustomLibrary()
        {
            // Visit the name scopes in the runtime.
            NameScopeGenerator extensionScope = null, globalScope = null;  
            if (this.Parameters.ExtensionScopeInitializer == null)
            {
                extensionScope = new NameScopeGenerator(this, "ExtensionScope", true);
                this.Parameters.Runtime.ExtensionScope.Accept(extensionScope);
            }
            globalScope = new NameScopeGenerator(this, "GlobalScope", false);
            this.Parameters.Runtime.GlobalScope.Accept(globalScope);

            // Generate types from the result of the name scope visiting
            MethodInfo extensionScopeInitializer, globalScopeInitializer;
            if (this.Parameters.ExtensionScopeInitializer == null)
                extensionScopeInitializer = extensionScope.GenerateNameScopeInitializerMethod();
            else
                extensionScopeInitializer = this.Parameters.ExtensionScopeInitializer;
            globalScopeInitializer = globalScope.GenerateNameScopeInitializerMethod();
            // Generate the entry point class, the one that creates the new Smalltalk runtime.
            RuntimeGenerator runtime = new RuntimeGenerator(this, extensionScopeInitializer, globalScopeInitializer);
            runtime.GenerateCreateRuntimeMethods();
        }

        private void GenerateStanradLibrary()
        {
            System.Diagnostics.Debug.Assert(this.Parameters.Runtime.GlobalScope.IsEmpty);

            // Visit the name scopes in the runtime.
            NameScopeGenerator scope = new NameScopeGenerator(this, "StandardScope", true);
            this.Parameters.Runtime.ExtensionScope.Accept(scope);

            // Generate types from the result of the name scope visiting
            MethodInfo scopeInitializer = scope.GenerateNameScopeInitializerMethod();
            // Generate the entry point class, the one that creates the new Smalltalk runtime.
            StandardLibraryGenerator library = new StandardLibraryGenerator(this, scopeInitializer);
            library.GenerateEntryMethod();
        }    

        internal FieldInfo GetDynamicConvertBinder(Type type, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags)
        {
            FieldInfo field;
            Tuple<Type, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags> key = new Tuple<Type,Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags>(type, flags);
            this.ConvertBinders.TryGetValue(key, out field);
            if (field != null)
                return field;

            string nameSuggestion = String.Format("{0}_{1}", flags, type.Name).Replace(" ", "").Replace("|", "_").Replace(",", "_");
            string name = nameSuggestion;
            for (int i = 1; ; i++)
            {
                if (!this.ConvertBinders.Values.Any(f => f.Name == name))
                    break;
                name = String.Format("{0}_{1}", nameSuggestion, i);
            }

            field = this.ConvertBinderType.DefineField(name, 
                typeof(ConvertBinder),
                FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Assembly);

            this.ConvertBinders.Add(key, field);
            return field;
        }

        private Dictionary<Tuple<Type, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags>, FieldInfo> ConvertBinders = new Dictionary<Tuple<Type, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags>, FieldInfo>();

        private static readonly MethodInfo GetTypeFromHandleMethod = TypeUtilities.Method(typeof(Type), "GetTypeFromHandle");

        private static readonly MethodInfo ConvertMethod = TypeUtilities.Method(typeof(Microsoft.CSharp.RuntimeBinder.Binder), "Convert");

        private void GenerateDynamicConverterBinderInitializers()
        {
            if (this._ConvertBinderType == null)
                return;
            // It would have been nice to use the Lambda Compiler, but it can't compile constructors,
            // so we have to generate the constructor by emitting IL code by hand :/
            ConstructorBuilder ctor = this._ConvertBinderType.DefineConstructor(
                MethodAttributes.Private | MethodAttributes.Static, CallingConventions.Standard, new Type[0]);
            ILGenerator ctorIL = ctor.GetILGenerator();
            // ... now assign to each literal field
            foreach(var pair in this.ConvertBinders)
            {
                ctorIL.PushInt((int) pair.Key.Item2);
                ctorIL.Emit(OpCodes.Ldtoken, pair.Key.Item1);
                ctorIL.Emit(OpCodes.Call, NativeCompiler.GetTypeFromHandleMethod);
                ctorIL.Emit(OpCodes.Ldtoken, this._ConvertBinderType);
                ctorIL.Emit(OpCodes.Call, NativeCompiler.GetTypeFromHandleMethod);
                ctorIL.Emit(OpCodes.Call, NativeCompiler.ConvertMethod);
                ctorIL.Emit(OpCodes.Castclass, typeof(ConvertBinder));
                ctorIL.Emit(OpCodes.Stsfld, pair.Value);  // Store the value in the static field for the binder
            }
            ctorIL.Emit(OpCodes.Ret);

            this._ConvertBinderType.CreateType();
        }
	}
}
