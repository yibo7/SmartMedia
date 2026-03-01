 
namespace SmartMedia.Sites.Utils.Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// Facebook Graph API 工具类
/// 支持：发帖、上传图片/视频、获取帖子列表、获取统计数据、用户信息、删除帖子、评论、私信等常用功能
/// 认证方式：Page Access Token（长期令牌）
/// API 版本：v21.0
/// </summary>
public class FacebookClient : IDisposable
{
    #region 配置与初始化

    private readonly string _accessToken;  // Page Access Token
    private readonly string _pageId;       // 主页 ID
    private readonly HttpClient _http;

    private const string ApiBase = "https://graph.facebook.com/v21.0";

    /// <param name="accessToken">Page Access Token（长期令牌，建议使用永久令牌）</param>
    /// <param name="pageId">Facebook 主页 ID（数字格式，如 123456789）</param>
    public FacebookClient(string accessToken, string pageId)
    {
        _accessToken = accessToken;
        _pageId = pageId;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 一、用户 / 主页信息
    // ══════════════════════════════════════════════════════════

    #region 用户 / 主页信息

    /// <summary>
    /// 获取当前主页的基本信息和统计数据
    /// </summary>
    public async Task<PageInfo> GetPageInfoAsync()
    {
        string url = $"{ApiBase}/{_pageId}" +
                     $"?fields=id,name,about,bio,category,fan_count,followers_count," +
                     $"posts_count,website,picture,verification_status,created_time" +
                     $"&access_token={_accessToken}";

        var doc = await GetAsync(url);
        var root = doc.RootElement;

        return new PageInfo
        {
            PageId = root.TryGetString("id"),
            Name = root.TryGetString("name"),
            About = root.TryGetString("about"),
            Bio = root.TryGetString("bio"),
            Category = root.TryGetString("category"),
            FanCount = root.TryGetLong("fan_count"),
            FollowersCount = root.TryGetLong("followers_count"),
            PostsCount = root.TryGetLong("posts_count"),
            Website = root.TryGetString("website"),
            IsVerified = root.TryGetString("verification_status") == "verified",
            CreatedTime = root.TryGetString("created_time"),
            ProfilePictureUrl = root.TryGetNestedString("picture", "data", "url")
        };
    }

    /// <summary>
    /// 获取主页洞察数据（粉丝增长、触达、互动等）
    /// </summary>
    /// <param name="period">统计周期：day / week / month</param>
    public async Task<List<PageInsight>> GetPageInsightsAsync(string period = "month")
    {
        string metrics = string.Join(",", new[]
        {
            "page_fans",
            "page_fan_adds",
            "page_impressions",
            "page_impressions_unique",
            "page_engaged_users",
            "page_post_engagements",
            "page_views_total"
        });

        string url = $"{ApiBase}/{_pageId}/insights" +
                     $"?metric={metrics}&period={period}" +
                     $"&access_token={_accessToken}";

        var doc = await GetAsync(url);
        var result = new List<PageInsight>();

        if (!doc.RootElement.TryGetProperty("data", out var dataArr))
            return result;

        foreach (var item in dataArr.EnumerateArray())
        {
            string name = item.TryGetString("name") ?? "";
            string title = item.TryGetString("title") ?? name;
            long value = 0;

            if (item.TryGetProperty("values", out var values))
            {
                var last = values.EnumerateArray().LastOrDefault();
                if (last.ValueKind != JsonValueKind.Undefined &&
                    last.TryGetProperty("value", out var v))
                {
                    value = v.ValueKind == JsonValueKind.Number
                        ? v.GetInt64()
                        : 0;
                }
            }

            result.Add(new PageInsight { Name = name, Title = title, Value = value, Period = period });
        }

        return result;
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 二、发帖
    // ══════════════════════════════════════════════════════════

    #region 发帖

    /// <summary>
    /// 发布纯文字帖子
    /// </summary>
    /// <param name="message">帖子内容</param>
    public async Task<PostResult> PostTextAsync(string message)
    {
        string url = $"{ApiBase}/{_pageId}/feed";

        var body = new Dictionary<string, string>
        {
            ["message"] = message,
            ["access_token"] = _accessToken
        };

        var doc = await PostFormAsync(url, body);
        return new PostResult { PostId = doc.RootElement.TryGetString("id") ?? "" };
    }

    /// <summary>
    /// 发布带链接的帖子
    /// </summary>
    /// <param name="message">帖子内容</param>
    /// <param name="link">链接 URL</param>
    public async Task<PostResult> PostLinkAsync(string message, string link)
    {
        string url = $"{ApiBase}/{_pageId}/feed";

        var body = new Dictionary<string, string>
        {
            ["message"] = message,
            ["link"] = link,
            ["access_token"] = _accessToken
        };

        var doc = await PostFormAsync(url, body);
        return new PostResult { PostId = doc.RootElement.TryGetString("id") ?? "" };
    }

    /// <summary>
    /// 发布图文帖子（自动上传图片）
    /// </summary>
    /// <param name="message">帖子内容</param>
    /// <param name="imagePaths">图片本地路径列表（最多 10 张）</param>
    public async Task<PostResult> PostWithImagesAsync(string message, IEnumerable<string> imagePaths)
    {
        var paths = imagePaths
            .Where(File.Exists)
            .Take(10)
            .ToList();

        if (paths.Count == 0)
            return await PostTextAsync(message);

        // 上传每张图片，获取 media_fbid（不发布，只上传）
        var mediaFbIds = new List<string>();
        foreach (var path in paths)
        {
            string fbid = await UploadPhotoUnpublishedAsync(path);
            mediaFbIds.Add(fbid);
        }

        // 将多张图片组合成一个帖子
        string url = $"{ApiBase}/{_pageId}/feed";
        var formData = new List<KeyValuePair<string, string>>
        {
            new("message",      message),
            new("access_token", _accessToken)
        };

        // 每张图片作为 attached_media 参数
        foreach (var fbid in mediaFbIds)
            formData.Add(new("attached_media[]", $"{{\"media_fbid\":\"{fbid}\"}}"));

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(formData)
        };
        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new FacebookException($"发布图文帖子失败: {json}", (int)response.StatusCode);

        var doc = JsonDocument.Parse(json);
        return new PostResult { PostId = doc.RootElement.TryGetString("id") ?? "" };
    }

    /// <summary>
    /// 发布视频帖子（分块上传，支持大文件）
    /// </summary>
    /// <param name="message">视频描述</param>
    /// <param name="videoPath">视频本地路径（MP4）</param>
    /// <param name="title">视频标题（可选）</param>
    /// <param name="callBack">进度回调</param>
    public async Task<PostResult> PostVideoAsync(
        string message,
        string videoPath,
        string? title = null,
        Action<string>? callBack = null)
    {
        if (!File.Exists(videoPath))
            throw new FileNotFoundException($"视频文件不存在: {videoPath}");

        string videoId = await UploadVideoChunkedAsync(videoPath, message, title, callBack);
        callBack?.Invoke($"视频发布成功，视频 ID：{videoId}");

        return new PostResult { PostId = videoId };
    }

    /// <summary>
    /// 删除帖子
    /// </summary>
    public async Task<bool> DeletePostAsync(string postId)
    {
        string url = $"{ApiBase}/{postId}?access_token={_accessToken}";

        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new FacebookException($"删除帖子失败: {json}", (int)response.StatusCode);

        var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("success", out var s) && s.GetBoolean();
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 三、帖子列表查询
    // ══════════════════════════════════════════════════════════

    #region 帖子列表查询

    /// <summary>
    /// 获取主页发布的帖子列表
    /// </summary>
    /// <param name="limit">每页条数，最大 100</param>
    /// <param name="after">翻页游标，首次为 null</param>
    public async Task<PostListResult> GetPagePostsAsync(int limit = 20, string? after = null)
    {
        limit = Math.Clamp(limit, 1, 100);

        string fields = "id,message,story,created_time,full_picture," +
                        "permalink_url,attachments,likes.summary(true)," +
                        "comments.summary(true),shares";

        string url = $"{ApiBase}/{_pageId}/posts" +
                     $"?fields={fields}&limit={limit}&access_token={_accessToken}";

        if (!string.IsNullOrEmpty(after))
            url += $"&after={after}";

        var doc = await GetAsync(url);
        return ParsePostListResult(doc.RootElement);
    }

    /// <summary>
    /// 获取主页发布的视频列表
    /// </summary>
    public async Task<PostListResult> GetPageVideosAsync(int limit = 20, string? after = null)
    {
        limit = Math.Clamp(limit, 1, 100);

        string fields = "id,title,description,created_time,picture,embed_html," +
                        "permalink_url,likes.summary(true),comments.summary(true)," +
                        "views,length";

        string url = $"{ApiBase}/{_pageId}/videos" +
                     $"?fields={fields}&limit={limit}&access_token={_accessToken}";

        if (!string.IsNullOrEmpty(after))
            url += $"&after={after}";

        var doc = await GetAsync(url);
        return ParsePostListResult(doc.RootElement, isVideo: true);
    }

    /// <summary>
    /// 获取单个帖子详情
    /// </summary>
    public async Task<FbPost> GetPostByIdAsync(string postId)
    {
        string fields = "id,message,story,created_time,full_picture,permalink_url," +
                        "likes.summary(true),comments.summary(true),shares,attachments";

        string url = $"{ApiBase}/{postId}" +
                     $"?fields={fields}&access_token={_accessToken}";

        var doc = await GetAsync(url);
        return ParsePost(doc.RootElement);
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 四、统计数据
    // ══════════════════════════════════════════════════════════

    #region 统计数据

    /// <summary>
    /// 获取单个帖子的互动统计（点赞、评论、分享）
    /// </summary>
    public async Task<PostMetrics> GetPostMetricsAsync(string postId)
    {
        string url = $"{ApiBase}/{postId}" +
                     $"?fields=id,message,created_time," +
                     $"likes.summary(true),comments.summary(true),shares," +
                     $"reactions.summary(true)" +
                     $"&access_token={_accessToken}";

        var doc = await GetAsync(url);
        var root = doc.RootElement;

        long likes = 0, comments = 0, shares = 0, reactions = 0;

        if (root.TryGetProperty("likes", out var l) &&
            l.TryGetProperty("summary", out var ls))
            likes = ls.TryGetLong("total_count");

        if (root.TryGetProperty("comments", out var c) &&
            c.TryGetProperty("summary", out var cs))
            comments = cs.TryGetLong("total_count");

        if (root.TryGetProperty("shares", out var sh))
            shares = sh.TryGetLong("count");

        if (root.TryGetProperty("reactions", out var r) &&
            r.TryGetProperty("summary", out var rs))
            reactions = rs.TryGetLong("total_count");

        return new PostMetrics
        {
            PostId = root.TryGetString("id") ?? postId,
            Message = root.TryGetString("message") ?? "",
            CreatedTime = root.TryGetString("created_time") ?? "",
            LikeCount = likes,
            CommentCount = comments,
            ShareCount = shares,
            ReactionCount = reactions
        };
    }

    /// <summary>
    /// 批量获取多个帖子的统计数据
    /// </summary>
    public async Task<List<PostMetrics>> GetBatchPostMetricsAsync(IEnumerable<string> postIds)
    {
        var result = new List<PostMetrics>();
        // Facebook Batch API：一次最多 50 个请求
        var batches = postIds.Chunk(50);

        foreach (var batch in batches)
        {
            var requests = batch.Select(id => new
            {
                method = "GET",
                relative_url = $"{id}?fields=id,message,created_time,likes.summary(true),comments.summary(true),shares,reactions.summary(true)"
            }).ToList();

            string batchJson = JsonSerializer.Serialize(requests);
            string url = $"{ApiBase}?access_token={_accessToken}";

            var form = new Dictionary<string, string>
            {
                ["batch"] = batchJson,
                ["access_token"] = _accessToken
            };

            var doc = await PostFormAsync(url, form);

            foreach (var item in doc.RootElement.EnumerateArray())
            {
                int code = item.TryGetProperty("code", out var codeEl) ? codeEl.GetInt32() : 0;
                if (code != 200) continue;

                string body = item.TryGetProperty("body", out var bodyEl)
                    ? bodyEl.GetString() ?? "{}"
                    : "{}";

                try
                {
                    var postDoc = JsonDocument.Parse(body);
                    var postRoot = postDoc.RootElement;

                    long likes = 0, comments = 0, shares = 0, reactions = 0;

                    if (postRoot.TryGetProperty("likes", out var l) &&
                        l.TryGetProperty("summary", out var ls))
                        likes = ls.TryGetLong("total_count");

                    if (postRoot.TryGetProperty("comments", out var c) &&
                        c.TryGetProperty("summary", out var cs))
                        comments = cs.TryGetLong("total_count");

                    if (postRoot.TryGetProperty("shares", out var sh))
                        shares = sh.TryGetLong("count");

                    if (postRoot.TryGetProperty("reactions", out var r) &&
                        r.TryGetProperty("summary", out var rs))
                        reactions = rs.TryGetLong("total_count");

                    result.Add(new PostMetrics
                    {
                        PostId = postRoot.TryGetString("id") ?? "",
                        Message = postRoot.TryGetString("message") ?? "",
                        CreatedTime = postRoot.TryGetString("created_time") ?? "",
                        LikeCount = likes,
                        CommentCount = comments,
                        ShareCount = shares,
                        ReactionCount = reactions
                    });
                }
                catch { /* 跳过解析失败的单条 */ }
            }
        }

        return result;
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 五、评论管理
    // ══════════════════════════════════════════════════════════

    #region 评论管理

    /// <summary>
    /// 获取帖子的评论列表
    /// </summary>
    public async Task<CommentListResult> GetPostCommentsAsync(
        string postId,
        int limit = 20,
        string? after = null)
    {
        limit = Math.Clamp(limit, 1, 100);

        string url = $"{ApiBase}/{postId}/comments" +
                     $"?fields=id,message,from,created_time,like_count,comment_count" +
                     $"&limit={limit}&access_token={_accessToken}";

        if (!string.IsNullOrEmpty(after))
            url += $"&after={after}";

        var doc = await GetAsync(url);
        var result = new CommentListResult();

        if (doc.RootElement.TryGetProperty("data", out var dataArr))
        {
            foreach (var item in dataArr.EnumerateArray())
            {
                result.Comments.Add(new FbComment
                {
                    CommentId = item.TryGetString("id") ?? "",
                    Message = item.TryGetString("message") ?? "",
                    AuthorName = item.TryGetNestedString("from", "name") ?? "",
                    AuthorId = item.TryGetNestedString("from", "id") ?? "",
                    CreatedTime = item.TryGetString("created_time") ?? "",
                    LikeCount = item.TryGetLong("like_count"),
                    ReplyCount = item.TryGetLong("comment_count")
                });
            }
        }

        if (doc.RootElement.TryGetProperty("paging", out var paging) &&
            paging.TryGetProperty("cursors", out var cursors))
        {
            result.NextCursor = cursors.TryGetString("after");
        }

        return result;
    }

    /// <summary>
    /// 回复帖子评论
    /// </summary>
    public async Task<string> ReplyToCommentAsync(string commentId, string message)
    {
        string url = $"{ApiBase}/{commentId}/comments";

        var body = new Dictionary<string, string>
        {
            ["message"] = message,
            ["access_token"] = _accessToken
        };

        var doc = await PostFormAsync(url, body);
        return doc.RootElement.TryGetString("id") ?? "";
    }

    /// <summary>
    /// 删除评论
    /// </summary>
    public async Task<bool> DeleteCommentAsync(string commentId)
    {
        string url = $"{ApiBase}/{commentId}?access_token={_accessToken}";

        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new FacebookException($"删除评论失败: {json}", (int)response.StatusCode);

        var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("success", out var s) && s.GetBoolean();
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 六、私信（Messenger）
    // ══════════════════════════════════════════════════════════

    #region 私信

    /// <summary>
    /// 发送私信（需要用户先向主页发过消息）
    /// </summary>
    /// <param name="recipientId">收件人 PSID（Page-scoped User ID）</param>
    /// <param name="message">消息内容</param>
    public async Task<string> SendMessageAsync(string recipientId, string message)
    {
        string url = $"{ApiBase}/me/messages?access_token={_accessToken}";

        var payload = new
        {
            recipient = new { id = recipientId },
            message = new { text = message }
        };

        string jsonBody = JsonSerializer.Serialize(payload);
        var doc = await PostJsonAsync(url, jsonBody);
        return doc.RootElement.TryGetString("message_id") ?? "";
    }

    /// <summary>
    /// 发送带图片的私信
    /// </summary>
    public async Task<string> SendImageMessageAsync(string recipientId, string imageUrl)
    {
        string url = $"{ApiBase}/me/messages?access_token={_accessToken}";

        var payload = new
        {
            recipient = new { id = recipientId },
            message = new
            {
                attachment = new
                {
                    type = "image",
                    payload = new { url = imageUrl, is_reusable = true }
                }
            }
        };

        string jsonBody = JsonSerializer.Serialize(payload);
        var doc = await PostJsonAsync(url, jsonBody);
        return doc.RootElement.TryGetString("message_id") ?? "";
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 七、媒体上传（内部）
    // ══════════════════════════════════════════════════════════

    #region 媒体上传

    /// <summary>
    /// 上传图片（不发布）返回 media_fbid，用于组合多图帖子
    /// </summary>
    private async Task<string> UploadPhotoUnpublishedAsync(string imagePath)
    {
        string url = $"{ApiBase}/{_pageId}/photos";

        using var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(await File.ReadAllBytesAsync(imagePath)),
                 "source", Path.GetFileName(imagePath));
        form.Add(new StringContent("true"), "published");
        form.Add(new StringContent("true"), "temporary");
        form.Add(new StringContent(_accessToken), "access_token");

        // published=false 表示只上传不发布，获取 id 用于组合帖子
        // 注意：此处 published=true + temporary=true 是为了获取 fbid
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = form };
        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new FacebookException($"上传图片失败: {json}", (int)response.StatusCode);

        var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetString("id") ?? "";
    }

    /// <summary>
    /// 分块上传视频（支持大文件，Facebook 要求 > 1GB 必须分块）
    /// 流程：start → transfer（分块）→ finish
    /// </summary>
    private async Task<string> UploadVideoChunkedAsync(
        string videoPath,
        string description,
        string? title = null,
        Action<string>? callBack = null)
    {
        string uploadUrl = $"https://graph-video.facebook.com/v21.0/{_pageId}/videos";
        byte[] bytes = await File.ReadAllBytesAsync(videoPath);
        long fileSize = bytes.Length;

        // ── START ────────────────────────────────────────────
        var startForm = new MultipartFormDataContent();
        startForm.Add(new StringContent("start"), "upload_phase");
        startForm.Add(new StringContent(fileSize.ToString()), "file_size");
        startForm.Add(new StringContent(_accessToken), "access_token");

        var startResp = await _http.PostAsync(uploadUrl, startForm);
        string startJson = await startResp.Content.ReadAsStringAsync();
        if (!startResp.IsSuccessStatusCode)
            throw new FacebookException($"视频上传 START 失败: {startJson}", (int)startResp.StatusCode);

        var startDoc = JsonDocument.Parse(startJson).RootElement;
        string uploadSessionId = startDoc.TryGetString("upload_session_id") ?? "";
        string videoId = startDoc.TryGetString("video_id") ?? "";
        long startOffset = startDoc.TryGetLong("start_offset");
        long endOffset = startDoc.TryGetLong("end_offset");

        callBack?.Invoke($"视频上传 START 完成，session_id：{uploadSessionId}");

        // ── TRANSFER（分块）──────────────────────────────────
        int segIndex = 0;
        int totalSegs = (int)Math.Ceiling((double)fileSize / (endOffset - startOffset == 0 ? fileSize : endOffset - startOffset));

        while (startOffset < fileSize)
        {
            long chunkSize = endOffset - startOffset;
            byte[] chunk = bytes.Skip((int)startOffset).Take((int)chunkSize).ToArray();

            using var transferForm = new MultipartFormDataContent();
            transferForm.Add(new StringContent("transfer"), "upload_phase");
            transferForm.Add(new StringContent(uploadSessionId), "upload_session_id");
            transferForm.Add(new StringContent(startOffset.ToString()), "start_offset");
            transferForm.Add(new StringContent(_accessToken), "access_token");
            transferForm.Add(new ByteArrayContent(chunk), "video_file_chunk",
                             Path.GetFileName(videoPath));

            var transResp = await _http.PostAsync(uploadUrl, transferForm);
            string transJson = await transResp.Content.ReadAsStringAsync();
            if (!transResp.IsSuccessStatusCode)
                throw new FacebookException(
                    $"视频上传 TRANSFER 失败 (段 {segIndex}): {transJson}", (int)transResp.StatusCode);

            var transDoc = JsonDocument.Parse(transJson).RootElement;
            startOffset = transDoc.TryGetLong("start_offset");
            endOffset = transDoc.TryGetLong("end_offset");

            callBack?.Invoke($"上传进度：{segIndex + 1} 块，已上传 {startOffset:N0} / {fileSize:N0} 字节");
            segIndex++;

            if (startOffset >= fileSize) break;
        }

        // ── FINISH ───────────────────────────────────────────
        var finishForm = new MultipartFormDataContent();
        finishForm.Add(new StringContent("finish"), "upload_phase");
        finishForm.Add(new StringContent(uploadSessionId), "upload_session_id");
        finishForm.Add(new StringContent(description), "description");
        finishForm.Add(new StringContent(_accessToken), "access_token");

        if (!string.IsNullOrEmpty(title))
            finishForm.Add(new StringContent(title), "title");

        var finishResp = await _http.PostAsync(uploadUrl, finishForm);
        string finishJson = await finishResp.Content.ReadAsStringAsync();
        if (!finishResp.IsSuccessStatusCode)
            throw new FacebookException($"视频上传 FINISH 失败: {finishJson}", (int)finishResp.StatusCode);

        callBack?.Invoke("视频上传完成，正在等待 Facebook 处理...");

        // 等待视频处理完成
        await WaitForVideoProcessingAsync(videoId, callBack);

        return videoId;
    }

    /// <summary>
    /// 轮询等待视频处理完成
    /// </summary>
    private async Task WaitForVideoProcessingAsync(string videoId, Action<string>? callBack = null)
    {
        string url = $"{ApiBase}/{videoId}?fields=status&access_token={_accessToken}";
        int maxWait = 60;

        for (int i = 0; i < maxWait; i++)
        {
            await Task.Delay(3000);

            try
            {
                var doc = await GetAsync(url);
                var status = doc.RootElement.TryGetNestedString("status", "video_status");

                callBack?.Invoke($"视频处理状态：{status}");

                if (status == "ready") return;
                if (status == "error")
                    throw new FacebookException("Facebook 视频处理失败", 422);
            }
            catch (FacebookException) { throw; }
            catch { /* 忽略查询异常，继续等待 */ }
        }
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 内部辅助方法
    // ══════════════════════════════════════════════════════════

    #region 内部辅助

    private async Task<JsonDocument> GetAsync(string url)
    {
        var response = await _http.GetAsync(url);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new FacebookException($"请求失败 [{url}]: {json}", (int)response.StatusCode);

        return JsonDocument.Parse(json);
    }

    private async Task<JsonDocument> PostFormAsync(string url, Dictionary<string, string> form)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(form)
        };
        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new FacebookException($"请求失败: {json}", (int)response.StatusCode);

        return JsonDocument.Parse(json);
    }

    private async Task<JsonDocument> PostJsonAsync(string url, string jsonBody)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };
        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new FacebookException($"请求失败: {json}", (int)response.StatusCode);

        return JsonDocument.Parse(json);
    }

    private static PostListResult ParsePostListResult(JsonElement root, bool isVideo = false)
    {
        var result = new PostListResult();

        if (root.TryGetProperty("data", out var dataArr))
        {
            foreach (var item in dataArr.EnumerateArray())
                result.Posts.Add(ParsePost(item, isVideo));
        }

        if (root.TryGetProperty("paging", out var paging) &&
            paging.TryGetProperty("cursors", out var cursors))
        {
            result.NextCursor = cursors.TryGetString("after");
        }

        return result;
    }

    private static FbPost ParsePost(JsonElement item, bool isVideo = false)
    {
        long likes = 0, comments = 0, shares = 0;

        if (item.TryGetProperty("likes", out var l) &&
            l.TryGetProperty("summary", out var ls))
            likes = ls.TryGetLong("total_count");

        if (item.TryGetProperty("comments", out var c) &&
            c.TryGetProperty("summary", out var cs))
            comments = cs.TryGetLong("total_count");

        if (item.TryGetProperty("shares", out var sh))
            shares = sh.TryGetLong("count");

        return new FbPost
        {
            PostId = item.TryGetString("id") ?? "",
            Message = item.TryGetString(isVideo ? "description" : "message") ?? "",
            Title = item.TryGetString("title") ?? "",
            CreatedTime = item.TryGetString("created_time") ?? "",
            PictureUrl = item.TryGetString(isVideo ? "picture" : "full_picture") ?? "",
            PermalinkUrl = item.TryGetString("permalink_url") ?? "",
            ViewCount = item.TryGetLong("views"),
            Duration = item.TryGetLong("length"),
            LikeCount = likes,
            CommentCount = comments,
            ShareCount = shares,
            IsVideo = isVideo
        };
    }

    public void Dispose() => _http.Dispose();

    #endregion
}

// ══════════════════════════════════════════════════════════
// JsonElement 扩展方法
// ══════════════════════════════════════════════════════════

internal static class JsonElementExtensions
{
    public static string? TryGetString(this JsonElement el, string key)
    {
        return el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;
    }

