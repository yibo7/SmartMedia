using System;
using System.Collections.Generic;
using System.Text;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey">Consumer Key</param>
    /// <param name="apiSecret">Secret Key（即 Consumer Secret）</param>
    /// <param name="accessToken"></param>
    /// <param name="accessSecret"></param>
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
    /// 获取用户账号统计（粉丝数、关注数、推文数等）
    /// </summary>
    public async Task<UserMetrics> GetUserMetricsAsync(string userId)
    {
        string url = $"{ApiV2BaseUrl}/users/{userId}?user.fields=public_metrics,created_at,name,username";
        var doc = await GetV2Async(url);
        var data = doc.GetProperty("data");

        var metrics = data.GetProperty("public_metrics");
        return new UserMetrics
        {
            UserId = data.GetProperty("id").GetString()!,
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

        var body = new Dictionary<string, string> { ["media_data"] = base64 };
        var authHeader = BuildOAuthHeader("POST", url, new Dictionary<string, string>());
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(body)
        };
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

        // INIT
        var initBody = new Dictionary<string, string>
        {
            ["command"] = "INIT",
            ["total_bytes"] = fileBytes.Length.ToString(),
            ["media_type"] = mimeType,
            ["media_category"] = "tweet_image"
        };
        var authHeader = BuildOAuthHeader("POST", url, new Dictionary<string, string>());
        var initReq = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(initBody)
        };
        initReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
        var initResp = await _http.SendAsync(initReq);
        var initJson = JsonDocument.Parse(await initResp.Content.ReadAsStringAsync());
        string mediaId = initJson.RootElement.GetProperty("media_id_string").GetString()!;

        // APPEND（每块 ≤ 5MB）
        int chunkSize = 5 * 1024 * 1024;
        int segIndex = 0;
        for (int offset = 0; offset < fileBytes.Length; offset += chunkSize)
        {
            byte[] chunk = fileBytes.Skip(offset).Take(chunkSize).ToArray();
            var appendBody = new Dictionary<string, string>
            {
                ["command"] = "APPEND",
                ["media_id"] = mediaId,
                ["segment_index"] = segIndex.ToString(),
                ["media_data"] = Convert.ToBase64String(chunk)
            };
            authHeader = BuildOAuthHeader("POST", url, new Dictionary<string, string>());
            var appendReq = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(appendBody)
            };
            appendReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
            await _http.SendAsync(appendReq);
            segIndex++;
        }

        // FINALIZE
        var finalBody = new Dictionary<string, string>
        {
            ["command"] = "FINALIZE",
            ["media_id"] = mediaId
        };
        authHeader = BuildOAuthHeader("POST", url, new Dictionary<string, string>());
        var finalReq = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(finalBody)
        };
        finalReq.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
        var finalResp = await _http.SendAsync(finalReq);
        if (!finalResp.IsSuccessStatusCode)
            throw new TwitterException($"分块上传 FINALIZE 失败", (int)finalResp.StatusCode);

        return mediaId;
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
        var authHeader = BuildOAuthHeader("GET", CleanUrl(url), new Dictionary<string, string>());
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
        var authHeader = BuildOAuthHeader("POST", CleanUrl(url), new Dictionary<string, string>());
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
