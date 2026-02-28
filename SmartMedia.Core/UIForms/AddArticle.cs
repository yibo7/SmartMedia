 
using SmartMedia.Core.SitesBase.DB;
using XS.Core2.XsExtensions;
using XS.WinFormsTools;

namespace SmartMedia.Core.UIForms;


public partial class AddArticle : XsDockContent
{
    private IPushContentBll pushContentBll;
    private long _Id = 0;
    public AddArticle(IPushContentBll bllData, PushInfo _model = null)
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
            //htmlEditor.Text = _model.Info;
            //CurrentPicCoverPath = _model.ImgPath;

            htmlEditor.OnInited += () =>
            {
                if (_model != null)
                {
                    txtTitle.Text = _model.Title;

                    txtTags.Text = _model.Tags;
                    htmlEditor.SetHtmlValue(_model.Info);
                    CurrentPicCoverPath = _model.ImgPath;
                    if (!string.IsNullOrWhiteSpace(_model.ImgPath))
                    {
                        picCover.Image = Image.FromFile(CurrentPicCoverPath);
                        linkBarDelete.Visible = true;
                    }


                    _Id = _model.Id;
                }
            };

            CurrentVideoPath = _model.FilePath;
            if (!string.IsNullOrWhiteSpace(_model.ImgPath) && File.Exists(_model.ImgPath))
            {
                ShowCover(_model.ImgPath);
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

    private void btnSave_Click(object sender, EventArgs e)
    {
        _ = SaveAsync();
    }

    private async Task SaveAsync()
    {
        (bool IsOk, _Id) = await SaveDataAsync();

        if (IsOk)
        {
            btnPush.Enabled = true;
            Tips("保存成功");
        }
    }

    private async Task<(bool, long)> SaveDataAsync()
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
        model.Info = await htmlEditor.GetHtmlValue();

        model.FilePath = CurrentVideoPath;
        model.ImgPath = CurrentPicCoverPath;

        //model.IType = 1;

        model.Sites = dicSites.ToJsonString();

        model.PublishTimeStamp = publishTimer.DateTimestamp;

        model.ClassId = classCombox.Value;

        model.Id = _Id;
        //var bll = new ArticleBll();
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

        if (!XS.WinFormsTools.Dialogs.ConfirmDialog("确认要删除吗？"))
            return;

        linkBarDelete.Visible = false;
        picCover.Image = null;
        CurrentPicCoverPath = "";
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


    private void picCover_Click(object sender, EventArgs e)
    {
        CurrentPicCoverPath = Dialogs.OpenSelFile("选择图片|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.tif;*.webp;*.svg;");
        if (!string.IsNullOrEmpty(CurrentPicCoverPath))
        {
            ShowCover(CurrentPicCoverPath);
        }
    }

    
     
}
