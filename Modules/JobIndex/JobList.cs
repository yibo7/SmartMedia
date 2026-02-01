 
using SmartMedia.MCore;
using SmartMedia.Modules.PushContent.DB;
using WeifenLuo.WinFormsUI.Docking;
using XS.Core2.XsExtensions;
using XS.WinFormsTools.Forms;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SmartMedia.Modules.Job
{
    public partial class JobList : XsDockContent //, IModules 作为首页，不用在模块中加载
    {
        public string Title => "任务管理器";//要实现模块名称
        public System.Drawing.Image Ico => Resource.books; //要实现模块图标
        VisualStudioToolStripExtender vsToolStripExtender1;
        public JobList()
        {
            CloseButtonVisible = false; // 隐藏关闭按钮 
            InitializeComponent();

            //lvData.AddColum("", 42);
            lvData.AddColum("Id", 0);
            lvData.AddColum("任务名称", -100);
            lvData.AddColum("内容类别", 80);
            lvData.AddColum("发布时间", 120);
            lvData.AddColum("发布倒计时", 150);

            picCover.SizeMode = PictureBoxSizeMode.StretchImage;

            lvData.SelectedIndexChanged += lvData_SelectedIndexChanged;

            BindData();
        }
         
        private void AddItem(PushInfo model, int iIndex)
        {
            lvData.AddItem(model.Id.ToString(), $"{iIndex}、{model.Title}", model.ContentTypeName, model.PublishDateTime, model.FormatTimeRemaining());
             
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sTypeName">所有机器人,视频分发,文讯分发,其他</param>
        private void BindData()
        {

            lvData.Items.Clear();
            var lst = new VideoBll().GetScheduledContents();
            int iIndex = 1;
            foreach (var model in lst)
            {
                AddItem(model, iIndex);

                iIndex++;

            }
            lvData.SelectedItem(0);// 选中第一行


        }

        bool isPause = false;
        private void btnPassAll_Click(object sender, EventArgs e)
        {
            if (XS.WinFormsTools.Dialogs.ConfirmDialog(isPause ? "确定要继续所有任务吗？" : "确定要暂停所有任务吗？"))
            {
                if (!isPause)
                {
                    isPause = true;
                }
                else
                {
                    isPause = false;
                }

                this.BindData();

            }

        }


        private void lvData_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 检查是否有选中项
            if (lvData.SelectedItems.Count > 0)
            {
                // 执行你需要的操作，例如获取选中项的文本
                string jobId = GetSelJobJd;
                var model = new VideoBll().GetEntity(jobId.ToLong());
                lbTitle.Text = $"标题: {model.Title}"; 
                if (!string.IsNullOrWhiteSpace(model.ImgPath) && File.Exists(model.ImgPath))
                {
                    picCover.LoadImageFromFilePath(model.ImgPath);
                }

                txtDes.Text = $"{model.Tags}\n\n{model.Info}";
            }
        }



        private void btnRefesh_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        //private void btnInstallToService_Click(object sender, EventArgs e)
        //{
        //    if (XS.WinFormsTools.Dialogs.ConfirmDialog("注册到系统服务关闭软件依然可以执行定时任务，确定要注册吗?"))
        //    {

        //    }
        //}

        private string GetSelJobJd => lvData.GetSelectItemValue(0);
    }
}
