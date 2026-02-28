using SmartMedia.Core.Comm;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms;
namespace SmartMedia.Core.SitesBase.BLL;

public class ImagePostBll : PushContentBllBase<ImagePushBase, SiteSelectorImgPost>
{
    override public int IType => 4;
    override public string Title => "图文";
    protected override XsDockContent CreateAddWindow(PushInfo model)
    {
        return new AddImagePost(this, model);
    }
    //public override Task StartPush(long Id, Action<string, int, int> CallBack)
    //{
    //    return base.PushTask<ImagePushBase>(Id, CallBack);
    //}
    //override public SiteSelector NewSiteSelector()
    //{
    //    return new SiteSelectorImgPost();
    //}


}