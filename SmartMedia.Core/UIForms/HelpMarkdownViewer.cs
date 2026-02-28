 
using SmartMedia.Core.Comm; 

namespace SmartMedia.Core.UIForms;

public partial class HelpMarkdownViewer : XsDockContent
{
    public string Title => "使用帮助";//要实现模块名称
    public System.Drawing.Image Ico => Resource.help; //要实现模块图标

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
