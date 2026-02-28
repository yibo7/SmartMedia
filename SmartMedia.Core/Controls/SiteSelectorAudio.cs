
using SmartMedia.Core.SitesBase;

namespace SmartMedia.Core.Controls;

public class SiteSelectorAudio: SiteSelector
{
    override protected List<PushBase> GetPushPlugins()
    {
        //获取所有发布插件
        return PluginUtils.AudioPushList.Cast<PushBase>().ToList();

    }
}
