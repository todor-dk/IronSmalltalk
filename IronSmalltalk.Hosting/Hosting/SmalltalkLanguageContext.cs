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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Compiler.Visiting;
using IronSmalltalk.Hosting;
using IronSmalltalk.Hosting.Hosting;
using IronSmalltalk.Interchange;
using IronSmalltalk.InterchangeInstaller.Runtime;
using IronSmalltalk.Internals;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;

namespace IronSmalltalk.Hosting.Hosting
{
    /// <summary>
    /// Iron Smalltalk Language Context. This is the entry point from the DLR into IST.
    /// </summary>
    /// <remarks>
    /// The IronSmalltalk LanguageContext is the representation of the language and the
    /// workhorse at the language implementation level for supporting the DLR
    /// Hosting APIs. It has many members on it, but we only have to override
    /// a couple to get basic DLR hosting support enabled.
    ///
    /// Other things a LanguageContext might do are provide an implementation for
    /// ObjectOperations, offer other services (exception formatting, colorization,
    /// tokenization, etc), provide ExecuteProgram semantics, and so on.
    /// </remarks>
    public class SmalltalkLanguageContext : LanguageContext
    {
        public readonly SmalltalkEnvironment SmalltalkEnvironment;

        /// <summary>
        /// Include the standard IronSmalltalk class library.
        /// </summary>
        private readonly bool IncludeStandardClassLibrary;

        /// <summary>
        /// Collection of files and code to file-in.
        /// </summary>
        private readonly FileInCommand[] FileInCommands;

        private volatile bool IsInitialized = false;

        private object LockObject = new object();

        private SmalltalkLanguageContext(ScriptDomainManager domainManager)
            : base(domainManager)
        {
            this.SmalltalkEnvironment = new SmalltalkEnvironment();
        }

        public SmalltalkLanguageContext(ScriptDomainManager domainManager, IDictionary<string, object> options)
            : this(domainManager)
        {
            if (options.ContainsKey("IncludeStandardClassLibrary"))
                this.IncludeStandardClassLibrary = (bool)options["IncludeStandardClassLibrary"];
            else
                this.IncludeStandardClassLibrary = true;

            if (options.ContainsKey("FileIns"))
                this.FileInCommands = (FileInCommand[])options["FileIns"];
            if (this.FileInCommands == null)
                this.FileInCommands = new FileInCommand[0];
        }

        /// <summary>
        /// The Guid that uniqualy identifies the IronSmalltalk language
        /// </summary>
        public override Guid LanguageGuid
        {
            get { return SmalltalkLanguageSetup.LanguageGuid; }
        }

        /// <summary>
        /// The version of this IronSmalltalk implementation
        /// </summary>
        /// <remarks>
        /// Currently, we use the assembly version.
        /// </remarks>
        public override Version LanguageVersion
        {
            get { return SmalltalkLanguageSetup.Version; }
        }

        /// <summary>
        /// The Guid that uniqualy identifies the vendor of the IronSmalltalk language, 
        /// i.e. that is the IronSmalltalk Project (because we don't do other languages).
        /// </summary>
        public override Guid VendorGuid
        {
            get { return SmalltalkLanguageSetup.VendorGuid; }
        }

        /// <summary>
        /// Returns the file encoding for IST files. IST files use UTF-8 encoding.
        /// </summary>
        public System.Text.Encoding DefaultEncoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        private void EnsureInitialized()
        {
            if (this.IsInitialized)
                return;
            lock (this.LockObject)
            {
                if (this.IsInitialized)
                    return;

                this.InitializeEnvironment();
                this.IsInitialized = true;
            }
        }

