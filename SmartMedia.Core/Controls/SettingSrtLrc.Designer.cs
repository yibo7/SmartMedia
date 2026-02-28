namespace SmartMedia.Core.Controls;

partial class SettingSrtLrc
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
        txtFilePath = new XS.WinFormsTools.XsTextBox();
        btnSel = new XS.WinFormsTools.XsButton();
        lbTip = new Label();
        SuspendLayout();
        // 
        // lbTitle
        // 
        lbTitle.AutoSize = true;
        lbTitle.ForeColor = SystemColors.ControlDark;
        lbTitle.Location = new Point(6, 4);
        lbTitle.Name = "lbTitle";
        lbTitle.Size = new Size(56, 17);
        lbTitle.TabIndex = 69;
        lbTitle.Text = "字幕文件";
        // 
        // txtFilePath
        // 
        txtFilePath.BackColor = SystemColors.Window;
        txtFilePath.BorderColor = Color.Gainsboro;
        txtFilePath.BorderFocusColor = Color.DodgerBlue;
        txtFilePath.BorderSize = 2;
        txtFilePath.ConerRadius = 2;
        txtFilePath.Font = new Font("Microsoft Sans Serif", 9.5F);
        txtFilePath.ForeColor = Color.DimGray;
        txtFilePath.Location = new Point(6, 25);
        txtFilePath.Margin = new Padding(4);
        txtFilePath.Multiline = false;
        txtFilePath.Name = "txtFilePath";
        txtFilePath.Padding = new Padding(7);
        txtFilePath.PasswordChar = false;
        txtFilePath.PlaceholderText = "";
        txtFilePath.ScrollBar = ScrollBars.None;
        txtFilePath.Size = new Size(305, 31);
        txtFilePath.TabIndex = 70;
        txtFilePath.Texts = "";
        txtFilePath.UnderlinedStyle = false;
        // 
        // btnSel
        // 
        btnSel.BackColor = Color.DeepSkyBlue;
        btnSel.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnSel.ConerRadius = 5;
        btnSel.Font = new Font("Segoe UI", 9F);
        btnSel.Location = new Point(318, 25);
        btnSel.Name = "btnSel";
        btnSel.Size = new Size(75, 31);
        btnSel.TabIndex = 71;
        btnSel.Text = "选择字幕";
        btnSel.TextColor = Color.White;
        btnSel.Click += btnSel_Click;
        // 
        // lbTip
        // 
        lbTip.ForeColor = SystemColors.ControlDark;
        lbTip.Location = new Point(7, 62);
        lbTip.Name = "lbTip";
        lbTip.Size = new Size(387, 27);
        lbTip.TabIndex = 73;
        // 
        // SettingSrtLrc
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lbTip);
        Controls.Add(btnSel);
        Controls.Add(txtFilePath);
        Controls.Add(lbTitle);
        Name = "SettingSrtLrc";
        Size = new Size(418, 99);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lbTitle;
    private XS.WinFormsTools.XsTextBox txtFilePath;
    private XS.WinFormsTools.XsButton btnSel;
    private Label lbTip;
}
