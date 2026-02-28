namespace SmartMedia;

using SmartMedia.Core.Comm;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms;
using SmartMedia.MCore;
using SmartMedia.Modules;
using SmartMedia.Modules.PushContent.DB;
using SmartMedia.Modules.VideoManageModule; 
using System;
using WeifenLuo.WinFormsUI.Docking;
public partial class ModuleListPushBase : DockContent
{
    protected Main _main;
    protected ModuleMain pushContent;
    protected IPushContentBll dataBll;


    public ModuleListPushBase(Main main, IPushContentBll bll)
    {
        InitializeComponent();

        //if (DesignMode)
        //{
        //    // 设计模式下的简化初始化
        //    return;
        //}

        dataBll = bll;
        CloseButtonVisible = false; // 隐藏关闭按钮 

        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _main = main;
        //BindTools();

        panel2.Padding = new Padding(15, 0, 0, 0); // 设置Panel的内边距       
        btnAddVideo.Text = $"管理{bll.Title}";

        pushContent = new Modules.VideoManageModule.ModuleMain(bll);
    }


    private void btnAddVideo_Click(object sender, EventArgs e)
    {
        _main.ShowContent(btnAddVideo.Text, pushContent);
    }
    private void btnPush_Click(object sender, EventArgs e)
    {
        dataBll.OnOpenAdd(0);
    }
}


public class ModuleListPushBase<T> : ModuleListPushBase where T : PushBase
{
    public ImgListItemBll<T> Wins;
    //private Main _main;
    //private ModuleMain pushContent;
    //private PushContentBllBase dataBll;

    public ModuleListPushBase(Main main, IPushContentBll bll) : base(main, bll)
    {
    }

    protected void BindTools(List<T> plugins)
    {
        Wins = new ImgListItemBll<T>(this.lvTools);
        Wins.OnMouseDoubleClick += OnBindToolMouseDoubleClick;

        foreach (var plugin in plugins)
        {
            Wins.Add(plugin.PluginName, plugin.IcoName, plugin);
        }
        Wins.Bind();
    }
    private void OnBindToolMouseDoubleClick(ImgListItem<T> item)
    {
        var autoPush = item.DockContentObj; 
        XsDockContent winDock = autoPush.IsPushFromApi? new AutoPushFromApi(autoPush): new AutoPushSite(autoPush);
        _main.ShowContent(item.Name, winDock);
    }

    private void btnAddVideo_Click(object sender, EventArgs e)
    {
        _main.ShowContent(btnAddVideo.Text, pushContent);
    }
    private void btnPush_Click(object sender, EventArgs e)
    {
        dataBll.OnOpenAdd(0);
    }
}