using System.ComponentModel;
using XS.Core2.XsExtensions;

namespace SmartMedia.Core.Controls;

public partial class SettingCheckBox : SettingCtrBase
{
    public SettingCheckBox()
    {
        InitializeComponent();
        AutoSize = true;
    }

    override public void SetValue(string value)
    {
        if(!string.IsNullOrWhiteSpace(value))
            this.cbCheckBox.Checked = value.ToBool();
    }

    [Category("设置文本")]
    override public string Text
    {
        get { return cbCheckBox.Checked.ToString(); }
        set { cbCheckBox.Checked = value.ToBool(); }
    }

    public override string GetValue()
    {
        return cbCheckBox.Checked.ToString();
    }

    //[Category("设置Tips文本")]
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    //public string Tips
    //{
    //    get { return lbTips.Text; }
    //    set
    //    {
    //        lbTips.Text = value;
    //    }
    //}
    //[Category("设置Title文本")]
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    //public string Title
    //{
    //    get { return cbCheckBox.Text; }
    //    set
    //    {
    //        cbCheckBox.Text = value;
    //    }
    //}


    override public void SetTitle(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            cbCheckBox.Text = value;
    }
    override public void SetTips(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            lbTips.Text = value;
    }

}
