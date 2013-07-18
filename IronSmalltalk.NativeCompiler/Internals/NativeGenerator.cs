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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.Internals
{
    internal class NativeGenerator
    {
        internal readonly string OutputPath;
        internal readonly NativeCompilerParameters Parameters;
        internal readonly AssemblyName AssemblyName;
        internal readonly AssemblyBuilder AssemblyBuilder;
        internal readonly ModuleBuilder ModuleBuilder;
        internal readonly List<TypeBuilder> DefinedTypes = new List<TypeBuilder>();
        internal readonly DebugInfoGenerator DebugInfoGenerator;

        internal NativeGenerator(NativeCompilerParameters parameters)
        {
            this.Parameters = parameters;
            this.AssemblyName = new AssemblyName(parameters.AssemblyName);
            string filename = String.Format("{0}.{1}", this.AssemblyName.Name, parameters.FileExtension);
            this.OutputPath = System.IO.Path.Combine(parameters.OutputDirectory, filename);

            if (this.Parameters.EmitDebugSymbols)
                this.DebugInfoGenerator = DebugInfoGenerator.CreatePdbGenerator();

            CustomAttributeBuilder[] attributes = new CustomAttributeBuilder[] {};

            this.AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                this.AssemblyName,
                AssemblyBuilderAccess.Save,
                this.Parameters.OutputDirectory,
                false,
                attributes);

            if (this.Parameters.EmitDebugSymbols)
                this.SetDebuggableAttributes();

            this.AssemblyBuilder.DefineVersionInfoResource(parameters.Product, parameters.ProductVersion, parameters.Company, 
                parameters.Copyright, parameters.Trademark);

            this.ModuleBuilder = this.AssemblyBuilder.DefineDynamicModule(this.AssemblyName.Name,  filename, this.Parameters.EmitDebugSymbols);

            var a = this.AssemblyBuilder.ManifestModule;
        }

        private void SetDebuggableAttributes()
        {
            DebuggableAttribute.DebuggingModes attrs =
                DebuggableAttribute.DebuggingModes.Default |
                // Disable optimizations performed by the compiler to make your output file smaller, faster, and more efficient. 
                // Optimizations result in code rearrangement in the output file, which can make debugging difficult. 
                // Typically optimization should be disabled while debugging. 
                DebuggableAttribute.DebuggingModes.DisableOptimizations |
                // Sequence points are used to indicate locations in the Microsoft intermediate language (MSIL) code that 
                // a debugger user will expect to be able to refer to uniquely, such as for setting a breakpoint. 
                // The JIT compiler ensures it does not compile the MSIL at two different sequence points into a 
                // single native instruction. By default, the JIT compiler examines the symbol store in the program 
                // database (PDB) file for a list of additional sequence points. However, loading the PDB file requires 
                // that the file be available and has a negative performance impact. In version 2.0, compilers can emit 
                // "implicit sequence points" in the MSIL code stream through the use of MSIL "nop" instructions. 
                // Such compilers should set the IgnoreSymbolStoreSequencePoints flag to notify the common language 
                // runtime to not load the PDB file.
                DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints;

            Type[] argTypes = new Type[] { typeof(DebuggableAttribute.DebuggingModes) };
            Object[] argValues = new Object[] { attrs };

            var debuggableCtor = typeof(DebuggableAttribute).GetConstructor(argTypes);

            // To suppress optimizations when debugging dynamic modules, apply the DebuggableAttribute attribute 
            // to the dynamic assembly before calling DefineDynamicModule. Create an instance of DebuggableAttribute 
            // with the DisableOptimizations flag and apply it using the SetCustomAttribute method. The attribute 
            // must be applied to the dynamic assembly. It has no effect if applied to the module.
            this.AssemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(debuggableCtor, argValues));
        }

        internal void SaveAssembly()
        {
            foreach (TypeBuilder type in this.DefinedTypes)
            {
                if (!type.IsCreated())
                    type.CreateType();                    
            }
            string filename = System.IO.Path.GetFileName(this.OutputPath);
            this.AssemblyBuilder.Save(filename, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
        }

        internal TypeBuilder DefineType(string name, Type parent, TypeAttributes attr)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (parent == null)
                throw new ArgumentNullException("parent");

            StringBuilder sb = new StringBuilder(name);

            // There is a bug in Reflection.Emit that leads to 
            // Unhandled Exception: System.Runtime.InteropServices.COMException (0x80131130): Record not found on lookup.
            // if there is any of the characters []*&+,\ in the type name and a method defined on the type is called.
            sb.Replace('+', '_').Replace('[', '_').Replace(']', '_').Replace('*', '_').Replace('&', '_').Replace(',', '_').Replace('\\', '_');

            name = sb.ToString();

            TypeBuilder type = this.ModuleBuilder.DefineType(name, attr, parent);
            this.DefinedTypes.Add(type);
            return type;
        }

        internal string AsLegalTypeName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            // There is a bug in Reflection.Emit that leads to 
            // Unhandled Exception: System.Runtime.InteropServices.COMException (0x80131130): Record not found on lookup.
            // if there is any of the characters []*&+,\ in the type name and a method defined on the type is called.
            return name.Replace('+', '_').Replace('[', '_').Replace(']', '_').Replace('*', '_').Replace('&', '_').Replace(',', '_').Replace('\\', '_');
        }

        internal string AsLegalMethodName(string name)
        {
            return this.AsLegalMemberName(name);
        }

        internal string AsLegalArgumentName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            return name; // No restrictions ... so far.            
        }

        private string AsLegalMemberName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            return name; // No restrictions ... so far.
        }


        // _myAssembly.Save(_outFileName, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);

        /*

        public Type MakeDelegateType(string name, Type[] parameters, Type returnType) {
            TypeBuilder builder = DefineType(name, typeof(MulticastDelegate), DelegateAttributes, false);
            builder.DefineConstructor(CtorAttributes, CallingConventions.Standard, _DelegateCtorSignature).SetImplementationFlags(ImplAttributes);
            builder.DefineMethod("Invoke", InvokeAttributes, returnType, parameters).SetImplementationFlags(ImplAttributes);
            return builder.CreateType();
        }
         */


        /*
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal static void PeVerifyAssemblyFile(string fileLocation) {
#if FEATURE_FILESYSTEM
            Debug.WriteLine("Verifying generated IL: " + fileLocation);
            string outDir = Path.GetDirectoryName(fileLocation);
            string outFileName = Path.GetFileName(fileLocation);
            string peverifyPath = FindPeverify();
            if (peverifyPath == null) {
                Debug.WriteLine("PEVerify not available");
                return;
            }

            int exitCode = 0;
            string strOut = null;
            string verifyFile = null;

            try {
                string pythonPath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;

                string assemblyFile = Path.Combine(outDir, outFileName).ToLower(CultureInfo.InvariantCulture);
                string assemblyName = Path.GetFileNameWithoutExtension(outFileName);
                string assemblyExtension = Path.GetExtension(outFileName);
                Random rnd = new System.Random();

                for (int i = 0; ; i++) {
                    string verifyName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}{3}", assemblyName, i, rnd.Next(1, 100), assemblyExtension);
                    verifyName = Path.Combine(Path.GetTempPath(), verifyName);

                    try {
                        File.Copy(assemblyFile, verifyName);
                        verifyFile = verifyName;
                        break;
                    } catch (IOException) {
                    }
                }

                // copy any DLLs or EXEs created by the process during the run...
                CopyFilesCreatedSinceStart(Path.GetTempPath(), Environment.CurrentDirectory, outFileName);
                CopyDirectory(Path.GetTempPath(), pythonPath);
                if (Snippets.Shared.SnippetsDirectory != null && Snippets.Shared.SnippetsDirectory != Path.GetTempPath()) {
                    CopyFilesCreatedSinceStart(Path.GetTempPath(), Snippets.Shared.SnippetsDirectory, outFileName);
                }

                // /IGNORE=80070002 ignores errors related to files we can't find, this happens when we generate assemblies
                // and then peverify the result.  Note if we can't resolve a token thats in an external file we still
                // generate an error.
                ProcessStartInfo psi = new ProcessStartInfo(peverifyPath, "/IGNORE=80070002 \"" + verifyFile + "\"");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                Process proc = Process.Start(psi);
                Thread thread = new Thread(
                    new ThreadStart(
                        delegate {
                            using (StreamReader sr = proc.StandardOutput) {
                                strOut = sr.ReadToEnd();
                            }
                        }
                        ));

                thread.Start();
                proc.WaitForExit();
                thread.Join();
                exitCode = proc.ExitCode;
                proc.Close();
            } catch (Exception e) {
                strOut = "Unexpected exception: " + e.ToString();
                exitCode = 1;
            }

            if (exitCode != 0) {
                Console.WriteLine("Verification failed w/ exit code {0}: {1}", exitCode, strOut);
                throw Error.VerificationException(
                    outFileName,
                    verifyFile,
                    strOut ?? "");
            }

            if (verifyFile != null) {
                File.Delete(verifyFile);
            }
#endif
        }

         */
    }
}
