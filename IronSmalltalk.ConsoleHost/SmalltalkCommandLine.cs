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
using IronSmalltalk.Hosting.Hosting;
using IronSmalltalk.Runtime.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace IronSmalltalk.Hosting.Host
{
    public class SmalltalkCommandLine : CommandLine
    {
        /// <summary>
        /// Returns the language context for the IronSmalltalk language.
        /// </summary>
        public new SmalltalkLanguageContext Language
        {
            get { return (SmalltalkLanguageContext) base.Language; }
        }

        /// <summary>
        /// Returns the language context for the IronSmalltalk language.
        /// </summary>
        public SmalltalkLanguageContext SmalltalkLanguageContext
        {
            get { return this.Language; }
        }

        /// <summary>
        /// Return the command line options.
        /// </summary>
        public new SmalltalkConsoleOptions Options
        {
            get { return (SmalltalkConsoleOptions)base.Options; }
        }

        /// <summary>
        /// Returns the script engine for the IronSmalltalk language.
        /// </summary>
        /// <remarks>
        /// Overridden to make this property public.
        /// </remarks>
        public new ScriptEngine Engine { get { return base.Engine; } }

        /// <summary>
        /// The information displayed when entering the interactive console.
        /// </summary>
        protected override string Logo
        {
            get
            {
                return String.Format("IronSmalltalk {0}. Copyright The IronSmalltalk Project. All rights reserved.{1}{1}",
                    this.SmalltalkLanguageContext.LanguageVersion, Environment.NewLine);
            }
        }

        /// <summary>
        /// Runs the command line. This is the main method for executing the work.
        /// </summary>
        /// <returns></returns>
        protected override int Run()
        {
            // If command or file was given using the DLR standard syntax, convert it to IST style.
            if (this.Options.Command != null)
            {
                this.Options.AddCommandLineCommand(new EvaluateCode(this.Options.Command));
                this.Options.Command = null;
            }
            //if (this.Options.FileName != null)
            //{
            //    this.Options.AddCommandLineCommand(new FileInCode(this.Options.FileName));
            //    this.Options.FileName = null;
            //}

            // If no file or code to file-in or evaluate, run in interactive mode.
            if (this.Options.CommandLineCommands.Count == 0)
                return this.RunInteractive();


            int result = 0;
            foreach (SmalltalkCommandLineCommand command in this.Options.CommandLineCommands)
                result = command.Execute(this);

            if (this.Options.Introspection)
                return this.RunInteractiveLoop();

            return result;
        }

        protected override int RunCommand(string command)
        {
            throw new InvalidOperationException();
        }

        protected override int RunFile(string fileName)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Runs the specified filename
        /// </summary>
        public new int RunFile(ScriptSource source)
        {
            return base.RunFile(source);
        }


        protected override void ExecuteCommand(string command)
        {
            base.ExecuteCommand(command);
        }


        protected override Microsoft.Scripting.ScriptCodeParseResult GetCommandProperties(string code)
        {
            return base.GetCommandProperties(code);
        }


        protected override int? TryInteractiveAction()
        {
            return base.TryInteractiveAction();
        }

        protected override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
