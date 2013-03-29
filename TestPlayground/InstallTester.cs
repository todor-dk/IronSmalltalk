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
using IronSmalltalk.Common;


namespace TestPlayground
{
    public partial class InstallTester : Form
    {
        SmalltalkEnvironment Environment;
        public InstallTester()
        {
            InitializeComponent();
        }

        private void buttonCreateEnvironment_Click(object sender, EventArgs e)
        {
            this.Environment = new SmalltalkEnvironment();
            this.listErrors.Items.Clear();
        }

        private void buttonDebug_Click(object sender, EventArgs e)
        {
            SmalltalkEnvironment env = this.Environment;
            System.Diagnostics.Debugger.Break();
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            if (this.Environment == null)
            {
                MessageBox.Show("First, create the environment.");
                return;
            }
            Properties.Settings.Default.LastInstallerSource = this.txtSource.Text;
            Properties.Settings.Default.Save();
            this.listErrors.Items.Clear();

            this.Environment.CompilerService.InstallSource(this.txtSource.Text, new ErrorSink(this), new ErrorSink(this));
        }

        private void AddError(string type, SourceLocation startPosition, SourceLocation stopPosition, string errorMessage)
        {
            ListViewItem lvi = this.listErrors.Items.Add(type);
            lvi.SubItems.Add(startPosition.ToString());
            lvi.SubItems.Add(stopPosition.ToString());
            lvi.SubItems.Add(errorMessage);
            lvi.Tag = new SourceLocation[] {startPosition, stopPosition};
        }

        private class ErrorSink : IronSmalltalk.Internals.ErrorSinkBase
        {
            private InstallTester Tester;
            public ErrorSink(InstallTester tester)
            {
                this.Tester = tester;
            }
            protected override void ReportError(string message, SourceLocation start, SourceLocation end, IronSmalltalk.Internals.ErrorSinkBase.ErrorType type, params object[] offenders)
            {
                this.Tester.AddError(type.ToString(), start, end, message);
            }
        }

        private void listErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listErrors.FocusedItem == null)
            {
                this.txtSource.SelectionLength = 0;
                return;
            }
            if (!(this.listErrors.FocusedItem.Tag is SourceLocation[]))
            {
                this.txtSource.SelectionLength = 0;
                return;
            }
            SourceLocation[] sel = (SourceLocation[])this.listErrors.FocusedItem.Tag;
            this.txtSource.SelectionStart = sel[0].Position;
            this.txtSource.SelectionLength = sel[1].Position - sel[0].Position + 1;
        }

        private void InstallTester_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.LastInstallerSource != null)
                this.txtSource.Text = Properties.Settings.Default.LastInstallerSource;
            this.Environment = new SmalltalkEnvironment();
        }
    }
}
