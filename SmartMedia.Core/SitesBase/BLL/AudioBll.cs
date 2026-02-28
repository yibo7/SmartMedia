using Newtonsoft.Json.Linq; 
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms; 
using System.Diagnostics; 

namespace SmartMedia.Core.SitesBase.BLL;


public class AudioBll : PushContentBllBase<AudioPushBase, SiteSelectorAudio>
{
    override public int IType => 2;
    override public string Title => "音频"; 
    protected override XsDockContent CreateAddWindow(PushInfo model)
    {
        return new AddAudio(this, model);
    }

    //public override Task StartPush(long Id, Action<string, int, int> CallBack)
    //{
    //    return base.PushTask<AudioPushBase>(Id, CallBack);
    //}

    //override public SiteSelector NewSiteSelector()
    //{
    //    return new SiteSelectorAudio();
    //}

    override public string ImportData(List<ImportModel> importModels)
    {
        // 遍历所有导入模型
        foreach (var model in importModels)
        {
            try
            {
                // 检查 FilePath 是否有效
                if (!string.IsNullOrEmpty(model.FilePath))
                {
                    // 获取音频文件所在的目录
                    string audioFileDirectory = Path.GetDirectoryName(model.FilePath);

                    // 构建 LRC 子目录路径
                    string lrcDirectory = Path.Combine(audioFileDirectory, "LRC");

                    // 检查 LRC 子目录是否存在
                    if (Directory.Exists(lrcDirectory))
                    {
                        // 获取音频文件名（不含扩展名）
                        string audioFileNameWithoutExt = Path.GetFileNameWithoutExtension(model.FilePath);

                        // 构建预期的 LRC 文件路径
                        string expectedLrcPath = Path.Combine(lrcDirectory, audioFileNameWithoutExt + ".lrc");

                        // 检查对应的 .lrc 文件是否存在
                        if (File.Exists(expectedLrcPath))
                        {
                            // 解析现有的 Sites JSON
                            if (!string.IsNullOrEmpty(model.Sites))
                            {
                                try
                                {
                                    var sitesJson = JObject.Parse(model.Sites);

                                    // 遍历所有平台，在每个平台下添加 LrcSrt
                                    foreach (var site in sitesJson)
                                    {
                                        var platformData = site.Value as JObject;
                                        if (platformData != null)
                                        {
                                            // 添加 LrcSrt 键值对
                                            platformData["LrcSrt"] = expectedLrcPath;
                                        }
                                    }

                                    // 更新模型的 Sites 属性
                                    model.Sites = sitesJson.ToString();

                                    Debug.WriteLine($"已为文件 {model.FilePath} 在所有平台下添加 LRC 文件路径: {expectedLrcPath}");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"解析 Sites JSON 时出错: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"处理 ImportModel 时出错: {ex.Message}");
                // 继续处理下一个模型，不中断整个导入过程
                continue;
            }
        }

        return base.ImportData(importModels);
    }

}
