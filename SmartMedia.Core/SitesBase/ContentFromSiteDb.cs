using SmartMedia.Core; 
using System.Diagnostics; 
using XS.Data2.LiteDBBase;

namespace SmartMedia.Core.SitesBase;


[LiteTable("ContentFromSite")]// 获取平台上发布的内容持久化在本地，目前包括视频，音频，文章
public class ContentFromSite : LiteModelBaseInt
{/// <summary>
 /// 视频或文章ID（平台上的）
 /// </summary>
    public string VideoId { get; set; }

    /// <summary>
    /// 标题-如果没有可获取Description中的第一句
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 视频标签列表，多个用#号分开
    /// </summary>
    public string Tags { get; set; }

    /// <summary>
    /// 分类ID-如果没有可为空
    /// </summary>
    public string CategoryId { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// 默认缩略图信息-如果没有可为空
    /// </summary>
    public string ThumbnailDefaultUrl { get; set; }

    

    /// <summary>
    /// 观看次数
    /// </summary>
    public long? ViewCount { get; set; }

    /// <summary>
    /// 点赞数
    /// </summary>
    public long? LikeCount { get; set; }

    /// <summary>
    /// 评论数
    /// </summary>
    public long? CommentCount { get; set; }        

    /// <summary>
    /// 收藏次数
    /// </summary>
    public long? FavoriteCount { get; set; }
    /// <summary>
    /// 来自哪个平台的处理插件名称
    /// </summary>
    public string PluginName { get; set; }
        

     
}

public class ContentFromSiteBll : LiteDbInt<ContentFromSite>
{
    /// <summary>
    /// 更新数据到数据，如果不存在就添加
    /// </summary>
    /// <param name="contentFromSite"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public long UpdateData(ContentFromSite contentFromSite)
    {
        long iId = 0;
        if (contentFromSite == null)
            return iId;

        // 生成Md5值
        //contentFromSite.MdWu = $"{contentFromSite.PluginName}{contentFromSite.VideoId}".Md5();

        // 检查是否已存在相同MdWu的记录
        if (ExistsMd5(contentFromSite.MdWu))
        {
            // 获取已存在的记录ID并更新
            var existing = GetByMdWu(contentFromSite.MdWu);
            iId = existing.Id;
            contentFromSite.Id = iId;
            Debug.WriteLine($"已经存在：{existing.Title}，执行更新操作");
            base.Update(contentFromSite);
        }
        else
        {
            Debug.WriteLine($"添加入库：{contentFromSite.Title}");
            // 添加新记录
            iId = base.Add(contentFromSite);
        }

        return iId; 
    }

    public List<ContentFromSite> GetListByPlugin(string pluginName)
    {
        var lst = base.Find($"PluginName='{pluginName}'",limit:5000);

        return lst;
    }

     
}