using SmartMedia.Controls;
using XS.WinFormsTools.XsListView;

namespace SmartMedia.Modules
{
    partial class AutoPushFromApi
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
            components = new System.ComponentModel.Container();
            toolMainBar = new XsToolBar();
            btnRefesh = new ToolStripButton();
            btnShowHelp = new ToolStripButton();
            btnShowUserInfo = new ToolStripButton();
            btnUpdateClassData = new ToolStripButton();
            btnLogin = new ToolStripButton();
            btnDelData = new ToolStripButton();
            lvData = new XsListViewBox();
            ucSplitLine_h1 = new XS.WinFormsTools.Forms.UCSplitLine_H();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            lbTitle = new Label();
            picContentSmallImg = new XS.WinFormsTools.Controls.PictureBox();
            txtDes = new XS.WinFormsTools.XsRichTextBox();
            toolStrip2 = new XsToolBar();
            btnJobImg = new ToolStripButton();
            lbContentName = new ToolStripLabel();
            toolMainBar.SuspendLayout();
            lvData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picContentSmallImg).BeginInit();
            toolStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // toolMainBar
            // 
            toolMainBar.GripMargin = new Padding(3);
            toolMainBar.Items.AddRange(new ToolStripItem[] { btnRefesh, btnShowHelp, btnShowUserInfo, btnUpdateClassData, btnLogin, btnDelData });
            toolMainBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolMainBar.Location = new Point(0, 0);
            toolMainBar.Name = "toolMainBar";
            toolMainBar.Size = new Size(1062, 39);
            toolMainBar.TabIndex = 2;
            toolMainBar.Text = "toolStrip1";
            // 
            // btnRefesh
            // 
            btnRefesh.Image = Resource.refesh;
            btnRefesh.ImageScaling = ToolStripItemImageScaling.None;
            btnRefesh.ImageTransparentColor = Color.Magenta;
            btnRefesh.Name = "btnRefesh";
            btnRefesh.Size = new Size(68, 36);
            btnRefesh.Text = "刷新";
            btnRefesh.Click += btnRefesh_Click;
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
            btnShowHelp.Click += btnShowHelp_Click;
            // 
            // btnShowUserInfo
            // 
            btnShowUserInfo.Alignment = ToolStripItemAlignment.Right;
            btnShowUserInfo.Image = Resource.bar;
            btnShowUserInfo.ImageScaling = ToolStripItemImageScaling.None;
            btnShowUserInfo.ImageTransparentColor = Color.Magenta;
            btnShowUserInfo.Name = "btnShowUserInfo";
            btnShowUserInfo.Size = new Size(116, 36);
            btnShowUserInfo.Text = "查看统计数据";
            btnShowUserInfo.Click += btnShowUserInfo_Click;
            // 
            // btnUpdateClassData
            // 
            btnUpdateClassData.Alignment = ToolStripItemAlignment.Right;
            btnUpdateClassData.Image = Resource.adddoc;
            btnUpdateClassData.ImageScaling = ToolStripItemImageScaling.None;
            btnUpdateClassData.ImageTransparentColor = Color.Magenta;
            btnUpdateClassData.Name = "btnUpdateClassData";
            btnUpdateClassData.Size = new Size(116, 36);
            btnUpdateClassData.Text = "更新分类数据";
            btnUpdateClassData.ToolTipText = "更新分类数据";
            btnUpdateClassData.Visible = false;
            btnUpdateClassData.Click += btnUpdateClassData_Click;
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
            // btnDelData
            // 
            btnDelData.Image = Resource.deldoc;
            btnDelData.ImageScaling = ToolStripItemImageScaling.None;
            btnDelData.ImageTransparentColor = Color.Magenta;
            btnDelData.Name = "btnDelData";
            btnDelData.Size = new Size(92, 36);
            btnDelData.Text = "删除所选";
            btnDelData.Click += btnDelData_Click;
            // 
            // lvData
            // 
            lvData.BorderStyle = BorderStyle.None;
            lvData.Controls.Add(ucSplitLine_h1);
            lvData.Dock = DockStyle.Fill;
            lvData.Font = new Font("Microsoft YaHei UI", 9F);
            lvData.FullRowSelect = true;
            lvData.GridLines = true;
            lvData.HeaderBackgroundColor = Color.AliceBlue;
            lvData.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvData.Location = new Point(0, 0);
            lvData.Name = "lvData";
            lvData.OwnerDraw = true;
            lvData.Size = new Size(827, 452);
            lvData.TabIndex = 3;
            lvData.UseCompatibleStateImageBehavior = false;
            lvData.View = View.Details;
            // 
            // ucSplitLine_h1
            // 
            ucSplitLine_h1.BackColor = Color.FromArgb(232, 232, 232);
            ucSplitLine_h1.Dock = DockStyle.Top;
            ucSplitLine_h1.Location = new Point(0, 0);
            ucSplitLine_h1.Name = "ucSplitLine_h1";
            ucSplitLine_h1.Size = new Size(827, 1);
            ucSplitLine_h1.TabIndex = 1;
            ucSplitLine_h1.TabStop = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 39);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvData);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Panel2.Controls.Add(toolStrip2);
            splitContainer1.Size = new Size(1062, 452);
            splitContainer1.SplitterDistance = 827;
            splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            splitContainer2.BackColor = Color.WhiteSmoke;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
            splitContainer2.Location = new Point(0, 25);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(lbTitle);
            splitContainer2.Panel1.Controls.Add(picContentSmallImg);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(txtDes);
            splitContainer2.Panel2.Padding = new Padding(5);
            splitContainer2.Size = new Size(231, 427);
            splitContainer2.SplitterDistance = 163;
            splitContainer2.TabIndex = 70;
            // 
            // lbTitle
            // 
            lbTitle.AutoSize = true;
            lbTitle.Location = new Point(15, 133);
            lbTitle.Name = "lbTitle";
            lbTitle.Size = new Size(32, 17);
            lbTitle.TabIndex = 69;
            lbTitle.Text = "标题";
            // 
            // picContentSmallImg
            // 
            picContentSmallImg.ErrorColor = Color.FromArgb(255, 240, 240);
            picContentSmallImg.LoadedColor = Color.FromArgb(245, 245, 245);
            picContentSmallImg.LoadingColor = Color.FromArgb(240, 240, 240);
            picContentSmallImg.Location = new Point(15, 13);
            picContentSmallImg.Name = "picContentSmallImg";
            picContentSmallImg.ShowLoadingIndicator = true;
            picContentSmallImg.Size = new Size(151, 108);
            picContentSmallImg.TabIndex = 68;
            picContentSmallImg.TabStop = false;
            // 
            // txtDes
            // 
            txtDes.BackColor = SystemColors.Window;
            txtDes.BorderColor = Color.Gainsboro;
            txtDes.BorderFocusColor = Color.DodgerBlue;
            txtDes.BorderSize = 2;
            txtDes.ConerRadius = 2;
            txtDes.Dock = DockStyle.Fill;
            txtDes.Font = new Font("Microsoft Sans Serif", 9.5F);
            txtDes.ForeColor = Color.DimGray;
            txtDes.Location = new Point(5, 5);
            txtDes.Margin = new Padding(4);
            txtDes.Name = "txtDes";
            txtDes.Padding = new Padding(7);
            txtDes.ReadOnly = true;
            txtDes.Size = new Size(221, 250);
            txtDes.TabIndex = 0;
            txtDes.Texts = "";
            txtDes.UnderlinedStyle = false;
            // 
            // toolStrip2
            // 
            toolStrip2.GripMargin = new Padding(3);
            toolStrip2.Items.AddRange(new ToolStripItem[] { btnJobImg, lbContentName });
            toolStrip2.Location = new Point(0, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.RenderMode = ToolStripRenderMode.System;
            toolStrip2.Size = new Size(231, 25);
            toolStrip2.TabIndex = 18;
            toolStrip2.Text = "toolStrip2";
            // 
            // btnJobImg
            // 
            btnJobImg.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnJobImg.Image = Resource.doc;
            btnJobImg.ImageTransparentColor = Color.Magenta;
            btnJobImg.Name = "btnJobImg";
            btnJobImg.Size = new Size(23, 22);
            btnJobImg.Text = "toolStripButton1";
            // 
            // lbContentName
            // 
            lbContentName.BackColor = Color.White;
            lbContentName.Name = "lbContentName";
            lbContentName.Size = new Size(56, 22);
            lbContentName.Text = "内容详情";
            // 
            // AutoPushFromApi
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1062, 491);
            Controls.Add(splitContainer1);
            Controls.Add(toolMainBar);
            Name = "AutoPushFromApi";
            Text = "API发布内容管理";
            Load += AutoPushFromApi_Load;
            toolMainBar.ResumeLayout(false);
            toolMainBar.PerformLayout();
            lvData.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picContentSmallImg).EndInit();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private XsToolBar toolMainBar;
        private XsListViewBox lvData;
        private ToolStripButton btnPassAll;
        private SplitContainer splitContainer1;
        private XsToolBar toolStrip2;
        private ToolStripLabel lbContentName;
        private ToolStripButton btnJobImg;
        private XS.WinFormsTools.Forms.UCSplitLine_H ucSplitLine_h1;
        private ToolStripButton btnRefesh;
        private ToolStripButton btnShowHelp;
        private ToolStripButton btnLogin;
        private SplitContainer splitContainer2;
        private XS.WinFormsTools.Controls.PictureBox picContentSmallImg;
        private ToolStripButton btnUpdateClassData;
        private ToolStripButton btnShowUserInfo;
        private Label lbTitle;
        private XS.WinFormsTools.XsRichTextBox txtDes;
        private ToolStripButton btnDelData;
    }
}