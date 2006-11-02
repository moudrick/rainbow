namespace RainbowSimpleNantRunner
{
    partial class Main
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.textCommandLine = new System.Windows.Forms.TextBox();
			this.buttonRun = new System.Windows.Forms.Button();
			this.splitContainerOutput = new System.Windows.Forms.SplitContainer();
			this.textStandardOutput = new System.Windows.Forms.TextBox();
			this.textStandardError = new System.Windows.Forms.TextBox();
			this.panelRunInfo = new System.Windows.Forms.Panel();
			this.textTarget = new System.Windows.Forms.TextBox();
			this.textBuildFile = new System.Windows.Forms.TextBox();
			this.textProperty_InWebAppTemplate = new System.Windows.Forms.TextBox();
			this.textLogFile = new System.Windows.Forms.TextBox();
			this.labelProperty_InBranch = new System.Windows.Forms.Label();
			this.labelProperty_InWebAppTemplate = new System.Windows.Forms.Label();
			this.labelBuildFile = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboProperty_InBranch = new System.Windows.Forms.ComboBox();
			this.Property_InRevision = new System.Windows.Forms.Label();
			this.textProperty_InRevision = new System.Windows.Forms.TextBox();
			this.splitContainerOutput.Panel1.SuspendLayout();
			this.splitContainerOutput.Panel2.SuspendLayout();
			this.splitContainerOutput.SuspendLayout();
			this.panelRunInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// textCommandLine
			// 
			this.textCommandLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textCommandLine.Location = new System.Drawing.Point(2, 148);
			this.textCommandLine.Name = "textCommandLine";
			this.textCommandLine.ReadOnly = true;
			this.textCommandLine.Size = new System.Drawing.Size(662, 20);
			this.textCommandLine.TabIndex = 0;
			// 
			// buttonRun
			// 
			this.buttonRun.Location = new System.Drawing.Point(12, 13);
			this.buttonRun.Name = "buttonRun";
			this.buttonRun.Size = new System.Drawing.Size(74, 23);
			this.buttonRun.TabIndex = 1;
			this.buttonRun.Text = "Run";
			this.buttonRun.UseVisualStyleBackColor = true;
			this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
			// 
			// splitContainerOutput
			// 
			this.splitContainerOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerOutput.Location = new System.Drawing.Point(0, 29);
			this.splitContainerOutput.Name = "splitContainerOutput";
			// 
			// splitContainerOutput.Panel1
			// 
			this.splitContainerOutput.Panel1.Controls.Add(this.textStandardOutput);
			// 
			// splitContainerOutput.Panel2
			// 
			this.splitContainerOutput.Panel2.Controls.Add(this.textStandardError);
			this.splitContainerOutput.Size = new System.Drawing.Size(665, 411);
			this.splitContainerOutput.SplitterDistance = 428;
			this.splitContainerOutput.TabIndex = 3;
			// 
			// textStandardOutput
			// 
			this.textStandardOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textStandardOutput.Location = new System.Drawing.Point(0, 0);
			this.textStandardOutput.Multiline = true;
			this.textStandardOutput.Name = "textStandardOutput";
			this.textStandardOutput.ReadOnly = true;
			this.textStandardOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textStandardOutput.Size = new System.Drawing.Size(428, 411);
			this.textStandardOutput.TabIndex = 2;
			// 
			// textStandardError
			// 
			this.textStandardError.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textStandardError.Location = new System.Drawing.Point(0, 0);
			this.textStandardError.Multiline = true;
			this.textStandardError.Name = "textStandardError";
			this.textStandardError.ReadOnly = true;
			this.textStandardError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textStandardError.Size = new System.Drawing.Size(233, 411);
			this.textStandardError.TabIndex = 3;
			// 
			// panelRunInfo
			// 
			this.panelRunInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelRunInfo.Controls.Add(this.splitContainerOutput);
			this.panelRunInfo.Location = new System.Drawing.Point(2, 174);
			this.panelRunInfo.Name = "panelRunInfo";
			this.panelRunInfo.Size = new System.Drawing.Size(665, 443);
			this.panelRunInfo.TabIndex = 4;
			// 
			// textTarget
			// 
			this.textTarget.Location = new System.Drawing.Point(92, 92);
			this.textTarget.Multiline = true;
			this.textTarget.Name = "textTarget";
			this.textTarget.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.textTarget.Size = new System.Drawing.Size(570, 50);
			this.textTarget.TabIndex = 5;
			this.textTarget.Text = resources.GetString("textTarget.Text");
			this.textTarget.TextChanged += new System.EventHandler(this.text_TextChanged);
			// 
			// textBuildFile
			// 
			this.textBuildFile.Location = new System.Drawing.Point(92, 40);
			this.textBuildFile.Name = "textBuildFile";
			this.textBuildFile.Size = new System.Drawing.Size(144, 20);
			this.textBuildFile.TabIndex = 6;
			this.textBuildFile.Text = "nant.build";
			this.textBuildFile.TextChanged += new System.EventHandler(this.text_TextChanged);
			// 
			// textProperty_InWebAppTemplate
			// 
			this.textProperty_InWebAppTemplate.Location = new System.Drawing.Point(413, 65);
			this.textProperty_InWebAppTemplate.Name = "textProperty_InWebAppTemplate";
			this.textProperty_InWebAppTemplate.Size = new System.Drawing.Size(249, 20);
			this.textProperty_InWebAppTemplate.TabIndex = 8;
			this.textProperty_InWebAppTemplate.Text = "Rainbow2.0-Trunk";
			this.textProperty_InWebAppTemplate.TextChanged += new System.EventHandler(this.text_TextChanged);
			// 
			// textLogFile
			// 
			this.textLogFile.Location = new System.Drawing.Point(92, 66);
			this.textLogFile.Name = "textLogFile";
			this.textLogFile.Size = new System.Drawing.Size(144, 20);
			this.textLogFile.TabIndex = 9;
			this.textLogFile.Text = ".build.branch.nant.log";
			this.textLogFile.TextChanged += new System.EventHandler(this.text_TextChanged);
			// 
			// labelProperty_InBranch
			// 
			this.labelProperty_InBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelProperty_InBranch.Location = new System.Drawing.Point(260, 47);
			this.labelProperty_InBranch.Name = "labelProperty_InBranch";
			this.labelProperty_InBranch.Size = new System.Drawing.Size(86, 13);
			this.labelProperty_InBranch.TabIndex = 10;
			this.labelProperty_InBranch.Text = "-D:in.branch=";
			this.labelProperty_InBranch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProperty_InWebAppTemplate
			// 
			this.labelProperty_InWebAppTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelProperty_InWebAppTemplate.Location = new System.Drawing.Point(260, 69);
			this.labelProperty_InWebAppTemplate.Name = "labelProperty_InWebAppTemplate";
			this.labelProperty_InWebAppTemplate.Size = new System.Drawing.Size(147, 16);
			this.labelProperty_InWebAppTemplate.TabIndex = 10;
			this.labelProperty_InWebAppTemplate.Text = "-D:in.webAppTemplate=";
			this.labelProperty_InWebAppTemplate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelBuildFile
			// 
			this.labelBuildFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelBuildFile.Location = new System.Drawing.Point(12, 40);
			this.labelBuildFile.Name = "labelBuildFile";
			this.labelBuildFile.Size = new System.Drawing.Size(74, 20);
			this.labelBuildFile.TabIndex = 11;
			this.labelBuildFile.Text = " -buildfile:";
			this.labelBuildFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(12, 65);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 20);
			this.label1.TabIndex = 11;
			this.label1.Text = "-logfile:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProperty_InBranch
			// 
			this.comboProperty_InBranch.FormattingEnabled = true;
			this.comboProperty_InBranch.Items.AddRange(new object[] {
            "RainbowDotNet2/devint/trunk/",
            "sandboxes/MGF/trunk/",
            "sandboxes/moudrick/branches/devint_trunk_620/"});
			this.comboProperty_InBranch.Location = new System.Drawing.Point(344, 39);
			this.comboProperty_InBranch.Name = "comboProperty_InBranch";
			this.comboProperty_InBranch.Size = new System.Drawing.Size(313, 21);
			this.comboProperty_InBranch.TabIndex = 12;
			this.comboProperty_InBranch.Text = "RainbowDotNet2/devint/trunk/";
			this.comboProperty_InBranch.TextChanged += new System.EventHandler(this.text_TextChanged);
			// 
			// Property_InRevision
			// 
			this.Property_InRevision.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Property_InRevision.Location = new System.Drawing.Point(89, 16);
			this.Property_InRevision.Name = "Property_InRevision";
			this.Property_InRevision.Size = new System.Drawing.Size(90, 16);
			this.Property_InRevision.TabIndex = 14;
			this.Property_InRevision.Text = "-D:in.revision=";
			this.Property_InRevision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textProperty_InRevision
			// 
			this.textProperty_InRevision.Location = new System.Drawing.Point(185, 12);
			this.textProperty_InRevision.Name = "textProperty_InRevision";
			this.textProperty_InRevision.Size = new System.Drawing.Size(51, 20);
			this.textProperty_InRevision.TabIndex = 13;
			this.textProperty_InRevision.Text = "HEAD";
			this.textProperty_InRevision.TextChanged += new System.EventHandler(this.text_TextChanged);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(669, 617);
			this.Controls.Add(this.Property_InRevision);
			this.Controls.Add(this.textProperty_InRevision);
			this.Controls.Add(this.comboProperty_InBranch);
			this.Controls.Add(this.textCommandLine);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelBuildFile);
			this.Controls.Add(this.labelProperty_InWebAppTemplate);
			this.Controls.Add(this.labelProperty_InBranch);
			this.Controls.Add(this.textLogFile);
			this.Controls.Add(this.textProperty_InWebAppTemplate);
			this.Controls.Add(this.textBuildFile);
			this.Controls.Add(this.textTarget);
			this.Controls.Add(this.panelRunInfo);
			this.Controls.Add(this.buttonRun);
			this.Name = "Main";
			this.Text = "Rainbow Simple Nant Runner";
			this.Load += new System.EventHandler(this.Main_Load);
			this.splitContainerOutput.Panel1.ResumeLayout(false);
			this.splitContainerOutput.Panel1.PerformLayout();
			this.splitContainerOutput.Panel2.ResumeLayout(false);
			this.splitContainerOutput.Panel2.PerformLayout();
			this.splitContainerOutput.ResumeLayout(false);
			this.panelRunInfo.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textCommandLine;
		private System.Windows.Forms.Button buttonRun;
		private System.Windows.Forms.SplitContainer splitContainerOutput;
		private System.Windows.Forms.TextBox textStandardOutput;
		private System.Windows.Forms.TextBox textStandardError;
		private System.Windows.Forms.Panel panelRunInfo;
		private System.Windows.Forms.TextBox textTarget;
        private System.Windows.Forms.TextBox textBuildFile;
		private System.Windows.Forms.TextBox textProperty_InWebAppTemplate;
		private System.Windows.Forms.TextBox textLogFile;
		private System.Windows.Forms.Label labelProperty_InBranch;
		private System.Windows.Forms.Label labelProperty_InWebAppTemplate;
		private System.Windows.Forms.Label labelBuildFile;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboProperty_InBranch;
		private System.Windows.Forms.Label Property_InRevision;
		private System.Windows.Forms.TextBox textProperty_InRevision;
    }
}

