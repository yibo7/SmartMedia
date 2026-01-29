
using SmartMedia.Modules.PushContent.DB;
using SmartMedia.Plugins;
using SmartMedia.Plugins.AutoPush.Article;

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
