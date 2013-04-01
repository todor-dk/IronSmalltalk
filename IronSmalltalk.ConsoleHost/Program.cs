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

// *** 
// This is the old console host implementation.
// It is now replaced with the DLR based implementation.
// See the ConsoleHost class.
// ***

/*
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using IronSmalltalk.Common;
//using IronSmalltalk.Interchange;

//namespace IronSmalltalk.Console
//{
//    public static class Program
//    {
//        public static void Main(string[] args)
//        {
//            System.Console.WriteLine("IronSmalltalk Console Host");
//            System.Console.WriteLine();

//            if (args == null)
//                args = new string[0];

//            // Check if help is requested
//            foreach (string arg in args)
//            {
//                if (Program.IsHelpRequest(arg))
//                {
//                    Program.PrintHelp();
//                    return;
//                }
//            }

//            bool ist = true;
//            List<TextReader> sources = new List<TextReader>();
//            List<string> evaluates = new List<string>();
//            for (int i = 0; i < args.Length; i++)
//            {
//                string arg = args[i];
//                if (arg == null)
//                    arg = String.Empty;
//                arg = arg.Trim();

//                // We don't care if '/' or '-' is used
//                if ((arg.Length >= 1) && (arg[0] == '/'))
//                    arg = "-" + arg.Substring(1);

//                if (arg.StartsWith("-f:", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    // File-in Smalltalk interchange format file
//                    sources.Add(File.OpenText(arg.Substring(3)));
//                }
//                else if (arg.StartsWith("-r:", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    // Evaluate Smalltalk workspace style file
//                    evaluates.Add(System.IO.File.ReadAllText(arg.Substring(3), System.Text.Encoding.UTF8));
//                }
//                else if (arg.StartsWith("-n:", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    // File-in interchange format Smalltalk code
//                    sources.Add(new StringReader(arg.Substring(3)));
//                }
//                else if (arg.StartsWith("-e:", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    // Evaluate Smalltalk workspace style code"
//                    evaluates.Add(arg.Substring(3));
//                }
//                else if (arg.Equals("-ist:yes", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    // Include the standard IronSmalltalk class library
//                    ist = true;
//                }
//                else if (arg.Equals("-ist:no", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    // Include the standard IronSmalltalk class library
//                    ist = false;
//                }
//                else
//                {
//                    // Error
//                    System.Console.WriteLine("Input Error: Unknown option \"{0}\" specified.", arg);
//                    return;
//                }
//            }

//            if ((evaluates.Count == 0) && (sources.Count == 0))
//            {
//                Program.PrintHelp();
//                return;
//            }

//            if (ist)
//                sources.Insert(0, Program.GetIronSmalltalkClassLibraryCode());

//            SmalltalkEnvironment env = new SmalltalkEnvironment();
//            env.CompilerService.Install(sources.Select(src => new DelegateFileInInformation(() => src, ConsoleErrorSink.Current)));

//            object na;
//            foreach (string code in evaluates)
//            {
//                if (!env.CompilerService.Evaluate(code, ConsoleErrorSink.Current, out na))
//                {
//                    System.Console.WriteLine("Source Error: \"{0}\"", code);
//                    return;
//                }
//            }

//            System.Console.WriteLine();
//            System.Console.WriteLine("Success.");

//        }

//        private static bool IsHelpRequest(string arg)
//        {
//            if (String.IsNullOrWhiteSpace(arg))
//                return false;

//            if ((arg == "/?") || (arg == "-?") || (arg == "?"))
//                return true;

//            return false;
//        }

//        private static void PrintHelp()
//        {
//            string exeName = System.IO.Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location);
//            System.Console.WriteLine("Usage: {0} [options...]", exeName);
//            System.Console.WriteLine();
//            System.Console.WriteLine("Options:");
//            System.Console.WriteLine(" -ist:{yes|no}   Include the standard IronSmalltalk class library");
//            System.Console.WriteLine(" -f:<file>       File-in Smalltalk interchange format file");
//            System.Console.WriteLine(" -r:<file>       Evaluate Smalltalk workspace style file");
//            System.Console.WriteLine(" -n:<code>       File-in interchange format Smalltalk code");
//            System.Console.WriteLine(" -e:<code>       Evaluate Smalltalk workspace style code");
//            System.Console.WriteLine();
//            System.Console.WriteLine("Examples:");
//            System.Console.WriteLine("    {0} -e:\"Transcript show: (1976 + 12 + 14) asString; cr\"", exeName);
//            System.Console.WriteLine();
//            System.Console.WriteLine("    {0} -r:\"ExampleCode.st\"", exeName);
//            System.Console.WriteLine();
//            System.Console.WriteLine("    {0} -f:\"InterchangeCode.st\"", exeName);
//            System.Console.WriteLine();
//            System.Console.WriteLine("    {0} -n:\"Smalltalk interchangeVersion: '1.0'!", exeName);
//            System.Console.WriteLine("             Global variable: 'Today'!");
//            System.Console.WriteLine("             Today initializer!");
//            System.Console.WriteLine("                 DateTime now!");
//            System.Console.WriteLine("             Global initializer!");
//            System.Console.WriteLine("                 Transcript show: Today asString!\"");
//            System.Console.WriteLine();
//            System.Console.WriteLine("    {0} -ist:no -f:\"AlternativeClassLibrary.st\" -e:\"MyTranscript show: 'Hello'; cr\"", exeName);
//        }

//        private static TextReader GetIronSmalltalkClassLibraryCode()
//        {
//            Stream ist = typeof(Program).Assembly.GetManifestResourceStream(typeof(Program), "IronSmalltalk.ist");
//            return new StreamReader(ist, System.Text.Encoding.UTF8);
//        }

//        public class ConsoleErrorSink : IronSmalltalk.Internals.ErrorSinkBase
//        {
//            protected override void ReportError(string message, SourceLocation start, SourceLocation end, IronSmalltalk.Internals.ErrorSinkBase.ErrorType type, params object[] offenders)
//            {
//                System.Console.WriteLine("Error [{0} - {1}]: {2}", start, end, message);
//            }

//            public static readonly ConsoleErrorSink Current = new ConsoleErrorSink();
//        }
//    }
//}
*/