﻿/*
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
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.CommandLineCompiler
{
    internal static class CommandLineCompiler
    {
        private static string PrintUsage()
        {
            CommandLineCompiler.PrintCopyrightMessage();

            string exeName = typeof(CommandLineCompiler).Assembly.GetName().Name;
            Console.WriteLine(@"
Usage: {0} <Options> <SourceFiles>

Options: 
-assemblyname <name>        Optional. The display name of the assembly.
                            File extension should not be included.
                            If ommitted, the filename of the first source file
                            will be used for the assmebly name.
                            Short form: -a
-namespace <name>           Optional. Namespace of the containing .Net type.
                            If ommitted, the assembly name will be used.
                            Short form: -n
-out <path>                 Optional. Output directory.
                            If ommitted, the current directory will be used.
                            Short form: -o
-target <option>            Optional. Assembly target type. Default is exe.
                            Options: dll | exe | exe32 | exe64
                            Short form: -t
-classlibrary <path>        Optional. The path to the base library assembly.
                            If this is not given, the name name of the base
                            library assembly is 'IronSmalltalk.ClassLibrary'
                            and is expected to be found in the directory
                            of the Command Line Compiler.
                            Short form: -c
-noclasslibrary             Optional. Disables binding to a standard class
                            library. If set, the source code is expected
                            to contain all the base classes normally 
                            found in the standard class library.
                            This option is incompatible with -classlibrary.
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
                            This requires -target dll.
                            This option is incompatible with -classlibrary
                            and automatically adds -noclasslibrary.
-nologo                     Optional. Suppress compiler copyright message.
-quiet                      Optional. Do not display information messages.
                            Short form: -q

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
  {0} -a HelloWorld HelloWorldSources.ist
", exeName);
            return null;
        }

        private static void PrintCopyrightMessage()
        {
            Assembly assembly = typeof(CommandLineCompiler).Assembly;
            Console.WriteLine();
            Console.WriteLine("IronSmalltalk Command Line Compiler Version {0}", assembly.GetName().Version);
            Console.WriteLine("Copyright (C) The IronSmalltalk Project. All rights reserved.");
        }

        internal static string Compile(string[] args)
        {
            if (args == null)
                return CommandLineCompiler.PrintUsage();

            bool showCopyrightMessage = true;
            bool printParameters = true;
            string classLibrary = null;
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
                        parameters.OutputDirectory = Path.GetFullPath(arg);
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
                    else if ((option == "c") || (option == "classlibrary"))
                    {
                        string cl = arg.Trim();
                        if (String.IsNullOrWhiteSpace(cl))
                            return CommandLineCompiler.PrintUsage();
                        classLibrary = Path.GetFullPath(cl);
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
                    else if (option == "noclasslibrary")
                    {
                        classLibrary = "";
                        option = null;
                    }
                    else if (option == "nologo")
                    {
                        showCopyrightMessage = false;
                        option = null;
                    }
                    else if ((option == "q") || (option == "quiet"))
                    {
                        printParameters = false;
                        option = null;
                    }
                    else if (option == "istscl")
                    {
                        // Undocumented parameter .... shortcut to set the parameters for compiling the 
                        // IronSmalltalk Standard Class Library. Pats are relative to the .Net project.
                        parameters.AssemblyName = "IronSmalltalk.ClassLibrary";
                        parameters.AssemblyName = "IronSmalltalk.ClassLibrary";
                        parameters.OutputDirectory = Path.GetFullPath(".");
                        parameters.AssemblyType = NativeCompilerParameters.AssemblyTypeEnum.Dll;
                        classLibrary = "";
                        parameters.AssemblyVersion = typeof(CommandLineCompiler).Assembly.GetName().Version.ToString();
                        parameters.FileVersion = parameters.AssemblyVersion;
                        parameters.ProductVersion = parameters.AssemblyVersion;
                        parameters.IsBaseLibrary = true;
                        parameters.Company = "The IronSmalltalk Project";
                        parameters.Copyright = "Copyright © The IronSmalltalk Project 2013";
                        parameters.Product = "IronSmalltalk";
                        parameters.ProductTitle = "IronSmalltalk Standard Class Library";
                        parameters.ProductDescription = "IronSmalltalk Standard Class Library";
#if DEBUG
                        parameters.EmitDebugSymbols = true;
#endif
                        option = null;
                        sourceFiles.Add(Path.GetFullPath(@"..\..\..\ClassLibraryBrowser\External\IronSmalltalk.ist"));
                    }
                }
                else
                {
                    sourceFiles.Add(Path.GetFullPath(arg));
                }
            }

            if (sourceFiles.Count == 0)
                return CommandLineCompiler.PrintUsage();
            if (String.IsNullOrWhiteSpace(parameters.AssemblyName))
                parameters.AssemblyName = Path.GetFileNameWithoutExtension(sourceFiles[0]);
            if (String.IsNullOrWhiteSpace(parameters.RootNamespace))
                parameters.RootNamespace = parameters.AssemblyName;
            if (String.IsNullOrWhiteSpace(parameters.OutputDirectory))
                parameters.OutputDirectory = Directory.GetCurrentDirectory();
            if (parameters.IsBaseLibrary && (parameters.AssemblyType != NativeCompilerParameters.AssemblyTypeEnum.Dll))
                return CommandLineCompiler.PrintUsage();

            if (classLibrary == null)
                classLibrary = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "IronSmalltalk.ClassLibrary.dll");

            try
            {
                if (showCopyrightMessage)
                    CommandLineCompiler.PrintCopyrightMessage();

                if (!CommandLineCompiler.GetStandardClassLibraryEntryPoint(parameters, classLibrary))
                    return null;

                if (!Directory.Exists(parameters.OutputDirectory))
                {
                    Console.WriteLine();
                    Console.WriteLine("Output Directory does not exists.");
                    return null;
                }

                if (printParameters)
                    CommandLineCompiler.PrintParameters(parameters, installMetaAnnotations, sourceFiles);

                string path = CommandLineCompiler.Compile(parameters, installMetaAnnotations, sourceFiles);
                if (path == null)
                    return null;

                if (printParameters)
                    Console.WriteLine("Assembly saved to:\n    {0}", path);

                return path;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                return null;
            }
        }

        private static void PrintParameters(NativeCompilerParameters parameters, bool installMetaAnnotations, IEnumerable<string> sourceFiles)
        {
            string classLibrary = "N/A";
            if (!parameters.IsBaseLibrary)
                classLibrary = parameters.ExtensionScopeInitializer.DeclaringType.Assembly.Location;
            Console.WriteLine(@"
Assembly Name:    {0}
Namespace:        {1}
Output Location:  {2}
Target Type:      {3}
Class Library:    {4}
Debug Info:       {5}
Meta Annotations: {6}
IST Base Library: {7}
Assembly Version: {8}
Company:          {9}
Copyright:        {10}
Description:      {11}
File Version:     {12}
Product:          {13}
Product Title:    {14}
Product Version:  {15}
Trademark:        {16}
Source Files:", 
            parameters.AssemblyName,
            parameters.RootNamespace,
            parameters.OutputDirectory,
            parameters.AssemblyType,
            classLibrary,
            parameters.EmitDebugSymbols,
            installMetaAnnotations,
            parameters.IsBaseLibrary,
            parameters.AssemblyVersion,
            parameters.Company,
            parameters.Copyright,
            parameters.ProductDescription,
            parameters.FileVersion,
            parameters.Product,
            parameters.ProductTitle,
            parameters.ProductVersion,
            parameters.Trademark);

            foreach (string file in sourceFiles)
                Console.WriteLine("    " + file);

            Console.WriteLine();
        }

        private static string Compile(NativeCompilerParameters parameters, bool installMetaAnnotations, IEnumerable<string> sourceFiles)
        {
            List<PathFileInInformation> fileIns = new List<PathFileInInformation>();
            foreach (string sourceFile in sourceFiles)
            {
                SymbolDocumentInfo symbolDocument = null;
                if (parameters.EmitDebugSymbols)
                    symbolDocument = Expression.SymbolDocument(sourceFile, GlobalConstants.LanguageGuid, GlobalConstants.VendorGuid);
                PathFileInInformation fileIn = new PathFileInInformation(sourceFile, System.Text.Encoding.UTF8, new FileInErrorSink(sourceFile), symbolDocument);
                fileIns.Add(fileIn);
            }

            if (fileIns.Count == 0)
                return null;

            SmalltalkRuntime runtime;

            if (parameters.ExtensionScopeInitializer == null)
            {
                runtime = new SmalltalkRuntime();
            }
            else
            {
                runtime = NativeLoadHelper.CreateRuntime(false,
                    (rt, scope) => { parameters.ExtensionScopeInitializer.Invoke(null, new object[] { rt, scope }); },
                    (rt, scope) => { /* Nothing here */ });
            }

            FileInService compilerService = new FileInService(runtime, installMetaAnnotations, fis => 
                parameters.IsBaseLibrary ? new InternalInstallerContext(fis.Runtime) : new InterchangeInstallerContext(fis.Runtime));

            InterchangeInstallerContext installer = compilerService.Read(fileIns);

            foreach (var fileIn in fileIns)
            {
                if (((FileInErrorSink)fileIn.ErrorSink).HadError)
                    return null;    // Some of the source file had errors, do not attempt the rest - it's meaningless
            }

            installer.ErrorSink = new InstallErrorSink();
            installer.InstallMetaAnnotations = compilerService.InstallMetaAnnotations;
            if (!installer.Install())
                return null;

            parameters.Runtime = runtime;
            return NativeCompiler.NativeCompiler.GenerateNativeAssembly(parameters);
        }

        private static bool GetStandardClassLibraryEntryPoint(NativeCompilerParameters parameters, string classLibrary)
        {
            if (parameters.IsBaseLibrary)
                return true;
            if (classLibrary == "")
                return true;

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(classLibrary);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Cannot find or load the standard class library assembly: {0}.", classLibrary);
                return false;
            }

            foreach (Type type in assembly.GetExportedTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    if (method.GetCustomAttribute(typeof(ScopeInitializerAttribute)) != null)
                    {
                        ParameterInfo[] ps = method.GetParameters();
                        if ((ps != null) && (ps.Length == 2))
                        {
                            if ((ps[0].ParameterType == typeof(SmalltalkRuntime)) && (ps[1].ParameterType == typeof(SmalltalkNameScope)))
                            {
                                if (parameters.ExtensionScopeInitializer != null)
                                {
                                    Console.WriteLine("The assembly {0} does not appear to be IronSmalltalk class library.", classLibrary);
                                    Console.WriteLine("Several public methods were decorated with the [ScopeInitializer] attribute.");
                                    return false;

                                }
                                parameters.ExtensionScopeInitializer = method;
                            }
                        }
                    }
                }
            }

            if (parameters.ExtensionScopeInitializer == null)
            {
                Console.WriteLine("The assembly {0} does not appear to be IronSmalltalk class library.", classLibrary);
                Console.WriteLine("IronSmalltalk class libraries have a public method decorated with the [ScopeInitializer] attribute.");
                return false;
            }

            return true;
        }


        private class FileInErrorSink : IronSmalltalk.Internals.ErrorSinkBase
        {
            private readonly string SourceFile;

            public bool HadError { get; private set; }

            public FileInErrorSink(string sourceFile)
            {
                this.SourceFile = sourceFile;
                this.HadError = false;
            }

            protected override void ReportError(string message, SourceLocation start, SourceLocation end, IronSmalltalk.Internals.ErrorSinkBase.ErrorType type, params object[] offenders)
            {
                this.HadError = true;
                this.ReportError(String.Format("{0} ({1} --> {2})\n\r{3} Error: {4}",
                    this.SourceFile, start, end, type, message));
            }

            private void ReportError(string msg)
            {
                Console.WriteLine(msg);
                Console.WriteLine();
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
