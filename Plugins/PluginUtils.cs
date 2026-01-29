using SmartMedia.Plugins.AutoPush.Article;
using SmartMedia.Plugins.AutoPush.ImagePosts;
using SmartMedia.Plugins.AutoPush.Video;
using System.Reflection;

namespace SmartMedia.Plugins
{
    public static class PluginUtils
    {

        //private static readonly object lockObject = new object();



        public static List<VideoPushBase> VideoPushList = null;
        public static List<ArticlePushBase> ArticlePushList = null;
        public static List<AudioPushBase> AudioPushList = null;

        public static List<ImagePushBase> ImagePushList = null;


        static public T GetByClasName<T>(string ClassName) where T : PluginBase
        {
            IEnumerable<PluginBase> values = new List<PluginBase>();
            var pluginType = typeof(T);

            if (typeof(VideoPushBase).IsAssignableFrom(pluginType))
            {
                values = VideoPushList;
            }
            else if (typeof(ArticlePushBase).IsAssignableFrom(pluginType))
            {
                values = ArticlePushList;
            }
            else if (typeof(AudioPushBase).IsAssignableFrom(pluginType))
            {
                values = AudioPushList;
            }
            else if (typeof(ImagePushBase).IsAssignableFrom(pluginType))
            {
                values = ImagePushList;
            }
            else
            {
                return null;
            }

            foreach (T t in values)
            {
                if (t.ClassName == ClassName)
                    return t;
            }
            return null;
        }

        public static void LoadPlugins()
        {

            // 获取包含抽象内部类的程序集
            Assembly assembly = Assembly.GetExecutingAssembly();

            VideoPushList = loadAbstract<VideoPushBase>(assembly);
            VideoPushList = VideoPushList.OrderBy(d => d.OrderIndex).ToList();

            ArticlePushList = loadAbstract<ArticlePushBase>(assembly);
            ArticlePushList = ArticlePushList.OrderBy(d => d.OrderIndex).ToList();

            AudioPushList = loadAbstract<AudioPushBase>(assembly);
            AudioPushList = AudioPushList.OrderBy(d => d.OrderIndex).ToList();

            ImagePushList = loadAbstract<ImagePushBase>(assembly);
            ImagePushList = ImagePushList.OrderBy(d => d.OrderIndex).ToList();

            
        }

        static public List<T> loadAbstract<T>(Assembly assembly) where T : class
        {
            List<T> lstModules = new List<T>();
            // 获取所有派生自TTSBase的非抽象类
            var ttsImplementations = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)));

            foreach (var ttsImpl in ttsImplementations)
            {

                // 如果需要，可以创建实例（如果它们是公开的）
                if (ttsImpl.IsPublic || ttsImpl.IsNestedPublic)
                {
                    var instance = Activator.CreateInstance(ttsImpl) as T;
                    lstModules.Add(instance);
                }
            }

            return lstModules;
        }

    }
}
