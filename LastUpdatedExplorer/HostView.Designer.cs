namespace LastUpdatedExplorer
{
    partial class HostView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._dtpTo = new System.Windows.Forms.DateTimePicker();
            this._dtpFrom = new System.Windows.Forms.DateTimePicker();
            this._btnGo = new System.Windows.Forms.Button();
            this._cbxLastAccessed = new System.Windows.Forms.CheckBox();
            this._cbxLastModified = new System.Windows.Forms.CheckBox();
            this._cbxCreationTime = new System.Windows.Forms.CheckBox();
            this._txtPath = new System.Windows.Forms.TextBox();
            this._btnBrowse = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label3 = new System.Windows.Forms.Label();
            this._explorer = new LastUpdatedExplorer.LazyFilteredExplorer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this._dtpTo);
            this.splitContainer1.Panel1.Controls.Add(this._dtpFrom);
            this.splitContainer1.Panel1.Controls.Add(this._btnGo);
            this.splitContainer1.Panel1.Controls.Add(this._cbxLastAccessed);
            this.splitContainer1.Panel1.Controls.Add(this._cbxLastModified);
            this.splitContainer1.Panel1.Controls.Add(this._cbxCreationTime);
            this.splitContainer1.Panel1.Controls.Add(this._txtPath);
            this.splitContainer1.Panel1.Controls.Add(this._btnBrowse);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._explorer);
            this.splitContainer1.Size = new System.Drawing.Size(855, 497);
            this.splitContainer1.SplitterDistance = 90;
            this.splitContainer1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "To:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "From:";
            // 
            // _dtpTo
            // 
            this._dtpTo.Location = new System.Drawing.Point(48, 63);
            this._dtpTo.Name = "_dtpTo";
            this._dtpTo.Size = new System.Drawing.Size(200, 20);
            this._dtpTo.TabIndex = 3;
            // 
            // _dtpFrom
            // 
            this._dtpFrom.Location = new System.Drawing.Point(48, 39);
            this._dtpFrom.Name = "_dtpFrom";
            this._dtpFrom.Size = new System.Drawing.Size(200, 20);
            this._dtpFrom.TabIndex = 2;
            // 
            // _btnGo
            // 
            this._btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnGo.Location = new System.Drawing.Point(759, 39);
            this._btnGo.Name = "_btnGo";
            this._btnGo.Size = new System.Drawing.Size(84, 44);
            this._btnGo.TabIndex = 7;
            this._btnGo.Text = "Go";
            this._btnGo.UseVisualStyleBackColor = true;
            this._btnGo.Click += new System.EventHandler(this.BtnGo_Click);
            // 
            // _cbxLastAccessed
            // 
            this._cbxLastAccessed.AutoSize = true;
            this._cbxLastAccessed.Location = new System.Drawing.Point(457, 41);
            this._cbxLastAccessed.Name = "_cbxLastAccessed";
            this._cbxLastAccessed.Size = new System.Drawing.Size(95, 17);
            this._cbxLastAccessed.TabIndex = 6;
            this._cbxLastAccessed.Text = "Last accessed";
            this._cbxLastAccessed.UseVisualStyleBackColor = true;
            // 
            // _cbxLastModified
            // 
            this._cbxLastModified.AutoSize = true;
            this._cbxLastModified.Location = new System.Drawing.Point(363, 41);
            this._cbxLastModified.Name = "_cbxLastModified";
            this._cbxLastModified.Size = new System.Drawing.Size(88, 17);
            this._cbxLastModified.TabIndex = 5;
            this._cbxLastModified.Text = "Last modified";
            this._cbxLastModified.UseVisualStyleBackColor = true;
            // 
            // _cbxCreationTime
            // 
            this._cbxCreationTime.AutoSize = true;
            this._cbxCreationTime.Location = new System.Drawing.Point(269, 41);
            this._cbxCreationTime.Name = "_cbxCreationTime";
            this._cbxCreationTime.Size = new System.Drawing.Size(87, 17);
            this._cbxCreationTime.TabIndex = 4;
            this._cbxCreationTime.Text = "Creation time";
            this._cbxCreationTime.UseVisualStyleBackColor = true;
            // 
            // _txtPath
            // 
            this._txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtPath.Location = new System.Drawing.Point(48, 12);
            this._txtPath.Name = "_txtPath";
            this._txtPath.ReadOnly = true;
            this._txtPath.Size = new System.Drawing.Size(705, 20);
            this._txtPath.TabIndex = 0;
            this._txtPath.Text = "C:\\";
            // 
            // _btnBrowse
            // 
            this._btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnBrowse.Location = new System.Drawing.Point(759, 11);
            this._btnBrowse.Name = "_btnBrowse";
            this._btnBrowse.Size = new System.Drawing.Size(84, 22);
            this._btnBrowse.TabIndex = 1;
            this._btnBrowse.Text = "Search";
            this._btnBrowse.UseVisualStyleBackColor = true;
            this._btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Root:";
            // 
            // _explorer
            // 
            this._explorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._explorer.Filter = null;
            this._explorer.Location = new System.Drawing.Point(0, 0);
            this._explorer.Name = "_explorer";
            this._explorer.RootDirectory = null;
            this._explorer.Size = new System.Drawing.Size(855, 403);
            this._explorer.TabIndex = 8;
            // 
            // HostView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 497);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(725, 420);
            this.Name = "HostView";
            this.Text = "HostView";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button _btnBrowse;
        private System.Windows.Forms.TextBox _txtPath;
        private System.Windows.Forms.CheckBox _cbxCreationTime;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button _btnGo;
        private LazyFilteredExplorer _explorer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker _dtpFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker _dtpTo;
        private System.Windows.Forms.CheckBox _cbxLastModified;
        private System.Windows.Forms.CheckBox _cbxLastAccessed;
        private System.Windows.Forms.Label label3;
    }
}