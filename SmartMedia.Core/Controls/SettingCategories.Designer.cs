namespace SmartMedia.Core.Controls;

partial class SettingCategories
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
        cbCategory3 = new XS.WinFormsTools.XsComboBox();
        cbCategory2 = new XS.WinFormsTools.XsComboBox();
        cbCategory1 = new XS.WinFormsTools.XsComboBox();
        lbTips = new Label();
        lbTitle = new Label();
        SuspendLayout();
        // 
        // cbCategory3
        // 
        cbCategory3.BackColor = Color.White;
        cbCategory3.BorderColor = Color.Gainsboro;
        cbCategory3.ConerRadius = 2;
        cbCategory3.DrawMode = DrawMode.OwnerDrawFixed;
        cbCategory3.DropDownStyle = ComboBoxStyle.DropDownList;
        cbCategory3.FlatStyle = FlatStyle.Flat;
        cbCategory3.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
        cbCategory3.FormattingEnabled = true;
        cbCategory3.ItemHeight = 32;
        cbCategory3.Location = new Point(314, 29);
        cbCategory3.Name = "cbCategory3";
        cbCategory3.Size = new Size(151, 38);
        cbCategory3.TabIndex = 75;
        cbCategory3.SelectedIndexChanged += cbCategory3_SelectedIndexChanged;
        // 
        // cbCategory2
        // 
        cbCategory2.BackColor = Color.White;
        cbCategory2.BorderColor = Color.Gainsboro;
        cbCategory2.ConerRadius = 2;
        cbCategory2.DrawMode = DrawMode.OwnerDrawFixed;
        cbCategory2.DropDownStyle = ComboBoxStyle.DropDownList;
        cbCategory2.FlatStyle = FlatStyle.Flat;
        cbCategory2.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
        cbCategory2.FormattingEnabled = true;
        cbCategory2.ItemHeight = 32;
        cbCategory2.Location = new Point(157, 29);
        cbCategory2.Name = "cbCategory2";
        cbCategory2.Size = new Size(151, 38);
        cbCategory2.TabIndex = 74;
        cbCategory2.SelectedIndexChanged += cbCategory2_SelectedIndexChanged;
        // 
        // cbCategory1
        // 
        cbCategory1.BackColor = Color.White;
        cbCategory1.BorderColor = Color.Gainsboro;
        cbCategory1.ConerRadius = 2;
        cbCategory1.DrawMode = DrawMode.OwnerDrawFixed;
        cbCategory1.DropDownStyle = ComboBoxStyle.DropDownList;
        cbCategory1.FlatStyle = FlatStyle.Flat;
        cbCategory1.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
        cbCategory1.FormattingEnabled = true;
        cbCategory1.ItemHeight = 32;
        cbCategory1.Location = new Point(0, 29);
        cbCategory1.Name = "cbCategory1";
        cbCategory1.Size = new Size(151, 38);
        cbCategory1.TabIndex = 73;
        // 
        // lbTips
        // 
        lbTips.AutoSize = true;
        lbTips.ForeColor = SystemColors.ControlDark;
        lbTips.Location = new Point(59, 3);
        lbTips.Name = "lbTips";
        lbTips.Size = new Size(92, 17);
        lbTips.TabIndex = 72;
        lbTips.Text = "此平台上的分类";
        // 
        // lbTitle
        // 
        lbTitle.AutoSize = true;
        lbTitle.Location = new Point(-1, 2);
        lbTitle.Name = "lbTitle";
        lbTitle.Size = new Size(56, 17);
        lbTitle.TabIndex = 71;
        lbTitle.Text = "选择分类";
        // 
        // SettingCategories
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(cbCategory3);
        Controls.Add(cbCategory2);
        Controls.Add(cbCategory1);
        Controls.Add(lbTips);
        Controls.Add(lbTitle);
        Name = "SettingCategories";
        Size = new Size(468, 67);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private XS.WinFormsTools.XsComboBox cbCategory3;
    private XS.WinFormsTools.XsComboBox cbCategory2;
    private XS.WinFormsTools.XsComboBox cbCategory1;
    private Label lbTips;
    private Label lbTitle;
}
