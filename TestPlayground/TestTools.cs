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
using System.Windows.Forms;
using IronSmalltalk;
using IronSmalltalk.Hosting.Hosting;
using IronSmalltalk.InterchangeInstaller;
using IronSmalltalk.Internals;
using IronSmalltalk.Runtime.Hosting;
using Microsoft.Scripting.Hosting;


namespace TestPlayground
{
    public partial class TestTools : Form
    {
        public TestTools()
        {
            InitializeComponent();
        }

        private void buttonLexTester_Click(object sender, EventArgs e)
        {
            LexTester tester = new LexTester();
            tester.Show();
        }

        private void buttonParseTester_Click(object sender, EventArgs e)
        {
            ParseTester tester = new ParseTester();
            tester.Show();
        }

        private void buttonBulkParseTest_Click(object sender, EventArgs e)
        {
            BulkParseTester tester = new BulkParseTester();
            tester.Show();
        }

        private void buttonProcessInterchangeFiles_Click(object sender, EventArgs e)
        {
            string path = @"D:\Users\TT\Documents\Visual Studio 2010\Projects\IronSmalltalk\TestPlayground\Test.ist";
            path = @"D:\Users\TT\Desktop\IronSmalltalk.ist";
            SmalltalkEnvironment env = new SmalltalkEnvironment();
            FileInService compiler = env.CompilerService;
            compiler.Install(new PathFileInInformation(path, System.Text.Encoding.UTF8, new ConsoleErrorSink()));
        }

        private void buttonInstallTester_Click(object sender, EventArgs e)
        {
            InstallTester tester = new InstallTester();
            tester.Show();
        }

        private void buttonSimpleRuntimeTest_Click(object sender, EventArgs e)
        {
            // Setup DLR ScriptRuntime with our languages.  We hardcode them here
            // but a .NET app looking for general language scripting would use
            // an app.config file and ScriptRuntime.CreateFromConfiguration.
            ScriptRuntimeSetup setup = new ScriptRuntimeSetup();
            string qualifiedname = typeof(SmalltalkLanguageContext).AssemblyQualifiedName;
            setup.LanguageSetups.Add(new LanguageSetup(
                typeof(SmalltalkLanguageContext).AssemblyQualifiedName,
                "IronSmalltalk",
                new string[] { "IronSmalltalk", "Iron Smalltalk", "ist" },
                new string[] { "ist" }));

            ScriptRuntime dlrRuntime = new ScriptRuntime(setup);

            // Get a IST engine and run stuff ...
            ScriptEngine engine = dlrRuntime.GetEngine("IronSmalltalk");
            string filename = @"D:\Users\TT\Documents\Visual Studio 2010\Projects\IronSmalltalk\TestPlayground\Test.ist";
            var x = engine.ExecuteFile(filename);
            var y = engine.Execute("1234");
        }

        private void buttonWorkspaceTester_Click(object sender, EventArgs e)
        {
            WorkspaceTester tester = new WorkspaceTester();
            tester.Show();
        }

        private void buttonNativeCompileTest_Click(object sender, EventArgs e)
        {
            NativeCompileTester tester = new NativeCompileTester();
            tester.Show();
        }
    }
}
