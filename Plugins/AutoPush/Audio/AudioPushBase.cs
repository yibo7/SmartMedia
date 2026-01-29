

using SmartMedia.Controls;
using SmartMedia.Modules.PushContent.DB;

namespace SmartMedia.Plugins.AutoPush.Article
{
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
                { "LrcSrt",BuildCtr<SettingSrtLrc>("选择字幕", "字幕格式一般为Lrc") }
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
}
