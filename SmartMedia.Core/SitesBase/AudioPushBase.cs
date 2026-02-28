

using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;

namespace SmartMedia.Core.SitesBase;

abstract public class AudioPushBase : PushBase
{
    override protected Dictionary<string, SettingCtrBase> SiteCtrls
    {
        get
        {
         return new Dictionary<string, SettingCtrBase>()
        { 
            { "SpecialName",BuildCtr<SettingTextBox>("合集名称","可为空，你在此平台上创建的合集名称") },
            { "IsAiMake",BuildCtr<SettingCheckBox>("是否AI生成", "", "False") },
            { "LrcSrt",BuildCtr<SettingSrtLrc>("选择字幕", "字幕格式一般为Lrc，如果是导入数据会自动读取当前音频子目录‘LRC’下对应的.lrc文件") }  // 如果自定义LrcSrt控件，要保留name为LrcSrt，才能在导入数据时正确设置
        };
        }
    }
     

    protected string GetLrcFilePath()
    {
        return base.GetCtrValue("LrcSrt");
    }

    protected string GetLrcContent()
    {
        string path = GetLrcFilePath();
        string content = File.ReadAllText(path);

        return content;
    }
    protected string CheckModel(PushInfo? model)
    {

        if (Equals(model, null))
            return "model 不可为空";

        if (string.IsNullOrWhiteSpace(model.Title))
            return "音频标题不能为空";

        if (string.IsNullOrWhiteSpace(model.FilePath))
            return "音频地址不能为空";


        return "";
    }
}
