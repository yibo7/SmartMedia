using LiteDB;
using System.Collections.Concurrent;
using XS.Data2.LiteDBBase;
using XS.Core2.XsExtensions;
using System.Text.Json.Serialization;
using SmartMedia.Core.SitesBase.BLL;
namespace SmartMedia.Core.SitesBase.DB;

[LiteTable("PushContent")]// 发布内容的表，目前包括视频，音频，文章
public class PushInfo : LiteModelBaseInt
{

    public string Title { get; set; } // 标题
    public int Status { get; set; }  // 0.未发布，1.已发布 -1 发布失败      
    public int Original { get; set; } // 0.非原创，1.是原创     
    public string Tags { get; set; } // 标签，多个用#号分开
    public string Info { get; set; } // 内容或简介

    public string FilePath { get; set; } // 视频路径，音频路径，图片中的图片（多个逗号分开）
    public string ImgPath { get; set; } // 封面图片

    public string AdInfo { get; set; } // 广告语，目前没有用到
    //public string MdWu { get; set; }
    public DateTime CreateTime { get; set; }
    public long CreateTimeInt { get; set; }

    public int IType { get; set; }  // 1.视频，2.音频, 3.文章， 4.图文  

    public string Sites { get; set; } // 发布站点的配置，目前主要包含分类配置

    /// <summary>
    /// 定时发布时间， Unix 时间戳（秒或毫秒），如果等于0表示未开启定时发布 
    /// </summary>
    public long PublishTimeStamp { get; set; }
    public string PublishLog { get; set; } // 发布日志，可以是成功日志，也可以是错误日志
    public int ClassId { get; set; }
    //public string SrtLrc { get; set; } // 字幕内容还是路径根据不同的平台要求
    /// <summary>
    /// 将站点配置转换字典
    /// </summary>
    /// <returns>
    /// {
    ///     "DouYin":{"是否同步":"True","合集名称":"test","IsEnable":"true"},
    ///     "YouTube":{"发布分类":"ddd","IsEnable":"true"}
    /// }
    /// </returns>
    [BsonIgnore]
    public Dictionary<string, Dictionary<string, string>> ConverSiteSettingToDict()
    {
        var sites = this.Sites.ToJson<Dictionary<string, Dictionary<string, string>>>();
        return sites;
    }
    // 辅助属性（不存入数据库）
    [BsonIgnore]
    public string PublishDateTime
    {
        get
        {
            if (PublishTimeStamp <= 0)
                return "未设置定时";

            // UTC时间转换到北京时间（UTC+8）
            //var utcTime = DateTimeOffset.FromUnixTimeSeconds(PublishTimeStamp).UtcDateTime;
            //var beijingTime = utcTime.AddHours(8);

            var publishBeijingTime = DateTimeOffset.FromUnixTimeSeconds(PublishTimeStamp)
                .UtcDateTime.Add(BeijingOffset);

            return publishBeijingTime.ToString("yyyy-MM-dd HH:mm");
        }
    }

    // 类型映射字典（静态，只初始化一次）
    private static readonly ConcurrentDictionary<int, string> ContentTypeMap = new()
    {
        [1] = "视频",
        [2] = "音频",
        [3] = "文章",
        [4] = "图文"
    };

    [BsonIgnore]
    public string ContentTypeName =>
        ContentTypeMap.TryGetValue(IType, out var name) ? name : "未知类型";
    [BsonIgnore]
    public List<string> TagList()
    {
        return string.IsNullOrEmpty(this.Tags)
            ? new List<string>()
            : this.Tags.Split('#', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                       .ToList();
    }
    /// <summary>
    /// 获取带#号的标准标签
    /// </summary>
    /// <returns>#a #b #c</returns>
    public string FormatTags()
    {
        var tags = this.TagList();  // 先获取标签列表

        if (tags == null || !tags.Any())
        {
            return string.Empty;
        }

        var cleanTags = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => "#" + tag.Trim());

        return string.Join(" ", cleanTags);
    }
    // 北京时区偏移量（UTC+8）
    private static readonly TimeSpan BeijingOffset = TimeSpan.FromHours(8);

    /// <summary>
    /// 是否已达到北京时间发布时间
    /// </summary>
    [BsonIgnore]
    public bool IsPublishTimeReachedBeijing
    {
        get
        {
            if (PublishTimeStamp <= 0)
                return false;

            // 发布时间对应的北京时间
            var publishBeijingTime = DateTimeOffset.FromUnixTimeSeconds(PublishTimeStamp)
                .UtcDateTime.Add(BeijingOffset);

            // 当前北京时间
            var currentBeijingTime = DateTime.UtcNow.Add(BeijingOffset);

            return currentBeijingTime >= publishBeijingTime;
        }
    }

    /// <summary>
    /// 格式化剩余时间为友好字符串
    /// </summary>
    public string FormatTimeRemaining()
    {
        // 未设置定时
        if (PublishTimeStamp <= 0)
            return "未定时";

        // 获取当前时间戳
        long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 计算剩余秒数 = 发布时间 - 当前时间
        long remainingSeconds = PublishTimeStamp - currentTimestamp;

        // 已过期
        if (remainingSeconds <= 0)
            return "即将发布";

        // 计算时间间隔
        TimeSpan remaining = TimeSpan.FromSeconds(remainingSeconds);

        // 提取各个部分
        int days = remaining.Days;
        int hours = remaining.Hours;
        int minutes = remaining.Minutes;

        // 构建字符串
        List<string> parts = new List<string>();

        if (days > 0)
            parts.Add($"{days}天");

        if (hours > 0)
            parts.Add($"{hours}小时");

        if (minutes > 0)
            parts.Add($"{minutes:00}分");
        else if (days == 0 && hours == 0)
            parts.Add($"{remaining.Seconds:00}秒");

        return string.Join("", parts);
    }


    public IPushContentBll GetBll()
    {
        IPushContentBll bll = null;
        switch (this.IType)  //  1.视频，2.音频, 3.文章， 4.图文  
        {
            case 1:
                bll = new VideoBll();
                break;
            case 2:
                bll = new AudioBll();
                break;
            case 3:
                bll = new ArticleBll();
                break;
            case 4:
                bll = new ImagePostBll();
                break; 
        }

        return bll;
    }
}


public class ImportModel
{

    public string Title { get; set; } // 标题 
    public string Description { get; set; } // 内容或简介
    public string FilePath { get; set; } // 视频路径，音频路径，图片中的图片（多个逗号分开）
    public string ImgPath { get; set; } // 默认封面图片 
    // 方案1：使用 List<string> 存储标签
    public List<string> Tags { get; set; } = new List<string>();

    // 可选：保留原有的 Tags 字符串属性，用于向后兼容
    [JsonIgnore]
    public string TagsString => Tags != null && Tags.Count > 0
        ? "#" + string.Join("#", Tags)
        : string.Empty;
    
    [JsonIgnore]
    public long PublishTimeStamp { get; set; }

    [JsonIgnore]
    public string Sites { get; set; } // 发布站点的配置，目前主要包含分类配置

    [JsonIgnore]
    public int Original { get; set; }

    [JsonIgnore]
    public int ClassId { get; set; }
     

}
