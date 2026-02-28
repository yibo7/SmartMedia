using SmartMedia.Core.SitesBase;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace SmartMedia.Core;

public static class PluginUtils
{
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
        string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        // 确保Plugins目录存在
        if (!Directory.Exists(pluginsPath))
        {
            Directory.CreateDirectory(pluginsPath);
        }

        // 获取主程序集
        var mainAssembly = Assembly.GetExecutingAssembly();

        // 收集所有要加载的程序集（包括主程序集和插件DLL）
        var assemblies = new List<Assembly> { mainAssembly };

        // 加载Plugins目录下的所有DLL文件
        var pluginFiles = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (var pluginFile in pluginFiles)
        {
            try
            {
                // 加载插件程序集
                var assembly = Assembly.LoadFrom(pluginFile);
                assemblies.Add(assembly);

                // 可选：处理依赖项
                // 可以在这里添加依赖项解析逻辑
            }
            catch (Exception ex)
            {
                // 记录加载失败但继续加载其他插件
                Debug.WriteLine($"加载插件 {Path.GetFileName(pluginFile)} 失败: {ex.Message}");
                // 或者使用日志框架记录错误
            }
        }

        // 从所有加载的程序集中查找插件
        VideoPushList = LoadAbstractFromAssemblies<VideoPushBase>(assemblies);
        VideoPushList = VideoPushList.OrderBy(d => d.OrderIndex).ToList();

        ArticlePushList = LoadAbstractFromAssemblies<ArticlePushBase>(assemblies);
        ArticlePushList = ArticlePushList.OrderBy(d => d.OrderIndex).ToList();

        AudioPushList = LoadAbstractFromAssemblies<AudioPushBase>(assemblies);
        AudioPushList = AudioPushList.OrderBy(d => d.OrderIndex).ToList();

        ImagePushList = LoadAbstractFromAssemblies<ImagePushBase>(assemblies);
        ImagePushList = ImagePushList.OrderBy(d => d.OrderIndex).ToList();
    }

    static public List<T> LoadAbstractFromAssemblies<T>(IEnumerable<Assembly> assemblies) where T : class
    {
        var lstModules = new List<T>();

        foreach (var assembly in assemblies)
        {
            try
            {
                // 获取所有派生自T的非抽象类
                var implementations = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)));

                foreach (var impl in implementations)
                {
                    try
                    {
                        // 如果需要，可以创建实例（如果它们是公开的）
                        if (impl.IsPublic || impl.IsNestedPublic)
                        {
                            var instance = Activator.CreateInstance(impl) as T;
                            if (instance != null)
                            {
                                lstModules.Add(instance);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录单个类型实例化失败
                        Debug.WriteLine($"创建插件实例 {impl.FullName} 失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录程序集处理失败
                Debug.WriteLine($"处理程序集 {assembly.FullName} 失败: {ex.Message}");
            }
        }

        return lstModules;
    }

    // 可选：添加重载方法以保持向后兼容性
    static public List<T> loadAbstract<T>(Assembly assembly) where T : class
    {
        return LoadAbstractFromAssemblies<T>(new[] { assembly });
    }

    // 可选：添加重新加载插件的方法
    public static void ReloadPlugins()
    {
        // 清空现有列表
        VideoPushList?.Clear();
        ArticlePushList?.Clear();
        AudioPushList?.Clear();
        ImagePushList?.Clear();

        // 重新加载
        LoadPlugins();
    }

    // 可选：添加仅从Plugins目录加载的方法
    public static void LoadPluginsFromDirectory(string pluginsDirectory = "plugins")
    {
        string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginsDirectory);

        if (!Directory.Exists(pluginsPath))
        {
            throw new DirectoryNotFoundException($"插件目录不存在: {pluginsPath}");
        }

        var assemblies = new List<Assembly>();
        var pluginFiles = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (var pluginFile in pluginFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(pluginFile);
                assemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"加载插件 {Path.GetFileName(pluginFile)} 失败: {ex.Message}");
            }
        }

        // 只从插件程序集加载，不包括主程序集
        VideoPushList = LoadAbstractFromAssemblies<VideoPushBase>(assemblies);
        VideoPushList = VideoPushList.OrderBy(d => d.OrderIndex).ToList();

        ArticlePushList = LoadAbstractFromAssemblies<ArticlePushBase>(assemblies);
        ArticlePushList = ArticlePushList.OrderBy(d => d.OrderIndex).ToList();

        AudioPushList = LoadAbstractFromAssemblies<AudioPushBase>(assemblies);
        AudioPushList = AudioPushList.OrderBy(d => d.OrderIndex).ToList();

        ImagePushList = LoadAbstractFromAssemblies<ImagePushBase>(assemblies);
        ImagePushList = ImagePushList.OrderBy(d => d.OrderIndex).ToList();
    }
}