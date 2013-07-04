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
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.NativeCompiler.Internals;

namespace IronSmalltalk.NativeCompiler
{
    public class NativeCompiler
    {
        public static void GenerateNativeAssembly(NativeCompilerParameters parameters)
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
            if (String.IsNullOrWhiteSpace(parameters.FileExtension))
                throw new ArgumentNullException("parameters.FileExtension");

            NativeCompiler compiler = new NativeCompiler(parameters);
            compiler.Generate();
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

        private void Generate()
        {
            NameScopeGenerator extensionScope = new NameScopeGenerator(this, "ExtensionScope", true);
            this.Parameters.Runtime.ExtensionScope.Accept(extensionScope);
            NameScopeGenerator globalScope = new NameScopeGenerator(this, "GlobalScope", false);
            this.Parameters.Runtime.GlobalScope.Accept(globalScope);

            //RuntimeGenerator runtimeGen = new RuntimeGenerator(this);
            //runtimeGen.GenerateTypes();
            extensionScope.Generate();
            globalScope.Generate();

            RuntimeGenerator runtime = new RuntimeGenerator(this, extensionScope, globalScope);
            runtime.Generate();
            

            /* Order should be:
             * 1. Create Global Bindings
             * 2. Create Objects (for global bindings)
             * 3. Create Pool Variable Bindings
             * 
             * 
             *             
                this.CreateTemporaryNameSpace();
                
                if (!this.CreateGlobalBindings())
                    return false;
                if (!this.CreateGlobalObjects())
                    return false;
                if (!this.ValidateGlobalObjects())
                    return false;
                if (!this.CreatePoolVariableBindings())
                    return false;
                if (!this.ValidateMethods())
                    return false;
                if (!this.ValidateInitializers())
                    return false;
            if (!this.CreateMethods())
                return false;
            if (!this.CreateInitializers())
                return false;
            if (!this.AddAnnotation())      ** Methods + Initializers
                return false;

                this.ReplaceSmalltalkContextNameSpace();
                return this.RecompileClasses(); // Must be after ReplaceSmalltalkContextNameSpace(), otherwise class cannot find subclasses.
             * 
             * 
             * 
             * 
             * 
N-Space.
	Globals
		Constant
		Variable
	Classes
	Pools
	
	


C:\Users\tt\Documents\Visual Studio 2010\Projects\IronSmalltalk\ClassLibraryBrowser\External\IronSmalltalk.ist




’!’ | ’%’ | ’&’’ | ’*’ | ’+’ | ’,’’ | ’/’ | ’<’ | ’=’ | ’>’ | ’?’ | ’@’ | ’\’ | ’~’ | ’|’ | ’-’


 ' !"#$%&''()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~'

 "#$'(),.:;[]^_`{}



public static void AnnotateObject(IDiscreteGlobalBinding binding, string key, string value)


{x:Static sys:Double.MaxValue}

Width="{x:Static s:Double.MaxValue}"


Dictionary<Symbol, CompiledMethod>
           */

            this.NativeGenerator.SaveAssembly();
        }

        
    }
}
