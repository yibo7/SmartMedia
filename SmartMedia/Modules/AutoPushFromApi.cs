 
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.UIForms;
using SmartMedia.MCore; 
using XS.Core2.XsExtensions;
using XS.WinFormsTools; 

namespace SmartMedia.Modules
{
    public partial class AutoPushFromApi : XsDockContent //, IModules 作为首页，不用在模块中加载
    {
        public string Title => "任务管理器";//要实现模块名称
        public System.Drawing.Image Ico => Resource.books; //要实现模块图标 

        private PushBase AutoPush;
        private ContentFromSiteBll bllContentFromSite;
        public AutoPushFromApi(PushBase autoPush)
        {
            AutoPush = autoPush;
            bllContentFromSite = new ContentFromSiteBll();
            CloseButtonVisible = false; // 隐藏关闭按钮 
            InitializeComponent();

            //lvData.AddColum("", 42);
            lvData.AddColum("Id", 0);
            lvData.AddColum("视频名称", -100);
            lvData.AddColum("分类名称", 150);
            lvData.AddColum("发布时间", 120);
            lvData.AddColum("观看数", 100);
            lvData.AddColum("点赞数", 100);
            lvData.AddColum("评论数", 100);
            lvData.AddColum("收藏数", 100);


            lvData.SelectedIndexChanged += lvData_SelectedIndexChanged;

            txtDes.Text = "选择左边数据列表中的某条数据即可查看详情！";

            BindData();
        }

        private void AddItem(ContentFromSite model, int iIndex)
        {
            lvData.AddItem(model.Id.ToString(), $"{iIndex}、{model.Title}", AutoPush.GetCategoryNameById(model.CategoryId), model.PublishedAt.Value.ToString("yyyy-MM-dd HH:mm"), model.ViewCount.ToString(), model.LikeCount.ToString(), model.CommentCount.ToString(), model.FavoriteCount.ToString());

        }

        private void BindData()
        {

            lvData.Items.Clear();
            var lst = bllContentFromSite.GetListByPlugin(AutoPush.ClassName);
            int iIndex = 1;
            foreach (var model in lst)
            {
                AddItem(model, iIndex);

                iIndex++;

            }

            
            if (!string.IsNullOrWhiteSpace(AutoPush.CategoryFileName))
            {
                btnUpdateClassData.Visible = true;
            }
            //lvData.SelectedItem(0);// 选中第一行

        }

        private async Task RefeshData()
        {
            btnRefesh.Enabled = false;
            lvData.Items.Clear();
            var lst = await AutoPush.GetDataList();

            foreach (var model in lst)
            {
                model.PluginName = AutoPush.ClassName;
                bllContentFromSite.UpdateData(model);

            }
            //lvData.SelectedItem(0);// 选中第一行
            BindData();
            btnRefesh.Enabled = true;
        }




        private async void lvData_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 检查是否有选中项
            if (lvData.SelectedItems.Count > 0)
            {
                // 执行你需要的操作，例如获取选中项的文本
                string jobId = GetSelJobJd;
                var model = bllContentFromSite.GetEntity(jobId.ToLong());
                lbTitle.Text = $"标题：{model.Title}";

                txtDes.Text = string.IsNullOrWhiteSpace(model.Description) ? "暂无" : $"{model.Tags}\n\t{model.Description}";

                if (!string.IsNullOrWhiteSpace(model.ThumbnailDefaultUrl))
                {
                    _ = Task.Run(async () =>
                    {
                        await picContentSmallImg.LoadImageFromUrlAsync(model.ThumbnailDefaultUrl);
                    });
                }


            }
        }



        private void btnRefesh_Click(object sender, EventArgs e)
        {
            if (XS.WinFormsTools.Dialogs.ConfirmDialog("将获取线上数据，同时更新到本地，确定吗？"))
            {
                _ = RefeshData();
            }

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            GoToLogin();
        }

        private async void GoToLogin()
        {
            btnLogin.Enabled = false;
            string err = await AutoPush.LoginAsync();
            if (string.IsNullOrWhiteSpace(err))
            {
                Tips("登录成功！");
            }
            else
            {
                XS.Core2.LogHelper.Error<AutoPushSite>($"登录发生异常：{err}");
                MessageBox.Show("登录发生异常,具体原因可查看日志！");
            }

            btnLogin.Enabled = true;
        }

        private void btnShowHelp_Click(object sender, EventArgs e)
        {
            base.OpenHelpDockToMain($"{AutoPush.PluginName}使用帮助", AutoPush.Help);
        }

        private void btnUpdateClassData_Click(object sender, EventArgs e)
        {
            if (XS.WinFormsTools.Dialogs.ConfirmDialog("这是发布时可选分类的数据，一般第一次使用时初始化一次即可，确认更新吗？"))
            {
                _ = UpdateClassData();
            }

        }

        private async Task UpdateClassData()
        {
            string errInfo = await AutoPush.UpdateClassData();
            if (string.IsNullOrEmpty(errInfo))
            {
                Tips("更新成功");
            }
            else
            {
                Tips(errInfo, TipsState.Error);
            }

        }

        private void AutoPushFromApi_Load(object sender, EventArgs e)
        {
            //AutoPush.LoginAsync();
        }

        private void btnShowUserInfo_Click(object sender, EventArgs e)
        {
            ShowUserInfo();
        }
        private async Task ShowUserInfo()
        {
            btnShowUserInfo.Enabled = false;
            var info = await AutoPush.GetUserInfo();
            if (!string.IsNullOrEmpty(info))
            {
                base.OpenHelpDockToMain($"{AutoPush.PluginName}统计", info);
            }
            else
            {
                Tips("获取到的数据为空！");
            }
            btnShowUserInfo.Enabled = true;
        }

        private void btnDelData_Click(object sender, EventArgs e)
        {
            if (!XS.WinFormsTools.Dialogs.ConfirmDialog("此操作只删除本地所选数据，线上数据请到平台操作，确认删除吗?"))
                return;

            var keys = lvData.GetSelectItemKeys();

            foreach (var key in keys) {
                bllContentFromSite.Delete(key.ToLong());
            }

            this.BindData();

        }

        private string GetSelJobJd => lvData.GetSelectItemValue(0);
    }
}
