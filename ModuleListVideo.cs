using SmartMedia.Modules.PushContent.DB;
using SmartMedia.Plugins;
using SmartMedia.Plugins.AutoPush.Video;

namespace SmartMedia
{
    public partial class ModuleListVideo : ModuleListPushBase<VideoPushBase>
    {
        public ModuleListVideo(Main main) : base(main, new VideoBll())
        {
            BindTools(PluginUtils.VideoPushList);
        }
    }
}
