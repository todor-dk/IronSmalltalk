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

using IronSmalltalk.Common;
using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.InterchangeInstaller;
using IronSmalltalk.NativeCompiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.CommandLineCompiler
{
    internal static class CommandLineCompiler
    {
        private static string PrintUsage()
        {
            string exeName = typeof(CommandLineCompiler).Assembly.GetName().Name;
            Console.WriteLine(@"IronSmalltalk Command Line Compiler

Usage: {0} <Options> <SourceFiles>

Options: 
-assemblyname <name>        Required. The display name of the assembly.
                            File extension should not be included.
                            Short form: -a
-namespace <name>           Required. Namespace of the containing .Net type.
                            Short form: -n
-out <path>                 Required. Output directory.
                            Short form: -o
-target <option>            Optional. Assembly target type. Default is exe.
                            Options: dll | exe | exe32 | exe64
                            Short form: -t
-version <version>          Optional. Shortcut for setting file, product and
                            assembly version simultaneously.
                            Short form: -v
-debug                      Optional. Emit debug code and debug symbols.
                            Short form: -d
-metaannotations            Optional. Add meta-annotations (comments, 
                            documentation, etc.) are installed (added) to the
                            corresponding runtime objects.
                            Meta-annotations start with the key: ist.meta.*
-assemblyversion <version>  Optional. Assembly version.
-baselibrary                Optional. This is used internally to compile the 
                            IronSmalltalk base library assembly.

                - UNMANEGED RESOURCE INFO OPTIONS -
-company <str>              Optional. Company that produced the file.
-copyright <str>            Optional. Copyright notices that apply to the file.
                            This should include the full text of all notices, 
                            legal symbols, copyright dates, and so on.
-product <str>              Optional. Name of the product with which the file
                            is distributed.
-fileversion <version>      Optional. File description to be presented to 
                            users.
-productversion <version>   Optional. Version of the product with which the
                            file is distributed.
-producttitle <version>     Optional. File description to be presented to 
                            users. 
-productdescription <str>   Optional. Product description to be presented to 
                            users. 
-trademark <str>            Optional. Trademarks and registered trademarks 
                            that apply to the file. This should include the 
                            full text of all notices, legal symbols, trademark
                            numbers, and so on.

Example:
  {0} -a HelloWorld -n HelloWorldApp -o C:\Temp HelloWorld.ist
", exeName);
            return null;
        }

        internal static string Compile(string[] args)
        {
            if (args == null)
                return CommandLineCompiler.PrintUsage();

            bool installMetaAnnotations = false;
            NativeCompilerParameters parameters = new NativeCompilerParameters();
            parameters.Product = "IronSmalltalk Application";
            parameters.ProductTitle = "IronSmalltalk Application";
            parameters.AssemblyType = NativeCompilerParameters.AssemblyTypeEnum.Exe;
            List<string> sourceFiles = new List<string>();
            string option = null;
            foreach (string arg in args)
            {
                if (arg == null)
                    return CommandLineCompiler.PrintUsage();
                if (option != null)
                {
                    if ((option == "a") || (option == "assemblyname"))
                    {
                        parameters.AssemblyName = arg;
                    }
                    else if ((option == "n") || (option == "namespace"))
                    {
                        parameters.RootNamespace = arg;
                    }
                    else if ((option == "o") || (option == "out"))
                    {
                        parameters.OutputDirectory = arg;
                    }
                    else if ((option == "t") || (option == "target"))
                    {
                        option = arg.ToLower().Trim();
                        if (option == "dll")
                            parameters.AssemblyType = NativeCompilerParameters.AssemblyTypeEnum.Dll;
                        else if (option == "exe")
                            parameters.AssemblyType = NativeCompilerParameters.AssemblyTypeEnum.Exe;
                        else if (option == "exe32")
                            parameters.AssemblyType = NativeCompilerParameters.AssemblyTypeEnum.Exe32;
                        else if (option == "exe64")
                            parameters.AssemblyType = NativeCompilerParameters.AssemblyTypeEnum.Exe64;
                        else
                            return CommandLineCompiler.PrintUsage();    
                    }
                    else if ((option == "v") || (option == "version"))
                    {
                        parameters.AssemblyVersion = arg;
                        parameters.FileVersion = arg;
                        parameters.ProductVersion = arg;
                    }
                    else if (option == "company")
                    {
                        parameters.Company = arg;
                    }
                    else if (option == "copyright")
                    {
                        parameters.Copyright = arg;
                    }
                    else if (option == "product")
                    {
                        parameters.Product = arg;
                    }
                    else if (option == "assemblyversion")
                    {
                        parameters.AssemblyVersion = arg;
                    }
                    else if (option == "fileversion")
                    {
                        parameters.FileVersion = arg;
                    }
                    else if (option == "productversion")
                    {
                        parameters.ProductVersion = arg;
                    }
                    else if (option == "producttitle")
                    {
                        parameters.ProductTitle = arg;
                    }
                    else if (option == "productdescription")
                    {
                        parameters.ProductDescription = arg;
                    }
                    else if (option == "trademark")
                    {
                        parameters.Trademark = arg;
                    }
                    else
                    {
                        return CommandLineCompiler.PrintUsage();
                    }
                    option = null;
                }
                else if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    option = arg.Substring(1).ToLower().Trim();
                    if (option.Length == 0)
                        return CommandLineCompiler.PrintUsage();
                    if (option[option.Length - 1] == ':')
                        option = option.Substring(0, option.Length - 1);
                    if (option.Length == 0)
                        return CommandLineCompiler.PrintUsage();

                    if ((option == "d") || (option == "debug"))
                    {
                        parameters.EmitDebugSymbols = true;
                        option = null;
                    } else if (option == "metaannotations")
                    {
                        parameters.EmitDebugSymbols = true;
                        option = null;
                    }
                    else if (option == "baselibrary")
                    {
                        parameters.IsBaseLibrary = true;
                        option = null;
                    }
                }
                else
                {
                    sourceFiles.Add(arg);
                }
            }

            if (String.IsNullOrWhiteSpace(parameters.RootNamespace))
                return CommandLineCompiler.PrintUsage();
            if (String.IsNullOrWhiteSpace(parameters.OutputDirectory))
                return CommandLineCompiler.PrintUsage();
            if (String.IsNullOrWhiteSpace(parameters.AssemblyName))
                return CommandLineCompiler.PrintUsage();
            if (sourceFiles.Count == 0)
                return CommandLineCompiler.PrintUsage();

            try
            {
                return CommandLineCompiler.Compile(parameters, installMetaAnnotations, sourceFiles);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                return null;
            }
        }

        internal static string Compile(NativeCompilerParameters parameters, bool installMetaAnnotations, IEnumerable<string> sourceFiles)
        {
            IFileInErrorSink errorSink = new ErrorSink();

            List<PathFileInInformation> fileIns = new List<PathFileInInformation>();
            foreach (string sourceFile in sourceFiles)
            {
                SymbolDocumentInfo symbolDocument = null;
                if (parameters.EmitDebugSymbols)
                    symbolDocument = Expression.SymbolDocument(sourceFile, GlobalConstants.LanguageGuid, GlobalConstants.VendorGuid);
                PathFileInInformation fileIn = new PathFileInInformation(sourceFile, System.Text.Encoding.UTF8, errorSink, symbolDocument);
                fileIns.Add(fileIn);
            }

            if (fileIns.Count == 0)
                return null;

            SmalltalkRuntime runtime = new SmalltalkRuntime();
            FileInService compilerService = new FileInService(runtime, installMetaAnnotations, fis => 
                parameters.IsBaseLibrary ? new InternalInstallerContext(fis.Runtime) : new InterchangeInstallerContext(fis.Runtime));

            InterchangeInstallerContext installer = compilerService.Read(fileIns);

            installer.ErrorSink = new InstallErrorSink();
            installer.InstallMetaAnnotations = compilerService.InstallMetaAnnotations;
            if (!installer.Install())
                return null;

            parameters.Runtime = runtime;
            return NativeCompiler.NativeCompiler.GenerateNativeAssembly(parameters);
        }

        private class ErrorSink : IronSmalltalk.Internals.ErrorSinkBase
        {
            protected override void ReportError(string message, SourceLocation start, SourceLocation end, IronSmalltalk.Internals.ErrorSinkBase.ErrorType type, params object[] offenders)
            {
                this.ReportError(String.Format("Error: {0} ({1} ==> {2}) : {3}", type, start, end, message));
            }

            private void ReportError(string msg)
            {
                Console.WriteLine(msg);
            }
        }

        private class InstallErrorSink : IInstallErrorSink
        {
            public void AddInstallError(string installErrorMessage, ISourceReference sourceReference)
            {
                if (sourceReference == null)
                    throw new ArgumentNullException("sourceReference");
                FileInInformation sourceObject = sourceReference.Service.SourceObject as FileInInformation;
#if DEBUG
                System.Diagnostics.Debug.Assert(sourceObject != null);
#endif
                if (sourceObject == null)
                    return; // This is like having no error sink
                if (sourceObject.ErrorSink == null)
                    return;
                sourceObject.ErrorSink.AddInstallError(sourceReference.StartPosition, sourceReference.StopPosition, installErrorMessage);
            }
        }
    }
}
