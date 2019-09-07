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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronSmalltalk;
using IronSmalltalk.Common;
using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.DefinitionInstaller.Definitions;
using IronSmalltalk.InterchangeInstaller;
using IronSmalltalk.Internals;
using IronSmalltalk.Runtime;
using System.Linq.Expressions;
using System.Dynamic;
using System.Runtime.CompilerServices;
using IronSmalltalk.Common.Internal;

namespace TestPlayground
{
    public partial class NativeCompileTester : Form
    {
        public NativeCompileTester()
        {
            InitializeComponent();
        }

        private Type XXX()
        {
            Type type = typeof(BulkParseTester);
            return type;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            ErrorSink errorSink = new ErrorSink(this);

            var paths = this.textSourceFiles.Text.Split('\n').Concat(this.openFileDialog.FileNames);
            paths = paths.Select(s => s.Trim()).Where(s => s.Length > 0);
            this.textSourceFiles.Text = String.Join("\r\n", paths);
            Properties.Settings.Default.LastNativePaths = this.textSourceFiles.Text;
            Properties.Settings.Default.Save();
           
            var fileIns = paths.Select(p => new PathFileInInformation(p, System.Text.Encoding.UTF8, errorSink,
                Expression.SymbolDocument(p, GlobalConstants.LanguageGuid, GlobalConstants.VendorGuid)));
            this.listErrors.Items.Clear();
            if (!fileIns.Any())
                return;

            //Dictionary<string, InterchangeVersionService> versionServicesMap = new Dictionary<string, InterchangeVersionService>();
            //versionServicesMap.Add("1.0", new InterchangeVersionService10());
            //versionServicesMap.Add("IronSmalltalk 1.0", new InterchangeVersionServiceIST10());

            //IInterchangeFileInProcessor installer = new Installer();

            //foreach (FileInInformation info in fileIns)
            //{
            //    using (TextReader souceCodeReader = info.GetTextReader())
            //    {
            //        InterchangeFormatProcessor processor = new InterchangeFormatProcessor(info, souceCodeReader, installer, versionServicesMap);
            //        processor.ProcessInterchangeFile();
            //    }
            //}

            var runtime = new SmalltalkRuntime();
            var compilerService = new FileInService(runtime, true, fis => new InternalInstallerContext(fis.Runtime));

            InterchangeInstallerContext installer = compilerService.Read(fileIns);

            installer.ErrorSink = new InstallErrorSink();
            installer.InstallMetaAnnotations = compilerService.InstallMetaAnnotations;
            if (!installer.Install())
                return;

            IronSmalltalk.NativeCompiler.NativeCompilerParameters parameters = new IronSmalltalk.NativeCompiler.NativeCompilerParameters();
            parameters.AssemblyName = "IronSt";
            parameters.Company = "Iron Company";
            parameters.Copyright = "Copy(right)";
            parameters.EmitDebugSymbols = true;
            parameters.AssemblyType = IronSmalltalk.NativeCompiler.NativeCompilerParameters.AssemblyTypeEnum.Dll;
            parameters.OutputDirectory = "c:\\temp";
            parameters.Product = "Iron Smalltalk Product";
            parameters.AssemblyVersion = "1.2.3.4";
            parameters.FileVersion = "1.2.3.4";
            parameters.ProductVersion = "1.2.3.4";
            parameters.ProductTitle = "Iron Title";
            parameters.ProductDescription = "Just a test of the Iron Smalltalk native compiler";
            parameters.RootNamespace = "IronSmalltalk.Test";
            parameters.Runtime = runtime;
            parameters.Trademark = "Iron(tm)";

            IronSmalltalk.NativeCompiler.NativeCompiler.GenerateNativeAssembly(parameters);

            MessageBox.Show("SUCCESS!");
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.LoadFile("c:\\temp\\IronSt.dll");
            Type type = assembly.GetType("IronSmalltalk.Test.Smalltalk");
            MethodInfo method = TypeUtilities.Method(type, "CreateRuntime", typeof(bool));
            object runtime = method.Invoke(null, new object[] { true });

            MessageBox.Show("SUCCESS!");
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

        private void AddError(string type, SourceLocation startPosition, SourceLocation stopPosition, string errorMessage)
        {
            ListViewItem lvi = this.listErrors.Items.Add(type);
            lvi.SubItems.Add(startPosition.ToString());
            lvi.SubItems.Add(stopPosition.ToString());
            lvi.SubItems.Add(errorMessage);
            lvi.Tag = new SourceLocation[] { startPosition, stopPosition };
        }

        private class ErrorSink : IronSmalltalk.Internals.ErrorSinkBase
        {
            private NativeCompileTester Tester;
            public ErrorSink(NativeCompileTester tester)
            {
                this.Tester = tester;
            }
            protected override void ReportError(string message, SourceLocation start, SourceLocation end, IronSmalltalk.Internals.ErrorSinkBase.ErrorType type, params object[] offenders)
            {
                this.Tester.AddError(type.ToString(), start, end, message);
            }
        }
        
        private void buttonAddFiles_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                return;
            var paths = this.textSourceFiles.Text.Split('\n').Concat(this.openFileDialog.FileNames);
            paths = paths.Select(s => s.Trim()).Where(s => s.Length > 0);
            this.textSourceFiles.Text = String.Join("\r\n", paths);
            Properties.Settings.Default.LastNativePaths = this.textSourceFiles.Text;
            Properties.Settings.Default.Save();
        }

        private void NativeCompileTester_Load(object sender, EventArgs e)
        {
            this.textSourceFiles.Text = Properties.Settings.Default.LastNativePaths;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ParameterExpression self = Expression.Parameter(typeof(object), "self");
            //ParameterExpression arg = Expression.Parameter(typeof(object), "arg");
            //Expression<Func<object, object, object>> lambda = (Expression<Func<object, object, object>>) Expression.Lambda(
            //    Expression.Dynamic(new TestBinder(), typeof(object), self, arg), self, arg);

            //CallSite<Func<CallSite, object, object, object>> x = CallSite<Func<CallSite, object, object, object>>.Create(new TestBinder());
            //var res1 = x.Target(x, "abc : {0}", "ced");

            //Func<object, object, object> func;
            //Microsoft.Scripting.Generation.Snippets.SetSaveAssemblies(true, "c:\\temp");
            //func = Microsoft.Scripting.Generation.CompilerHelpers.Compile(lambda, true);
            

            ////Microsoft.Scripting.Generation.Snippets.Shared.SaveSnippets
            ////Func<object, object, object> func = (Func<object, object, object>) lambda.Compile();
            //object res = func("abc : {0}", "def");

            //Microsoft.Scripting.Generation.Snippets.SaveAndVerifyAssemblies();
        }

        public class TestBinder : InvokeBinder
        {
            public TestBinder()
                : base(new CallInfo(1, "obj"))
            {

            }

            public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
            {
                MethodInfo method = TypeUtilities.Method(typeof(string), "Format", typeof(string), typeof(object));
               
                var result = new DynamicMetaObject(
                    Expression.Call(method, Expression.Convert(target.Expression, typeof(string)), args[0].Expression),
                    BindingRestrictions.GetTypeRestriction(target.Expression, typeof(string)));
                return result;
            }
        }

    }
}
