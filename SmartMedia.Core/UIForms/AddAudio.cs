

using SmartMedia.Core.Comm;
using SmartMedia.Core.SitesBase.BLL;
using SmartMedia.Core.SitesBase.DB;
using XS.Core2.XsExtensions;
using XS.WinFormsTools;

namespace SmartMedia.Core.UIForms;


public partial class AddAudio : XsDockContent
{
    private IPushContentBll pushContentBll;
    private long _Id = 0;
    public AddAudio(IPushContentBll bllData, PushInfo _model = null)
    {
        pushContentBll = bllData;
        InitializeComponent();
        btnPlay.Visible = false;
        this.Resize += new EventHandler(Form_Resize);
        audioPlayer = new AudioPlayer();
        classCombox.BindClass(pushContentBll.IType);
        if (_model != null)
        {
            classCombox.Value = _model.ClassId;
            txtTitle.Text = _model.Title;

            txtTags.Text = _model.Tags;
            txtInfo.Text = _model.Info; 
            CurrentPath = _model.FilePath;
       
            if (!string.IsNullOrWhiteSpace(CurrentPath))
            {
                lbAudioName.Text = Path.GetFileName(CurrentPath);
            }

            cbIsRc.Checked = _model.Original == 1;

            publishTimer.DateTimestamp = _model.PublishTimeStamp;

            _Id = _model.Id;

            siteSelector.SiteSettingValues = _model.ConverSiteSettingToDict();

            btnPush.Enabled = true;
            btnPlay.Visible = true;
        }
    }



    private void Form_Resize(object sender, EventArgs e)
    {
        panelCenter.Left = (panel1.Width - panelCenter.Width) / 2;
        panelCenter.Top = (panel1.Height - panelCenter.Height) / 2; // 可选：同时居中垂直位置
    }


    private void btnSelVideo_Click(object sender, EventArgs e)
    {
        CurrentPath = Dialogs.OpenSelFile("选择音频文件|*.mp3;*.wav;*.flac;*.aac;*.m4a;*.ogg;*.opus;*.wma;*.amr;");

        if (!string.IsNullOrEmpty(CurrentPath))
        {
            lbAudioName.Text = Path.GetFileName(CurrentPath);

            if (string.IsNullOrEmpty(txtTitle.Texts))
            {
                txtTitle.Texts = Path.GetFileNameWithoutExtension(CurrentPath);
                btnPlay.Visible = true;
            }



            //_ = MakeeCoverImage();
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
        model.Original = cbIsRc.Checked ? 1 : 0;
        model.Info = txtInfo.Texts.Trim();

        model.FilePath = CurrentPath;
        

        model.Sites = dicSites.ToJsonString();

        model.PublishTimeStamp = publishTimer.DateTimestamp;

        model.ClassId = classCombox.Value;

        model.Id = _Id;
        //var bll = new AudioBll();
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

    private string CurrentPath = "";
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
                    //var bll = new AudioBll();
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
    private AudioPlayer audioPlayer = null;
        
    private void btnPlay_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CurrentPath))
        {
            Tips("请选选择音频！");
            return;
        }
        if(btnPlay.Text == "试听")
        {
            audioPlayer.Load(CurrentPath);
            audioPlayer.Play();
            btnPlay.Text = "暂停";
        }
        else
        {
            audioPlayer.Stop();
            btnPlay.Text = "试听";
        } 

    }
}
