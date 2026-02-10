using SmartMedia.Controls;

namespace SmartMedia.Modules.VideoManageModule
{
    partial class ModuleMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            删除ToolStripMenuItem = new ToolStripMenuItem();
            menuContext = new ContextMenuStrip(components);
            发布ToolStripMenuItem = new ToolStripMenuItem();
            设为已发布ToolStripMenuItem = new ToolStripMenuItem();
            重置为未发布ToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new XsToolBar();
            btnOpenClassManager = new ToolStripButton();
            btnImport = new ToolStripButton();
            btnDelSels = new ToolStripButton();
            toolStripLabel1 = new ToolStripLabel();
            txtSearchKey = new ToolStripTextBox();
            cbStatus = new ToolStripComboBox();
            cbClass = new ToolStripComboBox();
            btnSearch = new ToolStripButton();
            toolStripMenuItem1 = new ToolStripMenuItem();
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolStripMenuItem2 = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lbStatusBar = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            lvData = new XS.WinFormsTools.XsListView.XsListViewBox();
            splitContainer2 = new SplitContainer();
            lbTitle = new Label();
            picContentSmallImg = new XS.WinFormsTools.Controls.PictureBox();
            splitContainer3 = new SplitContainer();
            txtDes = new XS.WinFormsTools.XsRichTextBox();
            txtPushLogs = new XS.WinFormsTools.XsRichTextBox();
            panel1 = new Panel();
            label1 = new Label();
            toolStrip2 = new XsToolBar();
            btnJobImg = new ToolStripButton();
            lbContentName = new ToolStripLabel();
            menuContext.SuspendLayout();
            toolStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picContentSmallImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            panel1.SuspendLayout();
            toolStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // 删除ToolStripMenuItem
            // 
            删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            删除ToolStripMenuItem.Size = new Size(136, 22);
            删除ToolStripMenuItem.Text = "删除";
            删除ToolStripMenuItem.Click += 删除ToolStripMenuItem_Click;
            // 
            // menuContext
            // 
            menuContext.Items.AddRange(new ToolStripItem[] { 发布ToolStripMenuItem, 删除ToolStripMenuItem, 设为已发布ToolStripMenuItem, 重置为未发布ToolStripMenuItem });
            menuContext.Name = "menuContext";
            menuContext.Size = new Size(137, 92);
            // 
            // 发布ToolStripMenuItem
            // 
            发布ToolStripMenuItem.Name = "发布ToolStripMenuItem";
            发布ToolStripMenuItem.Size = new Size(136, 22);
            发布ToolStripMenuItem.Text = "发布";
            发布ToolStripMenuItem.Click += 发布ToolStripMenuItem_Click;
            // 
            // 设为已发布ToolStripMenuItem
            // 
            设为已发布ToolStripMenuItem.Name = "设为已发布ToolStripMenuItem";
            设为已发布ToolStripMenuItem.Size = new Size(136, 22);
            设为已发布ToolStripMenuItem.Text = "设为已发布";
            设为已发布ToolStripMenuItem.Click += 设为已发布ToolStripMenuItem_Click;
            // 
            // 重置为未发布ToolStripMenuItem
            // 
            重置为未发布ToolStripMenuItem.Name = "重置为未发布ToolStripMenuItem";
            重置为未发布ToolStripMenuItem.Size = new Size(136, 22);
            重置为未发布ToolStripMenuItem.Text = "设为未发布";
            重置为未发布ToolStripMenuItem.Click += 重置为未发布ToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.GripMargin = new Padding(3);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnOpenClassManager, btnImport, btnDelSels, toolStripLabel1, txtSearchKey, cbStatus, cbClass, btnSearch });
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(990, 39);
            toolStrip1.TabIndex = 6;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnOpenClassManager
            // 
            btnOpenClassManager.Alignment = ToolStripItemAlignment.Right;
            btnOpenClassManager.Image = Resource.doc;
            btnOpenClassManager.ImageTransparentColor = Color.Magenta;
            btnOpenClassManager.Name = "btnOpenClassManager";
            btnOpenClassManager.Size = new Size(76, 36);
            btnOpenClassManager.Text = "管理分类";
            btnOpenClassManager.Click += btnOpenClassManager_Click;
            // 
            // btnImport
            // 
            btnImport.Alignment = ToolStripItemAlignment.Right;
            btnImport.Image = Resource.putin;
            btnImport.ImageScaling = ToolStripItemImageScaling.None;
            btnImport.ImageTransparentColor = Color.Magenta;
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(92, 36);
            btnImport.Text = "批量导入";
            btnImport.Click += btnImport_Click;
            // 
            // btnDelSels
            // 
            btnDelSels.Alignment = ToolStripItemAlignment.Right;
            btnDelSels.Image = Resource.deldoc;
            btnDelSels.ImageScaling = ToolStripItemImageScaling.None;
            btnDelSels.ImageTransparentColor = Color.Magenta;
            btnDelSels.Name = "btnDelSels";
            btnDelSels.Size = new Size(92, 36);
            btnDelSels.Text = "删除所选";
            btnDelSels.Click += btnDelSels_Click;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.BackColor = Color.Transparent;
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(44, 36);
            toolStripLabel1.Text = "关键字";
            // 
            // txtSearchKey
            // 
            txtSearchKey.BackColor = Color.White;
            txtSearchKey.BorderStyle = BorderStyle.None;
            txtSearchKey.Font = new Font("Microsoft YaHei UI", 14F);
            txtSearchKey.Name = "txtSearchKey";
            txtSearchKey.Size = new Size(150, 39);
            // 
            // cbStatus
            // 
            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStatus.Items.AddRange(new object[] { "所有状态", "待发布", "已发布" });
            cbStatus.Name = "cbStatus";
            cbStatus.Size = new Size(121, 39);
            // 
            // cbClass
            // 
            cbClass.DropDownStyle = ComboBoxStyle.DropDownList;
            cbClass.Items.AddRange(new object[] { "所有状态", "待发布", "已发布" });
            cbClass.Name = "cbClass";
            cbClass.Size = new Size(121, 39);
            // 
            // btnSearch
            // 
            btnSearch.Image = Resource.search2;
            btnSearch.ImageScaling = ToolStripItemImageScaling.None;
            btnSearch.ImageTransparentColor = Color.Magenta;
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(68, 36);
            btnSearch.Text = "搜索";
            btnSearch.Click += btnSearch_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(160, 22);
            toolStripMenuItem1.Text = "在百度中搜索";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem2, toolStripMenuItem1 });
            contextMenuStrip1.Name = "menuContext";
            contextMenuStrip1.Size = new Size(161, 48);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(160, 22);
            toolStripMenuItem2.Text = "查看发布文章页";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lbStatusBar });
            statusStrip1.Location = new Point(0, 536);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(990, 22);
            statusStrip1.TabIndex = 8;
            statusStrip1.Text = "statusStrip1";
            // 
            // lbStatusBar
            // 
            lbStatusBar.Name = "lbStatusBar";
            lbStatusBar.Size = new Size(32, 17);
            lbStatusBar.Text = "状态";
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
            splitContainer1.Size = new Size(990, 497);
            splitContainer1.SplitterDistance = 769;
            splitContainer1.TabIndex = 9;
            // 
            // lvData
            // 
            lvData.BorderStyle = BorderStyle.None;
            lvData.Dock = DockStyle.Fill;
            lvData.Font = new Font("Microsoft YaHei UI", 9F);
            lvData.FullRowSelect = true;
            lvData.GridLines = true;
            lvData.HeaderBackgroundColor = Color.AliceBlue;
            lvData.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvData.Location = new Point(0, 0);
            lvData.Name = "lvData";
            lvData.OwnerDraw = true;
            lvData.Size = new Size(769, 497);
            lvData.TabIndex = 3;
            lvData.UseCompatibleStateImageBehavior = false;
            lvData.View = View.Details;
            lvData.SelectedIndexChanged += lvData_SelectedIndexChanged;
            lvData.MouseDoubleClick += lvData_MouseDoubleClick;
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
            splitContainer2.Panel2.Controls.Add(splitContainer3);
            splitContainer2.Panel2.Padding = new Padding(5);
            splitContainer2.Size = new Size(217, 472);
            splitContainer2.SplitterDistance = 163;
            splitContainer2.TabIndex = 70;
            // 
            // lbTitle
            // 
            lbTitle.AutoSize = true;
            lbTitle.Location = new Point(15, 135);
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
            picContentSmallImg.Location = new Point(15, 14);
            picContentSmallImg.Name = "picContentSmallImg";
            picContentSmallImg.ShowLoadingIndicator = true;
            picContentSmallImg.Size = new Size(173, 108);
            picContentSmallImg.TabIndex = 68;
            picContentSmallImg.TabStop = false;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(5, 5);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(txtDes);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(txtPushLogs);
            splitContainer3.Panel2.Controls.Add(panel1);
            splitContainer3.Size = new Size(207, 295);
            splitContainer3.SplitterDistance = 147;
            splitContainer3.TabIndex = 1;
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
            txtDes.Location = new Point(0, 0);
            txtDes.Margin = new Padding(4);
            txtDes.Name = "txtDes";
            txtDes.Padding = new Padding(7);
            txtDes.ReadOnly = true;
            txtDes.Size = new Size(207, 147);
            txtDes.TabIndex = 0;
            txtDes.Texts = "";
            txtDes.UnderlinedStyle = false;
            // 
            // txtPushLogs
            // 
            txtPushLogs.BackColor = SystemColors.Window;
            txtPushLogs.BorderColor = Color.Gainsboro;
            txtPushLogs.BorderFocusColor = Color.DodgerBlue;
            txtPushLogs.BorderSize = 2;
            txtPushLogs.ConerRadius = 2;
            txtPushLogs.Dock = DockStyle.Fill;
            txtPushLogs.Font = new Font("Microsoft Sans Serif", 9.5F);
            txtPushLogs.ForeColor = Color.DimGray;
            txtPushLogs.Location = new Point(0, 32);
            txtPushLogs.Margin = new Padding(4);
            txtPushLogs.Name = "txtPushLogs";
            txtPushLogs.Padding = new Padding(7);
            txtPushLogs.ReadOnly = true;
            txtPushLogs.Size = new Size(207, 112);
            txtPushLogs.TabIndex = 1;
            txtPushLogs.Texts = "";
            txtPushLogs.UnderlinedStyle = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(207, 32);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 7);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 0;
            label1.Text = "发布日志";
            // 
            // toolStrip2
            // 
            toolStrip2.GripMargin = new Padding(3);
            toolStrip2.Items.AddRange(new ToolStripItem[] { btnJobImg, lbContentName });
            toolStrip2.Location = new Point(0, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.RenderMode = ToolStripRenderMode.System;
            toolStrip2.Size = new Size(217, 25);
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
            // ModuleMain
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(990, 558);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Name = "ModuleMain";
            Text = "管理所有发布内容";
            menuContext.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
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
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStripMenuItem 删除ToolStripMenuItem;
        private ContextMenuStrip menuContext;
        private ToolStripMenuItem 发布ToolStripMenuItem;
        private ToolStripButton btnDelSels;
        private ToolStripTextBox txtSearchKey;
        private ToolStripComboBox cbStatus;
        private ToolStripButton btnSearch;
        private ToolStripMenuItem toolStripMenuItem1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lbStatusBar;
        private XsToolBar toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripMenuItem 重置为未发布ToolStripMenuItem;
        private SplitContainer splitContainer1;
        private XS.WinFormsTools.XsListView.XsListViewBox lvData;
        private SplitContainer splitContainer2;
        private Label lbTitle;
        private XS.WinFormsTools.Controls.PictureBox picContentSmallImg;
        private XS.WinFormsTools.XsRichTextBox txtDes;
        private XsToolBar toolStrip2;
        private ToolStripButton btnJobImg;
        private ToolStripLabel lbContentName;
        private SplitContainer splitContainer3;
        private XS.WinFormsTools.XsRichTextBox txtPushLogs;
        private Panel panel1;
        private Label label1;
        private ToolStripMenuItem 设为已发布ToolStripMenuItem;
        private ToolStripButton btnImport;
        private ToolStripButton btnOpenClassManager;
        private ToolStripComboBox cbClass;
    }
}