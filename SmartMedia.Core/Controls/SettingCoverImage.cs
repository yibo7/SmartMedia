
using XS.WinFormsTools;

namespace SmartMedia.Core.Controls;

public partial class SettingCoverImage : SettingCtrBase
{
    public SettingCoverImage()
    {
        InitializeComponent();
        AutoSize = true;
        picCoverImage.SizeMode = PictureBoxSizeMode.Zoom;
        linkBarDelete.Visible = false;
    }

    override public void SetValue(string value)
    {
        CurrentPicCoverPath = value;
        if (!string.IsNullOrEmpty(CurrentPicCoverPath))
        {
            picCoverImage.LoadImageFromFilePath(CurrentPicCoverPath);
            linkBarDelete.Visible = true;
        }
    }
    override public void SetTitle(string value) {
        lbTitle.Text = value;
    }
    override public void SetTips(string value) {
        lbTip.Text = value;
    }
    public override string GetValue()
    {
        return CurrentPicCoverPath;
    }

    private string CurrentPicCoverPath = "";
    private void picCoverImage_Click(object sender, EventArgs e)
    {
        CurrentPicCoverPath = Dialogs.OpenSelFile("选择图片|*.bmp;*.jpg;*.jpeg; *.png; *.gif;");
        if (!string.IsNullOrEmpty(CurrentPicCoverPath))
        {
            picCoverImage.LoadImageFromFilePath(CurrentPicCoverPath);
            linkBarDelete.Visible = true;
        }
    }

    private void linkBarDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (!XS.WinFormsTools.Dialogs.ConfirmDialog("确认要删除吗？"))
            return;
        linkBarDelete.Visible = false;
        picCoverImage.Image = null;
        CurrentPicCoverPath = "";
    }
}
