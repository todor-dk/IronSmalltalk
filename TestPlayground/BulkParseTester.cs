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
using System.Text;
using System.Windows.Forms;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.Compiler.VseCompatibility;


namespace TestPlayground
{
    public partial class BulkParseTester : Form 
    {
        public bool Stop;
        public ParseTester ParseTester;
        public Dictionary<object, List<string>> Errors;

        public BulkParseTester()
        {
            InitializeComponent();
        }

        private void buttonChooseFile_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                return;

            this.txtFilename.Text = this.openFileDialog.FileName;

            this.buttonParse.Enabled = !String.IsNullOrWhiteSpace(this.txtFilename.Text);
        }

        private void buttonParse_Click(object sender, EventArgs e)
        {
            this.Stop = false;
            this.buttonParse.Visible = false;
            this.buttonStop.Visible = true;
            this.listErrors.Items.Clear();
            int processed = 0;
            int errors = 0;
            try
            {
                using (FileStream fs = File.OpenRead(this.txtFilename.Text))
                {
                    this.progressBar.Value = 0;
                    this.progressBar.Maximum = (int)fs.Length;

                    int b;
                    do
                    {
                        StringBuilder sb = new StringBuilder(200);
                        int start = (int)fs.Position;
                        while (true)
                        {
                            b = fs.ReadByte();
                            if (b > 0)
                                sb.Append((char)b);
                            else
                                break;
                        }

                        this.ParseMethod(sb.ToString(), start, ref processed, ref errors);
                        this.progressBar.Value = (int)fs.Position;
                        Application.DoEvents();
                        if (this.Stop)
                            break;
                    } while (b != -1);
                }

                this.listErrors.Items.Add(String.Format("Processed: {0}, Errors: {1}", processed, errors));
            }
            finally
            {
                this.buttonStop.Visible = false;
                this.buttonParse.Visible = true;
            }
        }

        private void ParseMethod(string src, int position, ref int processed, ref int errors)
        {
            if (String.IsNullOrWhiteSpace(src))
                return;

            StringReader reader = new StringReader(src);

            bool ok = true;
            string err = null;
            try
            {
                Parser parser = new VseCompatibleParser();
                this.Errors = new Dictionary<object, List<string>>();
                parser.ErrorSink = new ErrorSink(this);
                MethodNode node = parser.ParseMethod(reader);
                ok = (this.Errors.Count == 0);
                if (!ok)
                    err = "Errors: " + this.Errors.Count.ToString();
            }
            catch (Exception ex)
            {
                ok = false;
                err = ex.Message;
            }

            if (!ok)
                this.listErrors.Items.Add("Pos: " + position + ", " + err).Tag = src;
            if (!ok)
                errors++;
            processed++;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.Stop = true;
        }

        private void listErrors_DoubleClick(object sender, EventArgs e)
        {
            if (this.listErrors.FocusedItem == null)
                return;
            string src = this.listErrors.FocusedItem.Tag as string;
            if (String.IsNullOrWhiteSpace(src))
                return;

            if ((this.ParseTester == null) || this.ParseTester.IsDisposed)
                this.ParseTester = new ParseTester();

            this.ParseTester.txtSource.Text = src;
            this.ParseTester.Show();
        }

        private void listErrors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private class ErrorSink : IronSmalltalk.Internals.ErrorSinkBase
        {
            private BulkParseTester Tester;
            public ErrorSink(BulkParseTester tester)
            {
                this.Tester = tester;
            }

            protected override void ReportError(string message, SourceLocation start, SourceLocation end, IronSmalltalk.Internals.ErrorSinkBase.ErrorType type, params object[] offenders)
            {
                if (!this.Tester.Errors.ContainsKey(this))
                    this.Tester.Errors.Add(this, new List<string>());
                this.Tester.Errors[this].Add(message);
            }
        }
    }
}