        private void InitializeEnvironment()
        {
            //// Install the Standard Class Library
            //if (this.IncludeStandardClassLibrary)
            //{
            //    ResourceStreamContentProvider contentProvider = new ResourceStreamContentProvider(this.GetType(), "IronSmalltalk.ist");
            //    SourceUnit sourceUnit = this.CreateSourceUnit(contentProvider, null, this.DefaultEncoding, SourceCodeKind.Statements);
            //    ErrorSinkWrapper errorSink = new ErrorSinkWrapper(sourceUnit, ErrorSink.Default);
            //    InternalCompilerService service = new InternalCompilerService(this.SmalltalkEnvironment.Runtime);
            //    service.Install(new DelegateFileInInformation(() => sourceUnit.GetReader(), errorSink));
            //}

            //// Install the rest that is to be filed-in
            //this.SmalltalkEnvironment.CompilerService.Install(this.FileInCommands.Select(command =>
            //{
            //    SourceUnit sourceUnit = command.GetSourceUnit(this);
            //    ErrorSinkWrapper errorSink = new ErrorSinkWrapper(sourceUnit, ErrorSink.Default);
            //    return new DelegateFileInInformation(() => sourceUnit.GetReader(), errorSink);
            //}));

            // Install the Standard Class Library
            IEnumerable<FileInInformation> ist = new FileInInformation[0];
            if (this.IncludeStandardClassLibrary)
            {
                ResourceStreamContentProvider contentProvider = new ResourceStreamContentProvider(this.GetType(), "IronSmalltalk.ist");
                SourceUnit sourceUnit = this.CreateSourceUnit(contentProvider, null, this.DefaultEncoding, SourceCodeKind.Statements);
                ErrorSinkWrapper errorSink = new ErrorSinkWrapper(sourceUnit, ErrorSink.Default);
                FileInInformation fileIn = new DelegateFileInInformation(() => sourceUnit.GetReader(), errorSink, sourceUnit.Document);
                // BUG BUG - Can't get the InternalCompilerService to work yet.
                //InternalCompilerService service = new InternalCompilerService(this.SmalltalkEnvironment.Runtime);
                //service.Install(fileIn);

                // File in normally
                ist = new FileInInformation[] { fileIn };
            }

            var fileIns = this.FileInCommands.Select(command =>
            {
                SourceUnit sourceUnit = command.GetSourceUnit(this);
                ErrorSinkWrapper errorSink = new ErrorSinkWrapper(sourceUnit, ErrorSink.Default);
                return new DelegateFileInInformation(() => sourceUnit.GetReader(), errorSink, sourceUnit.Document);
            });

            // Install the rest that is to be filed-in
            this.SmalltalkEnvironment.CompilerService.Install(ist.Concat(fileIns));
        }

        /// <summary>
        /// Parses the source code within a specified compiler context. 
        /// The source unit to parse is held on by the context.
        /// </summary>
        /// <returns><b>null</b> on failure.</returns>
        /// <remarks>Could also set the code properties and line/file mappings on the source unit.</remarks>
        public override ScriptCode CompileSourceCode(SourceUnit sourceUnit, CompilerOptions options, ErrorSink errorSink)
        {
            if (sourceUnit == null)
                throw new ArgumentNullException("sourceUnit");
            if (options == null)
                throw new ArgumentNullException("options");
            if (errorSink == null)
                throw new ArgumentNullException("errorSink");
            if (sourceUnit.LanguageContext != this)
                throw new ArgumentException("Language context mismatch");

            // Ensure that sources are installed into the image.
            this.EnsureInitialized();


            // THE CODE BELOW NEEDS CLEAN-UP !!!
            // 1. Parse the code to an AST
            Parser parser = new Parser();
            parser.ErrorSink = new ErrorSinkWrapper(sourceUnit, errorSink); 
            InitializerNode node = parser.ParseInitializer(sourceUnit.GetReader());
            if ((node == null) || !node.Accept(ParseTreeValidatingVisitor.Current))
                return null; // Failed to compile the code, return null

            // 2. Compile the AST to RuntimeProgramInitializer
            RuntimeProgramInitializer code = new RuntimeProgramInitializer(node, null);
            if (code.Validate(this.SmalltalkEnvironment.Runtime.GlobalScope, new ErrorSinkWrapper(sourceUnit, errorSink)))
                return null; // Failed to compile the code, return null

            // 3. Create a script-source that wraps the Lambda Expression and compiles it to a delegate
            return new SmalltalkScriptCode(code, this.SmalltalkEnvironment.Runtime, sourceUnit);
        }

        #region Stuff that we can potentially override

        public override System.Dynamic.BinaryOperationBinder CreateBinaryOperationBinder(System.Linq.Expressions.ExpressionType operation)
        {
            return base.CreateBinaryOperationBinder(operation);
        }

        public override System.Dynamic.InvokeMemberBinder CreateCallBinder(string name, bool ignoreCase, System.Dynamic.CallInfo callInfo)
        {
            return base.CreateCallBinder(name, ignoreCase, callInfo);
        }

        public override System.Dynamic.ConvertBinder CreateConvertBinder(Type toType, bool? explicitCast)
        {
            return base.CreateConvertBinder(toType, explicitCast);
        }

        public override System.Dynamic.CreateInstanceBinder CreateCreateBinder(System.Dynamic.CallInfo callInfo)
        {
            return base.CreateCreateBinder(callInfo);
        }

