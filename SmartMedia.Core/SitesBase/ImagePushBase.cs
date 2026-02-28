
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;
using XS.Core2.XsExtensions;

namespace SmartMedia.Core.SitesBase;

abstract public class ImagePushBase : PushBase
{
    //override public Dictionary<string, SettingCtrBase> GetSiteCtrls()
    //{

    //    var result = new Dictionary<string, SettingCtrBase>();

    //    // 先添加"封面图片"
    //    result["picCoverImageList"] = base.BuildCtr<SettingCoverImage>("封面图片", "可为空，默认封面图片，一般为用来设置横屏图片,图片格式以平台规定为准，一般为4:3,16:9");

    //    // 然后添加原始的所有元素
    //    foreach (var kvp in SiteCtrls)
    //    {
    //        result[kvp.Key] = kvp.Value;
    //    }

    //    return result;
    //}

    override protected Dictionary<string, SettingCtrBase> SiteCtrls => new Dictionary<string, SettingCtrBase>()
    {
    };
    /// <summary>
    /// 获取图文列表
    /// </summary>
    /// <returns></returns>
    protected string[] GetCoverPaths(PushInfo? model)
    {
        var paths = GetCtrValue("picCoverImageList");
        if(Equals(model,null)|| string.IsNullOrWhiteSpace(model.FilePath)) 
            return [];

        return model.FilePath.SplitByWrap();
    }
     

    protected string CheckModel(PushInfo? model)
    {

        if (Equals(model, null))
            return "model 不可为空";

        if (string.IsNullOrWhiteSpace(model.Title))
            return "图文标题不能为空";
         


        return "";
    }
}
