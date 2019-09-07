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
using System.Linq;
using IronSmalltalk.Hosting.Hosting;
using IronSmalltalk.Runtime.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using Microsoft.Scripting.Runtime;

namespace IronSmalltalk.Hosting.Host
{
    /// <summary>
    /// IronSmalltalk command line (options) parser.
    /// </summary>
    public class SmalltalkOptionsParser : OptionsParser<SmalltalkConsoleOptions>
    {
        protected override void BeforeParse()
        {
            this.LanguageSetup.Options["IncludeStandardClassLibrary"] = ScriptingRuntimeHelpers.True;
            base.BeforeParse();
        }

        /// <summary>
        /// Parse a command line argument that was given to the host.
        /// </summary>
        /// <param name="arg"></param>
        protected override void ParseArgument(string arg)
        {
            string file;
            string code;

            // Arguments that are used by the IronSmalltalk command line host
            switch (arg)
            {
                case "-ist:yes":
                case "/ist:yes":
                    this.LanguageSetup.Options["IncludeStandardClassLibrary"] = ScriptingRuntimeHelpers.True;
                    break;

                case "-ist:no":
                case "/ist:no":
                    this.LanguageSetup.Options["IncludeStandardClassLibrary"] = ScriptingRuntimeHelpers.False;
                    break;

                case "-f":
                case "/f":
                    file = this.PopNextArg();
                    this.AddLanguageOptionValue<FileInCommand>("FileIns", new FileInFile(file));
                    break;

                case "-r":
                case "/r":
                    file = this.PopNextArg();
                    this.ConsoleOptions.AddCommandLineCommand(new EvaluateFile(file));
                    break;

                case "-n":
                case "/n":
                    code = this.PopNextArg();
                    this.AddLanguageOptionValue<FileInCommand>("FileIns", new FileInCode(code));
                    break;

                case "-e":
                case "/e":
                    code = this.PopNextArg();
                    this.ConsoleOptions.AddCommandLineCommand(new EvaluateCode(code));
                    break;

                // This should be implemented by the superclass, but for some reason it's missing.
                case "-V":
                    this.ConsoleOptions.PrintVersion = true;
                    this.ConsoleOptions.Exit = true;
                    this.IgnoreRemainingArgs();
                    break;

                // This should be implemented by the superclass, but for some reason it's missing.
                case "-i":
                    this.ConsoleOptions.Introspection = true;
                    this.LanguageSetup.Options["Inspect"] = ScriptingRuntimeHelpers.True;
                    break;

                default:
                    // Probably a standard argument
                    base.ParseArgument(arg);
                    break;
            }
        }

        /// <summary>
        /// Generates help (usage) information.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="options"></param>
        /// <param name="environmentVariables"></param>
        /// <param name="comments"></param>
        public override void GetHelp(out string commandLine, out string[,] options, out string[,] environmentVariables, out string comments)
        {
            string[,] standardOptions;
            base.GetHelp(out commandLine, out standardOptions, out environmentVariables, out comments);

            string exeName = System.IO.Path.GetFileNameWithoutExtension(typeof(IronSmalltalkConsoleHost).Assembly.Location);

            commandLine = String.Format("Usage: {0} [options...] [file]", exeName);

            /// The IronSmalltalk options
            var allOptions = new[] {
                new { Option = "-ist:{yes|no}",   Description = "Include the standard IronSmalltalk class library" },
                new { Option = "-f <file>",       Description = "File-in Smalltalk interchange format file" },
                new { Option = "-r <file>",       Description = "Evaluate Smalltalk workspace style file" },
                new { Option = "-n <code>",       Description = "File-in interchange format Smalltalk code" },
                new { Option = "-e <code>",       Description = "Evaluate Smalltalk workspace style code" }
            }.ToList();

            // Add the standard DLR options 
            for (int i = 0; i < standardOptions.GetLength(0); i++)
                allOptions.Add(new { Option = standardOptions[i, 0], Description = standardOptions[i, 1] });
            
            // Remove the not-wanted standard DLR options
            string[] ignore = new string[] { "-c cmd" };
            allOptions = allOptions.Where(opt => !ignore.Contains(opt.Option)).ToList();

            // Sort the options
            allOptions = allOptions.OrderBy(opt => opt.Option, StringComparer.InvariantCultureIgnoreCase).ToList();

            // Build the stupid DLR multi-dimensional array format
            options = new string[allOptions.Count, 2];
            for (int i = 0; i < allOptions.Count; i++)
            {
                options[i, 0] = allOptions[i].Option;
                options[i, 1] = allOptions[i].Description;
            }            
        }

        private void AddLanguageOptionValue<TValue>(string optionName, TValue optionValue)
        {
            if (!this.LanguageSetup.Options.ContainsKey(optionName))
                this.LanguageSetup.Options[optionName] = new TValue[0];

            TValue[] values = (TValue[])this.LanguageSetup.Options[optionName];
            this.LanguageSetup.Options[optionName] = values.Concat(new TValue[] { optionValue }).ToArray();
        }
    }
}
