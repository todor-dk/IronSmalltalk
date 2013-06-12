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
        public static void GenerateNativeAssembly(SmalltalkRuntime runtime, string rootNamespace, string outputDirectory, string assemblyName, string fileExtension, bool emitDebugSymbols, 
            string product, string productVersion, string company, string copyright, string trademark)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (String.IsNullOrWhiteSpace(rootNamespace))
                throw new ArgumentNullException("rootNamespace");
            if (String.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentNullException("outputDirectory");
            if (String.IsNullOrWhiteSpace(assemblyName))
                throw new ArgumentNullException("assemblyName");
            if (String.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentNullException("fileExtension");

            NativeCompiler compiler = new NativeCompiler(rootNamespace, outputDirectory, assemblyName, fileExtension, emitDebugSymbols,
                product, productVersion, company, copyright, trademark);
            compiler.Generate(runtime);
        }

        internal readonly string RootNamespace;

        internal NativeGenerator NativeGenerator { get; private set; }

        private NativeCompiler(string rootNamespace, string outputDirectory, string assemblyName, string fileExtension, bool emitDebugSymbols, 
            string product, string productVersion, string company, string copyright, string trademark)
        {
            this.RootNamespace = rootNamespace;

            this.NativeGenerator = new NativeGenerator(outputDirectory, assemblyName, fileExtension, emitDebugSymbols,
                product, productVersion, company, copyright, trademark);
        }

        internal string GetTypeName(string partialName)
        {
            if (String.IsNullOrWhiteSpace(partialName))
                throw new ArgumentNullException();
            return String.Format("{0}.{1}", this.RootNamespace, partialName);
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

        private void Generate(SmalltalkRuntime runtime)
        {
            NameScopeGenerator extensionScope = new NameScopeGenerator(this, "ExtensionScope");
            runtime.ExtensionScope.Accept(extensionScope);
            NameScopeGenerator globalScope = new NameScopeGenerator(this, "GlobalScope");
            runtime.GlobalScope.Accept(globalScope);

            //RuntimeGenerator runtimeGen = new RuntimeGenerator(this);
            //runtimeGen.GenerateTypes();
            extensionScope.Generate();
            globalScope.Generate();



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
