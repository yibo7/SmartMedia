using Microsoft.Playwright;
using SmartMedia.Core;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Sites.Utils.Twitter; 
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SmartMedia.Sites.ImagePosts;

public class Facebook : ImagePushBase
{
    public override string PluginName => "Facebook";
    override public Image IcoName => Resource.facebook;
    protected override string UploadPage => "";
    /// <summary>
    /// UploadPage 与 DataPage为空，将视为API上传插件，打开平台时会有专门管理界面，否则将以平台后台页面为管理界面
    /// </summary>
    public override string DataPage => "";
    override protected Dictionary<string, SettingCtrBase> SiteCtrls
    {
        get
        {
            return new Dictionary<string, SettingCtrBase>() { };
        }
    }
    public Facebook()
    {
        OnChangeSetting();
    }
    private TwitterXClient client;
    override public void OnChangeSetting()
    {
        string apiKey = base.GetCf("ApiKey");
        string apiSecret = base.GetCf("ApiSecret");
        string accessToken = base.GetCf("AccessToken");
        string accessSecret = base.GetCf("AccessSecret");

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret) || string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(accessSecret))
        {
            PrintLog("还没有配置API");
        }

        client = new TwitterXClient(
                   apiKey: apiKey,
                   apiSecret: apiSecret,
                   accessToken: accessToken,
                   accessSecret: accessSecret
               ); 
    }
    override public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
            { "ApiKey",""},
            { "ApiSecret",""},
            { "AccessToken",""},
            { "AccessSecret",""}
        };
    override public async Task<string> LoginAsync()
    {
        var dlg = new PluginConfigs(this);
        dlg.ShowDialog();
        return ""; 
    }

    /// <summary>
    /// 展示频道的统计信息，返回 Markdown 表格
    /// 仅使用 Free 套餐免费接口（GET /2/users/me）
    /// </summary>
    public override async Task<string> GetUserInfo()
    {
        var sbInfo = new StringBuilder();

        try
        {
            // 一次请求同时拿到基本信息 + 统计数据，不需要第二次调用
            JsonElement myProfile = await client.GetMyProfileAsync();

            if (!myProfile.TryGetProperty("data", out JsonElement userData))
                return "无法获取用户信息，返回数据格式不正确";

            string userId = userData.GetProperty("id").GetString()!;
            string username = userData.GetProperty("username").GetString()!;
            string name = userData.GetProperty("name").GetString()!;

            // public_metrics 已包含在 GetMyProfileAsync 的返回中
            long followersCount = 0, followingCount = 0, tweetCount = 0, listedCount = 0;
            if (userData.TryGetProperty("public_metrics", out JsonElement pm))
            {
                followersCount = pm.GetProperty("followers_count").GetInt64();
                followingCount = pm.GetProperty("following_count").GetInt64();
                tweetCount = pm.GetProperty("tweet_count").GetInt64();
                listedCount = pm.GetProperty("listed_count").GetInt64();
            }

            // 可选信息（有就显示，没有就跳过）
            string description = userData.TryGetProperty("description", out var d) ? d.GetString()! : "";
            string location = userData.TryGetProperty("location", out var l) ? l.GetString()! : "";
            string createdAt = userData.TryGetProperty("created_at", out var c) ? c.GetString()! : "";

            // 构建 Markdown 表格
            sbInfo.AppendLine("| 参数项 | 参数值 |");
            sbInfo.AppendLine("|:------|:------|");
            sbInfo.AppendLine($"| 用户ID | `{userId}` |");
            sbInfo.AppendLine($"| 用户名 | @{username} |");
            sbInfo.AppendLine($"| 显示名称 | {name} |");

            if (!string.IsNullOrEmpty(description))
                sbInfo.AppendLine($"| 简介 | {description} |");

            if (!string.IsNullOrEmpty(location))
                sbInfo.AppendLine($"| 地区 | {location} |");

            if (!string.IsNullOrEmpty(createdAt))
                sbInfo.AppendLine($"| 注册时间 | {createdAt} |");

            sbInfo.AppendLine($"| 粉丝数 | {followersCount:N0} |");
            sbInfo.AppendLine($"| 关注数 | {followingCount:N0} |");
            sbInfo.AppendLine($"| 推文总数 | {tweetCount:N0} |");
            sbInfo.AppendLine($"| 列表数 | {listedCount:N0} |");
            sbInfo.AppendLine($"| 统计时间 | {DateTime.Now:yyyy-MM-dd HH:mm:ss} |");
        }
        catch (TwitterException ex)
        {
            return $"Twitter API 错误 (状态码: {ex.StatusCode}): {ex.Message}";
        }
        catch (JsonException ex)
        {
            return $"数据解析错误: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"获取用户信息失败: {ex.Message}";
        }

        return sbInfo.ToString();
    }

    protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
    {
        try
        {
            CallBack("获取推文图片路径");
            string[] imgPaths = base.GetCoverPaths(model);
            if (imgPaths.Length < 1)
                CallBack("没有可上传的图片，将发布纯文字推文");

            // ── 拼接 Hashtag ──────────────────────────────────
            List<string> tags = model.TagList();
            string hashTags = string.Empty;
            if (tags?.Count > 0)
            {
                // 过滤空值，去掉 tag 本身已有的 # 号再统一加上
                hashTags = string.Join(" ", tags
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => "#" + t.TrimStart('#').Trim()));
            }

            // ── 拼接推文正文 ──────────────────────────────────
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                sb.AppendLine(model.Title);
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(model.Info))
                sb.AppendLine(model.Info.Trim());

            // Hashtag 追加到末尾（与正文空一行）
            if (!string.IsNullOrWhiteSpace(hashTags))
            {
                sb.AppendLine();
                sb.Append(hashTags);
            }

            string tweetText = sb.ToString().Trim();

            // X 推文最多 280 字符，超出时优先保留正文，截断 Hashtag
            if (tweetText.Length > 280)
            {
                // 先尝试去掉 Hashtag 看是否够用
                string textWithoutTags = sb.ToString()
                    .Replace(hashTags, "").Trim();

                if (textWithoutTags.Length <= 280)
                {
                    tweetText = textWithoutTags;
                    CallBack("推文加 Hashtag 超过 280 字符，已自动去除标签");
                }
                else
                {
                    tweetText = textWithoutTags[..277] + "...";
                    CallBack("推文内容超过 280 字符，已自动截断");
                }
            }

            if (string.IsNullOrWhiteSpace(tweetText))
                return "推文内容为空，已跳过发布";

            CallBack($"推文内容准备完成，共 {tweetText.Length} 字符，标签：{(string.IsNullOrEmpty(hashTags) ? "无" : hashTags)}");

            // ── 发布推文 ──────────────────────────────────────
            TweetResult result;

            if (imgPaths.Length > 0)
            {
                bool hasGif = imgPaths.Any(p =>
                    Path.GetExtension(p).ToLower() == ".gif");

                var validPaths = hasGif
                    ? imgPaths.Where(p => Path.GetExtension(p).ToLower() == ".gif")
                              .Take(1).ToArray()
                    : imgPaths.Where(File.Exists)
                              .Take(4).ToArray();

                if (hasGif) CallBack("检测到 GIF，仅上传第 1 张");

                if (validPaths.Length > 0)
                {
                    CallBack($"开始上传图片（共 {validPaths.Length} 张）");
                    result = await client.PostTweetWithImagesAsync(tweetText, validPaths);
                    CallBack($"图文推文发布成功，推文 ID：{result.TweetId}");
                }
                else
                {
                    CallBack("图片文件不存在，改为发布纯文字推文");
                    result = await client.PostTweetAsync(tweetText);
                    CallBack($"纯文字推文发布成功，推文 ID：{result.TweetId}");
                }
            }
            else
            {
                CallBack("开始发布纯文字推文");
                result = await client.PostTweetAsync(tweetText);
                CallBack($"纯文字推文发布成功，推文 ID：{result.TweetId}");
            }

            return "";//result.TweetId
        }
        catch (TwitterException ex)
        {
            string msg = $"Twitter API 错误 (状态码: {ex.StatusCode}): {ex.Message}";
            CallBack(msg);
            return msg;
        }
        catch (Exception ex)
        {
            string msg = $"发布推文失败：{ex.Message}";
            CallBack(msg);
            return msg;
        }
    } 


    #region 获取推广

    /// <summary>
    /// 获取当前用户已发布的推文列表（Free 套餐免费方案）
    /// 使用 GET /2/users/me 获取 userId，再用 GET /2/users/:id/tweets 获取推文
    /// 注意：GET /2/users/:id/tweets 在 Free 套餐下每月有限额，建议减少调用频率
    /// </summary>
    /// <param name="limit">获取数量，取值范围 1-20</param>
    public override async Task<List<ContentFromSite>> GetDataList(int limit = 20)
    {
     

        // 弹出确认框
        DialogResult result = MessageBox.Show(
            "此接口调用至少需要升级 Basic 套餐，也就是非免费调用，如有需要请到Twitter开发者后台升级套餐。？",        // 提示信息
            "确认我已经是付费API",                     // 标题
            MessageBoxButtons.YesNo,        // 按钮类型：是/否
            MessageBoxIcon.Question         // 图标：问号（表示询问）
        );

        // 判断用户点击了什么按钮
        if (result != DialogResult.Yes)
        {
             return new List<ContentFromSite>();
        } 

        var articleList = new List<ContentFromSite>();

        try
        {
            // 第一步：获取当前用户 ID（免费）
            JsonElement myProfile = await client.GetMyProfileAsync();
            if (!myProfile.TryGetProperty("data", out JsonElement userData))
                return articleList;

            string userId = userData.GetProperty("id").GetString()!;
            string username = userData.GetProperty("username").GetString()!;

            // 第二步：获取推文列表
            // Free 套餐下 /2/users/:id/tweets 有月度积分限制，尽量减少 maxResults
            var tweetList = await client.GetUserTweetsAsync(
                userId: userId,
                maxResults: Math.Clamp(limit, 5, 20), // Free 套餐最小值为 5
                onlyWithMedia: false
            );

            if (tweetList?.Tweets == null || tweetList.Tweets.Count == 0)
                return articleList;

            // 第三步：转换为 ContentFromSite
            foreach (var tweet in tweetList.Tweets)
            {
                // 解析发布时间（格式示例：2024-01-15T08:30:00.000Z）
                DateTime? publishedAt = null;
                if (!string.IsNullOrEmpty(tweet.CreatedAt) &&
                    DateTime.TryParse(tweet.CreatedAt, null,
                        System.Globalization.DateTimeStyles.RoundtripKind,
                        out var parsedTime))
                {
                    publishedAt = parsedTime.ToLocalTime();
                }

                // 提取推文中的图片 URL（取第一张）
                string thumbnailUrl = string.Empty;
                if (tweet.MediaKeys?.Count > 0 &&
                    tweetList.MediaMap.TryGetValue(tweet.MediaKeys[0], out var media))
                {
                    thumbnailUrl = media.Url;
                }

                // 提取推文中的 hashtag 作为 Tags
                string tags = ExtractHashtags(tweet.Text);

                // 推文正文超长时截断作为 Description
                string description = tweet.Text.Length > 200
                    ? tweet.Text[..200] + "..."
                    : tweet.Text;

                var item = new ContentFromSite
                {
                    VideoId = tweet.TweetId,
                    Title = BuildTitle(tweet.Text),       // 用推文首句作标题
                    Description = description,
                    Tags = tags,
                    CategoryId = string.Empty,
                    PublishedAt = publishedAt,
                    ThumbnailDefaultUrl = thumbnailUrl,
                    ViewCount = null,                         // Free 套餐无法获取浏览量
                    LikeCount = tweet.LikeCount > 0 ? tweet.LikeCount : null,
                    CommentCount = tweet.ReplyCount > 0 ? tweet.ReplyCount : null,
                    FavoriteCount = null,                         // X API 不提供收藏数
                    PluginName = $"Twitter @{username}"
                };

                articleList.Add(item);
            }
        }
        catch (TwitterException ex)
        {
            Debug.WriteLine($"【获取推文列表 Twitter API 异常】状态码:{ex.StatusCode} {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"【获取推文列表异常】{ex.Message}");
        }

        return articleList;
    }

    /// <summary>
    /// 从推文正文中提取首句作为标题（取第一个换行或前 50 字）
    /// </summary>
    private static string BuildTitle(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        // 去掉 hashtag 后取首句
        string clean = System.Text.RegularExpressions.Regex.Replace(text, @"#\w+", "").Trim();
        int newline = clean.IndexOfAny(new[] { '\n', '\r' });
        string first = newline > 0 ? clean[..newline].Trim() : clean;

        return first.Length > 50 ? first[..50] + "…" : first;
    }

    /// <summary>
    /// 从推文正文中提取所有 hashtag，用 # 分隔
    /// </summary>
    private static string ExtractHashtags(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var matches = System.Text.RegularExpressions.Regex.Matches(text, @"#(\w+)");
        return matches.Count > 0
            ? string.Join("#", matches.Select(m => m.Groups[1].Value))
            : string.Empty;
    }

    #endregion

    public override string Help => @" 

# TwitterXClient 配置说明

> 适用于 · OAuth 1.0a · X API v2

推文发布规则：推文内容不能超过280个字，图片不能超过4张，gif图片只能发一张，视频只能发一条。

---

## 一、配置密钥

 打开内容管理界面->点击【设置接口参数】
如果还没有申请开发者身份，请参考第三节。

| 构造函数参数 | 开发者后台名称 | 你手中的凭证 | 是否必须 | 说明 |
|---|---|---|---|---|
| `apiKey` | API Key (Consumer Key) | **Consumer Key** | ✅ 必须 | 应用唯一标识符 |
| `apiSecret` | API Key Secret | **Secret Key** | ✅ 必须 | Consumer Secret，用于签名 |
| `accessToken` | Access Token | ⚠️ 需手动生成 | ✅ 必须 | 代表用户的操作令牌 |
| `accessSecret` | Access Token Secret | ⚠️ 需手动生成 | ✅ 必须 | Access Token 对应的密钥 |

> **注意：** `Access Token` 和 `Access Token Secret` 初始不会自动提供，需在开发者后台手动点击生成，详见第三节。

---

## 二、你手中所有凭证的用途说明

开发者后台会提供多种凭证，以下是它们的完整说明：

| 凭证名称 | 作用 | 工具类使用情况 |
|---|---|---|
| Consumer Key | OAuth 1.0a 签名 — 应用标识 | ✅ 使用（`apiKey`） |
| Secret Key | OAuth 1.0a 签名 — 应用密钥 | ✅ 使用（`apiSecret`） |
| OAuth 1.0 Client Secret | 与 Secret Key 本质相同，后台不同页面的不同叫法 | ✅ 同上 |
| Access Token | 代表用户的操作令牌（需手动生成） | ✅ 使用（`accessToken`） |
| Access Token Secret | Access Token 对应密钥（需手动生成） | ✅ 使用（`accessSecret`） |
| Bearer Token | App-only 只读请求，无需用户授权 | — 未使用（可扩展） |
| OAuth 2.0 Client Secret | 第三方用户 OAuth 2.0 授权登录流程专用 | — 不需要 |

> **关于 OAuth 1.0 Client Secret：** 这只是 Secret Key（Consumer Secret）在后台某些页面的另一种叫法，本质是同一个值，不必困惑。

---

## 三、X 开发者账号注册与申请步骤

### 第一步：注册 / 登录 X 账号

前往 [x.com](https://x.com) 注册普通用户账号，如已有账号直接登录。建议先完善个人资料（头像、简介），有助于降低开发者申请被拒的概率。

---

### 第二步：申请开发者账号

1. 访问 [developer.x.com](https://developer.x.com)，点击右上角 **Sign up** 或 **Apply**。
2. 填写申请表单：
   - **使用目的** — 用中文或英文描述你的场景，例如：`个人项目，用于管理自己的账号、发布内容和获取统计数据`
   - **国家 / 地区** — 选择你所在的地区
3. 同意开发者协议，提交申请。

> Free 套餐通常即时审批通过，部分账号需等待 1–3 个工作日，审批结果会发送到注册邮箱。

---

### 第三步：创建项目和应用（App）

1. 进入开发者后台 → **Projects & Apps** → **Create Project**
2. 填写项目名称，选择使用场景
3. 填写 App 名称（全局唯一）
4. 创建成功后，后台会立即展示：
   - `API Key`（即 Consumer Key）
   - `API Key Secret`（即 Secret Key）

> ⚠️ **这两个值只显示一次，必须立即复制保存！** 关闭后无法再次查看，只能重新生成（重新生成会使旧凭证失效）。

---

### 第四步：配置应用权限（必须设为 Read and Write）

1. 进入 App → **Settings** → **User authentication settings** → **Edit**
2. 将 **App permissions** 设置为 **Read and Write**
   - 默认为只读（Read Only），**必须修改，否则无法发帖**
3. **Type of App** 选择 **Web App, Automated App or Bot**
4. **Callback URI** 填写任意地址，例如：`https://localhost/callback`
5. **Website URL** 填写你的网站或 GitHub 地址
6. 保存设置

> ⚠️ **重要：** 修改权限后，已生成的 Access Token 会立即失效，需要重新生成。

---

### 第五步：生成 Access Token 和 Access Token Secret

1. 进入 App → **Keys and Tokens** 页面
2. 找到 **Authentication Tokens** 区域下的 **Access Token and Secret**
3. 点击 **Generate** 按钮
4. 立即复制并保存：
   - `Access Token`（形如 `1234567890-xxxxxxxxxxxxxxxx`）
   - `Access Token Secret`（一串随机字符串）

> ⚠️ **同样只显示一次，立即保存！** 关闭后只能重新生成，旧 Token 随即失效。

---

## 四、开发者套餐说明

| 套餐 | 月费 | 每月发推限额 | 搜索推文 | 适用场景 |
|---|---|---|---|---|
| **Free** | 免费 | 1,500 条 | 仅限自己的推文 | 个人测试 |
| **Basic** | $100 / 月 | 3,000 条 | 最近 7 天 | 小型应用 |
| **Pro** | $5,000 / 月 | 300,000 条 | 最近 30 天 | 商业应用 |
| **Enterprise** | 定制 | 无限制 | 完整历史 | 企业级 |

工具类的所有功能在 **Free 套餐**下均可使用，但 `SearchTweetsAsync`（搜索推文）需要 **Basic 及以上套餐**，个人项目从 Free 开始即可。

---

## 五、安全注意事项

### 凭证保护

- **不要**将任何密钥硬编码在源代码中，尤其不能上传到 GitHub 等公开仓库
- 推荐使用**环境变量**或 `dotnet user-secrets` 管理凭证
- 使用 `.gitignore` 排除包含密钥的配置文件（如 `appsettings.Development.json`）

 

### 权限最小化

- 如果只需读取数据，将 App 权限设为 **Read Only**，不申请多余权限
- Access Token 的权限与生成时 App 的权限设置绑定，修改 App 权限后需重新生成 Token
- 建议定期轮换（重新生成）Access Token，降低泄露风险

---

## 六、快速检查清单

在开始使用工具类前，确认以下所有项目已完成：

- [ ] 已注册 X 账号并完善个人资料
- [ ] 已在 developer.x.com 申请开发者账号并通过审批
- [ ] 已创建项目和 App
- [ ] 已将 App 权限设置为 **Read and Write**
- [ ] 已获取并保存 `API Key`（Consumer Key）
- [ ] 已获取并保存 `API Key Secret`（Secret Key）
- [ ] 已生成并保存 `Access Token`
- [ ] 已生成并保存 `Access Token Secret`
- [ ] 凭证已通过环境变量或 user-secrets 管理，未硬编码在代码中

---

## 七、常用链接

| 链接 | 地址 |
|---|---|
| X 开发者门户 | https://developer.x.com |
| 申请开发者账号 | https://developer.x.com/en/portal/petition/essential/basic-info |
| X API v2 官方文档 | https://developer.x.com/en/docs/x-api |
| API 套餐与限额说明 | https://developer.x.com/en/docs/x-api/rate-limits |
| OAuth 1.0a 文档 | https://developer.x.com/en/docs/authentication/oauth-1-0a |

";

}
