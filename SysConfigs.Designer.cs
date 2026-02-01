namespace SmartMedia
{
    partial class SysConfigs
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
            cbIsOpenBrowser = new XS.WinFormsTools.Controls.CheckBox();
            cbIsCopyFiles = new XS.WinFormsTools.Controls.CheckBox();
            btnSaveConfigs = new XS.WinFormsTools.XsButton();
            SuspendLayout();
            // 
            // cbIsOpenBrowser
            // 
            cbIsOpenBrowser.AutoSize = true;
            cbIsOpenBrowser.Font = new Font("Segoe UI", 12F);
            cbIsOpenBrowser.Location = new Point(29, 37);
            cbIsOpenBrowser.Name = "cbIsOpenBrowser";
            cbIsOpenBrowser.Size = new Size(185, 20);
            cbIsOpenBrowser.TabIndex = 0;
            cbIsOpenBrowser.Text = "发布是否打开浏览器";
            cbIsOpenBrowser.UseVisualStyleBackColor = true;
            // 
            // cbIsCopyFiles
            // 
            cbIsCopyFiles.AutoSize = true;
            cbIsCopyFiles.Font = new Font("Segoe UI", 12F);
            cbIsCopyFiles.Location = new Point(29, 78);
            cbIsCopyFiles.Name = "cbIsCopyFiles";
            cbIsCopyFiles.Size = new Size(185, 20);
            cbIsCopyFiles.TabIndex = 1;
            cbIsCopyFiles.Text = "是否复制文件到本地";
            cbIsCopyFiles.UseVisualStyleBackColor = true;
            // 
            // btnSaveConfigs
            // 
            btnSaveConfigs.BackColor = Color.DeepSkyBlue;
            btnSaveConfigs.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
            btnSaveConfigs.ConerRadius = 5;
            btnSaveConfigs.Font = new Font("Segoe UI", 9F);
            btnSaveConfigs.Location = new Point(124, 147);
            btnSaveConfigs.Name = "btnSaveConfigs";
            btnSaveConfigs.Size = new Size(92, 33);
            btnSaveConfigs.TabIndex = 2;
            btnSaveConfigs.Text = "保存配置";
            btnSaveConfigs.TextColor = Color.White;
            btnSaveConfigs.Click += btnSaveConfigs_Click;
            // 
            // SysConfigs
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(344, 201);
            Controls.Add(btnSaveConfigs);
            Controls.Add(cbIsCopyFiles);
            Controls.Add(cbIsOpenBrowser);
            Name = "SysConfigs";
            Text = "系统设置";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private XS.WinFormsTools.Controls.CheckBox cbIsOpenBrowser;
        private XS.WinFormsTools.Controls.CheckBox cbIsCopyFiles;
        private XS.WinFormsTools.XsButton btnSaveConfigs;
    }
}