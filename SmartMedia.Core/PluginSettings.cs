using XS.Core2.Json;

namespace SmartMedia.Core;

public class PluginSettings : BaseJsonFile
{
    public static readonly PluginSettings Instance = new PluginSettings();
    public Dictionary<string, Dictionary<string, string>> CustomConfigs = new Dictionary<string, Dictionary<string, string>>();

    public PluginSettings() : base("PluginSetting/settings.json")
    {

    }
    public Dictionary<string, string> GetCustomConfigs(string key)
    {
        if (CustomConfigs.ContainsKey(key))
        {
            return CustomConfigs[key];
        }
        return null;
    }
    public void SaveCustomConfigs(string key, Dictionary<string, string> cf)
    {
        if (cf != null)
        {
            CustomConfigs[key] = cf;
            Save();

        }
    }
}
