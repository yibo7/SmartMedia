using SmartMedia.Core;
using SmartMedia.Core.Comm;
using SmartMedia.Core.CustomEvents;
using SmartMedia.Core.UIForms;
using SmartMedia.MCore;
using SmartMedia.Modules.Job; 
using WeifenLuo.WinFormsUI.Docking;
using XS.WinFormsTools;

namespace SmartMedia
{
    public partial class Main : HeaderBarMainForm
    {

        private XsDockContent IndexPage;
        private ModuleListArticle articleList;
        private ModuleListVideo videoList;
        private ModuleListAudio audioList;

        private ModuleListImagePost imagePost;

        public Main()
        {
            Splash.Status = "状态:初始化开始";
            InitializeComponent();
            this.Load += Main_Load;
            cxfTitleBar.ThemeColor = ColorTranslator.FromHtml("#0094BC");


            dockPanel.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();


            VisualStudioToolStripExtender vsToolStripExtender1 = new VisualStudioToolStripExtender();
            vsToolStripExtender1.SetStyle(toolMainBar, VisualStudioToolStripExtender.VsVersion.Vs2005, new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme());


            //设置左栏宽
            dockPanel.DockLeftPortion = 195;
            //dockPanel.DockRightPortion = 300;

            Splash.Status = "状态:正在加载插件...";
            PluginUtils.LoadPlugins();


            Splash.Status = "状态:正在加载扩展模块...";
            AppData.moduleList = new ModuleList(this);
            articleList = new ModuleListArticle(this);
            videoList = new ModuleListVideo(this);

            audioList = new ModuleListAudio(this);
            imagePost = new ModuleListImagePost(this);

            this.ShowContent("图文", imagePost, DockState.DockLeft); // AppData.moduleList         
            this.ShowContent("音频", audioList, DockState.DockLeft);
            this.ShowContent("文章", articleList, DockState.DockLeft);
            this.ShowContent("视频", videoList, DockState.DockLeft);

            //SiteList
            Splash.Status = "状态:加载自动化服务...";
            IndexPage = new JobList();

            this.ShowContent("定时任务", IndexPage); // JobList 并没有在模块列表中加载，所以可以直接 new JobList()
             

            #region 事件处理

            MainForm.EvCopy += (txt, e) =>
            { // 调用复现
                if (!Equals(txt, null))
                {
                    BeginInvoke(((Action)(() =>
                    {
                        Clipboard.SetText(txt.ToString());
                    })));
                }
            };

            MainForm.EvShowTips += (object? sender, TipEventArgs e) =>
            {

                FrmTips.ShowTips(this, e.Title, 2000, true, e.Alignment, null, TipsSizeMode.Medium, new Size(300, 50), e.State);
            };

            ModuleUtils.EvShowToRight += (object? win, EventArgsOnShowWin e) =>
            {
                ShowContent(e.Name, e.DockWin);
            };

            #endregion

            this.BorderColor = Color.FromArgb(207, 214, 229);


        }



        private void MainForm_EvShowTips(object? sender, TipEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Main_Load(object? sender, EventArgs e)
        {


            // 使用 BeginInvoke 确保在窗体完全显示后关闭启动画面
            this.BeginInvoke(new Action(() =>
            {
                Splash.Close();
                // 再次确保主窗体在前台
                this.Activate();
            }));

        }

        private void 首页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowContent("主页", IndexPage);
        }

        private void 工具栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowContent("工具栏", AppData.moduleList, DockState.DockLeft);
        }

        #region DockContent 操作

        /// <summary>
        /// 获取一个DockContent对象
        /// </summary>
        /// <param name="text">标签名称</param>
        /// <returns></returns>
        private DockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                {
                    if (form.Text == text)
                    {
                        return form as DockContent;
                    }
                }
                return null;
            }
            else
            {
                foreach (DockContent content in dockPanel.Documents)
                {
                    if (content.DockHandler.TabText == text)
                    {
                        return content;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// 显示一个标签窗体，程序会根据标题来寻找，是否已经加载过相同标题的窗体，如果有，就直接激活焦点
        /// </summary>
        /// <param name="Title">窗体显示名称，如果</param>
        /// <param name="formType">要加载的标签窗体，这个要自己写类，继承于 DockContent</param>
        /// <param name="dockState">窗体的显示方式，一般为左，右等等</param>
        public void ShowContent(string Title, DockContent formType, DockState dockState = DockState.Document)
        {
            try
            {
                DockContent frm = FindDocument(Title);
                if (frm == null)
                {
                    formType.DockHandler.TabText = Title;
                    formType.Show(this.dockPanel, dockState);
                }
                else
                {
                    frm.Show(this.dockPanel, dockState);
                    frm.BringToFront();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"打开窗口发生错误：{e.Message}");
            }

        }
        #endregion

         

        private void btnConfigs_Click(object sender, EventArgs e)
        {
            var dlg = new SysConfigs();
            dlg.Show();
        }

        private void btnOpenVideoList_Click(object sender, EventArgs e)
        {

            this.ShowContent("视频", videoList, DockState.DockLeft);

        }

        private void btnOpenArticleList_Click(object sender, EventArgs e)
        {

            this.ShowContent("文章", articleList, DockState.DockLeft);
        }



        private void btnLiveSreaming_Click(object sender, EventArgs e)
        {
            AppData.moduleList.OpenModuleByName("语言模型");
        }

        private void btnJobs_Click(object sender, EventArgs e)
        {
            this.ShowContent("定时任务", IndexPage);
            //IndexPage
        }

        private void btnAboutUs_Click(object sender, EventArgs e)
        {
             
        }

        private void btnAudioPush_Click(object sender, EventArgs e)
        {
            this.ShowContent("音频", audioList, DockState.DockLeft);
        }

        private void btnOpenImagePost_Click(object sender, EventArgs e)
        {
            this.ShowContent("图文", imagePost, DockState.DockLeft);
        }
    }
}