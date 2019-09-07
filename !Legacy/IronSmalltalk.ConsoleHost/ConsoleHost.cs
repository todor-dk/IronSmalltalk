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
	/// <summary>
	/// Implements an interactive console for IronSmalltalk.
	/// </summary>
	public class IronSmalltalkConsoleHost : ConsoleHost
	{
		/// <summary>
		/// The main entry point of the application.
		/// </summary>
		/// <param name="args">Command-line arguments.</param>
		/// <returns>Status information communicated to other programs or scripts that invoke the executable file.</returns>
		public static int Main(string[] args)
		{
			/*
				*** Here's what happens when the app runs ***
			 
				1. Create the Runtime Setup	
						See: CreateRuntimeSetup (Locally overridden)

				2. Parse Host Options
						See: ParseHostOptions
			 
				3. Set environment variables dictated by command line args (by System.Environment.SetEnvironmentVariable)
						See: SetEnvironment
			 
				4. Set (merge) search paths (/paths option) (see: options["SearchPaths"]) 	
						See: InsertSearchPaths
			 
				5. Parse options
						See: ParseOptions
					a. Create options parser (SmalltalkOptionsParser)
						See: CreateOptionsParser
					b. Parse the command line options
						See: SmalltalkOptionsParser.Parse
						
				6. Create new ScriptRuntime
						See: new ScriptRuntime
					a. Creates the ScriptHost
					b. Creates the ScriptDomainManager (globals, assembly list etc.)
					c. Creates other stuff
					d. Created the Globals ScriptScope
					e. Adds standard assemblies to the list of loaded assemblies

				7. Get the ScriptEngine for the default language (IronSmalltalk)	
						See: ScriptRuntime.GetEngineByTypeName
					a. Construct and initialize SmalltalkLanguageContext
					b. Cache it.

				8. EXECUTE !!! This is where the real work starts
						See: Execute and ExecuteInternal
					a. If ConsoleOptions.IsMta (MTA, i.e. not an STA), then run the rest in a separate thread.
					b. If ConsoleOptions.PrintVersion, then PrintVersion		We must set this in SmalltalkOptionsParser
					c. If ConsoleOptions.PrintUsage, then PrintUsage		    -h -help -? /?
					d. If ConsoleOptions.Exit, exit with success code		    Set by PrintVersion & PrintUsage
					e. Depending on ConsoleHostOptions:                         NB: This is DISABLED by the IST host, so always run RunCommandLine
						- None 		    => RunCommandLine			command line arg: <default>
						- RunConsole	=> RunCommandLine			command line arg: console 
						- RunFile	    => RunFile				    command line arg: run arg.

				======== RunFile ======== 
					1. Create a ScriptSource from the given file path and using the default text encoding
						See: ScriptEngine.CreateScriptSourceFromFile
			 
					2. Execute the 
				======== RunCommandLine ======== 
			*/
			return new IronSmalltalkConsoleHost().Run(args);
		}

		/// <summary>
		/// This is the language context for the default language, in our case for IronSmalltalk.
		/// </summary>
		protected override Type Provider
		{
			get { return typeof(SmalltalkLanguageContext); }
		}

		/// <summary>
		/// This creates the setup parameters for the ScriptRuntime.
		/// </summary>
		/// <remarks>
		/// We've overridden this to set the HostType, so we get our own 
		/// SmalltalkScriptHost to be the owner of the ScriptRuntime.
		/// </remarks>
		protected override ScriptRuntimeSetup CreateRuntimeSetup()
		{
			ScriptRuntimeSetup setup = base.CreateRuntimeSetup();
			setup.HostType = typeof(SmalltalkScriptHost);
			// NB: We can also set parameters to be given to the ScriptHost's constructor
			// setup.HostArguments = new object[] { 1, 2, 3 };
			return setup;
		}

		/// <summary>
		/// Creates the language setup for the default language, i.e. for IronSmalltalk.
		/// </summary>
		protected override LanguageSetup CreateLanguageSetup()
		{
			return new LanguageSetup(
				SmalltalkLanguageSetup.LanguageContextType.AssemblyQualifiedName,
				SmalltalkLanguageSetup.DisplayName,
				SmalltalkLanguageSetup.SecondaryNames,
				SmalltalkLanguageSetup.FileExtensions);
		}

		/// <summary>
		/// Create the options parser that will parse the command line parameters.
		/// </summary>
		/// <returns></returns>
		protected override OptionsParser CreateOptionsParser()
		{
			return new SmalltalkOptionsParser();
		}

		/// <summary>
		/// Create the command line object that will perform the actial execution.
		/// </summary>
		/// <returns></returns>
		protected override CommandLine CreateCommandLine()
		{
			return new SmalltalkCommandLine();
		}

		/// <summary>
		/// Parse global options for the DLR host ... overridden to ignore those in the IST console host.
		/// </summary>
		/// <param name="args"></param>
		protected override void ParseHostOptions(string[] args)
		{
			// IronSmalltalk doesn't want any of the DLR base options.
			foreach (string arg in args)
				this.Options.IgnoredArgs.Add(arg);
		}

	}
}

