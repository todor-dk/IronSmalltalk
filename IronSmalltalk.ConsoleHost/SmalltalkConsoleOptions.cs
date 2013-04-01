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
using Microsoft.Scripting.Hosting.Shell;

namespace IronSmalltalk.Hosting.Host
{
    public class SmalltalkConsoleOptions : ConsoleOptions
    {
        ///// <summary>
        ///// Include the standard IronSmalltalk class library.
        ///// </summary>
        //public bool IncludeStandardClassLibrary { get; set; }

        private List<SmalltalkCommandLineCommand> _CommandLineCommands = new List<SmalltalkCommandLineCommand>();
        /// <summary>
        /// List of commands to be executed. Those can be file-ins or evaluates.
        /// </summary>
        public IReadOnlyList<SmalltalkCommandLineCommand> CommandLineCommands { get { return this._CommandLineCommands; } }

        public SmalltalkConsoleOptions()
        {
            //this.IncludeStandardClassLibrary = true;
        }

        /// <summary>
        /// Add a command line command to be executed.
        /// </summary>
        /// <param name="command"></param>
        public void AddCommandLineCommand(SmalltalkCommandLineCommand command)
        {
            if (command == null)
                throw new ArgumentNullException();
            this._CommandLineCommands.Add(command);
        }
    }
}
