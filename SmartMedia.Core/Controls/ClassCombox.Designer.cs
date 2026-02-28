namespace SmartMedia.Core.Controls;

partial class ClassCombox
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
        cbClass = new XS.WinFormsTools.XsComboBox();
        SuspendLayout();
        // 
        // cbClass
        // 
        cbClass.BackColor = Color.White;
        cbClass.BorderColor = Color.Gainsboro;
        cbClass.ConerRadius = 2;
        cbClass.DrawMode = DrawMode.OwnerDrawFixed;
        cbClass.DropDownStyle = ComboBoxStyle.DropDownList;
        cbClass.FlatStyle = FlatStyle.Flat;
        cbClass.Font = new Font("微软雅黑", 12F);
        cbClass.FormattingEnabled = true;
        cbClass.ItemHeight = 32;
        cbClass.Location = new Point(0, 0);
        cbClass.Name = "cbClass";
        cbClass.Size = new Size(168, 38);
        cbClass.TabIndex = 0;
        // 
        // ClassCombox
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(cbClass);
        Name = "ClassCombox";
        Size = new Size(169, 38);
        ResumeLayout(false);
    }

    #endregion

    private XS.WinFormsTools.XsComboBox cbClass;
}
