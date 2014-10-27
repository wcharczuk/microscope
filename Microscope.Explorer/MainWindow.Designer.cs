namespace Microscope.Explorer
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.query = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.listBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runButton = new System.Windows.Forms.Button();
            this.testCaseView = new System.Windows.Forms.ListBox();
            this.listBoxContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // query
            // 
            this.query.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.query.Location = new System.Drawing.Point(12, 29);
            this.query.Multiline = true;
            this.query.Name = "query";
            this.query.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.query.Size = new System.Drawing.Size(695, 172);
            this.query.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter Microscope Query Here:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Test Cases:";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(657, 208);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(51, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Add New";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // listBoxContextMenu
            // 
            this.listBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewCaseToolStripMenuItem,
            this.editCaseToolStripMenuItem,
            this.removeCaseToolStripMenuItem});
            this.listBoxContextMenu.Name = "contextMenuStrip1";
            this.listBoxContextMenu.Size = new System.Drawing.Size(152, 70);
            // 
            // addNewCaseToolStripMenuItem
            // 
            this.addNewCaseToolStripMenuItem.Name = "addNewCaseToolStripMenuItem";
            this.addNewCaseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addNewCaseToolStripMenuItem.Text = "Add New Case";
            this.addNewCaseToolStripMenuItem.Click += new System.EventHandler(this.addNewCaseToolStripMenuItem_Click);
            // 
            // editCaseToolStripMenuItem
            // 
            this.editCaseToolStripMenuItem.Name = "editCaseToolStripMenuItem";
            this.editCaseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editCaseToolStripMenuItem.Text = "Edit Case";
            this.editCaseToolStripMenuItem.Click += new System.EventHandler(this.editCaseToolStripMenuItem_Click);
            // 
            // removeCaseToolStripMenuItem
            // 
            this.removeCaseToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeCaseToolStripMenuItem.Name = "removeCaseToolStripMenuItem";
            this.removeCaseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.removeCaseToolStripMenuItem.Text = "Remove Case";
            this.removeCaseToolStripMenuItem.Click += new System.EventHandler(this.removeCaseToolStripMenuItem_Click);
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Location = new System.Drawing.Point(631, 562);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 6;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // testCaseView
            // 
            this.testCaseView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testCaseView.FormattingEnabled = true;
            this.testCaseView.Location = new System.Drawing.Point(15, 225);
            this.testCaseView.Name = "testCaseView";
            this.testCaseView.Size = new System.Drawing.Size(693, 329);
            this.testCaseView.TabIndex = 7;
            this.testCaseView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.testCaseView_MouseDown);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 594);
            this.Controls.Add(this.testCaseView);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.query);
            this.Name = "MainWindow";
            this.Text = "Microscope Explorer";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.listBoxContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox query;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ContextMenuStrip listBoxContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addNewCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeCaseToolStripMenuItem;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.ListBox testCaseView;
    }
}

