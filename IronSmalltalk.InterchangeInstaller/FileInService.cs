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
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Compiler.Visiting;
using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.InterchangeInstaller
{
    /// <summary>
    /// Service for compiling and installing smalltalk code into the smalltalk environment / context.
    /// </summary>
    public sealed class FileInService
    {
        public Dictionary<string, InterchangeVersionService> VersionServicesMap { get; private set; }

        public SmalltalkRuntime Runtime { get; private set; }

        public Func<FileInService, InterchangeInstallerContext> CreateInstallerContextFunction { get; private set; }

        /// <summary>
        /// Determines if meta-annotations (comments, documentation, etc.) 
        /// are installed (saved) in the corresponding runtime objects.
        /// </summary>
        public bool InstallMetaAnnotations { get; set; }

        public FileInService(SmalltalkRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            // TODO : Move constants out of code into a the InterchangeFormatConstants class

            this.Runtime = runtime;
            this.VersionServicesMap = new Dictionary<string, InterchangeVersionService>();
            this.VersionServicesMap.Add("1.0", new InterchangeVersionService10());
            this.VersionServicesMap.Add("IronSmalltalk 1.0", new InterchangeVersionServiceIST10());
#if DEBUG
            this.InstallMetaAnnotations = true;
#else
            this.InstallMetaAnnotations = false;
#endif
        }

        public FileInService(SmalltalkRuntime runtime, bool installMetaAnnotations, Func<FileInService, InterchangeInstallerContext> createInstallerContextFunction)
            : this(runtime)
        {
            this.InstallMetaAnnotations = installMetaAnnotations;
            this.CreateInstallerContextFunction = createInstallerContextFunction;
        }

        public void Install(FileInInformation fileIn)
        {
            if (fileIn == null)
                throw new ArgumentNullException();
            this.Install(new FileInInformation[] { fileIn });
        }

        public void Install(IEnumerable<FileInInformation> fileIns)
        {
            if (fileIns == null)
                throw new ArgumentNullException("fileIns");

            InterchangeInstallerContext context = this.Read(fileIns);

            this.CompleteInstall(context);
        }

        public InterchangeInstallerContext Read(FileInInformation fileIn)
        {
            if (fileIn == null)
                throw new ArgumentNullException();
            return this.Read(new FileInInformation[] { fileIn });
        }

        public InterchangeInstallerContext Read(IEnumerable<FileInInformation> fileIns)
        {
            if (fileIns == null)
                throw new ArgumentNullException("fileIns");

            InterchangeInstallerContext installer = this.CreateInstallerContext();

            foreach (FileInInformation info in fileIns)
            {
                using (TextReader souceCodeReader = info.GetTextReader())
                {
                    InterchangeFormatProcessor processor = new InterchangeFormatProcessor(info, souceCodeReader, installer, this.VersionServicesMap);
                    processor.ProcessInterchangeFile();
                }
            }

            return installer;
        }

        //public object Evaluate(string initializerCode)
        //{
        //    object result;
        //    if (this.Evaluate(initializerCode, null, out result))
        //        return result;
        //    return null;
        //}

        //public object Evaluate(TextReader initializerCode)
        //{
        //    object result;
        //    if (this.Evaluate(initializerCode, null, out result))
        //        return result;
        //    return null;
        //}

        //public bool Evaluate(string initializerCode, IParseErrorSink errorSink, out object result)
        //{
        //    if (initializerCode == null)
        //        throw new ArgumentNullException("initializerCode");

        //    return this.Evaluate(new StringReader(initializerCode), errorSink, out result);
        //}

        //public bool Evaluate(TextReader initializerCode, IParseErrorSink errorSink, out object result)
        //{
        //    if (initializerCode == null)
        //        throw new ArgumentNullException("initializerCode");

        //    result = null;
        //    Parser parser = new Parser();
        //    parser.ErrorSink = errorSink;
        //    InitializerNode node = parser.ParseInitializer(initializerCode);
        //    if ((node == null) || !node.Accept(ParseTreeValidatingVisitor.Current))
        //        return false;

        //    Expression<Func<SmalltalkRuntime, object, object>> lambda;
        //    RuntimeProgramInitializer code = new RuntimeProgramInitializer(node, null);
        //    var compilationResult = code.Compile(this.Runtime);
        //    if (compilationResult == null)
        //        return false;
        //    lambda = compilationResult.ExecutableCode;
        //    if (lambda == null)
        //        return false;

        //    var function = lambda.Compile();
        //    result = function(this.Runtime, null);
        //    return true;
        //}

        private void CompleteInstall(InterchangeInstallerContext installer)
        {
            installer.ErrorSink = new InstallErrorSink();
            installer.InstallMetaAnnotations = this.InstallMetaAnnotations;
            if (installer.Install())
            {
                ExecutionContext executionContext = new ExecutionContext(this.Runtime);
                installer.NameScope.ExecuteInitializers(executionContext);
            }
        }

        private InterchangeInstallerContext CreateInstallerContext()
        {
            if (this.CreateInstallerContextFunction != null)
                return this.CreateInstallerContextFunction(this);
            else
                return new InterchangeInstallerContext(this.Runtime);
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
                if (sourceObject.ErrorSink == null)
                    return;
                sourceObject.ErrorSink.AddInstallError(sourceReference.StartPosition, sourceReference.StopPosition, installErrorMessage);
            }
        }
    }
}
