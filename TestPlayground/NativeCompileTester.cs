﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronSmalltalk;
using IronSmalltalk.Common;
using IronSmalltalk.Interchange;
using IronSmalltalk.InterchangeInstaller;
using IronSmalltalk.Internals;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Installer;
using IronSmalltalk.Runtime.Installer.Definitions;

namespace TestPlayground
{
    public partial class NativeCompileTester : Form
    {
        public NativeCompileTester()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ErrorSink errorSink = new ErrorSink(this);

            var paths = this.textSourceFiles.Text.Split('\n').Concat(this.openFileDialog.FileNames);
            paths = paths.Select(s => s.Trim()).Where(s => s.Length > 0);
            this.textSourceFiles.Text = String.Join("\r\n", paths);
            Properties.Settings.Default.LastNativePaths = this.textSourceFiles.Text;
            Properties.Settings.Default.Save();

            var fileIns = paths.Select(p => new PathFileInInformation(p, System.Text.Encoding.UTF8, errorSink));
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
            var compilerService = new FileInService(runtime, true, null);

            compilerService.Install(fileIns);

            IronSmalltalk.NativeCompiler.NativeCompiler.GenerateNativeAssembly(runtime,
                "IronSmalltalk.Test", "C:\\Temp", "IronSt", "dll", true,
                "Iron Smalltalk Product", "1.2.3.4", "Iron Company", "Copy(right)", "Iron(tm)");

            MessageBox.Show("SUCCESS!");
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

        private class Installer : IInterchangeFileInProcessor
        {
            private List<GlobalDefinition> _globals = new List<GlobalDefinition>();
            private List<ClassDefinition> _classes = new List<ClassDefinition>();
            private List<PoolDefinition> _pools = new List<PoolDefinition>();

            private List<PoolValueDefinition> _poolVariables = new List<PoolValueDefinition>();
            private List<MethodDefinition> _methods = new List<MethodDefinition>();
            private List<InitializerDefinition> _initializers = new List<InitializerDefinition>();
            private List<Tuple<SmalltalkClass, ISourceReference>> _newClasses = new List<Tuple<SmalltalkClass, ISourceReference>>();

            public bool FileInClass(IronSmalltalk.Runtime.Installer.Definitions.ClassDefinition definition)
            {
                this._classes.Add(definition);
                return true;
            }

            public bool FileInGlobal(IronSmalltalk.Runtime.Installer.Definitions.GlobalDefinition definition)
            {
                this._globals.Add(definition);
                return true;
            }

            public bool FileInGlobalInitializer(IronSmalltalk.Runtime.Installer.Definitions.GlobalInitializer initializer)
            {
                return true;
            }

            public bool FileInMethod(IronSmalltalk.Runtime.Installer.Definitions.MethodDefinition definition)
            {
                this._methods.Add(definition);
                return true;
            }

            public bool FileInPool(IronSmalltalk.Runtime.Installer.Definitions.PoolDefinition definition)
            {
                this._pools.Add(definition);
                return true;
            }

            public bool FileInPoolVariable(IronSmalltalk.Runtime.Installer.Definitions.PoolValueDefinition definition)
            {
                this._poolVariables.Add(definition);
                return true;
            }

            public bool FileInPoolVariableInitializer(IronSmalltalk.Runtime.Installer.Definitions.PoolVariableInitializer initializer)
            {
                return true;
            }

            public bool FileInProgramInitializer(IronSmalltalk.Runtime.Installer.Definitions.ProgramInitializer initializer)
            {
                return true;
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
    }
}