

using SmartMedia.Controls;
using SmartMedia.MCore;
using SmartMedia.Modules.VideoManageModule;
using SmartMedia.Plugins.AutoPush.Article;
using SmartMedia.Plugins.AutoPush.ImagePosts;
using SmartMedia.Plugins.AutoPush.Video;

namespace SmartMedia.Modules.PushContent.DB;

internal class VideoBll : PushContentBllBase
{
    override protected int IType => 1;
    override public string Title => "视频";

    override public void OnOpenAdd(long Id)
    {
        if (Id > 0)
        {
            var model = GetEntity(Id);
            var addWin = new AddVideo(model);
            var win = new EventArgsOnShowWin(addWin, $"修改视频-{model.Title}");
            ModuleUtils.OnEvShowToRight(win);
        }
        else
        {
            var addWin = new AddVideo();
            var win = new EventArgsOnShowWin(addWin, $"新建视频");
            ModuleUtils.OnEvShowToRight(win);
        }
       

    }

    public override async Task StartPush(long Id, Action<string, int, int> CallBack)
    {
        await base.PushTask<VideoPushBase>(Id, CallBack);
    }

    override public SiteSelector NewSiteSelector()
    {
        return new SiteSelector();
    }

   

}


internal class AudioBll : PushContentBllBase
{
    override protected int IType => 2;
    override public string Title => "音频";
    override public void OnOpenAdd(long Id)
    {
        var model = GetEntity(Id);
        var addWin = new AddAudio(model);
        var win = new EventArgsOnShowWin(addWin, "发布音频");
        ModuleUtils.OnEvShowToRight(win);

    }

    public override Task StartPush(long Id, Action<string, int, int> CallBack)
    {

        return base.PushTask<AudioPushBase>(Id, CallBack);
    }

    override public SiteSelector NewSiteSelector()
    {
        return new SiteSelectorAudio();
    }

 
}
internal class ArticleBll : PushContentBllBase
{
    override protected int IType => 3;
    override public string Title => "文章";
    override public void OnOpenAdd(long Id)
    {
        var model = GetEntity(Id);
        var addWin = new AddArticle(model);
        var win = new EventArgsOnShowWin(addWin, "发布文章");
        ModuleUtils.OnEvShowToRight(win);
    }
    public override Task StartPush(long Id, Action<string, int, int> CallBack)
    {
        return base.PushTask<ArticlePushBase>(Id, CallBack);
    }
    override public SiteSelector NewSiteSelector()
    {
        return new SiteSelectorArticle();
    }
   

}

internal class ImagePostBll : PushContentBllBase
{
    override protected int IType => 4;
    override public string Title => "图文";
    override public void OnOpenAdd(long Id)
    {
        var model = GetEntity(Id);
        var addWin = new AddImagePost(model);
        var win = new EventArgsOnShowWin(addWin, "发布图文");
        ModuleUtils.OnEvShowToRight(win);
    }
    public override Task StartPush(long Id, Action<string, int, int> CallBack)
    {
        return base.PushTask<ImagePushBase>(Id, CallBack);
    }
    override public SiteSelector NewSiteSelector()
    {
        return new SiteSelectorImgPost();
    }
 

}
