using SmartMedia.Modules.PushContent.DB;
using SmartMedia.Plugins;
using SmartMedia.Plugins.AutoPush.Article;

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
