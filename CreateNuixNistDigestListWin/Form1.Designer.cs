
namespace CreateNuixNistDigestList
{
    partial class Form1
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
            this.btGetRDS = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.tbBaseURL = new System.Windows.Forms.TextBox();
            this.lbBaseURL = new System.Windows.Forms.Label();
            this.gbRDSVersion = new System.Windows.Forms.GroupBox();
            this.linkLabelCurrentVersion = new System.Windows.Forms.LinkLabel();
            this.tbCustomVersion = new System.Windows.Forms.TextBox();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.rbCurrent = new System.Windows.Forms.RadioButton();
            this.cbSkipDownload = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbRDSSelectionHelp = new System.Windows.Forms.TextBox();
            this.cbiOS = new System.Windows.Forms.CheckBox();
            this.cbAndroid = new System.Windows.Forms.CheckBox();
            this.cbLegacy = new System.Windows.Forms.CheckBox();
            this.cbModernMinimal = new System.Windows.Forms.CheckBox();
            this.cbModernUnique = new System.Windows.Forms.CheckBox();
            this.cbModern = new System.Windows.Forms.CheckBox();
            this.tbConsole = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.cbDeleteHashcodeFiles = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.gbRDSVersion.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btGetRDS
            // 
            this.btGetRDS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGetRDS.Location = new System.Drawing.Point(391, 384);
            this.btGetRDS.Name = "btGetRDS";
            this.btGetRDS.Size = new System.Drawing.Size(75, 23);
            this.btGetRDS.TabIndex = 1;
            this.btGetRDS.Text = "Go";
            this.btGetRDS.UseVisualStyleBackColor = true;
            this.btGetRDS.Click += new System.EventHandler(this.btGetRDS_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(484, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Resize += new System.EventHandler(this.statusStrip1_Resize);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.AutoToolTip = true;
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(230, 17);
            this.toolStripStatusLabel1.Text = "status";
            this.toolStripStatusLabel1.ToolTipText = "Shows the current operation";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.AutoToolTip = true;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(235, 16);
            // 
            // tbBaseURL
            // 
            this.tbBaseURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBaseURL.Location = new System.Drawing.Point(70, 19);
            this.tbBaseURL.Name = "tbBaseURL";
            this.tbBaseURL.Size = new System.Drawing.Size(384, 20);
            this.tbBaseURL.TabIndex = 3;
            this.tbBaseURL.Text = "https://s3.amazonaws.com/rds.nsrl.nist.gov/RDS";
            this.tbBaseURL.Click += new System.EventHandler(this.tbBaseURL_Click);
            this.tbBaseURL.DoubleClick += new System.EventHandler(this.tbBaseURL_DoubleClick);
            // 
            // lbBaseURL
            // 
            this.lbBaseURL.AutoSize = true;
            this.lbBaseURL.Location = new System.Drawing.Point(8, 22);
            this.lbBaseURL.Name = "lbBaseURL";
            this.lbBaseURL.Size = new System.Drawing.Size(56, 13);
            this.lbBaseURL.TabIndex = 4;
            this.lbBaseURL.Text = "Base URL";
            // 
            // gbRDSVersion
            // 
            this.gbRDSVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRDSVersion.Controls.Add(this.linkLabelCurrentVersion);
            this.gbRDSVersion.Controls.Add(this.tbCustomVersion);
            this.gbRDSVersion.Controls.Add(this.rbCustom);
            this.gbRDSVersion.Controls.Add(this.rbCurrent);
            this.gbRDSVersion.Location = new System.Drawing.Point(12, 158);
            this.gbRDSVersion.Name = "gbRDSVersion";
            this.gbRDSVersion.Size = new System.Drawing.Size(460, 87);
            this.gbRDSVersion.TabIndex = 5;
            this.gbRDSVersion.TabStop = false;
            this.gbRDSVersion.Text = "RDS Version";
            // 
            // linkLabelCurrentVersion
            // 
            this.linkLabelCurrentVersion.AutoSize = true;
            this.linkLabelCurrentVersion.Location = new System.Drawing.Point(98, 30);
            this.linkLabelCurrentVersion.Name = "linkLabelCurrentVersion";
            this.linkLabelCurrentVersion.Size = new System.Drawing.Size(0, 13);
            this.linkLabelCurrentVersion.TabIndex = 4;
            this.linkLabelCurrentVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCurrentVersion_LinkClicked);
            // 
            // tbCustomVersion
            // 
            this.tbCustomVersion.Location = new System.Drawing.Point(105, 51);
            this.tbCustomVersion.Name = "tbCustomVersion";
            this.tbCustomVersion.Size = new System.Drawing.Size(100, 20);
            this.tbCustomVersion.TabIndex = 2;
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(39, 51);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(60, 17);
            this.rbCustom.TabIndex = 1;
            this.rbCustom.Text = "Custom";
            this.rbCustom.UseVisualStyleBackColor = true;
            // 
            // rbCurrent
            // 
            this.rbCurrent.AutoSize = true;
            this.rbCurrent.Checked = true;
            this.rbCurrent.Location = new System.Drawing.Point(39, 28);
            this.rbCurrent.Name = "rbCurrent";
            this.rbCurrent.Size = new System.Drawing.Size(59, 17);
            this.rbCurrent.TabIndex = 0;
            this.rbCurrent.TabStop = true;
            this.rbCurrent.Text = "Current";
            this.rbCurrent.UseVisualStyleBackColor = true;
            // 
            // cbSkipDownload
            // 
            this.cbSkipDownload.AutoSize = true;
            this.cbSkipDownload.Location = new System.Drawing.Point(14, 45);
            this.cbSkipDownload.Name = "cbSkipDownload";
            this.cbSkipDownload.Size = new System.Drawing.Size(98, 17);
            this.cbSkipDownload.TabIndex = 6;
            this.cbSkipDownload.Text = "Skip Download";
            this.cbSkipDownload.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbRDSSelectionHelp);
            this.groupBox1.Controls.Add(this.cbiOS);
            this.groupBox1.Controls.Add(this.cbAndroid);
            this.groupBox1.Controls.Add(this.cbLegacy);
            this.groupBox1.Controls.Add(this.cbModernMinimal);
            this.groupBox1.Controls.Add(this.cbModernUnique);
            this.groupBox1.Controls.Add(this.cbModern);
            this.groupBox1.Location = new System.Drawing.Point(12, 254);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 124);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RDS Selection";
            // 
            // tbRDSSelectionHelp
            // 
            this.tbRDSSelectionHelp.BackColor = System.Drawing.SystemColors.Menu;
            this.tbRDSSelectionHelp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRDSSelectionHelp.Location = new System.Drawing.Point(12, 25);
            this.tbRDSSelectionHelp.Multiline = true;
            this.tbRDSSelectionHelp.Name = "tbRDSSelectionHelp";
            this.tbRDSSelectionHelp.Size = new System.Drawing.Size(374, 20);
            this.tbRDSSelectionHelp.TabIndex = 6;
            this.tbRDSSelectionHelp.Text = "Choose which RDS hash sets to include in the Nuix digest file.";
            // 
            // cbiOS
            // 
            this.cbiOS.AutoSize = true;
            this.cbiOS.Checked = true;
            this.cbiOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbiOS.Location = new System.Drawing.Point(347, 57);
            this.cbiOS.Name = "cbiOS";
            this.cbiOS.Size = new System.Drawing.Size(43, 17);
            this.cbiOS.TabIndex = 5;
            this.cbiOS.Text = "iOS";
            this.cbiOS.UseVisualStyleBackColor = true;
            // 
            // cbAndroid
            // 
            this.cbAndroid.AutoSize = true;
            this.cbAndroid.Checked = true;
            this.cbAndroid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAndroid.Location = new System.Drawing.Point(179, 88);
            this.cbAndroid.Name = "cbAndroid";
            this.cbAndroid.Size = new System.Drawing.Size(62, 17);
            this.cbAndroid.TabIndex = 4;
            this.cbAndroid.Text = "Android";
            this.cbAndroid.UseVisualStyleBackColor = true;
            // 
            // cbLegacy
            // 
            this.cbLegacy.AutoSize = true;
            this.cbLegacy.Checked = true;
            this.cbLegacy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLegacy.Location = new System.Drawing.Point(347, 88);
            this.cbLegacy.Name = "cbLegacy";
            this.cbLegacy.Size = new System.Drawing.Size(61, 17);
            this.cbLegacy.TabIndex = 3;
            this.cbLegacy.Text = "Legacy";
            this.cbLegacy.UseVisualStyleBackColor = true;
            // 
            // cbModernMinimal
            // 
            this.cbModernMinimal.AutoSize = true;
            this.cbModernMinimal.Checked = true;
            this.cbModernMinimal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbModernMinimal.Location = new System.Drawing.Point(12, 88);
            this.cbModernMinimal.Name = "cbModernMinimal";
            this.cbModernMinimal.Size = new System.Drawing.Size(100, 17);
            this.cbModernMinimal.TabIndex = 2;
            this.cbModernMinimal.Text = "Modern Minimal";
            this.cbModernMinimal.UseVisualStyleBackColor = true;
            // 
            // cbModernUnique
            // 
            this.cbModernUnique.AutoSize = true;
            this.cbModernUnique.Location = new System.Drawing.Point(179, 57);
            this.cbModernUnique.Name = "cbModernUnique";
            this.cbModernUnique.Size = new System.Drawing.Size(99, 17);
            this.cbModernUnique.TabIndex = 1;
            this.cbModernUnique.Text = "Modern Unique";
            this.cbModernUnique.UseVisualStyleBackColor = true;
            // 
            // cbModern
            // 
            this.cbModern.AutoSize = true;
            this.cbModern.Location = new System.Drawing.Point(12, 57);
            this.cbModern.Name = "cbModern";
            this.cbModern.Size = new System.Drawing.Size(62, 17);
            this.cbModern.TabIndex = 0;
            this.cbModern.Text = "Modern";
            this.cbModern.UseVisualStyleBackColor = true;
            // 
            // tbConsole
            // 
            this.tbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConsole.Location = new System.Drawing.Point(12, 420);
            this.tbConsole.Multiline = true;
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbConsole.Size = new System.Drawing.Size(460, 104);
            this.tbConsole.TabIndex = 8;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(12, 37);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(456, 24);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "This application helps with creating a Nuix compatible digest list using NIST NSR" +
    "L RDS from the NSRL website.";
            // 
            // cbDeleteHashcodeFiles
            // 
            this.cbDeleteHashcodeFiles.AutoSize = true;
            this.cbDeleteHashcodeFiles.Location = new System.Drawing.Point(145, 45);
            this.cbDeleteHashcodeFiles.Name = "cbDeleteHashcodeFiles";
            this.cbDeleteHashcodeFiles.Size = new System.Drawing.Size(133, 17);
            this.cbDeleteHashcodeFiles.TabIndex = 10;
            this.cbDeleteHashcodeFiles.Text = "Delete Hashcode Files";
            this.cbDeleteHashcodeFiles.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbDeleteHashcodeFiles);
            this.groupBox2.Controls.Add(this.cbSkipDownload);
            this.groupBox2.Controls.Add(this.tbBaseURL);
            this.groupBox2.Controls.Add(this.lbBaseURL);
            this.groupBox2.Location = new System.Drawing.Point(12, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(460, 71);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.digestToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(484, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // digestToolStripMenuItem
            // 
            this.digestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem,
            this.extractToolStripMenuItem});
            this.digestToolStripMenuItem.Name = "digestToolStripMenuItem";
            this.digestToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.digestToolStripMenuItem.Text = "Digest";
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.createToolStripMenuItem.Text = "&Create";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extractToolStripMenuItem.Text = "&Extract";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.btGetRDS;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.tbConsole);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbRDSVersion);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btGetRDS);
            this.Controls.Add(this.groupBox2);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 600);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Create Nuix Digest List";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbRDSVersion.ResumeLayout(false);
            this.gbRDSVersion.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btGetRDS;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.TextBox tbBaseURL;
        private System.Windows.Forms.Label lbBaseURL;
        private System.Windows.Forms.GroupBox gbRDSVersion;
        private System.Windows.Forms.TextBox tbCustomVersion;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.RadioButton rbCurrent;
        private System.Windows.Forms.CheckBox cbSkipDownload;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbRDSSelectionHelp;
        private System.Windows.Forms.CheckBox cbiOS;
        private System.Windows.Forms.CheckBox cbAndroid;
        private System.Windows.Forms.CheckBox cbLegacy;
        private System.Windows.Forms.CheckBox cbModernMinimal;
        private System.Windows.Forms.CheckBox cbModernUnique;
        private System.Windows.Forms.CheckBox cbModern;
        private System.Windows.Forms.TextBox tbConsole;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.LinkLabel linkLabelCurrentVersion;
        private System.Windows.Forms.CheckBox cbDeleteHashcodeFiles;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem digestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

