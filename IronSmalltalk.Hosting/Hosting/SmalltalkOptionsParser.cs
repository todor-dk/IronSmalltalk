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
using IronSmalltalk.Console;
using Microsoft.Scripting.Hosting.Shell;

namespace IronSmalltalk.Runtime.Hosting
{
    public class SmalltalkOptionsParser : OptionsParser<SmalltalkConsoleOptions>
    {
        protected override void ParseArgument(string arg)
        {
            switch (arg)
            {
                case "-f":
                case "-F":
                case "/f":
                case "/F":

                case "-r":
                case "-R":
                case "/r":
                case "/R":

                case "-n":
                case "-N":
                case "/n":
                case "/N":

                case "-e":
                case "-E":
                case "/e":
                case "/E": 
                    string code = this.PopNextArg();
                    break;
                default:
                    break;
            }
            base.ParseArgument(arg);
        }

        public override void GetHelp(out string commandLine, out string[,] options, out string[,] environmentVariables, out string comments)
        {
            string[,] standardOptions;
            base.GetHelp(out commandLine, out standardOptions, out environmentVariables, out comments);

            string exeName = System.IO.Path.GetFileNameWithoutExtension(typeof(IronSmalltalkConsoleHost).Assembly.Location);

            commandLine = String.Format("Usage: {0} [options...]", exeName);

            string[,] istOptions = new string[,] 
            {
                { " -ist:{yes|no}",   "Include the standard IronSmalltalk class library" },
                { " -f:<file>",       "File-in Smalltalk interchange format file" },
                { " -r:<file>",       "Evaluate Smalltalk workspace style file" },
                { " -n:<code>",       "File-in interchange format Smalltalk code" }
            };

            options = Microsoft.Scripting.Utils.ArrayUtils.Concatenate(standardOptions, istOptions);
        }
    }
}