        public override System.Dynamic.DeleteMemberBinder CreateDeleteMemberBinder(string name, bool ignoreCase)
        {
            return base.CreateDeleteMemberBinder(name, ignoreCase);
        }

        public override System.Dynamic.GetMemberBinder CreateGetMemberBinder(string name, bool ignoreCase)
        {
            return base.CreateGetMemberBinder(name, ignoreCase);
        }

        public override System.Dynamic.InvokeBinder CreateInvokeBinder(System.Dynamic.CallInfo callInfo)
        {
            return base.CreateInvokeBinder(callInfo);
        }

        public override ScopeExtension CreateScopeExtension(Scope scope)
        {
            return base.CreateScopeExtension(scope);
        }

        public override System.Dynamic.SetMemberBinder CreateSetMemberBinder(string name, bool ignoreCase)
        {
            return base.CreateSetMemberBinder(name, ignoreCase);
        }

        public override System.Dynamic.UnaryOperationBinder CreateUnaryOperationBinder(System.Linq.Expressions.ExpressionType operation)
        {
            return base.CreateUnaryOperationBinder(operation);
        }

        public override int ExecuteProgram(SourceUnit program)
        {
            if (program == null)
                throw new ArgumentNullException();

            object returnValue = program.Execute();
            if (returnValue == null)
                return 0;

            // Very naive way of converting to int. Improve!
            if (returnValue is int)
                return (int) returnValue;
            else
                return 0;
        }

        public override string FormatException(System.Exception exception)
        {
            return base.FormatException(exception);
        }

        public override string FormatObject(DynamicOperations operations, object obj)
        {
            return base.FormatObject(operations, obj);
        }

        public override SourceUnit GenerateSourceCode(System.CodeDom.CodeObject codeDom, string path, SourceCodeKind kind)
        {
            return base.GenerateSourceCode(codeDom, path, kind);
        }

        public override IList<string> GetCallSignatures(object obj)
        {
            return base.GetCallSignatures(obj);
        }

        public override ErrorSink GetCompilerErrorSink()
        {
            return base.GetCompilerErrorSink();
        }

        public override CompilerOptions GetCompilerOptions()
        {
            return base.GetCompilerOptions();
        }

        public override CompilerOptions GetCompilerOptions(Scope scope)
        {
            return base.GetCompilerOptions(scope);
        }

        public override string GetDocumentation(object obj)
        {
            return base.GetDocumentation(obj);
        }

        public override void GetExceptionMessage(Exception exception, out string message, out string errorTypeName)
        {
            base.GetExceptionMessage(exception, out message, out errorTypeName);
        }

        public override IList<string> GetMemberNames(object obj)
        {
            return base.GetMemberNames(obj);
        }

        public override Scope GetScope(string path)
        {
            return base.GetScope(path);
        }

        public override ICollection<string> GetSearchPaths()
        {
            return base.GetSearchPaths();
        }

        public override TService GetService<TService>(params object[] args)
        {
            //if (typeof(TService) == typeof(SmalltalkEnvironment))
            //    return (TService)this.SmalltalkEnvironment;
            return base.GetService<TService>(args);
        }

        public override SourceCodeReader GetSourceReader(Stream stream, System.Text.Encoding defaultEncoding, string path)
        {
            return base.GetSourceReader(stream, defaultEncoding, path);
        }

        public override IList<DynamicStackFrame> GetStackFrames(Exception exception)
        {
            return base.GetStackFrames(exception);
        }

        public override bool IsCallable(object obj)
        {
            return base.IsCallable(obj);
        }

        public override ScriptCode LoadCompiledCode(Delegate method, string path, string customData)
        {
            return base.LoadCompiledCode(method, path, customData);
        }

        public override T ScopeGetVariable<T>(Scope scope, string name)
        {
            return base.ScopeGetVariable<T>(scope, name);
        }

        public override dynamic ScopeGetVariable(Scope scope, string name)
        {
            return base.ScopeGetVariable(scope, name);
        }

        public override void ScopeSetVariable(Scope scope, string name, object value)
        {
            base.ScopeSetVariable(scope, name, value);
        }

        public override bool ScopeTryGetVariable(Scope scope, string name, out dynamic value)
        {
            return base.ScopeTryGetVariable(scope, name, out value);
        }

        public override void SetSearchPaths(ICollection<string> paths)
        {
            base.SetSearchPaths(paths);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        #endregion
    }
}
