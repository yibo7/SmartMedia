using System.Diagnostics;
using XS.Core2; 

namespace SmartMedia.Core;

abstract public class PluginBase //: Iplugin
{
    abstract public string PluginName { get; }

     

    protected void LogErr(string err)
    {

        LogHelper.Error<PluginBase>(err);
    }
    protected void PrintLog(string log)
    {
        Debug.WriteLine($"【{this.PluginName}】:{log}");
    }
    //public bool IsEnable { get; set; }

    virtual public string Help => $"暂无{PluginName}的使用说明";


    

    /// <summary>
    /// 用来记录最日志
    /// </summary>
    private QueuFixed<string> logPlugin;
    /// <summary>
    /// 也可看作是插件的Id,因为可以用ClassName来查找相应的插件，所以同类插件的ClassName一定不能一样
    /// </summary>
    public string ClassName;
    public string ClassNameFull;
    protected PluginBase()
    {
        Type t = GetType();
        ClassName = t.Name;
        ClassNameFull = t.FullName;
        InitCustomConfigs();
        logPlugin = new QueuFixed<string>(Settings.Instance.MaxLastReport);
        //CustomConfigs = new Dictionary<string, string>();
    }

    #region 基于插件本身的配置，参考公众号图文插件

    public Dictionary<string, string> CustomConfigs = new Dictionary<string, string>();
    public bool IsEnable { get; set; }
    private void InitCustomConfigs()
    {
        var cf = PluginSettings.Instance.GetCustomConfigs(ClassNameFull);
        if (cf == null)
        {
            cf = InitConfig();
            cf.Add("IsEnable", "false");
        }

        CustomConfigs = cf;

        if (CustomConfigs["IsEnable"] == "true")
            IsEnable = true;
    }
    /// <summary>
    /// 设置自动定义配置，key为键值，value为默认值
    /// </summary>
    /// <returns></returns> 
    virtual public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
        { "API地址","******"}
    };

    public string GetCf(string key)
    {
        var cf = CustomConfigs;// GetCustomConfigs();
        if (cf.ContainsKey(key))
        {
            return cf[key];
        }
        return "";
    }

    ///// <summary>
    ///// 临时更新插件的配置，不做保留，只为当时使用
    ///// </summary>
    ///// <param name="cf"></param>
    //public void UpdateCf(Dictionary<string,string> cf)
    //{
    //    CustomConfigs = cf;
    //}
    public void SaveCf()
    {
        if (CustomConfigs["IsEnable"] == "true")
            IsEnable = true;
        else
            IsEnable = false;
        PluginSettings.Instance.SaveCustomConfigs(this.ClassNameFull, CustomConfigs);

        OnChangeSetting();
    }
    virtual public void OnChangeSetting()
    {

    }
    virtual public async Task OnBtnActionAsync(string actionname)
    {

    }
    /// <summary>
    /// 自定义按钮事件，key为按钮文本，value为按钮图片名称，比如在资源中Resource.bricks;这只要填写bricks即可.
    /// </summary>
    virtual public Dictionary<string, string> BtnActionNames => new Dictionary<string, string>();

    #endregion

    public string LastLog = "";
    public void AddLog(string log)
    {
        LastLog = log;
        logPlugin.Enqueue(log);
    }
    public QueuFixed<string> LastReports()
    {
        return logPlugin;
    }




    //private void InitCustomConfigs()
    //{
    //    var cf = PluginSettings.Instance.GetCustomConfigs(ClassNameFull);
    //    if (cf == null)
    //    {
    //        cf = InitConfig();
    //        cf.Add("IsEnable", "false");
    //    }

    //    CustomConfigs = cf;

    //    if (CustomConfigs["IsEnable"] == "true")
    //        IsEnable = true;
    //}

    /// <summary>
    /// 用来缓存用户自定义控件的设置
    /// </summary>
    //public Dictionary<string, string> CustomConfigs { get; }

    
    ///// <summary>
    ///// 临时更新插件的配置，不做保留，只为当时使用
    ///// </summary>
    ///// <param name="cf"></param>
    //public void UpdateCf(Dictionary<string,string> cf)
    //{
    //    CustomConfigs = cf;
    //}
    //public void SaveCf()
    //{
    //    //if (CustomConfigs["IsEnable"] == "true")
    //    //    IsEnable = true;
    //    //else
    //    //    IsEnable = false;
    //    PluginSettings.Instance.SaveCustomConfigs(this.ClassNameFull, CustomConfigs);

    //    OnChangeSetting();
    //}


    ///// <summary>
    ///// 保存配置
    ///// </summary>
    ///// <param name="key">指定键</param>
    ///// <param name="value">指定值</param>
    //public void SaveCf(string key, string value)
    //{
    //    var cf = GetCustomConfigs();
    //    cf[key] = value;
    //    JobSetModel.Instance.SaveCustomConfigs(this.ClassName, cf);
    //}

    //protected int GetCfToInt(string key)
    //{
    //    return GetCf(key).ToInt();
    //}
    //protected long GetCfToLong(string key)
    //{
    //    return GetCf(key).ToLong();
    //}
    //protected float GetCfToFloat(string key)
    //{
    //    return GetCf(key).ToFloat();
    //}
}
