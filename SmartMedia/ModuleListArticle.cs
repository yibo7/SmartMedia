using SmartMedia.Core;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.BLL;
using SmartMedia.Modules.PushContent.DB; 

namespace SmartMedia
{
    public partial class ModuleListArticle : ModuleListPushBase<ArticlePushBase>
    {
        public ModuleListArticle(Main main) : base(main, new ArticleBll())
        {
            BindTools(PluginUtils.ArticlePushList);
        }
    }
}
