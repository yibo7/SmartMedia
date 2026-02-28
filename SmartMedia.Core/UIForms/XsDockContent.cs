
using SmartMedia.Core.Comm;
using SmartMedia.Core.CustomEvents; 
using WeifenLuo.WinFormsUI.Docking;
using XS.WinFormsTools;

namespace SmartMedia.Core.UIForms;

public class XsDockContent : DockContent
{
    protected Dictionary<string, object> Prams;
    public XsDockContent()
    {

        this.BackColor = Color.White;

    }
    protected void Tips(string sTitle, TipsState tipsState = TipsState.Success, ContentAlignment alignment = ContentAlignment.MiddleCenter)
    {
        MainForm.OnShowTips(sTitle, tipsState, alignment);
    }
    /// <summary>
    /// 向主窗口打开一个DockContent
    /// </summary>
    /// <param name="sTitle">标题，一样的标题永远只打开一个</param>
    /// <param name="dockContent"></param>
    protected void OpenDockToMain(string sTitle, DockContent dockContent)
    {
        var win = new EventArgsOnShowWin(dockContent, sTitle);
        ModuleUtils.OnEvShowToRight(win);
    }
    protected void OpenHelpDockToMain(string sTitle, string sMarkdown)
    {
        OpenDockToMain(sTitle, new HelpMarkdownViewer(sTitle, sMarkdown));
    }

    virtual public void InitPrams(Dictionary<string, object> prams)
    {
        Prams = prams;
    }
    /// <summary>
    /// 修复，打开标签后再关闭会出错实例被释放的BUG
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (!Equals(this.Parent, null))
                this.Hide();
        }
        catch (Exception)
        {
            base.Dispose(disposing);
        }
    }

}
