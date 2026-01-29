using SmartMedia.AtqCore;
using SmartMedia.MCore;
using WeifenLuo.WinFormsUI.Docking;

namespace SmartMedia.Modules
{
    public partial class HelpMarkdownViewer : XsDockContent
    {
        public string Title => "使用帮助";//要实现模块名称
        public System.Drawing.Image Ico => Resource.help; //要实现模块图标
        VisualStudioToolStripExtender vsToolStripExtender1;
        public HelpMarkdownViewer(string title, string markdown)
        {

            CloseButtonVisible = false; // 隐藏关闭按钮 
            InitializeComponent();
            lbTitle.Text = title;

            SetHtmlAsync(markdown);
        }
        private async Task SetHtmlAsync(string markdown)
        {
            await webBox.EnsureCoreWebView2Async();
            webBox.NavigateToString(MarkDownHelper.ToHtmlFull(markdown));
        }

    }
}
