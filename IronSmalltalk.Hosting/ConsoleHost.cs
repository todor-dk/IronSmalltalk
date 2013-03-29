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
using IronSmalltalk.Runtime.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace IronSmalltalk.Console
{
    public class IronSmalltalkConsoleHost : ConsoleHost
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>Status information communicated to other programs or scripts that invoke the executable file.</returns>
        public static int Main(string[] args)
        {
            return new IronSmalltalkConsoleHost().Run(args);
        }

        protected override Type Provider
        {
            get { return typeof(SmalltalkLanguageContext); }
        }

        protected override CommandLine CreateCommandLine()
        {
            return new SmalltalkCommandLine();
        }

        protected override OptionsParser CreateOptionsParser()
        {
            return new SmalltalkOptionsParser();
        }

        protected override LanguageSetup CreateLanguageSetup()
        {
            return new LanguageSetup(
                typeof(SmalltalkLanguageContext).AssemblyQualifiedName,
                "IronSmalltalk",
                new string[] { "IronSmalltalk", "Iron Smalltalk", "ist" },
                new string[] { "ist" });
        }
    }
}

