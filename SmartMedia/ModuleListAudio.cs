
using SmartMedia.Core;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.BLL;

namespace SmartMedia
{
    public partial class ModuleListAudio : ModuleListPushBase<AudioPushBase>
    {
        public ModuleListAudio(Main main) : base(main, new AudioBll())
        {
            BindTools(PluginUtils.AudioPushList);
        }

    }
}
