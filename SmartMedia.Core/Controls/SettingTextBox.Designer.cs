namespace SmartMedia.Core.Controls;

partial class SettingTextBox
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
        lbTips = new Label();
        lbTitle = new Label();
        txtValue = new XS.WinFormsTools.XsTextBox();
        SuspendLayout();
        // 
        // lbTips
        // 
        lbTips.AutoSize = true;
        lbTips.ForeColor = SystemColors.ControlDark;
        lbTips.Location = new Point(63, 9);
        lbTips.Name = "lbTips";
        lbTips.Size = new Size(188, 17);
        lbTips.TabIndex = 69;
        lbTips.Text = "可为空，你在此平台上创建的合集";
        // 
        // lbTitle
        // 
        lbTitle.AutoSize = true;
        lbTitle.Location = new Point(3, 9);
        lbTitle.Name = "lbTitle";
        lbTitle.Size = new Size(56, 17);
        lbTitle.TabIndex = 68;
        lbTitle.Text = "合集名称";
        // 
        // txtValue
        // 
        txtValue.BackColor = SystemColors.Window;
        txtValue.BorderColor = Color.Gainsboro;
        txtValue.BorderFocusColor = Color.DodgerBlue;
        txtValue.BorderSize = 2;
        txtValue.ConerRadius = 2;
        txtValue.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        txtValue.ForeColor = Color.DimGray;
        txtValue.Location = new Point(3, 30);
        txtValue.Margin = new Padding(4);
        txtValue.Multiline = false;
        txtValue.Name = "txtValue";
        txtValue.Padding = new Padding(7);
        txtValue.PasswordChar = false;
        txtValue.Size = new Size(248, 31);
        txtValue.TabIndex = 67;
        txtValue.Texts = "";
        txtValue.UnderlinedStyle = false;
        // 
        // SettingTextBox
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lbTips);
        Controls.Add(lbTitle);
        Controls.Add(txtValue);
        Name = "SettingTextBox";
        Size = new Size(263, 70);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lbTips;
    private Label lbTitle;
    private XS.WinFormsTools.XsTextBox txtValue;
}
