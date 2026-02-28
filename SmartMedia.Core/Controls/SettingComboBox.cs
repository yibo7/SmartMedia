using System.ComponentModel;
using XS.Core2.XsExtensions;

namespace SmartMedia.Core.Controls;

public partial class SettingComboBox : SettingCtrBase
{
    public SettingComboBox()
    {
        InitializeComponent();
        AutoSize = true;

        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    /// <summary>
    /// 设置下拉选项
    /// </summary>
    /// <param name="sItems">多个选项用###号分开</param>
    override public void InitData(string sItems)
    {
        string[] items = sItems.SplitByTag("###");
        if (items.Length > 0)
        {
            comboBox.DataSource = items;
            comboBox.SelectedIndex = 0;
        }

    }

    [Category("设置文本")]
    override public string Text
    {
        get { return comboBox.Text; }
        set { comboBox.Text = value; }
    }

    override public void SetValue(string value)
    {
        comboBox.Text = value;
    } 

    public override string GetValue()
    {
        return comboBox.Text;
    }


    [Category("设置Width")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    new public int Width
    {
        get { return comboBox.Width; }
        set
        {
            //this.Width = value;
            this.comboBox.Width = value;
        }
    }

    override public void SetTitle(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            lbTitle.Text = value;
    }
    override public void SetTips(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            lbTips.Text = value;
    } 


}
