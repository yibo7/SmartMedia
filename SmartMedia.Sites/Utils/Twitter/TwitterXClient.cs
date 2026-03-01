

namespace SmartMedia.Sites.Utils.Twitter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// Twitter/X API 工具类
/// 支持：发推文、上传图片、获取图文列表、获取统计数据、用户信息、删除推文等常用功能
/// </summary>
public class TwitterXClient : IDisposable
{
    #region 配置与初始化

    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _accessToken;
    private readonly string _accessSecret;
    private readonly HttpClient _http;

    private const string UploadBaseUrl = "https://upload.twitter.com/1.1";
    private const string ApiV1BaseUrl = "https://api.twitter.com/1.1";
    private const string ApiV2BaseUrl = "https://api.twitter.com/2";

    public TwitterXClient(string apiKey, string apiSecret, string accessToken, string accessSecret)
    {
        _apiKey = apiKey;
        _apiSecret = apiSecret;
        _accessToken = accessToken;
        _accessSecret = accessSecret;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 一、用户信息
    // ══════════════════════════════════════════════════════════

    #region 用户信息

    /// <summary>
    /// 获取当前授权用户的基本信息
    /// </summary>
    public async Task<JsonElement> GetMyProfileAsync()
    {
        string url = $"{ApiV2BaseUrl}/users/me" +
                     "?user.fields=id,name,username,description,profile_image_url," +
                     "public_metrics,created_at,verified,location,url";

        return await GetV2Async(url);
    }

    /// <summary>
    /// 根据用户名查询用户信息
    /// </summary>
    public async Task<JsonElement> GetUserByUsernameAsync(string username)
    {
        string url = $"{ApiV2BaseUrl}/users/by/username/{username}" +
                     "?user.fields=id,name,username,description,public_metrics,created_at,verified";

        return await GetV2Async(url);
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 二、推文发布
    // ══════════════════════════════════════════════════════════

    #region 推文发布

    /// <summary>
    /// 发布纯文字推文
    /// </summary>
    /// <param name="text">推文内容（最多280字符）</param>
    public async Task<TweetResult> PostTweetAsync(string text)
    {
        return await PostTweetCoreAsync(text, null);
    }

    /// <summary>
    /// 发布图文推文（自动上传图片）
    /// </summary>
    /// <param name="text">推文内容</param>
    /// <param name="imagePaths">图片路径列表（最多4张）</param>
    public async Task<TweetResult> PostTweetWithImagesAsync(string text, IEnumerable<string> imagePaths)
    {
        var paths = imagePaths.Take(4).ToList();
        var mediaIds = new List<string>();

        foreach (var path in paths)
        {
            string mediaId = await UploadMediaAsync(path);
            mediaIds.Add(mediaId);
        }

        return await PostTweetCoreAsync(text, mediaIds);
    }

    /// <summary>
    /// 回复某条推文
    /// </summary>
    public async Task<TweetResult> ReplyToTweetAsync(string text, string replyToTweetId, IEnumerable<string>? imagePaths = null)
    {
        List<string>? mediaIds = null;
        if (imagePaths != null)
        {
            mediaIds = new List<string>();
            foreach (var path in imagePaths.Take(4))
                mediaIds.Add(await UploadMediaAsync(path));
        }

        string url = $"{ApiV2BaseUrl}/tweets";
        var payload = new Dictionary<string, object>
        {
            ["text"] = text,
            ["reply"] = new { in_reply_to_tweet_id = replyToTweetId }
        };
        if (mediaIds?.Count > 0)
            payload["media"] = new { media_ids = mediaIds };

        return await PostTweetRequestAsync(url, payload);
    }

    /// <summary>
    /// 转推
    /// </summary>
    public async Task<JsonElement> RetweetAsync(string userId, string tweetId)
    {
        string url = $"{ApiV2BaseUrl}/users/{userId}/retweets";
        string body = JsonSerializer.Serialize(new { tweet_id = tweetId });
        return await PostJsonV2Async(url, body);
    }

    /// <summary>
    /// 点赞推文
    /// </summary>
    public async Task<JsonElement> LikeTweetAsync(string userId, string tweetId)
    {
        string url = $"{ApiV2BaseUrl}/users/{userId}/likes";
        string body = JsonSerializer.Serialize(new { tweet_id = tweetId });
        return await PostJsonV2Async(url, body);
    }

    /// <summary>
    /// 删除推文
    /// </summary>
    public async Task<bool> DeleteTweetAsync(string tweetId)
    {
        string url = $"{ApiV2BaseUrl}/tweets/{tweetId}";
        var authHeader = BuildOAuthHeader("DELETE", url, new Dictionary<string, string>());
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new TwitterException($"删除推文失败: {json}", (int)response.StatusCode);

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").GetProperty("deleted").GetBoolean();
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 三、图文列表查询
    // ══════════════════════════════════════════════════════════

    #region 图文列表查询

    /// <summary>
    /// 获取指定用户的推文列表（含图文）
    /// </summary>
    /// <param name="userId">用户ID（可通过 GetMyProfileAsync 获取）</param>
    /// <param name="maxResults">每页条数，最大100</param>
    /// <param name="paginationToken">翻页令牌，首次为null</param>
    /// <param name="onlyWithMedia">是否只返回含图片/视频的推文</param>
    public async Task<TweetListResult> GetUserTweetsAsync(
        string userId,
        int maxResults = 20,
        string? paginationToken = null,
        bool onlyWithMedia = false)
    {
        maxResults = Math.Clamp(maxResults, 5, 100);

        var queryParams = new List<string>
        {
            $"max_results={maxResults}",
            "tweet.fields=id,text,created_at,public_metrics,attachments,possibly_sensitive",
            "expansions=attachments.media_keys",
            "media.fields=media_key,type,url,preview_image_url,width,height,public_metrics"
        };

        if (onlyWithMedia)
            queryParams.Add("exclude=retweets,replies");

        if (!string.IsNullOrEmpty(paginationToken))
            queryParams.Add($"pagination_token={paginationToken}");

        string url = $"{ApiV2BaseUrl}/users/{userId}/tweets?{string.Join("&", queryParams)}";
        var result = await GetV2Async(url);

        return ParseTweetListResult(result);
    }

    /// <summary>
    /// 获取主页时间线（关注的人的推文）
    /// </summary>
    public async Task<TweetListResult> GetHomeTimelineAsync(
        string userId,
        int maxResults = 20,
        string? paginationToken = null)
    {
        maxResults = Math.Clamp(maxResults, 1, 100);

        var queryParams = new List<string>
        {
            $"max_results={maxResults}",
            "tweet.fields=id,text,created_at,public_metrics,attachments,author_id",
            "expansions=attachments.media_keys,author_id",
            "media.fields=media_key,type,url,preview_image_url,width,height",
            "user.fields=id,name,username,profile_image_url"
        };

        if (!string.IsNullOrEmpty(paginationToken))
            queryParams.Add($"pagination_token={paginationToken}");

        string url = $"{ApiV2BaseUrl}/users/{userId}/timelines/reverse_chronological?{string.Join("&", queryParams)}";
        var result = await GetV2Async(url);

        return ParseTweetListResult(result);
    }

    /// <summary>
    /// 搜索推文
    /// </summary>
    /// <param name="query">搜索关键词，支持 AND/OR/from:user 等语法</param>
    /// <param name="maxResults">最多100条</param>
    public async Task<TweetListResult> SearchTweetsAsync(
        string query,
        int maxResults = 20,
        string? paginationToken = null)
    {
        maxResults = Math.Clamp(maxResults, 10, 100);

        var queryParams = new List<string>
        {
            $"query={Uri.EscapeDataString(query)}",
            $"max_results={maxResults}",
            "tweet.fields=id,text,created_at,public_metrics,attachments,author_id",
            "expansions=attachments.media_keys,author_id",
            "media.fields=media_key,type,url,preview_image_url,width,height",
            "user.fields=id,name,username,profile_image_url"
        };

        if (!string.IsNullOrEmpty(paginationToken))
            queryParams.Add($"next_token={paginationToken}");

        string url = $"{ApiV2BaseUrl}/tweets/search/recent?{string.Join("&", queryParams)}";
        var result = await GetV2Async(url);

        return ParseTweetListResult(result);
    }

    /// <summary>
    /// 获取某条推文详情
    /// </summary>
    public async Task<JsonElement> GetTweetByIdAsync(string tweetId)
    {
        string url = $"{ApiV2BaseUrl}/tweets/{tweetId}" +
                     "?tweet.fields=id,text,created_at,public_metrics,attachments,author_id" +
                     "&expansions=attachments.media_keys,author_id" +
                     "&media.fields=media_key,type,url,preview_image_url,width,height" +
                     "&user.fields=id,name,username,profile_image_url";

        return await GetV2Async(url);
    }

    /// <summary>
    /// 获取点赞过的推文列表
    /// </summary>
    public async Task<TweetListResult> GetLikedTweetsAsync(string userId, int maxResults = 20)
    {
        maxResults = Math.Clamp(maxResults, 1, 100);
        string url = $"{ApiV2BaseUrl}/users/{userId}/liked_tweets" +
                     $"?max_results={maxResults}" +
                     "&tweet.fields=id,text,created_at,public_metrics,author_id" +
                     "&expansions=author_id&user.fields=id,name,username";

        var result = await GetV2Async(url);
        return ParseTweetListResult(result);
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 四、统计数据
    // ══════════════════════════════════════════════════════════

    #region 统计数据

    /// <summary>
    /// 获取当前授权账号的统计数据（免费）
    /// 内部使用 /users/me 接口，Free 套餐可用
    /// 注意：Free 套餐无法查询其他用户的统计数据，需升级至 Basic 套餐
    /// </summary>
    public async Task<UserMetrics> GetUserMetricsAsync(string? userId = null)
    {
        // 使用 /users/me，Free 套餐免费且返回完整 public_metrics
        string url = $"{ApiV2BaseUrl}/users/me?user.fields=public_metrics,created_at,name,username";
        var doc = await GetV2Async(url);
        var data = doc.GetProperty("data");

        string myId = data.GetProperty("id").GetString()!;

        // 如果指定了 userId 且不是自己，明确提示需要付费套餐
        if (userId != null && userId != myId)
            throw new TwitterException(
                $"Free 套餐不支持查询其他用户（userId={userId}）的统计数据，请升级至 Basic 套餐。", 402);

        var metrics = data.GetProperty("public_metrics");
        return new UserMetrics
        {
            UserId = myId,
            Name = data.GetProperty("name").GetString()!,
            Username = data.GetProperty("username").GetString()!,
            FollowersCount = metrics.GetProperty("followers_count").GetInt64(),
            FollowingCount = metrics.GetProperty("following_count").GetInt64(),
            TweetCount = metrics.GetProperty("tweet_count").GetInt64(),
            LikedCount = metrics.TryGetProperty("like_count", out var lc) ? lc.GetInt64() : 0,
            ListedCount = metrics.GetProperty("listed_count").GetInt64()
        };
    }

    /// <summary>
    /// 获取单条推文的互动统计（点赞、转推、回复、浏览量）
    /// </summary>
    public async Task<TweetMetrics> GetTweetMetricsAsync(string tweetId)
    {
        string url = $"{ApiV2BaseUrl}/tweets/{tweetId}" +
                     "?tweet.fields=public_metrics,non_public_metrics,organic_metrics,created_at,text";

        var doc = await GetV2Async(url);
        var data = doc.GetProperty("data");

        var pub = data.GetProperty("public_metrics");
        var metrics = new TweetMetrics
        {
            TweetId = data.GetProperty("id").GetString()!,
            Text = data.GetProperty("text").GetString()!,
            CreatedAt = data.GetProperty("created_at").GetString()!,
            LikeCount = pub.GetProperty("like_count").GetInt64(),
            RetweetCount = pub.GetProperty("retweet_count").GetInt64(),
            ReplyCount = pub.GetProperty("reply_count").GetInt64(),
            QuoteCount = pub.GetProperty("quote_count").GetInt64(),
            ImpressionCount = pub.TryGetProperty("impression_count", out var ic) ? ic.GetInt64() : 0
        };

        // 私有指标（需要特殊权限）
        if (data.TryGetProperty("non_public_metrics", out var npm))
        {
            metrics.UrlLinkClicks = npm.TryGetProperty("url_link_clicks", out var ulc) ? ulc.GetInt64() : 0;
            metrics.UserProfileClicks = npm.TryGetProperty("user_profile_clicks", out var upc) ? upc.GetInt64() : 0;
        }

        return metrics;
    }

    /// <summary>
    /// 批量获取多条推文的统计数据
    /// </summary>
    public async Task<List<TweetMetrics>> GetBatchTweetMetricsAsync(IEnumerable<string> tweetIds)
    {
        var ids = tweetIds.Take(100).ToList();
        string url = $"{ApiV2BaseUrl}/tweets?ids={string.Join(",", ids)}" +
                     "&tweet.fields=public_metrics,created_at,text";

        var doc = await GetV2Async(url);
        var result = new List<TweetMetrics>();

        if (doc.TryGetProperty("data", out var dataArray))
        {
            foreach (var item in dataArray.EnumerateArray())
            {
                var pub = item.GetProperty("public_metrics");
                result.Add(new TweetMetrics
                {
                    TweetId = item.GetProperty("id").GetString()!,
                    Text = item.GetProperty("text").GetString()!,
                    CreatedAt = item.GetProperty("created_at").GetString()!,
                    LikeCount = pub.GetProperty("like_count").GetInt64(),
                    RetweetCount = pub.GetProperty("retweet_count").GetInt64(),
                    ReplyCount = pub.GetProperty("reply_count").GetInt64(),
                    QuoteCount = pub.GetProperty("quote_count").GetInt64()
                });
            }
        }

        return result;
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 五、关系管理
    // ══════════════════════════════════════════════════════════

    #region 关系管理

    /// <summary>
    /// 获取粉丝列表
    /// </summary>
    public async Task<JsonElement> GetFollowersAsync(string userId, int maxResults = 100)
    {
        maxResults = Math.Clamp(maxResults, 1, 1000);
        string url = $"{ApiV2BaseUrl}/users/{userId}/followers" +
                     $"?max_results={maxResults}" +
                     "&user.fields=id,name,username,public_metrics,profile_image_url";
        return await GetV2Async(url);
    }

    /// <summary>
    /// 获取关注列表
    /// </summary>
    public async Task<JsonElement> GetFollowingAsync(string userId, int maxResults = 100)
    {
        maxResults = Math.Clamp(maxResults, 1, 1000);
        string url = $"{ApiV2BaseUrl}/users/{userId}/following" +
                     $"?max_results={maxResults}" +
                     "&user.fields=id,name,username,public_metrics,profile_image_url";
        return await GetV2Async(url);
    }

    /// <summary>
    /// 关注用户
    /// </summary>
    public async Task<JsonElement> FollowUserAsync(string myUserId, string targetUserId)
    {
        string url = $"{ApiV2BaseUrl}/users/{myUserId}/following";
        string body = JsonSerializer.Serialize(new { target_user_id = targetUserId });
        return await PostJsonV2Async(url, body);
    }

    /// <summary>
    /// 取消关注
    /// </summary>
    public async Task<JsonElement> UnfollowUserAsync(string myUserId, string targetUserId)
    {
        string url = $"{ApiV2BaseUrl}/users/{myUserId}/following/{targetUserId}";
        var authHeader = BuildOAuthHeader("DELETE", url, new Dictionary<string, string>());
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new TwitterException($"取消关注失败: {json}", (int)response.StatusCode);

        return JsonDocument.Parse(json).RootElement;
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 六、媒体上传
    // ══════════════════════════════════════════════════════════

    #region 媒体上传

    /// <summary>
    /// 上传图片并返回 media_id（≤5MB 图片用简单上传，>5MB 自动切换分块上传）
    /// </summary>
    public async Task<string> UploadMediaAsync(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
            throw new FileNotFoundException($"文件不存在: {filePath}");

        long fileSizeMb = fileInfo.Length / 1024 / 1024;
        return fileSizeMb >= 5
            ? await UploadMediaChunkedAsync(filePath)
            : await UploadMediaSimpleAsync(filePath);
    }

    private async Task<string> UploadMediaSimpleAsync(string filePath)
    {
        string url = $"{UploadBaseUrl}/media/upload.json";
        byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
        string base64 = Convert.ToBase64String(imageBytes);

        // ✅ 修复：表单参数必须参与 OAuth 1.0a 签名
        // media_data 字段内容较大，签名时传入会导致 base string 过长，
        // upload.twitter.com 官方推荐：简单上传时只签名空参数，
        // 改用 multipart/form-data 避免签名参数冲突
        var sigParams = new Dictionary<string, string>(); // 不含 media_data，符合官方建议
        var authHeader = BuildOAuthHeader("POST", url, sigParams);

        // 使用 multipart/form-data 上传，比 base64 FormUrlEncoded 更稳定
        using var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(imageBytes), "media", Path.GetFileName(filePath));

        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = form };
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new TwitterException($"上传图片失败: {json}", (int)response.StatusCode);

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("media_id_string").GetString()!;
    }

    private async Task<string> UploadMediaChunkedAsync(string filePath)
    {
        string url = $"{UploadBaseUrl}/media/upload.json";
        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
        string mimeType = GetMimeType(filePath);

        // ── INIT ────────────────────────────────────────────────
        var initParams = new Dictionary<string, string>
        {
            ["command"] = "INIT",
            ["total_bytes"] = fileBytes.Length.ToString(),
            ["media_type"] = mimeType,
            ["media_category"] = "tweet_image"
        };
        // ✅ 修复：表单参数传入签名
        var authHeader = BuildOAuthHeader("POST", url, initParams);
        var initReq = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(initParams)
        };
        initReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
        var initResp = await _http.SendAsync(initReq);
        string initJson = await initResp.Content.ReadAsStringAsync();
        if (!initResp.IsSuccessStatusCode)
            throw new TwitterException($"分块上传 INIT 失败: {initJson}", (int)initResp.StatusCode);

        string mediaId = JsonDocument.Parse(initJson)
            .RootElement.GetProperty("media_id_string").GetString()!;

        // ── APPEND（每块 ≤ 5MB，media_data 不参与签名）──────────
        int chunkSize = 5 * 1024 * 1024;
        int segIndex = 0;
        for (int offset = 0; offset < fileBytes.Length; offset += chunkSize)
        {
            byte[] chunk = fileBytes.Skip(offset).Take(chunkSize).ToArray();
            string chunkB64 = Convert.ToBase64String(chunk);

            // APPEND 签名只含 command/media_id/segment_index，不含 media_data
            var appendSigParams = new Dictionary<string, string>
            {
                ["command"] = "APPEND",
                ["media_id"] = mediaId,
                ["segment_index"] = segIndex.ToString()
            };
            authHeader = BuildOAuthHeader("POST", url, appendSigParams);

            // 发送时包含 media_data，但不在签名里
            var appendBody = new Dictionary<string, string>(appendSigParams)
            {
                ["media_data"] = chunkB64
            };
            var appendReq = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(appendBody)
            };
            appendReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
            var appendResp = await _http.SendAsync(appendReq);
            if (!appendResp.IsSuccessStatusCode)
            {
                string appendErr = await appendResp.Content.ReadAsStringAsync();
                throw new TwitterException($"分块上传 APPEND 失败 (段 {segIndex}): {appendErr}", (int)appendResp.StatusCode);
            }
            segIndex++;
        }

        // ── FINALIZE ────────────────────────────────────────────
        var finalParams = new Dictionary<string, string>
        {
            ["command"] = "FINALIZE",
            ["media_id"] = mediaId
        };
        // ✅ 修复：表单参数传入签名
        authHeader = BuildOAuthHeader("POST", url, finalParams);
        var finalReq = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(finalParams)
        };
        finalReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
        var finalResp = await _http.SendAsync(finalReq);
        if (!finalResp.IsSuccessStatusCode)
        {
            string finalErr = await finalResp.Content.ReadAsStringAsync();
            throw new TwitterException($"分块上传 FINALIZE 失败: {finalErr}", (int)finalResp.StatusCode);
        }

        return mediaId;
    }

    #endregion


    #region 发布小视频推特

    /// <summary>
    /// 上传 MP4 视频（分块上传，带进度回调）
    /// 流程：INIT → APPEND（每块5MB）→ FINALIZE → 轮询等待处理完成
    /// </summary>
    public async Task<string> UploadVideoAsync(string filePath, Action<string>? callBack = null)
    {
        string url = $"{UploadBaseUrl}/media/upload.json";
        byte[] bytes = await File.ReadAllBytesAsync(filePath);
        long totalBytes = bytes.Length;

        // ── INIT ─────────────────────────────────────────────
        var initParams = new Dictionary<string, string>
        {
            ["command"] = "INIT",
            ["total_bytes"] = totalBytes.ToString(),
            ["media_type"] = "video/mp4",
            ["media_category"] = "tweet_video"  // 视频必须用 tweet_video，否则处理失败
        };
        var authHeader = BuildOAuthHeader("POST", url, initParams);
        var initReq = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(initParams)
        };
        initReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        var initResp = await _http.SendAsync(initReq);
        string initJson = await initResp.Content.ReadAsStringAsync();
        if (!initResp.IsSuccessStatusCode)
            throw new TwitterException($"视频上传 INIT 失败: {initJson}", (int)initResp.StatusCode);

        string mediaId = JsonDocument.Parse(initJson)
            .RootElement.GetProperty("media_id_string").GetString()!;
        callBack?.Invoke($"INIT 完成，开始分块上传...");

        // ── APPEND（每块 5MB）────────────────────────────────
        int chunkSize = 5 * 1024 * 1024;
        int segIndex = 0;
        int totalSegs = (int)Math.Ceiling((double)totalBytes / chunkSize);

        for (int offset = 0; offset < totalBytes; offset += chunkSize)
        {
            byte[] chunk = bytes.Skip(offset).Take(chunkSize).ToArray();
            string chunkB64 = Convert.ToBase64String(chunk);

            // media_data 不参与签名（体积太大），只签名其他三个字段
            var appendSigParams = new Dictionary<string, string>
            {
                ["command"] = "APPEND",
                ["media_id"] = mediaId,
                ["segment_index"] = segIndex.ToString()
            };
            authHeader = BuildOAuthHeader("POST", url, appendSigParams);

            var appendBody = new Dictionary<string, string>(appendSigParams)
            {
                ["media_data"] = chunkB64
            };
            var appendReq = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(appendBody)
            };
            appendReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

            var appendResp = await _http.SendAsync(appendReq);
            if (!appendResp.IsSuccessStatusCode)
            {
                string err = await appendResp.Content.ReadAsStringAsync();
                throw new TwitterException($"视频上传 APPEND 失败 (段 {segIndex}): {err}", (int)appendResp.StatusCode);
            }

            callBack?.Invoke($"上传进度：{segIndex + 1}/{totalSegs} 块");
            segIndex++;
        }

