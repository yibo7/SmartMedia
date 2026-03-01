using Google.Apis.YouTube.v3.Data;
using Microsoft.Playwright;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Plugins.AutoPush.Video.Youtube;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using XS.Core2.XsExtensions;

namespace SmartMedia.Sites.Videos.Youtube;

public class YouTube : VideoPushBase
{
    override public int OrderIndex => 10;
    override public string CategoryFileName => "YouTube.json"; 
    override public int CategoryLeve => 1;
    /// <summary>
    /// UploadPage 与 DataPage为空，将视为API上传插件，打开平台时会有专门管理界面，否则将以平台后台页面为管理界面
    /// </summary>
    protected override string UploadPage => "";
    /// <summary>
    /// UploadPage 与 DataPage为空，将视为API上传插件，打开平台时会有专门管理界面，否则将以平台后台页面为管理界面
    /// </summary>
    public override string DataPage => "";

    public override string PluginName => "YouTube";
    override public Image IcoName => Resource.youtube;

    private YouTubeUploadService youTube;
    public YouTube() {

        var SecretPath = Path.Combine(Application.StartupPath, "Datas\\Secrets\\yt_client_secret.json");
        youTube = new YouTubeUploadService(SecretPath, "SmartMediaWinUser");

    }
    //override protected Dictionary<string, SettingItem> SiteCtrls => new Dictionary<string, SettingItem>() {
    //    { "ClassName",new SettingItem(CtrlType.Categorie,"",value:"人物和博客")},{ "PlayListName",new SettingItem(CtrlType.TextBox,title:"播放列表",tip:"可为空，播放列表名称")}
    //};
    override protected Dictionary<string, SettingCtrBase> SiteCtrls
    {
        get
        {
            return new Dictionary<string, SettingCtrBase>()
            {
                { "ClassName",BuildCtr<SettingCategories>("发布分类",initData:CategoryFileName,defaultValue:"人物和博客")},
                { "PlayList",BuildCtr<YoutubePlaylist>("播放列表","可为空，你在Youtube的播放列表名称") }
            };
        }
    }

