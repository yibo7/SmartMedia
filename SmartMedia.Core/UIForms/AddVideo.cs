
using SmartMedia.Core.Comm;
using SmartMedia.Core.SitesBase.BLL;
using SmartMedia.Core.SitesBase.DB;
using XS.Core2.XsExtensions;
using XS.WinFormsTools;

namespace SmartMedia.Core.UIForms;


public partial class AddVideo : XsDockContent
{
     
    private long _Id = 0;
    private IPushContentBll pushContentBll;
    public AddVideo(IPushContentBll bllData,PushInfo _model = null)
    {
        pushContentBll = bllData;
        InitializeComponent();
        this.Resize += new EventHandler(Form_Resize);

        linkBarDelete.Visible = false;
        picCover.SizeMode = PictureBoxSizeMode.StretchImage;
        classCombox.BindClass(pushContentBll.IType);
        if (_model != null)
        {
            classCombox.Value = _model.ClassId;
            txtTitle.Text = _model.Title;

            txtTags.Text = _model.Tags;
            txtInfo.Text = _model.Info;
            //CurrentPicCoverPath = _model.ImgPath;
            CurrentVideoPath = _model.FilePath;
            if (!string.IsNullOrWhiteSpace(_model.ImgPath) && File.Exists(_model.ImgPath))
            { 
                ShowCover(_model.ImgPath);
            }
            if (!string.IsNullOrWhiteSpace(CurrentVideoPath))
            {
                lbVideoName.Text = Path.GetFileName(CurrentVideoPath);
            }

            cbIsRc.Checked = _model.Original == 1;

            publishTimer.DateTimestamp = _model.PublishTimeStamp;

            _Id = _model.Id;

            siteSelector.SiteSettingValues = _model.ConverSiteSettingToDict();

            btnPush.Enabled = true;
        }
    }



    private void Form_Resize(object sender, EventArgs e)
    {
        panelCenter.Left = (panel1.Width - panelCenter.Width) / 2;
        panelCenter.Top = (panel1.Height - panelCenter.Height) / 2; // 可选：同时居中垂直位置
    }

     
    private void btnSelVideo_Click(object sender, EventArgs e)
    {
        CurrentVideoPath = Dialogs.OpenSelFile("选择视频|*.mp4;*.mov;*.mkv;*.avi;*.flv;*.mpeg;*.ogg;*.vob;*.webm;*.wmv;*.rmvb;");

        if (!string.IsNullOrEmpty(CurrentVideoPath))
        {
            lbVideoName.Text = Path.GetFileName(CurrentVideoPath);

            if (string.IsNullOrEmpty(txtTitle.Texts))
                txtTitle.Texts = Path.GetFileNameWithoutExtension(CurrentVideoPath);

            _ = MakeeCoverImage();
        }

    }
     
    private void btnSave_Click(object sender, EventArgs e)
    {
        (bool IsOk, _Id) = SaveData();

        if (IsOk)
        {
            btnPush.Enabled = true;
            Tips("保存成功");
        }
    }



    private (bool, long) SaveData()
    {
         
        (string err, Dictionary<string, Dictionary<string, string>>? dicSites) = siteSelector.GetValueSettings();

        if (!string.IsNullOrWhiteSpace(err))
        {
            Tips(err, TipsState.Error);
            return (false, 0);
        }

        var model = new PushInfo();

        model.Title = txtTitle.Texts.Trim();
        model.Tags = txtTags.Text.Trim();
        //model.Status = 0;
        model.Original = cbIsRc.Checked ? 1 : 0;
        model.Info = txtInfo.Texts.Trim();

        model.FilePath = CurrentVideoPath;
        model.ImgPath = CurrentPicCoverPath;

        model.IType = 1;

        model.Sites = dicSites.ToJsonString();

        model.PublishTimeStamp = publishTimer.DateTimestamp;

        model.ClassId = classCombox.Value;

        model.Id = _Id;
        //var bll = new VideoBll();
        (string msg, long Id) = pushContentBll.AddData(model);
        if (!string.IsNullOrWhiteSpace(msg))
        {
            Tips(msg);
            return (false, 0);
        }
        else
        {
            return (true, Id);
        }
    }

    private string CurrentVideoPath = "";
    private async void btnPush_Click(object sender, EventArgs e)
    {
        //(bool IsOk, long Id) = SaveData();
        if (_Id > 0)
        {
            btnPush.Enabled = false;
            var frmLoading = new FrmLoading(); 
            await frmLoading.ShowWinAsync(async () =>
            {
                frmLoading.ToDo(100, 0, "正在加载上传插件...");
                try
                {
                    //var bll = new VideoBll();
                    await pushContentBll.StartPush(_Id, (tips, imax, icurrent) =>
                    {
                        frmLoading.ToDo(imax, icurrent, tips);
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发布失败:{ex.Message}");
                }

            });

            btnPush.Enabled = true;
        }


    }


    private string CurrentPicCoverPath = "";
    private void linkBarDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {

        if(!XS.WinFormsTools.Dialogs.ConfirmDialog("确认要删除吗？"))
            return;

        linkBarDelete.Visible = false;
        picCover.Image = null;
        CurrentPicCoverPath = "";
    }

    private async Task MakeeCoverImage(int second = 3)
    {

        var t = TimeSpan.FromSeconds(second);
        var rz = await new MediaToolkitHelper().CaptureFrameAsync(CurrentVideoPath, t);
        if (rz.Item1)
        { 
            ShowCover(rz.Item2);

        }
        else
        {
            Tips(rz.Item2, TipsState.Error);
        }
    }
    private void ShowCover(string sCoverPath)
    {
        if (!string.IsNullOrEmpty(sCoverPath))
        {
            CurrentPicCoverPath = sCoverPath;
            picCover.LoadImageFromFilePath(sCoverPath);

            linkBarDelete.Visible = true;
        }
    }
    private void btnMakeCoverImage_Click(object sender, EventArgs e)
    {
         

        if (string.IsNullOrEmpty(CurrentVideoPath))
        {
            Tips("请先选择视频！");
            return; 
        
        }
        var max = new MediaToolkitHelper().GetVideoDurationSeconds(CurrentVideoPath);
        if (max > 0)
        {
            var time = XS.Core2.XsUtils.GetRandNum(Convert.ToInt32(max), 1);
            _ = MakeeCoverImage(time);
        }
       
    }

    private void picCover_Click(object sender, EventArgs e)
    {
        CurrentPicCoverPath = Dialogs.OpenSelFile("选择图片|*.bmp;*.jpg;*.jpeg; *.png; *.gif;");
        if (!string.IsNullOrEmpty(CurrentPicCoverPath))
        {
            ShowCover(CurrentPicCoverPath);
        }
    }
}
