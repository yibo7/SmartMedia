

//using Newtonsoft.Json.Linq;
//using SmartMedia.Core.Comm;
//using SmartMedia.Core.Controls;
//using System.Diagnostics;

//namespace SmartMedia.Core.SitesBase.DB;

//public class VideoBll : PushContentBllBase
//{
//    override public int IType => 1;
//    override public string Title => "视频";

//    override public void OnOpenAdd(long Id)
//    {
//        if (Id > 0)
//        {
//            var model = GetEntity(Id);
//            var addWin = new AddVideo(this, model);
//            var win = new EventArgsOnShowWin(addWin, $"修改视频-{model.Title}");
//            ModuleUtils.OnEvShowToRight(win);
//        }
//        else
//        {
//            var addWin = new AddVideo(this, null);
//            var win = new EventArgsOnShowWin(addWin, $"新建视频");
//            ModuleUtils.OnEvShowToRight(win);
//        }


//    }

//    public override async Task StartPush(long Id, Action<string, int, int> CallBack)
//    {
//        await base.PushTask<VideoPushBase>(Id, CallBack);
//    }

//    override public SiteSelector NewSiteSelector()
//    {
//        return new SiteSelector();
//    }



//}


//internal class AudioBll : PushContentBllBase
//{
//    override public int IType => 2;
//    override public string Title => "音频";
//    override public void OnOpenAdd(long Id)
//    {
//        var model = GetEntity(Id);
//        var addWin = new AddAudio(this, model);
//        var win = new EventArgsOnShowWin(addWin, "发布音频");
//        ModuleUtils.OnEvShowToRight(win);

//    }

//    public override Task StartPush(long Id, Action<string, int, int> CallBack)
//    {
//        return base.PushTask<AudioPushBase>(Id, CallBack);
//    }

//    override public SiteSelector NewSiteSelector()
//    {
//        return new SiteSelectorAudio();
//    }

//    override public string ImportData(List<ImportModel> importModels)
//    {
//        // 遍历所有导入模型
//        foreach (var model in importModels)
//        {
//            try
//            {
//                // 检查 FilePath 是否有效
//                if (!string.IsNullOrEmpty(model.FilePath))
//                {
//                    // 获取音频文件所在的目录
//                    string audioFileDirectory = Path.GetDirectoryName(model.FilePath);

//                    // 构建 LRC 子目录路径
//                    string lrcDirectory = Path.Combine(audioFileDirectory, "LRC");

//                    // 检查 LRC 子目录是否存在
//                    if (Directory.Exists(lrcDirectory))
//                    {
//                        // 获取音频文件名（不含扩展名）
//                        string audioFileNameWithoutExt = Path.GetFileNameWithoutExtension(model.FilePath);

//                        // 构建预期的 LRC 文件路径
//                        string expectedLrcPath = Path.Combine(lrcDirectory, audioFileNameWithoutExt + ".lrc");

//                        // 检查对应的 .lrc 文件是否存在
//                        if (File.Exists(expectedLrcPath))
//                        {
//                            // 解析现有的 Sites JSON
//                            if (!string.IsNullOrEmpty(model.Sites))
//                            {
//                                try
//                                {
//                                    var sitesJson = JObject.Parse(model.Sites);

//                                    // 遍历所有平台，在每个平台下添加 LrcSrt
//                                    foreach (var site in sitesJson)
//                                    {
//                                        var platformData = site.Value as JObject;
//                                        if (platformData != null)
//                                        {
//                                            // 添加 LrcSrt 键值对
//                                            platformData["LrcSrt"] = expectedLrcPath;
//                                        }
//                                    }

//                                    // 更新模型的 Sites 属性
//                                    model.Sites = sitesJson.ToString();

//                                    Debug.WriteLine($"已为文件 {model.FilePath} 在所有平台下添加 LRC 文件路径: {expectedLrcPath}");
//                                }
//                                catch (Exception ex)
//                                {
//                                    Debug.WriteLine($"解析 Sites JSON 时出错: {ex.Message}");
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"处理 ImportModel 时出错: {ex.Message}");
//                // 继续处理下一个模型，不中断整个导入过程
//                continue;
//            }
//        }

//        return base.ImportData(importModels);
//    }

//}
//internal class ArticleBll : PushContentBllBase
//{
//    override public int IType => 3;
//    override public string Title => "文章";
//    override public void OnOpenAdd(long Id)
//    {
//        var model = GetEntity(Id);
//        var addWin = new AddArticle(this, model);
//        var win = new EventArgsOnShowWin(addWin, "发布文章");
//        ModuleUtils.OnEvShowToRight(win);
//    }
//    public override Task StartPush(long Id, Action<string, int, int> CallBack)
//    {
//        return base.PushTask<ArticlePushBase>(Id, CallBack);
//    }
//    override public SiteSelector NewSiteSelector()
//    {
//        return new SiteSelectorArticle();
//    }


//}

//internal class ImagePostBll : PushContentBllBase
//{
//    override public int IType => 4;
//    override public string Title => "图文";
//    override public void OnOpenAdd(long Id)
//    {
//        var model = GetEntity(Id);
//        var addWin = new AddImagePost(this, model);
//        var win = new EventArgsOnShowWin(addWin, "发布图文");
//        ModuleUtils.OnEvShowToRight(win);
//    }
//    public override Task StartPush(long Id, Action<string, int, int> CallBack)
//    {
//        return base.PushTask<ImagePushBase>(Id, CallBack);
//    }
//    override public SiteSelector NewSiteSelector()
//    {
//        return new SiteSelectorImgPost();
//    }


//}
