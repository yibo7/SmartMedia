using System.ComponentModel;
using XS.Core2.XsExtensions;
using XS.WinFormsTools;

namespace SmartMedia.Core.Controls;

public partial class SettingSrtLrc : SettingCtrBase
{
    public SettingSrtLrc()
    {
        InitializeComponent();
        AutoSize = true; 
    }

    override public void SetValue(string value)
    {
        CurrentPicCoverPath = value;
        if (!string.IsNullOrEmpty(CurrentPicCoverPath))
        {
            txtFilePath.Text = CurrentPicCoverPath;
        }
    }
    override public void SetTitle(string value)
    {
        lbTitle.Text = value;
    }
    override public void SetTips(string value)
    {
        lbTip.Text = value;
    }
    public override string GetValue()
    {
        return CurrentPicCoverPath;
    }

    public string GetSrtLrcContent()
    {
        if (string.IsNullOrWhiteSpace(CurrentPicCoverPath))
            return "";

        return File.ReadAllText(CurrentPicCoverPath);
    }

    private string CurrentPicCoverPath = "";
    
    private void btnSel_Click(object sender, EventArgs e)
    {
        CurrentPicCoverPath = Dialogs.OpenSelFile("选择字幕|*.lrc;*.srt;*.ass;");
        if (!string.IsNullOrEmpty(CurrentPicCoverPath))
        {
            txtFilePath.Text = CurrentPicCoverPath;
        }
    }
}
