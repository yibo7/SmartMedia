using Newtonsoft.Json.Linq;
using SmartMedia.Core.Comm;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms; 

namespace SmartMedia.Core.SitesBase.BLL;


public  class ArticleBll : PushContentBllBase<ArticlePushBase, SiteSelectorArticle>
{
    override public int IType => 3;
    override public string Title => "文章";
    protected override XsDockContent CreateAddWindow(PushInfo model)
    {
        return new AddArticle(this, model);
    }
    //override public void OnOpenAdd(long Id)
    //{
    //    var model = GetEntity(Id);
    //    var addWin = new AddArticle(this, model);

    //    var sTitle = !Equals(model, null) ? $"修改{Title}-{model.Title}" : $"发布{Title}";

    //    var win = new EventArgsOnShowWin(addWin, sTitle);
    //    ModuleUtils.OnEvShowToRight(win);
    //}
    //public override Task StartPush(long Id, Action<string, int, int> CallBack)
    //{
    //    return base.PushTask<ArticlePushBase>(Id, CallBack);
    //}
    //override public SiteSelector NewSiteSelector()
    //{
    //    return new SiteSelectorArticle();
    //}


}

