

using SmartMedia.Core.SitesBase.BLL;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms; 
using XS.Core2.XsExtensions;
using XS.WinFormsTools;

namespace SmartMedia.Core.UIForms;


public partial class AddImagePost : XsDockContent
{

    private IPushContentBll pushContentBll;
    private long _Id = 0;
    public AddImagePost(IPushContentBll bllData, PushInfo _model = null)
    {
        pushContentBll = bllData;
        InitializeComponent();
        this.Resize += new EventHandler(Form_Resize);

        classCombox.BindClass(pushContentBll.IType);
        if (_model != null)
        {
            classCombox.Value = _model.ClassId;

            txtTitle.Text = _model.Title;

            txtTags.Text = _model.Tags;
            txtInfo.Text = _model.Info;

            if (!string.IsNullOrWhiteSpace(_model.FilePath))
            {
                imageList.ImgPathValues = _model.FilePath.SplitByWrap();
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

        var imgPaths = imageList.ImgPathValues;
        if (!Equals(imgPaths, null) && imgPaths.Length > 0)
        {
            model.FilePath = Equals(imgPaths, null) ? "" : string.Join("\n", imgPaths);
            model.ImgPath = imgPaths[0];
        }

        model.Sites = dicSites.ToJsonString();

        model.PublishTimeStamp = publishTimer.DateTimestamp;

        model.ClassId = classCombox.Value;

        model.Id = _Id;
        //var bll = new ImagePostBll();
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
                    //var bll = new ImagePostBll();
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

     
}
