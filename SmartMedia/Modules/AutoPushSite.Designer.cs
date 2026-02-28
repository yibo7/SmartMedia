using SmartMedia.Controls;
using XS.WinFormsTools.XsListView;

namespace SmartMedia.Modules
{
    partial class AutoPushSite
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
            toolMainBar = new XsToolBar();
            btnLogin = new ToolStripButton();
            webBox = new Microsoft.Web.WebView2.WinForms.WebView2();
            btnShowHelp = new ToolStripButton();
            toolMainBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webBox).BeginInit();
            SuspendLayout();
            // 
            // toolMainBar
            // 
            toolMainBar.GripMargin = new Padding(3);
            toolMainBar.Items.AddRange(new ToolStripItem[] { btnShowHelp, btnLogin });
            toolMainBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolMainBar.Location = new Point(0, 0);
            toolMainBar.Name = "toolMainBar";
            toolMainBar.Size = new Size(951, 39);
            toolMainBar.TabIndex = 2;
            toolMainBar.Text = "toolStrip1";
            // 
            // btnLogin
            // 
            btnLogin.Alignment = ToolStripItemAlignment.Right;
            btnLogin.Image = Resource.user;
            btnLogin.ImageScaling = ToolStripItemImageScaling.None;
            btnLogin.ImageTransparentColor = Color.Magenta;
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(92, 36);
            btnLogin.Text = "登录网站";
            btnLogin.Click += btnLogin_Click;
            // 
            // webBox
            // 
            webBox.AllowExternalDrop = true;
            webBox.CreationProperties = null;
            webBox.DefaultBackgroundColor = Color.White;
            webBox.Dock = DockStyle.Fill;
            webBox.Location = new Point(0, 39);
            webBox.Name = "webBox";
            webBox.Size = new Size(951, 503);
            webBox.TabIndex = 3;
            webBox.ZoomFactor = 1D;
            // 
            // btnShowHelp
            // 
            btnShowHelp.Alignment = ToolStripItemAlignment.Right;
            btnShowHelp.Image = Resource.help;
            btnShowHelp.ImageScaling = ToolStripItemImageScaling.None;
            btnShowHelp.ImageTransparentColor = Color.Magenta;
            btnShowHelp.Name = "btnShowHelp";
            btnShowHelp.Size = new Size(92, 36);
            btnShowHelp.Text = "使用帮助";
            btnShowHelp.Click += btnShowReport_Click;
            // 
            // AutoPushSite
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(951, 542);
            Controls.Add(webBox);
            Controls.Add(toolMainBar);
            Name = "AutoPushSite";
            Text = "TestModule";
            toolMainBar.ResumeLayout(false);
            toolMainBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private XsToolBar toolMainBar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webBox;
        private ToolStripButton btnLogin;
        private ToolStripButton btnShowHelp;
    }
}