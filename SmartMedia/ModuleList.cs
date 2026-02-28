using SmartMedia.Core.Comm;
using SmartMedia.Core.UIForms;
using SmartMedia.MCore;
using WeifenLuo.WinFormsUI.Docking;

namespace SmartMedia
{
    public partial class ModuleList : DockContent
    {
        public ImgListItemBll<DockContent> Wins;
        private Main _main;
        public ModuleList(Main main)
        {
            CloseButtonVisible = false; // 隐藏关闭按钮 
            InitializeComponent();
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _main = main;
            BindTools();

            panel2.Padding = new Padding(15, 0, 0, 0); // 设置Panel的内边距

        }
        public void OpenModule(string name, DockContent doc)
        {
            _main.ShowContent(name, doc);
        }
        public void OpenModuleByName(string name, Dictionary<string, object>? prams = null)
        {
            foreach (var item in Wins.Items)
            {
                if (item.Name == name)
                {
                    if (prams != null)
                    {
                        var win = item.DockContentObj as XsDockContent;
                        win.InitPrams(prams);
                    }
                    _main.ShowContent(item.Name, item.DockContentObj as DockContent);
                }
            }
        }

        private void BindTools()
        {
            Wins = new ImgListItemBll<DockContent>(this.lvTools);
            Wins.OnMouseDoubleClick += OnBindToolMouseDoubleClick;
            //获取模块
            var modules = ModuleUtils.LoadModules();
            foreach (var md in modules)
            {
                DockContent mDockContent = md.Value as DockContent;
                Wins.Add(md.Key, md.Value.Ico, mDockContent);
            }
            Wins.Bind();
        }
        private void OnBindToolMouseDoubleClick(ImgListItem<DockContent> item)
        {
            _main.ShowContent(item.Name, item.DockContentObj);
        }
        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{ 
        //    //base.OnFormClosing(e); 
        //}
        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    // 设置 e.Cancel = true 以阻止关闭
        //    e.Cancel = true;
        //    Visible = false; // 隐藏窗体
        //}
    }
}
