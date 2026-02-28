namespace SmartMedia
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            flowLayoutPanel1 = new FlowLayoutPanel();
            cxfTitleBar = new XS.WinFormsTools.HeaderBar();
            toolMainBar = new ToolStrip();
            btnConfigs = new ToolStripButton();
            btnOpenImagePost = new ToolStripButton();
            btnOpenArticleList = new ToolStripButton();
            btnAudioPush = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btnOpenVideoList = new ToolStripButton();
            btnJobs = new ToolStripButton();
            dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            toolMainBar.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.Location = new Point(5, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(970, 40);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // cxfTitleBar
            // 
            cxfTitleBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cxfTitleBar.Font = new Font("Segoe UI", 12F);
            cxfTitleBar.Location = new Point(0, 0);
            cxfTitleBar.Margin = new Padding(0);
            cxfTitleBar.Name = "cxfTitleBar";
            cxfTitleBar.Size = new Size(980, 40);
            cxfTitleBar.TabIndex = 3;
            cxfTitleBar.Text = "新媒助手 1.0";
            cxfTitleBar.ThemeColor = Color.DarkSlateGray;
            // 
            // toolMainBar
            // 
            toolMainBar.Items.AddRange(new ToolStripItem[] { btnConfigs, btnOpenImagePost, btnOpenArticleList, btnAudioPush, toolStripSeparator1, btnOpenVideoList, btnJobs });
            toolMainBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolMainBar.Location = new Point(5, 40);
            toolMainBar.Name = "toolMainBar";
            toolMainBar.Size = new Size(970, 56);
            toolMainBar.TabIndex = 5;
            toolMainBar.Text = "toolStrip1";
            // 
            // btnConfigs
            // 
            btnConfigs.Alignment = ToolStripItemAlignment.Right;
            btnConfigs.Image = Resource.config;
            btnConfigs.ImageScaling = ToolStripItemImageScaling.None;
            btnConfigs.ImageTransparentColor = Color.Magenta;
            btnConfigs.Name = "btnConfigs";
            btnConfigs.Size = new Size(60, 53);
            btnConfigs.Text = "系统设置";
            btnConfigs.TextImageRelation = TextImageRelation.ImageAboveText;
            btnConfigs.Click += btnConfigs_Click;
            // 
            // btnOpenImagePost
            // 
            btnOpenImagePost.Image = Resource.selimg;
            btnOpenImagePost.ImageScaling = ToolStripItemImageScaling.None;
            btnOpenImagePost.ImageTransparentColor = Color.Magenta;
            btnOpenImagePost.Name = "btnOpenImagePost";
            btnOpenImagePost.Size = new Size(60, 53);
            btnOpenImagePost.Text = "图文分发";
            btnOpenImagePost.TextImageRelation = TextImageRelation.ImageAboveText;
            btnOpenImagePost.Click += btnOpenImagePost_Click;
            // 
            // btnOpenArticleList
            // 
            btnOpenArticleList.Image = Resource.doc;
            btnOpenArticleList.ImageScaling = ToolStripItemImageScaling.None;
            btnOpenArticleList.ImageTransparentColor = Color.Magenta;
            btnOpenArticleList.Name = "btnOpenArticleList";
            btnOpenArticleList.Size = new Size(60, 53);
            btnOpenArticleList.Text = "文章分发";
            btnOpenArticleList.TextImageRelation = TextImageRelation.ImageAboveText;
            btnOpenArticleList.ToolTipText = "文章分发";
            btnOpenArticleList.Click += btnOpenArticleList_Click;
            // 
            // btnAudioPush
            // 
            btnAudioPush.Image = Resource.audio2;
            btnAudioPush.ImageScaling = ToolStripItemImageScaling.None;
            btnAudioPush.ImageTransparentColor = Color.Magenta;
            btnAudioPush.Name = "btnAudioPush";
            btnAudioPush.Size = new Size(60, 53);
            btnAudioPush.Text = "音频分发";
            btnAudioPush.TextImageRelation = TextImageRelation.ImageAboveText;
            btnAudioPush.ToolTipText = "扩展模块";
            btnAudioPush.Click += btnAudioPush_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 56);
            // 
            // btnOpenVideoList
            // 
            btnOpenVideoList.Image = Resource.video2;
            btnOpenVideoList.ImageScaling = ToolStripItemImageScaling.None;
            btnOpenVideoList.ImageTransparentColor = Color.Magenta;
            btnOpenVideoList.Name = "btnOpenVideoList";
            btnOpenVideoList.Size = new Size(60, 53);
            btnOpenVideoList.Text = "视频分发";
            btnOpenVideoList.TextImageRelation = TextImageRelation.ImageAboveText;
            btnOpenVideoList.Click += btnOpenVideoList_Click;
            // 
            // btnJobs
            // 
            btnJobs.Image = Resource.jobs;
            btnJobs.ImageScaling = ToolStripItemImageScaling.None;
            btnJobs.ImageTransparentColor = Color.Magenta;
            btnJobs.Name = "btnJobs";
            btnJobs.Size = new Size(60, 53);
            btnJobs.Text = "定时任务";
            btnJobs.TextImageRelation = TextImageRelation.ImageAboveText;
            btnJobs.ToolTipText = "机 器 人";
            btnJobs.Click += btnJobs_Click;
            // 
            // dockPanel
            // 
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.Location = new Point(5, 96);
            dockPanel.Name = "dockPanel";
            dockPanel.Size = new Size(970, 508);
            dockPanel.TabIndex = 6;
            // 
            // Main
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(980, 609);
            Controls.Add(dockPanel);
            Controls.Add(toolMainBar);
            Controls.Add(cxfTitleBar);
            Controls.Add(flowLayoutPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            MaximumSize = new Size(1920, 1040);
            Name = "Main";
            Padding = new Padding(5, 0, 5, 5);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "新媒助手 1.0";
            toolMainBar.ResumeLayout(false);
            toolMainBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1; 
        private ToolStrip toolMainBar;
        private ToolStripButton btnConfigs;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private ToolStripButton btnOpenVideoList;
        private ToolStripButton btnOpenArticleList;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnJobs;
        private XS.WinFormsTools.HeaderBar cxfTitleBar;
        private ToolStripButton btnAudioPush;
        private ToolStripButton btnOpenImagePost;
    }
}