        // ── FINALIZE ──────────────────────────────────────────
        var finalParams = new Dictionary<string, string>
        {
            ["command"] = "FINALIZE",
            ["media_id"] = mediaId
        };
        authHeader = BuildOAuthHeader("POST", url, finalParams);
        var finalReq = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(finalParams)
        };
        finalReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        var finalResp = await _http.SendAsync(finalReq);
        string finalJson = await finalResp.Content.ReadAsStringAsync();
        if (!finalResp.IsSuccessStatusCode)
            throw new TwitterException($"视频上传 FINALIZE 失败: {finalJson}", (int)finalResp.StatusCode);

        // ── 轮询等待视频处理完成（X 后台转码需要时间）────────
        callBack?.Invoke("视频上传完成，等待 X 服务器处理转码...");
        await WaitForVideoProcessingAsync(mediaId, callBack);

        return mediaId;
    }

    /// <summary>
    /// 轮询等待 X 后台视频转码完成
    /// </summary>
    private async Task WaitForVideoProcessingAsync(string mediaId, Action<string>? callBack = null)
    {
        string url = $"{UploadBaseUrl}/media/upload.json?command=STATUS&media_id={mediaId}";
        int retries = 0;
        int maxWait = 60; // 最多等待 60 秒

        while (retries < maxWait)
        {
            await Task.Delay(3000); // 每 3 秒查询一次

            var queryParams = ParseQueryParams(url);
            var authHeader = BuildOAuthHeader("GET", CleanUrl(url), queryParams);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

            var resp = await _http.SendAsync(req);
            string json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode) break;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("processing_info", out var info)) break;

            string state = info.GetProperty("state").GetString()!;
            int pct = info.TryGetProperty("progress_percent", out var p) ? p.GetInt32() : 0;

            callBack?.Invoke($"视频处理中：{state}（{pct}%）");

            if (state == "succeeded") return;
            if (state == "failed")
                throw new TwitterException("X 服务器视频转码失败，请检查视频格式", 422);

            retries++;
        }
    }

    /// <summary>
    /// 使用已有 media_id 发布推文（适用于视频/图片上传后）
    /// </summary>
    public async Task<TweetResult> PostTweetWithMediaIdAsync(string text, string mediaId)
    {
        string url = $"{ApiV2BaseUrl}/tweets";
        var payload = new Dictionary<string, object>
        {
            ["text"] = text,
            ["media"] = new { media_ids = new[] { mediaId } }
        };
        return await PostTweetRequestAsync(url, payload);
    } 

    #endregion

    // ══════════════════════════════════════════════════════════
    // 七、私信（DM）
    // ══════════════════════════════════════════════════════════

    #region 私信

    /// <summary>
    /// 发送私信
    /// </summary>
    public async Task<JsonElement> SendDirectMessageAsync(string targetUserId, string text)
    {
        string url = $"{ApiV2BaseUrl}/dm_conversations/with/{targetUserId}/messages";
        string body = JsonSerializer.Serialize(new { text });
        return await PostJsonV2Async(url, body);
    }

    /// <summary>
    /// 获取私信列表
    /// </summary>
    public async Task<JsonElement> GetDirectMessagesAsync(int maxResults = 20)
    {
        maxResults = Math.Clamp(maxResults, 1, 100);
        string url = $"{ApiV2BaseUrl}/dm_events?max_results={maxResults}" +
                     "&dm_event.fields=id,text,created_at,sender_id,attachments" +
                     "&expansions=sender_id&user.fields=id,name,username";
        return await GetV2Async(url);
    }

    #endregion

    // ══════════════════════════════════════════════════════════
    // 内部辅助方法
    // ══════════════════════════════════════════════════════════

    #region 内部辅助

    private async Task<TweetResult> PostTweetCoreAsync(string text, List<string>? mediaIds)
    {
        string url = $"{ApiV2BaseUrl}/tweets";
        var payload = new Dictionary<string, object> { ["text"] = text };
        if (mediaIds?.Count > 0)
            payload["media"] = new { media_ids = mediaIds };

        return await PostTweetRequestAsync(url, payload);
    }

    private async Task<TweetResult> PostTweetRequestAsync(string url, Dictionary<string, object> payload)
    {
        var result = await PostJsonV2Async(url, JsonSerializer.Serialize(payload));
        var data = result.GetProperty("data");
        return new TweetResult
        {
            TweetId = data.GetProperty("id").GetString()!,
            Text = data.GetProperty("text").GetString()!
        };
    }

    private async Task<JsonElement> GetV2Async(string url)
    {
        // OAuth 1.0a 规范：查询参数必须参与签名，不能只用去掉参数后的 base URL
        var queryParams = ParseQueryParams(url);
        var authHeader = BuildOAuthHeader("GET", CleanUrl(url), queryParams);
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new TwitterException($"请求失败 [{url}]: {json}", (int)response.StatusCode);

        return JsonDocument.Parse(json).RootElement;
    }

    private async Task<JsonElement> PostJsonV2Async(string url, string jsonBody)
    {
        // POST + JSON body：body 内容不参与 OAuth 签名，但 URL 查询参数需要参与
        var queryParams = ParseQueryParams(url);
        var authHeader = BuildOAuthHeader("POST", CleanUrl(url), queryParams);
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        var response = await _http.SendAsync(request);
        string json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new TwitterException($"请求失败: {json}", (int)response.StatusCode);

        return JsonDocument.Parse(json).RootElement;
    }

    /// <summary>
    /// 生成 OAuth 1.0a 签名头
    /// </summary>
    private string BuildOAuthHeader(string method, string url, Dictionary<string, string> extraParams)
    {
        string nonce = Guid.NewGuid().ToString("N");
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var oauthParams = new Dictionary<string, string>
        {
            ["oauth_consumer_key"] = _apiKey,
            ["oauth_nonce"] = nonce,
            ["oauth_signature_method"] = "HMAC-SHA1",
            ["oauth_timestamp"] = timestamp,
            ["oauth_token"] = _accessToken,
            ["oauth_version"] = "1.0"
        };

        var allParams = new SortedDictionary<string, string>(oauthParams);
        foreach (var kv in extraParams) allParams[kv.Key] = kv.Value;

        string paramString = string.Join("&",
            allParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        string baseString = $"{method}&{Uri.EscapeDataString(url)}&{Uri.EscapeDataString(paramString)}";
        string signingKey = $"{Uri.EscapeDataString(_apiSecret)}&{Uri.EscapeDataString(_accessSecret)}";

        using var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
        string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(baseString)));

        oauthParams["oauth_signature"] = signature;

        return "OAuth " + string.Join(", ",
            oauthParams.OrderBy(kv => kv.Key)
                       .Select(kv => $"{Uri.EscapeDataString(kv.Key)}=\"{Uri.EscapeDataString(kv.Value)}\""));
    }

    private static string CleanUrl(string url)
    {
        int idx = url.IndexOf('?');
        return idx >= 0 ? url[..idx] : url;
    }

    /// <summary>
    /// 解析 URL 中的查询参数，用于 OAuth 1.0a 签名
    /// </summary>
    private static Dictionary<string, string> ParseQueryParams(string url)
    {
        var result = new Dictionary<string, string>();
        int idx = url.IndexOf('?');
        if (idx < 0) return result;

        string query = url[(idx + 1)..];
        foreach (var part in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            int eq = part.IndexOf('=');
            if (eq < 0) continue;
            string key = Uri.UnescapeDataString(part[..eq]);
            string val = Uri.UnescapeDataString(part[(eq + 1)..]);
            result[key] = val;
        }
        return result;
    }

    private static string GetMimeType(string path) =>
        Path.GetExtension(path).ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            _ => "application/octet-stream"
        };

    private static TweetListResult ParseTweetListResult(JsonElement doc)
    {
        var result = new TweetListResult();

        if (doc.TryGetProperty("data", out var dataArr))
        {
            foreach (var item in dataArr.EnumerateArray())
            {
                var tweet = new TweetItem
                {
                    TweetId = item.GetProperty("id").GetString()!,
                    Text = item.GetProperty("text").GetString()!,
                    CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetString()! : ""
                };

                if (item.TryGetProperty("public_metrics", out var pm))
                {
                    tweet.LikeCount = pm.GetProperty("like_count").GetInt64();
                    tweet.RetweetCount = pm.GetProperty("retweet_count").GetInt64();
                    tweet.ReplyCount = pm.GetProperty("reply_count").GetInt64();
                }

                if (item.TryGetProperty("attachments", out var att) &&
                    att.TryGetProperty("media_keys", out var keys))
                {
                    tweet.MediaKeys = keys.EnumerateArray()
                        .Select(k => k.GetString()!)
                        .ToList();
                }

                result.Tweets.Add(tweet);
            }
        }

        // 媒体详情展开
        if (doc.TryGetProperty("includes", out var includes) &&
            includes.TryGetProperty("media", out var mediaArr))
        {
            foreach (var m in mediaArr.EnumerateArray())
            {
                string key = m.GetProperty("media_key").GetString()!;
                string type = m.GetProperty("type").GetString()!;
                string url = type == "photo"
                    ? (m.TryGetProperty("url", out var u) ? u.GetString()! : "")
                    : (m.TryGetProperty("preview_image_url", out var pu) ? pu.GetString()! : "");

                result.MediaMap[key] = new MediaInfo { MediaKey = key, Type = type, Url = url };
            }
        }

        if (doc.TryGetProperty("meta", out var meta))
        {
            result.TotalCount = meta.TryGetProperty("result_count", out var rc) ? rc.GetInt32() : 0;
            result.NextPageToken = meta.TryGetProperty("next_token", out var nt) ? nt.GetString() : null;
        }

        return result;
    }

    public void Dispose() => _http.Dispose();

    #endregion
}