    /// <summary>
    /// 展示频道的统计信息，返回Markdown代码，灵活配置
    /// </summary>
    /// <returns></returns>
    override public async Task<string> GetUserInfo()
    {
        var sbInfo = new StringBuilder();
        // 调用异步方法获取 Channel
        try
        {
            var data = await youTube.GetMyChannelAsync();

            if (data != null)
            {
                // 开始构建Markdown表格
                sbInfo.AppendLine("| 项目 | 内容 |");
                sbInfo.AppendLine("|------|------|");

                // 频道名称
                sbInfo.AppendLine($"| 频道名称 | {data.Snippet?.Title ?? ""} |");

                // 频道图片 - 使用Markdown图片语法
                var thumbnailUrl = data.Snippet?.Thumbnails?.High?.Url ?? "";
                if (!string.IsNullOrEmpty(thumbnailUrl))
                {
                    sbInfo.AppendLine($"| 频道图片 | ![]({thumbnailUrl}){{width=150 height=150}} |");
                }
                else
                {
                    sbInfo.AppendLine($"| 频道图片 | 无 |");
                }

                // 频道简介
                var description = data.Snippet?.Description ?? "";
                // 处理简介中的换行符，用<br>代替以便在表格中显示
                description = description.Replace("\n", "<br>");
                sbInfo.AppendLine($"| 频道简介 | {description} |");

                // 统计信息
                sbInfo.AppendLine($"| 订阅数 | {data.Statistics?.SubscriberCount?.ToString() ?? "0"} |");
                sbInfo.AppendLine($"| 视频数 | {data.Statistics?.VideoCount?.ToString() ?? "0"} |");
                sbInfo.AppendLine($"| 总观看数 | {data.Statistics?.ViewCount?.ToString() ?? "0"} |");

                // 自定义 URL
                if (!string.IsNullOrEmpty(data.Snippet?.CustomUrl))
                    sbInfo.AppendLine($"| 频道链接 | {data.Snippet.CustomUrl} |");

                // 国家 / 地区
                if (!string.IsNullOrEmpty(data.Snippet?.Country))
                    sbInfo.AppendLine($"| 国家 | {data.Snippet.Country} |");

                // 频道创建时间
                if (data.Snippet?.PublishedAtDateTimeOffset != null)
                    sbInfo.AppendLine($"| 创建时间 | {data.Snippet.PublishedAtDateTimeOffset.Value.ToString("yyyy-MM-dd")} |");

                // 上传列表ID
                if (!string.IsNullOrEmpty(data.ContentDetails?.RelatedPlaylists?.Uploads))
                    sbInfo.AppendLine($"| 上传列表ID | {data.ContentDetails.RelatedPlaylists.Uploads} |");
            }
            else
            {
                return "获取到的数据实例为空！";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return sbInfo.ToString();
    }

    /// <summary>
    /// 获取当前用户的YouTube播放列表（合集）并返回字典<列表ID，列表名称>
    /// </summary> 
    /// <param name="IsRefesh">是否强制更新</param>
    /// <returns>包含播放列表ID和名称的字典</returns>
    public async Task<Dictionary<string, string>> GetPlayListAsync(bool IsRefesh = false)
    {
        try
        {
            // 确保目录存在
            string directoryPath = Path.Combine(AppContext.BaseDirectory, "Datas", "Categorys");

            // 第一个文件：只包含null值的字典（您要的格式）
            string filePathClassData = Path.Combine(directoryPath, CategoryFileName);

            // 第二个文件：包含ID映射的字典（原始功能保留）
            string filePathIdMapping = Path.Combine(directoryPath, "YouTubePlayListDict.json");

            // 创建目录（如果不存在）
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 如果不是强制刷新且缓存文件存在，则读取缓存
            if (!IsRefesh && File.Exists(filePathIdMapping))
            {
                try
                {
                    string cachedJson = await File.ReadAllTextAsync(filePathIdMapping);
                    var cachedData = JsonSerializer.Deserialize<Dictionary<string, string>>(cachedJson);
                    if (cachedData != null && cachedData.Count > 0)
                    {
                        PrintLog($"从缓存读取播放列表，共 {cachedData.Count} 个列表");
                        return cachedData;
                    }
                }
                catch (Exception cacheEx)
                {
                    PrintLog($"读取缓存失败，将重新获取: {cacheEx.Message}");
                }
            }

            // 如果强制刷新或缓存不存在/无效，则调用API获取数据
            var data = await youTube.GetMyPlaylistsAsync();

            // 创建结果字典
            var playlists = new Dictionary<string, string>();

            // 遍历每个播放列表
            foreach (var playlist in data)
            {
                string playlistId = playlist.Id?.Trim();
                string playlistName = playlist.Snippet?.Title?.Trim();

                // 确保ID和名称都不为空
                if (!string.IsNullOrEmpty(playlistId) && !string.IsNullOrEmpty(playlistName))
                {
                    // 添加到字典
                    playlists[playlistId] = playlistName;

                    // 打印日志（根据你的 PrintLog 方法）
                    PrintLog($"播放列表: {playlistName} (ID: {playlistId})");
                }
                else
                {
                    PrintLog($"跳过无效的播放列表条目");
                }
            }

            // 保存到缓存文件
            if (playlists.Count > 0)
            {
                try
                {
                    string json = JsonSerializer.Serialize(playlists, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    await File.WriteAllTextAsync(filePathIdMapping, json);
                    PrintLog($"播放列表已缓存到: {filePathIdMapping}");
                }
                catch (Exception saveEx)
                {
                    PrintLog($"缓存保存失败: {saveEx.Message}");
                }
            }

            // 如果没有找到任何播放列表，返回空字典而不是null
            return playlists ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            PrintLog($"获取播放列表失败: {ex.Message}");
            // 根据需求，这里可以抛出异常或返回空字典
            return new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// 更新平台分类数据到本地缓存文件
    /// </summary>
    /// <returns></returns>
    override public async Task<string> UpdateClassData()
    {
        string ErrInfo = string.Empty;
        try
        {
            // 调用API获取分类数据
            var data = await youTube.GetVideoCategoriesAsync("CN", "zh-CN");

            if (data != null && data.Count > 0)
            {
                // 创建字典，以分类名称为键，所有值设置为null
                var categoryDict = new Dictionary<string, object>();

                foreach (var category in data)
                {
                    string categoryName = category.Snippet?.Title?.Trim();

                    // 确保名称不为空
                    if (!string.IsNullOrEmpty(categoryName))
                    {
                        // 处理重复的分类名称（如果存在）
                        if (categoryDict.ContainsKey(categoryName))
                        {
                            // 为重复名称添加后缀
                            int counter = 1;
                            string newKey;
                            do
                            {
                                newKey = $"{categoryName}_{counter}";
                                counter++;
                            } while (categoryDict.ContainsKey(newKey));

                            categoryDict[newKey] = null;
                            //Console.WriteLine($"注意：分类名称 '{categoryName}' 重复，已保存为 '{newKey}'");
                        }
                        else
                        {
                            categoryDict[categoryName] = null;
                        }
                    }
                }

                if (categoryDict.Count > 0)
                {
                    // 确保目录存在
                    string directoryPath = Path.Combine(AppContext.BaseDirectory, "Datas", "Categorys");

                    // 第一个文件：只包含null值的字典（您要的格式）
                    string filePathClassData = Path.Combine(directoryPath, CategoryFileName);

                    // 第二个文件：包含ID映射的字典（原始功能保留）
                    string filePathIdMapping = Path.Combine(directoryPath, "YouTubeDict.json");

                    // 创建目录（如果不存在）
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // 1. 生成所有值为null的JSON文件
                    string jsonContentNull = JsonSerializer.Serialize(
                        categoryDict,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                            // 确保null值被序列化为null而不是被忽略
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
                        }
                    );

                    await File.WriteAllTextAsync(filePathClassData, jsonContentNull, Encoding.UTF8);
                    
                    //int count = 0;
                    //foreach (var key in categoryDict.Keys.Take(10))
                    //{
                    //    Console.WriteLine($"  - {key}");
                    //    count++;
                    //}
                    //if (categoryDict.Count > 10)
                    //{
                    //    Console.WriteLine($"  ... 还有 {categoryDict.Count - 10} 个分类");
                    //}

                    // 2. 同时生成带ID映射的字典文件（原始功能）
                    var idMappingDict = new Dictionary<string, string>();
                    foreach (var category in data)
                    {
                        string categoryName = category.Snippet?.Title?.Trim();
                        string categoryId = category.Id;

                        if (!string.IsNullOrEmpty(categoryName) && !string.IsNullOrEmpty(categoryId))
                        {
                            // 使用相同的重复名称处理逻辑
                            string finalKey = categoryName;
                            if (idMappingDict.ContainsKey(categoryName))
                            {
                                int counter = 1;
                                do
                                {
                                    finalKey = $"{categoryName}_{counter}";
                                    counter++;
                                } while (idMappingDict.ContainsKey(finalKey));
                            }

                            idMappingDict[finalKey] = categoryId;
                        }
                    }

                    string jsonContentWithIds = JsonSerializer.Serialize(
                        idMappingDict,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        }
                    );

                    await File.WriteAllTextAsync(filePathIdMapping, jsonContentWithIds, Encoding.UTF8);
                     
                }
                else
                {
                    ErrInfo = "获取到的分类数据为空或格式不正确！";
                }
            }
            else
            {
                ErrInfo = "获取到的数据实例为空！";
            }
        }
        catch (Exception ex)
        {
            ErrInfo = $"更新分类数据时发生错误: {ex.Message}";

            // 记录更详细的错误信息
            if (ex.InnerException != null)
            {
                ErrInfo += $" | 内部异常: {ex.InnerException.Message}";
            }
        }

        return ErrInfo;
    }
    /// <summary>
    /// 获取平台分类数据-可从本地缓存文件获取
    /// </summary>
    /// <returns></returns>
    override public Dictionary<string, string> GetClassData()
    {
        var dicClassData = new Dictionary<string, string>();

        try
        {
            // 构建文件路径
            string directoryPath = Path.Combine(AppContext.BaseDirectory, "Datas", "Categorys");
            string filePath = Path.Combine(directoryPath, "YouTubeDict.json");

            // 检查文件是否存在
            if (!File.Exists(filePath))
            {
                PrintLog($"警告：分类字典文件不存在: {filePath}");
                PrintLog($"请先调用 UpdateClassData() 方法生成文件");
                return dicClassData; // 返回空字典
            }

            // 读取JSON文件内容
            string jsonContent = File.ReadAllText(filePath, Encoding.UTF8);

            // 检查文件内容是否为空
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                PrintLog($"警告：分类字典文件内容为空: {filePath}");
                return dicClassData;
            }

            // 解析JSON到字典
            var jsonDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

            if (jsonDict != null && jsonDict.Count > 0)
            {
                dicClassData = jsonDict;
                PrintLog($"成功加载分类字典，共 {dicClassData.Count} 个分类");
                 
            }
            else
            {
                PrintLog($"警告：解析后的分类字典为空或格式不正确");
            }
        }
        catch (JsonException jsonEx)
        {
            PrintLog($"JSON解析错误: {jsonEx.Message}");
            PrintLog($"请确保YouTubeDic.json文件格式正确");
        }
        catch (IOException ioEx)
        {
            PrintLog($"文件读写错误: {ioEx.Message}");
        }
        catch (Exception ex)
        {
            PrintLog($"加载分类数据时发生错误: {ex.Message}");
        }

        return dicClassData;
    }
    /// <summary>
    /// 获取平台上的内容列表
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    override public async Task<List<ContentFromSite>> GetDataList(int limit = 30)
    {
        var mdList = new List<ContentFromSite>();
        try
        {
            var data = await youTube.GetMyVideosAsync(limit);

            if (data == null || data.Count == 0)
                return mdList;

            foreach (var video in data)
            {
                var model = new ContentFromSite
                {
                    VideoId = video.Id,
                    Title = video.Snippet?.Title,
                    Description = video.Snippet?.Description,

                    // 将标签列表转换为逗号分隔的字符串
                    Tags = video.Snippet?.Tags != null && video.Snippet.Tags.Count > 0
                           ? string.Join(",", video.Snippet.Tags)
                           : string.Empty,

                    CategoryId = video.Snippet?.CategoryId,
                    PublishedAt = video.Snippet?.PublishedAt,

                    // 获取默认缩略图URL
                    ThumbnailDefaultUrl = video.Snippet?.Thumbnails?.Default__?.Url,

                    // 统计信息 - 需要显式转换 ulong? 到 long?
                    ViewCount = (long?)video.Statistics?.ViewCount,
                    LikeCount = (long?)video.Statistics?.LikeCount,
                    CommentCount = (long?)video.Statistics?.CommentCount,
                    FavoriteCount = (long?)video.Statistics?.FavoriteCount,

                    MdWu = $"{ClassName}{video.Id}".Md5()
                };

                mdList.Add(model);
            }
        }
        catch (Exception ex)
        {
            PrintLog(ex.Message);
        }
        

        return mdList;
    }
    /// <summary>
    /// 点击登录按钮时触发受权
    /// </summary>
    /// <returns></returns>
    override public async Task<string> LoginAsync()
    {
        MessageBox.Show("如果是第一次点击此按钮YouTube将引导你登录网站.");
        bool ok = await youTube.AuthorizeAsync();

        return ok?"":"登录失败";
    }

    protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string>? CallBack)
    {
        //

        if (Equals(model, null))
            return "数据模型为空";
         

        var playListId = GetCtrValue("PlayList");

        var sClassName = GetCtrValue("ClassName");
        var classId = base.GetCategoryIdByName(sClassName);

        if (string.IsNullOrEmpty(classId))
            return "无法获取分类ID";
         
        string coverImgPath = GetCoverPath();
        if (string.IsNullOrEmpty(coverImgPath)) // 如果没有单独设置封面图，获取默认的封面图
            coverImgPath = model.ImgPath;

        string filePath = model.FilePath;

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return $"视频路径为空或不存在：{filePath}";

        if (!string.IsNullOrEmpty(coverImgPath) && !File.Exists(coverImgPath))
            return $"封面图片不存在：{coverImgPath}";

        string title = model.Title;

        if (string.IsNullOrEmpty(title))
            return "标题不能为空！";

        // 订阅上传进度事件
        youTube.UploadProgressChanged += (sender, e) =>
        {
            CallBack?.Invoke($"进度: {e.Status} - {FormatBytes(e.BytesSent)}");
            Debug.WriteLine($"进度: {e.Status} - {FormatBytes(e.BytesSent)}");
        };
        try
        {
            string videoId =  await youTube.UploadVideoAsync(filePath, title, model.Info, model.Tags, classId, "public");

            CallBack?.Invoke($"视频上传成功，开始设置封面图片");
            // 2. 等待片刻让YouTube处理视频
            await Task.Delay(5000);

            // 3. 设置缩略图
            string thumbnailErrInfo = await youTube.SetThumbnailAsync(videoId, coverImgPath);

            if (string.IsNullOrEmpty(thumbnailErrInfo))
            {
                CallBack?.Invoke($"视频封面设置成功！视频ID: {videoId}"); 
            }
            else
            {
                PrintLog($"视频封面设置失败:{thumbnailErrInfo}，但视频已上传。视频ID: {videoId}");
                CallBack?.Invoke($"视频封面设置失败:{thumbnailErrInfo}，但视频已上传。视频ID: {videoId}");
                await Task.Delay(10000);
            }
                        

            try
            {
                CallBack?.Invoke($"添加视频到播放列表：{playListId}");
                await Task.Delay(5000);
                // 将视频添加到播放列表
                PlaylistItem result = await youTube.AddVideoToPlaylistAsync(videoId, playListId);

                if (result != null)
                {
                    PrintLog($"添加视频到播放列表成功！");
                    //PrintLog($"播放列表项目ID: {result.Id}");
                    //PrintLog($"视频标题: {result.Snippet.Title}");
                    //PrintLog($"添加到播放列表的时间: {result.Snippet.PublishedAtDateTimeOffset}");
                    //PrintLog($"视频ID: {result.Snippet.ResourceId.VideoId}");
                    //PrintLog($"播放列表ID: {result.Snippet.PlaylistId}");
                    //PrintLog($"位置: {result.Snippet.Position}");
                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                CallBack?.Invoke($"添加视频到播放列表失败: {ex.Message}");
                PrintLog($"添加视频到播放列表失败: {ex.Message}");
                await Task.Delay(10000);
            }



        }
        catch (Exception ex)
        {
            return $"上传视频出错：{ex.Message}";
        }
       

        return "";

    }

    public override string Help => @"
 

# 关于 YouTube（油管）

YouTube，中文常称为“油管”，是全球最大的在线视频分享平台之一，成立于 **2005 年**，总部位于美国，现为 **Google（Alphabet）旗下**的重要产品。
YouTube 以“让每个人都能创作、分享和观看视频”为核心理念，覆盖全球 200 多个国家和地区，是目前**用户规模最大、内容生态最成熟**的视频平台。

YouTube 的内容类型极其丰富，几乎涵盖所有领域，包括但不限于：

* 娱乐、音乐、影视剪辑
* 游戏实况、电竞、直播回放
* 科技评测、编程、AI、数码
* 教育课程、知识科普、纪录片
* Vlog、生活方式、旅行、美食
* 新闻、时事、访谈、播客

YouTube 同时也是**创作者商业化程度最高的平台之一**，支持广告分成、会员订阅、超级留言、品牌合作等多种变现方式，是全球内容创作者的重要阵地。

---

# 如何成为 YouTube（油管）创作者

要在 YouTube 发布视频并成为一名创作者（YouTuber），可以按照以下步骤操作：

---

## 1. 注册 / 登录 Google 账号

YouTube 使用 Google 账号体系，首先需要一个 Google 账号。

> [https://www.youtube.com/](https://www.youtube.com/)

登录后即可使用 YouTube 的基础功能。

---

## 2. 创建 YouTube 频道

1. 点击右上角头像
2. 选择【创建频道】
3. 设置频道名称、头像、简介等信息

完成后，你就拥有了自己的 YouTube 频道。

---

## 3. 频道基础设置（推荐）

进入 **YouTube Studio（创作者工作室）**：

* 设置频道简介、关键词
* 选择频道类别
* 上传频道封面（Banner）
* 绑定邮箱与联系方式

> [https://studio.youtube.com/](https://studio.youtube.com/)

---

## 4. 上传视频

1. 点击右上角【创建】 →【上传视频】
2. 选择本地视频文件
3. 填写以下信息：

   * 视频标题（Title）
   * 视频简介（Description）
   * 标签（Tags）
   * 缩略图（Thumbnail）
   * 播放列表（可选）

---

## 5. 设置受众与可见性

* 选择是否为“儿童内容”
* 设置视频为：

  * **公开**
  * 不公开
  * 私享

---

## 6. 发布并等待处理完成

视频上传并处理完成后，即可正式在 YouTube 上对外展示，全球用户都可以观看。

---

## 7. 开启变现（进阶）

当频道满足以下条件后，可申请 **YouTube 合作伙伴计划（YPP）**：

* ≥ **1000 名订阅者**
* 过去 12 个月 ≥ **4000 小时公开视频观看时长**
  或
* 过去 90 天 ≥ **1000 万次 Shorts 播放**

通过审核后，即可开启广告收益、会员、打赏等功能。

---

# YouTube 平台特点总结

* 🌍 全球化程度极高，适合出海内容
* 💰 商业化成熟，创作者收益体系完善
* 🔍 搜索与推荐机制强（依托 Google）
* 🎞 内容生命周期长，适合“长尾内容”
* 🤖 非常适合 AI、技术、教程、纪录类内容
 

";

   
}
