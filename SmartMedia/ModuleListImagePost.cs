using SmartMedia.Core;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.BLL;

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
