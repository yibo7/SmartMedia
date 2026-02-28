namespace SmartMedia.Core.Controls;

partial class SettingComboBox
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
        comboBox = new XS.WinFormsTools.XsComboBox();
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
        // comboBox
        // 
        comboBox.BackColor = Color.White;
        comboBox.BorderColor = Color.Gainsboro;
        comboBox.ConerRadius = 2;
        comboBox.DrawMode = DrawMode.OwnerDrawFixed;
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.FlatStyle = FlatStyle.Flat;
        comboBox.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
        comboBox.FormattingEnabled = true;
        comboBox.ItemHeight = 32;
        comboBox.Location = new Point(3, 29);
        comboBox.Name = "comboBox";
        comboBox.Size = new Size(248, 38);
        comboBox.TabIndex = 70;
        // 
        // SettingComboBox
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(comboBox);
        Controls.Add(lbTips);
        Controls.Add(lbTitle);
        Name = "SettingComboBox";
        Size = new Size(263, 70);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lbTips;
    private Label lbTitle;
    private XS.WinFormsTools.XsComboBox comboBox;
}
