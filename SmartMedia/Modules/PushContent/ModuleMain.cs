using SmartMedia.Core.Comm;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms;
using SmartMedia.MCore;
using SmartMedia.Modules.PushContent;
using SmartMedia.Modules.PushContent.DB;
using XS.Core2.XsExtensions;
using XS.WinFormsTools;

namespace SmartMedia.Modules.VideoManageModule
{
    public partial class ModuleMain : XsDockContent
    {
        public string Title => "内容管理";
        public Image Ico => Resource.video;
        IPushContentBll dataBll;
        public ModuleMain(IPushContentBll bll)
        {
            dataBll = bll;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            lvData.AddColum("Id", 0);
            lvData.AddColum("发布状态", 80);
            lvData.AddColum("标题", 250);
            lvData.AddColum("标签", 200);
            lvData.AddColum("是否原创", 80);
            lvData.AddColum("定时发布", 80);
            lvData.AddColum("添加时间", 150);
            lvData.AddColum("发布平台", -100);

            // 上下文菜单 
            lvData.InitContextMenu(menuContext);
            cbStatus.SelectedIndex = 0;

            //btcMyWritter.Text = $"发布{bll.Title}";

            this.Load += ModuleMain_Load;

            picContentSmallImg.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private void ModuleMain_Load(object? sender, EventArgs e)
        {
            BindClass();
            BindData();
            
        }

        private void btcMyWritter_Click(object sender, EventArgs e)
        {
            dataBll.OnOpenAdd(0);

        }

        // 定义状态信息记录
        private record StatusInfo(Color Color, string Name);

        // 集中管理状态配置
        private static readonly IReadOnlyDictionary<int, StatusInfo> StatusConfig =
            new Dictionary<int, StatusInfo>
            {
                [-1] = new(Color.PaleVioletRed, "发布失败"),
                [0] = new(Color.White, "待发布"),
                [1] = new(Color.LightGreen, "发布成功")
            };

        private void BindData(int status = -1, string keyword = "")
        {
            lvData.Items.Clear();
            int iClassId = cbClass.ComboBox.SelectedValue.ToString().ToInt();

            var lst = dataBll.Search(status, keyword, iClassId);


            foreach (var item in lst)
            {
                var statusInfo = StatusConfig.TryGetValue(item.Status, out var info) ? info : new StatusInfo(Color.White, "未知状态");


                string resultSiteNames = string.Join(",", item.ConverSiteSettingToDict().Keys);

                lvData.AddItem(statusInfo.Color, item.Id.ToString(), statusInfo.Name, item.Title, item.Tags, item.Original == 0 ? "否" : "是", item.PublishTimeStamp > 0 ? "是" : "否", item.CreateTime.ToString("yyyy-MM-dd HH:mm"), resultSiteNames);
            }

            lbStatusBar.Text = $"共有记录{lst.Count}条";
            
            
        }
        private void BindClass()
        {
            // 添加默认分类
            var defaultItem = new PushContentClass { Id = 0, ClassName = "所有分类" };
            var bllClass = new PushContentClassBll();
            var classList = bllClass.FindByTypeId(dataBll.IType);
            // 绑定到ToolStripComboBox
            cbClass.ComboBox.DataSource = new List<PushContentClass>(classList)
                .Prepend(defaultItem)  // 将默认项添加到开头
                .ToList();

            // 设置显示和值字段
            cbClass.ComboBox.DisplayMember = "ClassName";
            cbClass.ComboBox.ValueMember = "Id";

            // 可选：设置默认选中项
            cbClass.ComboBox.SelectedIndex = 0;
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string key = txtSearchKey.Text.Trim();
            int status = -1;
            switch (cbStatus.Text)
            {
                case "待发布":
                    status = 0;
                    break;
                case "已发布":
                    status = 1;
                    break;
            }
            BindData(status, key);
        }

        private void btnDelSels_Click(object sender, EventArgs e)
        {
            if (XS.WinFormsTools.Dialogs.ConfirmDialog("是否要删除所选数据？"))
            {
                var lst = lvData.GetSelectItems();
                int iIndex = 0;
                foreach (var item in lst)
                {
                    string id = item.Text;
                    dataBll.Delete(long.Parse(id));
                    iIndex++;
                }
                this.BindData();
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (XS.WinFormsTools.Dialogs.ConfirmDialog("是否要删除所选数据？"))
            {
                var lst = lvData.GetSelectItems();
                int iIndex = 0;
                foreach (var item in lst)
                {
                    string id = item.Text;
                    dataBll.Delete(int.Parse(id));
                    iIndex++;
                }
                this.BindData();
            }
        }

        private async void 发布ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sId = lvData.GetSelectItemValue(0);

            if (string.IsNullOrEmpty(sId))
            {
                Tips("请选择一条记录");
                return;
            }

            long Id = sId.ToLong();

            var model = dataBll.GetEntity(Id);

            if (model == null || model.Status == 1)
            {
                Tips("所选记录已发布");
                return;
            }

            var frmLoading = new FrmLoading();
            await frmLoading.ShowWinAsync(async () =>
            {
                frmLoading.ToDo(100, 0, "正在加载上传插件...");
                try
                {
                    await dataBll.StartPush(Id, (tips, imax, icurrent) =>
                    {
                        frmLoading.ToDo(imax, icurrent, tips);
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发布失败:{ex.Message}");
                }

            });
        }

        private void 重置为未发布ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sId = lvData.GetSelectItemValue(0);

            if (string.IsNullOrEmpty(sId))
            {
                Tips("请选择一条记录");
                return;
            }

            long Id = sId.ToLong();

            var model = dataBll.GetEntity(Id);

            model.Status = 0;

            dataBll.Update(model);

            this.BindData();

        }


        private void lvData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var sId = lvData.GetSelectItemValue(0);

            if (string.IsNullOrEmpty(sId))
            {
                Tips("请选择一条记录");
                return;
            }
            dataBll.OnOpenAdd(sId.ToLong());
        }
        private string GetSelJobJd => lvData.GetSelectItemValue(0);
        private void lvData_SelectedIndexChanged(object sender, EventArgs e)
        {

            // 检查是否有选中项
            if (lvData.SelectedItems.Count > 0)
            {
                // 执行你需要的操作，例如获取选中项的文本
                string jobId = GetSelJobJd;
                var model = dataBll.GetEntity(jobId.ToLong());
                lbTitle.Text = $"标题：{model.Title}";


                txtDes.Text = string.IsNullOrWhiteSpace(model.Info) ? "暂无" : $"标签:{model.Tags}\n\n{model.Info}";

                txtPushLogs.Text = string.IsNullOrWhiteSpace(model.PublishLog) ? "暂无" : $"{model.PublishLog}";

                if (!string.IsNullOrWhiteSpace(model.ImgPath))
                {
                    picContentSmallImg.LoadImageFromFilePath(model.ImgPath);

                }


            }
        }

        private void 设为已发布ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sId = lvData.GetSelectItemValue(0);

            if (string.IsNullOrEmpty(sId))
            {
                Tips("请选择一条记录");
                return;
            }

            long Id = sId.ToLong();

            var model = dataBll.GetEntity(Id);

            model.Status = 1;

            dataBll.Update(model);

            this.BindData();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var win = new EventArgsOnShowWin(new ImportDatas(dataBll), $"批量导入[{dataBll.Title}]");
            ModuleUtils.OnEvShowToRight(win);
        }

        private void btnOpenClassManager_Click(object sender, EventArgs e)
        {
            
            var win = new ClassManager(dataBll.IType);
            win.ShowDialog();
        }
    }
}