    public static long TryGetLong(this JsonElement el, string key)
    {
        if (!el.TryGetProperty(key, out var v)) return 0;
        return v.ValueKind == JsonValueKind.Number ? v.GetInt64() : 0;
    }

    public static string? TryGetNestedString(this JsonElement el, params string[] keys)
    {
        var current = el;
        foreach (var key in keys)
        {
            if (!current.TryGetProperty(key, out current)) return null;
        }
        return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
    }
}

// ══════════════════════════════════════════════════════════
// 数据模型
// ══════════════════════════════════════════════════════════

public record PostResult
{
    public string PostId { get; init; } = "";
}

public class PageInfo
{
    public string? PageId { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Bio { get; set; }
    public string? Category { get; set; }
    public long FanCount { get; set; }
    public long FollowersCount { get; set; }
    public long PostsCount { get; set; }
    public string? Website { get; set; }
    public bool IsVerified { get; set; }
    public string? CreatedTime { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

public class PageInsight
{
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public long Value { get; set; }
    public string Period { get; set; } = "";
}

public class FbPost
{
    public string PostId { get; set; } = "";
    public string Message { get; set; } = "";
    public string Title { get; set; } = "";
    public string CreatedTime { get; set; } = "";
    public string PictureUrl { get; set; } = "";
    public string PermalinkUrl { get; set; } = "";
    public long LikeCount { get; set; }
    public long CommentCount { get; set; }
    public long ShareCount { get; set; }
    public long ViewCount { get; set; }
    public long Duration { get; set; } // 视频时长（秒）
    public bool IsVideo { get; set; }
}

public class PostListResult
{
    public List<FbPost> Posts { get; } = new();
    public string? NextCursor { get; set; }
}

public class PostMetrics
{
    public string PostId { get; set; } = "";
    public string Message { get; set; } = "";
    public string CreatedTime { get; set; } = "";
    public long LikeCount { get; set; }
    public long CommentCount { get; set; }
    public long ShareCount { get; set; }
    public long ReactionCount { get; set; }
}

public class FbComment
{
    public string CommentId { get; set; } = "";
    public string Message { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public string AuthorId { get; set; } = "";
    public string CreatedTime { get; set; } = "";
    public long LikeCount { get; set; }
    public long ReplyCount { get; set; }
}

public class CommentListResult
{
    public List<FbComment> Comments { get; } = new();
    public string? NextCursor { get; set; }
}

public class FacebookException : Exception
{
    public int StatusCode { get; }
    public FacebookException(string message, int statusCode) : base(message)
        => StatusCode = statusCode;
}