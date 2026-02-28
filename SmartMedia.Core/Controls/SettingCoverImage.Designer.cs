namespace SmartMedia.Core.Controls;

partial class SettingCoverImage
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
        lbTitle = new Label();
        picCoverImage = new XS.WinFormsTools.Controls.PictureBox();
        linkBarDelete = new LinkLabel();
        lbTip = new Label();
        ((System.ComponentModel.ISupportInitialize)picCoverImage).BeginInit();
        SuspendLayout();
        // 
        // lbTitle
        // 
        lbTitle.AutoSize = true;
        lbTitle.ForeColor = SystemColors.ControlDark;
        lbTitle.Location = new Point(16, 4);
        lbTitle.Name = "lbTitle";
        lbTitle.Size = new Size(56, 17);
        lbTitle.TabIndex = 69;
        lbTitle.Text = "封面图片";
        // 
        // picCoverImage
        // 
        picCoverImage.ErrorColor = Color.FromArgb(255, 240, 240);
        picCoverImage.LoadedColor = Color.FromArgb(245, 245, 245);
        picCoverImage.LoadingColor = Color.FromArgb(240, 240, 240);
        picCoverImage.Location = new Point(11, 28);
        picCoverImage.Name = "picCoverImage";
        picCoverImage.ShowLoadingIndicator = true;
        picCoverImage.Size = new Size(151, 108);
        picCoverImage.TabIndex = 70;
        picCoverImage.TabStop = false;
        picCoverImage.Click += picCoverImage_Click;
        // 
        // linkBarDelete
        // 
        linkBarDelete.AutoSize = true;
        linkBarDelete.LinkColor = Color.FromArgb(192, 64, 0);
        linkBarDelete.Location = new Point(130, 4);
        linkBarDelete.Name = "linkBarDelete";
        linkBarDelete.Size = new Size(32, 17);
        linkBarDelete.TabIndex = 71;
        linkBarDelete.TabStop = true;
        linkBarDelete.Text = "删除";
        linkBarDelete.LinkClicked += linkBarDelete_LinkClicked;
        // 
        // lbTip
        // 
        lbTip.ForeColor = SystemColors.ControlDark;
        lbTip.Location = new Point(168, 28);
        lbTip.Name = "lbTip";
        lbTip.Size = new Size(193, 108);
        lbTip.TabIndex = 72;
        // 
        // SettingCoverImage
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lbTip);
        Controls.Add(linkBarDelete);
        Controls.Add(picCoverImage);
        Controls.Add(lbTitle);
        Name = "SettingCoverImage";
        Size = new Size(379, 166);
        ((System.ComponentModel.ISupportInitialize)picCoverImage).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lbTitle;
    private XS.WinFormsTools.Controls.PictureBox picCoverImage;
    private LinkLabel linkBarDelete;
    private Label lbTip;
}