// ══════════════════════════════════════════════════════════
// 数据模型
// ══════════════════════════════════════════════════════════

public record TweetResult
{
    public string TweetId { get; init; } = "";
    public string Text { get; init; } = "";
}

public class TweetListResult
{
    public List<TweetItem> Tweets { get; } = new();
    public Dictionary<string, MediaInfo> MediaMap { get; } = new();
    public int TotalCount { get; set; }
    public string? NextPageToken { get; set; }
}

public class TweetItem
{
    public string TweetId { get; set; } = "";
    public string Text { get; set; } = "";
    public string CreatedAt { get; set; } = "";
    public long LikeCount { get; set; }
    public long RetweetCount { get; set; }
    public long ReplyCount { get; set; }
    public List<string> MediaKeys { get; set; } = new();
}

public class MediaInfo
{
    public string MediaKey { get; set; } = "";
    public string Type { get; set; } = "";  // photo / video / animated_gif
    public string Url { get; set; } = "";
}

public class UserMetrics
{
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public long FollowersCount { get; set; }
    public long FollowingCount { get; set; }
    public long TweetCount { get; set; }
    public long LikedCount { get; set; }
    public long ListedCount { get; set; }
}

public class TweetMetrics
{
    public string TweetId { get; set; } = "";
    public string Text { get; set; } = "";
    public string CreatedAt { get; set; } = "";
    public long LikeCount { get; set; }
    public long RetweetCount { get; set; }
    public long ReplyCount { get; set; }
    public long QuoteCount { get; set; }
    public long ImpressionCount { get; set; }
    public long UrlLinkClicks { get; set; }
    public long UserProfileClicks { get; set; }
}

public class TwitterException : Exception
{
    public int StatusCode { get; }
    public TwitterException(string message, int statusCode) : base(message)
        => StatusCode = statusCode;
}