namespace CFAndroidSync
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolStrip1 = new ToolStrip();
            tscCategory = new ToolStripComboBox();
            statusStrip1 = new StatusStrip();
            tssStatus = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            tvwFileSystem = new TreeView();
            cmsFolder = new ContextMenuStrip(components);
            copyLocalFilesToFolderToolStripMenuItem = new ToolStripMenuItem();
            copyLocalFolderToToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            cmsFolder.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tscCategory });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1245, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // tscCategory
            // 
            tscCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            tscCategory.Name = "tscCategory";
            tscCategory.Size = new Size(121, 25);
            tscCategory.SelectedIndexChanged += tscCategory_SelectedIndexChanged;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tssStatus });
            statusStrip1.Location = new Point(0, 685);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1245, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // tssStatus
            // 
            tssStatus.Name = "tssStatus";
            tssStatus.Size = new Size(118, 17);
            tssStatus.Text = "toolStripStatusLabel1";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 25);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tvwFileSystem);
            splitContainer1.Size = new Size(1245, 660);
            splitContainer1.SplitterDistance = 413;
            splitContainer1.TabIndex = 2;
            // 
            // tvwFileSystem
            // 
            tvwFileSystem.Dock = DockStyle.Fill;
            tvwFileSystem.Location = new Point(0, 0);
            tvwFileSystem.Name = "tvwFileSystem";
            tvwFileSystem.Size = new Size(413, 660);
            tvwFileSystem.TabIndex = 0;
            tvwFileSystem.BeforeExpand += tvwFileSystem_BeforeExpand;
            tvwFileSystem.AfterSelect += tvwFileSystem_AfterSelect;
            tvwFileSystem.MouseUp += tvwFileSystem_MouseUp;
            // 
            // cmsFolder
            // 
            cmsFolder.Items.AddRange(new ToolStripItem[] { copyLocalFilesToFolderToolStripMenuItem, copyLocalFolderToToolStripMenuItem });
            cmsFolder.Name = "cmsFolder";
            cmsFolder.Size = new Size(188, 70);
            // 
            // copyLocalFilesToFolderToolStripMenuItem
            // 
            copyLocalFilesToFolderToolStripMenuItem.Name = "copyLocalFilesToFolderToolStripMenuItem";
            copyLocalFilesToFolderToolStripMenuItem.Size = new Size(187, 22);
            copyLocalFilesToFolderToolStripMenuItem.Text = "Copy local files to...";
            copyLocalFilesToFolderToolStripMenuItem.Click += copyLocalFilesToFolderToolStripMenuItem_Click;
            // 
            // copyLocalFolderToToolStripMenuItem
            // 
            copyLocalFolderToToolStripMenuItem.Name = "copyLocalFolderToToolStripMenuItem";
            copyLocalFolderToToolStripMenuItem.Size = new Size(187, 22);
            copyLocalFolderToToolStripMenuItem.Text = "Copy local folder to...";
            copyLocalFolderToToolStripMenuItem.Click += copyLocalFolderToToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1245, 707);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Name = "MainForm";
            Text = "CF Android Sync";
            Load += Form1_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            cmsFolder.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private StatusStrip statusStrip1;
        private SplitContainer splitContainer1;
        private ToolStripComboBox tscCategory;
        private TreeView tvwFileSystem;
        private ToolStripStatusLabel tssStatus;
        private ContextMenuStrip cmsFolder;
        private ToolStripMenuItem copyLocalFilesToFolderToolStripMenuItem;
        private ToolStripMenuItem copyLocalFolderToToolStripMenuItem;
    }
}
