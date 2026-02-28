 
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;

namespace SmartMedia.Core.SitesBase
{
    abstract public class VideoPushBase : PushBase
    {
        override public Dictionary<string, SettingCtrBase> GetSiteCtrls()
        {

            var result = new Dictionary<string, SettingCtrBase>();

            // 先添加"封面图片"
            result["picCoverImage"] = base.BuildCtr<SettingCoverImage>("封面图片", "可为空，默认封面图片，一般为用来设置横屏图片,图片格式以平台规定为准，一般为4:3,16:9");

            // 然后添加原始的所有元素
            foreach (var kvp in SiteCtrls)
            {
                result[kvp.Key] = kvp.Value;
            }

            return result;
        }
        /// <summary>
        /// 获取站点单独配置的图片路径
        /// </summary>
        /// <returns></returns>
        protected string GetCoverPath()
        {
            return GetCtrValue("picCoverImage");
        }

        // 辅助方法：格式化字节大小
        protected string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        protected string CheckModel(PushInfo? model) {

            if (Equals(model, null))
                return "model 不可为空";

            if (string.IsNullOrWhiteSpace(model.Title))
                return "视频标题不能为空";

            if (string.IsNullOrWhiteSpace(model.FilePath))
                return "视频地址不能为空";


            return "";
        }

    }
}
