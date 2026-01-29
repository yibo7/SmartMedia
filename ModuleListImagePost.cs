using SmartMedia.Modules.PushContent.DB;
using SmartMedia.Plugins;
using SmartMedia.Plugins.AutoPush.ImagePosts;

namespace SmartMedia
{
    public partial class ModuleListImagePost : ModuleListPushBase<ImagePushBase>
    {
        public ModuleListImagePost(Main main) : base(main, new ImagePostBll())
        {
            BindTools(PluginUtils.ImagePushList);
        }
    }
}
