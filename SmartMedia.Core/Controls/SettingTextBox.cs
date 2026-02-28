using System.ComponentModel;

namespace SmartMedia.Core.Controls;

public partial class SettingTextBox : SettingCtrBase
{
    public SettingTextBox()
    {
        InitializeComponent();
        AutoSize = true;
    }

    

    [Category("设置文本")]
    override public string Text
    {
        get { return txtValue.Text; }
        set { txtValue.Text = value; }
    }
    override public void SetValue(string value)
    {
        txtValue.Text = value;
    }
    override public string GetValue() {
        return txtValue.Text;
    }

    [Category("设置文本")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    new public int Width
    {
        get { return txtValue.Width; }
        set
        {
            //this.Width = value;
            this.txtValue.Width = value;
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
    //    get { return lbTitle.Text; }
    //    set
    //    {
    //        lbTitle.Text = value;
    //    }
    //}


}
