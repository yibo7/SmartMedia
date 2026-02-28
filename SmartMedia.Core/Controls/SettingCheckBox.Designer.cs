namespace SmartMedia.Core.Controls;

partial class SettingCheckBox
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
        cbCheckBox = new CheckBox();
        SuspendLayout();
        // 
        // lbTips
        // 
        lbTips.AutoSize = true;
        lbTips.ForeColor = SystemColors.ControlDark;
        lbTips.Location = new Point(6, 4);
        lbTips.Name = "lbTips";
        lbTips.Size = new Size(188, 17);
        lbTips.TabIndex = 69;
        lbTips.Text = "可为空，你在此平台上创建的合集";
        // 
        // cbCheckBox
        // 
        cbCheckBox.AutoSize = true;
        cbCheckBox.Location = new Point(9, 24);
        cbCheckBox.Name = "cbCheckBox";
        cbCheckBox.Size = new Size(89, 21);
        cbCheckBox.TabIndex = 70;
        cbCheckBox.Text = "checkBox1";
        cbCheckBox.UseVisualStyleBackColor = true;
        // 
        // SettingCheckBox
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(cbCheckBox);
        Controls.Add(lbTips);
        Name = "SettingCheckBox";
        Size = new Size(203, 51);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lbTips;
    private CheckBox cbCheckBox;
}
