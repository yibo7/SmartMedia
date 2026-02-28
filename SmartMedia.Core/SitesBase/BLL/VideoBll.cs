using SmartMedia.Core.Comm;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms;

namespace SmartMedia.Core.SitesBase.BLL;


public class VideoBll : PushContentBllBase<VideoPushBase, SiteSelector>
{
    override public int IType => 1;
    override public string Title => "视频";
    protected override XsDockContent CreateAddWindow(PushInfo model)
    {
        return new AddVideo(this, model);
    }

    //override public void OnOpenAdd(long Id)
    //{

    //    var model = GetEntity(Id);
    //    var sTitle = !Equals(model,null) ? $"修改{Title}-{model.Title}" : $"发布{Title}";
    //    var addWin = new AddVideo(this, model);
    //    var win = new EventArgsOnShowWin(addWin, sTitle);
    //    ModuleUtils.OnEvShowToRight(win);

    //}

    //public override async Task StartPush(long Id, Action<string, int, int> CallBack)
    //{
    //    await base.PushTask<VideoPushBase>(Id, CallBack);
    //}

    //override public SiteSelector NewSiteSelector()
    //{
    //    return new SiteSelector();
    //}



}
