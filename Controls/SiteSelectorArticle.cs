using SmartMedia.Plugins;
using SmartMedia.Plugins.AutoPush;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartMedia.Controls
{
    public class SiteSelectorArticle : SiteSelector
    {
        override protected List<PushBase> GetPushPlugins()
        {
            //获取所有发布插件
            return PluginUtils.ArticlePushList.Cast<PushBase>().ToList();

        }
    }
}
