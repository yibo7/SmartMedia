namespace SmartMedia.Plugins.AutoPush.Video.Youtube
{
    partial class YoutubePlaylist
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            cbCategory1 = new XS.WinFormsTools.XsComboBox();
            lbTips = new Label();
            lbTitle = new Label();
            btnUpdate = new XS.WinFormsTools.XsButton();
            SuspendLayout();
            // 
            // cbCategory1
            // 
            cbCategory1.BackColor = Color.White;
            cbCategory1.BorderColor = Color.Gainsboro;
            cbCategory1.ConerRadius = 2;
            cbCategory1.DrawMode = DrawMode.OwnerDrawFixed;
            cbCategory1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategory1.FlatStyle = FlatStyle.Flat;
            cbCategory1.Font = new Font("微软雅黑", 12F);
            cbCategory1.FormattingEnabled = true;
            cbCategory1.ItemHeight = 32;
            cbCategory1.Location = new Point(4, 27);
            cbCategory1.Name = "cbCategory1";
            cbCategory1.Size = new Size(203, 38);
            cbCategory1.TabIndex = 76;
            // 
            // lbTips
            // 
            lbTips.AutoSize = true;
            lbTips.ForeColor = SystemColors.ControlDark;
            lbTips.Location = new Point(82, 1);
            lbTips.Name = "lbTips";
            lbTips.Size = new Size(212, 17);
            lbTips.TabIndex = 75;
            lbTips.Text = "如果创建了新列表要点击【下载数据】";
            // 
            // lbTitle
            // 
            lbTitle.AutoSize = true;
            lbTitle.Location = new Point(3, 0);
            lbTitle.Name = "lbTitle";
            lbTitle.Size = new Size(80, 17);
            lbTitle.TabIndex = 74;
            lbTitle.Text = "选择播放列表";
            // 
            // btnUpdate
            // 
            btnUpdate.BackColor = Color.DeepSkyBlue;
            btnUpdate.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
            btnUpdate.ConerRadius = 5;
            btnUpdate.Font = new Font("Segoe UI", 9F);
            btnUpdate.Location = new Point(211, 28);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 33);
            btnUpdate.TabIndex = 77;
            btnUpdate.Text = "下载数据";
            btnUpdate.TextColor = Color.White;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // YoutubePlaylist
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnUpdate);
            Controls.Add(cbCategory1);
            Controls.Add(lbTips);
            Controls.Add(lbTitle);
            Name = "YoutubePlaylist";
            Size = new Size(303, 79);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private XS.WinFormsTools.XsComboBox cbCategory1;
        private Label lbTips;
        private Label lbTitle;
        private XS.WinFormsTools.XsButton btnUpdate;
    }
}
