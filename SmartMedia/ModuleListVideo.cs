using SmartMedia.Core;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.BLL;

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
