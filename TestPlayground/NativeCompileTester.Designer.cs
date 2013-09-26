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

namespace TestPlayground
{
    partial class NativeCompileTester
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader3;
            this.buttonAddFiles = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textSourceFiles = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listErrors = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(16, 30);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(90, 17);
            label1.TabIndex = 1;
            label1.Text = "Source Files:";
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Type";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Start";
            columnHeader2.Width = 45;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Stop";
            columnHeader3.Width = 45;
            // 
            // buttonAddFiles
            // 
            this.buttonAddFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddFiles.Location = new System.Drawing.Point(628, 15);
            this.buttonAddFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonAddFiles.Name = "buttonAddFiles";
            this.buttonAddFiles.Size = new System.Drawing.Size(43, 31);
            this.buttonAddFiles.TabIndex = 0;
            this.buttonAddFiles.Text = "...";
            this.buttonAddFiles.UseVisualStyleBackColor = true;
            this.buttonAddFiles.Click += new System.EventHandler(this.buttonAddFiles_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "ist";
            this.openFileDialog.Filter = "Smalltalk Files|*.st;*.ist|All Files|*.*";
            this.openFileDialog.Multiselect = true;
            // 
            // textSourceFiles
            // 
            this.textSourceFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSourceFiles.Location = new System.Drawing.Point(16, 53);
            this.textSourceFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textSourceFiles.Multiline = true;
            this.textSourceFiles.Name = "textSourceFiles";
            this.textSourceFiles.Size = new System.Drawing.Size(653, 117);
            this.textSourceFiles.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(16, 178);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(654, 75);
            this.button1.TabIndex = 3;
            this.button1.Text = "Los!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listErrors
            // 
            this.listErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3,
            this.columnHeader4});
            this.listErrors.FullRowSelect = true;
            this.listErrors.HideSelection = false;
            this.listErrors.Location = new System.Drawing.Point(16, 358);
            this.listErrors.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listErrors.MultiSelect = false;
            this.listErrors.Name = "listErrors";
            this.listErrors.ShowGroups = false;
            this.listErrors.ShowItemToolTips = true;
            this.listErrors.Size = new System.Drawing.Size(653, 175);
            this.listErrors.TabIndex = 4;
            this.listErrors.UseCompatibleStateImageBehavior = false;
            this.listErrors.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Error Message";
            this.columnHeader4.Width = 550;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(16, 265);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(653, 43);
            this.button2.TabIndex = 5;
            this.button2.Text = "Load";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(19, 315);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(648, 36);
            this.button3.TabIndex = 6;
            this.button3.Text = "Test";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // NativeCompileTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 548);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listErrors);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textSourceFiles);
            this.Controls.Add(label1);
            this.Controls.Add(this.buttonAddFiles);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "NativeCompileTester";
            this.Text = "NativeCompileTester";
            this.Load += new System.EventHandler(this.NativeCompileTester_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAddFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox textSourceFiles;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listErrors;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}