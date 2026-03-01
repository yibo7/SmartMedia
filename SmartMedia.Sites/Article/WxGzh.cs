using Microsoft.Playwright;
using SmartMedia.Core;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Sites.Article.WeChatArticle;
using System.Diagnostics;
using System.Text;
using XS.Core2.XsExtensions;

namespace SmartMedia.Sites.Article
{
    public class WxGzh : ArticlePushBase
    {

        protected override string UploadPage => "";

        public override string DataPage => "";

        public override string PluginName => "公众号";
        override public Image IcoName => Resource.weixingzh;
        private WeChatArticlePublisher publisher;
        public WxGzh()
        {
            OnChangeSetting();
        }
        override public void OnChangeSetting()
        {
            string appId = base.GetCf("AppID");
            string appSecret = base.GetCf("AppSecret");

            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(appSecret))
            {
                PrintLog("还没有公众号的配置或AppID");
            }

            publisher = new WeChatArticlePublisher(appId, appSecret);
        }

        override protected Dictionary<string, SettingCtrBase> SiteCtrls
        {
            get
            {
                return new Dictionary<string, SettingCtrBase>(){};
            }
        }

        override public async Task<string> LoginAsync()
        {
            var dlg = new PluginConfigs(this);
            dlg.ShowDialog();
            return "";
        }
        override public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
            { "AppID",""},
            { "AppSecret",""}
        };

        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {
            string result = await publisher.PublishArticleAsync(
            title: model.Title,
            thumbMediaPath: model.ImgPath, // 提供本地图片路径，代码会自动上传
            author: "cqs",
            //digest: digest,
            content: model.Info, 
            isToAll: true // 设置为 true 表示群发给所有用户
        );

            PrintLog($"公众号文章发布结果：{result}");

            return "";
        }
         

        /// <summary>
        /// 获取当前用户在公众号上已发布的内容列表
        /// </summary>
        /// <param name="limit">获取数量，取值范围1-20</param>
        /// <returns>文章列表</returns>
        override public async Task<List<ContentFromSite>> GetDataList(int limit = 20)
        {
            var articleList = new List<ContentFromSite>();

            try
            {
                // 1. 调用之前已实现的方法获取微信官方数据结构
                var wechatData = await publisher.GetPublishedListAsync(count: Math.Clamp(limit, 1, 20));

                // 2. 检查数据
                if (wechatData?.Items == null || wechatData.Items.Count == 0)
                    return articleList;

                // 3. 转换数据
                foreach (var publishedItem in wechatData.Items)
                {
                    // 微信每次发布可能包含多篇文章（专题），这里取第一篇为主数据
                    var firstArticle = publishedItem.Content?.NewsItem?.FirstOrDefault();
                    if (firstArticle == null) continue;

                    var model = new ContentFromSite
                    {
                        // 关键ID：使用发布任务ID和文章ID组合，确保唯一
                        VideoId = $"{publishedItem.PublishId}_{publishedItem.ArticleId}",
                        Title = firstArticle.Title,
                        Description = firstArticle.Digest,

                        // 微信文章没有公开的标签（Tags）和分类（CategoryId）系统
                        Tags = string.Empty,
                        CategoryId = string.Empty,

                        // 时间转换：微信返回的是Unix时间戳（秒），需转换为DateTime
                        PublishedAt = DateTimeOffset.FromUnixTimeSeconds(publishedItem.CreateTimeUnixTimestamp).DateTime,

                        // 缩略图：使用封面图的URL
                        ThumbnailDefaultUrl = firstArticle.ThumbUrl,

                        // 注意：微信统计信息（阅读数等）需要另外调用数据统计接口，此处暂不包含
                        ViewCount = null,
                        LikeCount = null,
                        CommentCount = null,
                        FavoriteCount = null,

                        // 用于哈希的唯一标识
                        MdWu = $"WeChatArticle_{publishedItem.PublishId}".Md5()
                    };

                    articleList.Add(model);
                }
            }
            catch (Exception ex)
            {
                // 使用类似你的打印日志方式，这里简单输出到调试窗口
                Debug.WriteLine($"【获取微信文章列表异常】{ex.Message}");
                // 或者调用你的 PrintLog(ex.Message);
            }

            return articleList;
        }

        /// <summary>
        /// 展示频道的统计信息，返回Markdown代码，灵活配置
        /// </summary>
        /// <returns></returns>
        override public async Task<string> GetUserInfo()
        {
            var sbInfo = new StringBuilder("<h1>暂未实现</h1>");
             

            return sbInfo.ToString();
        }
        public override string Help => @" 你首先需要准备好一个“主战场”——一个经过配置的微信公众号。

#### **1. 注册公众号并完成认证**
*   **注册入口**：访问 [微信公众平台官网](https://mp.weixin.qq.com/) 点击“立即注册”。
*   **账号类型**：
    *   **订阅号**：适合个人或媒体，每天可群发1次消息。
    *   **服务号**：适合企业或组织，每月可群发4次消息，支持更高级的API接口。
    *   请注意：我们正在使用的“发布草稿”接口 (`freepublish/submit`) **通常需要公众号完成微信认证后才能调用**。如果你只是想测试，可以在微信公众平台申请一个**测试号**来快速体验。

#### **2. 获取 `AppID` 和 `AppSecret`**
这是你代码与公众号沟通的“账号密码”。登录 **微信开发者平台** (注意：是“开发者平台”，不是“公众平台”) 。
1.  扫码登录后，进入 **我的业务 > 公众号**，找到需要配置的公众号。
2.  在详情页，你可以直接查看 `AppID` 并**启用、重置或管理 `AppSecret`**。请务必**妥善保存 `AppSecret`**，因为平台不保存明文，忘记后只能重置。

#### **3. 配置IP白名单**
这是保证API调用成功的关键安全设置。
1.  在微信开发者平台的公众号详情页，找到**基础信息 > 开发信息 > API IP白名单**，点击配置。
2.  将你**部署了 `WeChatArticlePublisher` 的后端服务器公网IP**添加进去。
3.  **重要规则**：支持具体IP (如 `119.147.XX.XX`) 或 IP段 (如 `119.147.XX.XX/24`)，但公众号白名单**不支持 `172.0.0.*` 这种格式**。设置后约10分钟生效。

#### **4. 关键信息核对表**
请在运行代码前，确保你已获得以下信息：

| 配置项 | 获取位置 | 备注 |
| :--- | :--- | :--- |
| **AppID** | 微信开发者平台 -> 我的业务 -> 公众号 | 明文查看，直接复制。 |
| **AppSecret** | 同上，需启用或重置生成 | **务必保密**，是核心凭证。 |
| **服务器公网IP** | 你的服务器供应商控制台或使用 `curl ifconfig.me` 等命令查询 | 需添加到白名单。 |

#### **5. 个人主体无法认证**
 个人主体目前好像无法认证，无法认证没有获取列表数据与发表的权限，只能发布为草稿，自登录后台后操作发布。
";

    }
}
