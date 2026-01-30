using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartMedia.AtqCore;
 
#region 数据模型

/// <summary>
/// Facebook API 响应模型
/// </summary>
public class FacebookApiResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("post_id")]
    public string? PostId { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("error")]
    public FacebookError? Error { get; set; }

    [JsonPropertyName("video_id")]
    public string? VideoId { get; set; }

    [JsonPropertyName("upload_url")]
    public string? UploadUrl { get; set; }

    [JsonPropertyName("data")]
    public List<FacebookPost>? Data { get; set; }

    [JsonPropertyName("paging")]
    public PagingInfo? Paging { get; set; }
}

/// <summary>
/// Facebook 错误模型
/// </summary>
public class FacebookError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("error_subcode")]
    public int ErrorSubcode { get; set; }
}

/// <summary>
/// Facebook 帖子模型
/// </summary>
public class FacebookPost
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("story")]
    public string? Story { get; set; }

    [JsonPropertyName("created_time")]
    public DateTime? CreatedTime { get; set; }

    [JsonPropertyName("updated_time")]
    public DateTime? UpdatedTime { get; set; }

    [JsonPropertyName("permalink_url")]
    public string? PermalinkUrl { get; set; }

    [JsonPropertyName("full_picture")]
    public string? FullPicture { get; set; }

    [JsonPropertyName("attachments")]
    public AttachmentsData? Attachments { get; set; }

    [JsonPropertyName("likes")]
    public LikesSummary? Likes { get; set; }

    [JsonPropertyName("comments")]
    public CommentsSummary? Comments { get; set; }

    [JsonPropertyName("shares")]
    public SharesSummary? Shares { get; set; }
}

/// <summary>
/// 附件数据
/// </summary>
public class AttachmentsData
{
    [JsonPropertyName("data")]
    public List<Attachment>? Data { get; set; }
}

/// <summary>
/// 附件
/// </summary>
public class Attachment
{
    [JsonPropertyName("type")]
    public string? Type { get; set; } // photo, video, album, etc.

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("media")]
    public MediaData? Media { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 媒体数据
/// </summary>
public class MediaData
{
    [JsonPropertyName("image")]
    public MediaImage? Image { get; set; }
}

/// <summary>
/// 媒体图片
/// </summary>
public class MediaImage
{
    [JsonPropertyName("src")]
    public string? Src { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}

/// <summary>
/// 点赞摘要
/// </summary>
public class LikesSummary
{
    [JsonPropertyName("data")]
    public List<object>? Data { get; set; }

    [JsonPropertyName("summary")]
    public Summary? Summary { get; set; }
}

/// <summary>
/// 评论摘要
/// </summary>
public class CommentsSummary
{
    [JsonPropertyName("data")]
    public List<object>? Data { get; set; }

    [JsonPropertyName("summary")]
    public Summary? Summary { get; set; }
}

/// <summary>
/// 分享摘要
/// </summary>
public class SharesSummary
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>
/// 摘要信息
/// </summary>
public class Summary
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("can_like")]
    public bool CanLike { get; set; }

    [JsonPropertyName("has_liked")]
    public bool HasLiked { get; set; }
}

/// <summary>
/// 分页信息
/// </summary>
public class PagingInfo
{
    [JsonPropertyName("cursors")]
    public Cursors? Cursors { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public string? Previous { get; set; }
}

/// <summary>
/// 分页游标
/// </summary>
public class Cursors
{
    [JsonPropertyName("before")]
    public string? Before { get; set; }

    [JsonPropertyName("after")]
    public string? After { get; set; }
}

/// <summary>
/// 图片信息
/// </summary>
public class PhotoInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("created_time")]
    public DateTime? CreatedTime { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("picture")]
    public string? Picture { get; set; }

    [JsonPropertyName("images")]
    public List<PhotoImage>? Images { get; set; }
}

/// <summary>
/// 图片尺寸
/// </summary>
public class PhotoImage
{
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }
}

/// <summary>
/// 视频信息
/// </summary>
public class VideoInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("created_time")]
    public DateTime? CreatedTime { get; set; }

    [JsonPropertyName("permalink_url")]
    public string? PermalinkUrl { get; set; }

    [JsonPropertyName("length")]
    public double? Length { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("thumbnails")]
    public ThumbnailsData? Thumbnails { get; set; }
}

/// <summary>
/// 缩略图数据
/// </summary>
public class ThumbnailsData
{
    [JsonPropertyName("data")]
    public List<Thumbnail>? Data { get; set; }
}

/// <summary>
/// 缩略图
/// </summary>
public class Thumbnail
{
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("is_preferred")]
    public bool IsPreferred { get; set; }
}

/// <summary>
/// Reels 信息
/// </summary>
public class ReelsInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("created_time")]
    public DateTime? CreatedTime { get; set; }

    [JsonPropertyName("permalink_url")]
    public string? PermalinkUrl { get; set; }

    [JsonPropertyName("length")]
    public double? Length { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }
}

/// <summary>
/// 内容查询参数
/// </summary>
public class ContentQueryOptions
{
    public int Limit { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Since { get; set; } // UNIX 时间戳或日期字符串
    public string? Until { get; set; } // UNIX 时间戳或日期字符串
    public string? Order { get; set; } // "chronological" 或 "reverse_chronological"
    public bool IncludeSummary { get; set; } = true;
    public Dictionary<string, string>? AdditionalParams { get; set; }
}

/// <summary>
/// 发布参数配置
/// </summary>
public class PublishOptions
{
    public string? Link { get; set; }
    public string? ThumbnailPath { get; set; }
    public bool? Published { get; set; } = true;
    public bool? ScheduledPublish { get; set; } = false;
    public long? ScheduledPublishTime { get; set; }
    public Dictionary<string, string>? AdditionalParams { get; set; }
}

#endregion

#region 异常类

/// <summary>
/// Facebook API 异常
/// </summary>
public class FacebookApiException : Exception
{
    public int? ErrorCode { get; }

    public FacebookApiException(string message) : base(message) { }

    public FacebookApiException(string message, Exception innerException)
        : base(message, innerException) { }

    public FacebookApiException(string message, int errorCode, Exception innerException = null)
        : base($"{message} (Error Code: {errorCode})", innerException)
    {
        ErrorCode = errorCode;
    }
}

#endregion

#region 主类 - FacebookPagePublisher

/// <summary>
/// Facebook Page 内容发布器
/// </summary>
public class FacebookPagePublisher : IDisposable
{
    private readonly string _pageId;
    private readonly string _pageAccessToken;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly int _chunkSize = 5 * 1024 * 1024; // 5MB 分块大小

    private const string GraphApiBase = "https://graph.facebook.com/v19.0";
    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".mp4", "video/mp4" },
        { ".mov", "video/quicktime" },
        { ".avi", "video/x-msvideo" }
    };

    public FacebookPagePublisher(string pageId, string pageAccessToken, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(pageId))
            throw new ArgumentException("Page ID cannot be empty", nameof(pageId));

        if (string.IsNullOrWhiteSpace(pageAccessToken))
            throw new ArgumentException("Page Access Token cannot be empty", nameof(pageAccessToken));

        _pageId = pageId;
        _pageAccessToken = pageAccessToken;
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.Timeout = TimeSpan.FromMinutes(30); // 视频上传需要更长时间

        _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    #region 1. 发布纯文字 / 链接（Feed）

    public async Task<FacebookApiResponse> PublishFeedAsync(string message, PublishOptions? options = null)
    {
        ValidateMessage(message);

        var url = $"{GraphApiBase}/{_pageId}/feed";
        var payload = new Dictionary<string, object>
        {
            ["message"] = message,
            ["access_token"] = _pageAccessToken
        };

        AddOptionalParameters(payload, options);

        try
        {
            return await PostJsonAsync<FacebookApiResponse>(url, payload);
        }
        catch (Exception ex)
        {
            throw new FacebookApiException($"Failed to publish feed: {ex.Message}", ex);
        }
    }

    #endregion

    #region 2. 发布图片（Feed）

    public async Task<FacebookApiResponse> PublishPhotoAsync(string imagePath, string caption, PublishOptions? options = null)
    {
        ValidateFile(imagePath, new[] { ".jpg", ".jpeg", ".png", ".gif" });
        ValidateCaption(caption);

        var url = $"{GraphApiBase}/{_pageId}/photos";

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(caption), "caption");
        form.Add(new StringContent(_pageAccessToken), "access_token");

        AddOptionalFormParameters(form, options);

        var fileContent = await CreateFileContent(imagePath, "source");
        form.Add(fileContent);

        try
        {
            var response = await _httpClient.PostAsync(url, form);
            return await HandleResponseAsync<FacebookApiResponse>(response);
        }
        catch (Exception ex)
        {
            throw new FacebookApiException($"Failed to publish photo: {ex.Message}", ex);
        }
    }

    #endregion

    #region 3. 发布普通视频（支持分块上传）

    public async Task<FacebookApiResponse> PublishVideoAsync(string videoPath, string description, PublishOptions? options = null)
    {
        ValidateFile(videoPath, new[] { ".mp4", ".mov", ".avi" });
        ValidateDescription(description);

        var fileSize = new FileInfo(videoPath).Length;

        // 小文件使用简单上传，大文件使用分块上传
        if (fileSize <= _chunkSize)
        {
            return await SimpleVideoUploadAsync(videoPath, description, options);
        }
        else
        {
            return await ChunkedVideoUploadAsync(videoPath, description, options, isReel: false);
        }
    }

    private async Task<FacebookApiResponse> SimpleVideoUploadAsync(string videoPath, string description, PublishOptions? options)
    {
        var url = $"{GraphApiBase}/{_pageId}/videos";

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(description), "description");
        form.Add(new StringContent(_pageAccessToken), "access_token");

        AddOptionalFormParameters(form, options);

        var fileContent = await CreateFileContent(videoPath, "source");
        form.Add(fileContent);

        var response = await _httpClient.PostAsync(url, form);
        return await HandleResponseAsync<FacebookApiResponse>(response);
    }

    #endregion

    #region 4. 发布 Reels（完整的分块上传实现）

    public async Task<FacebookApiResponse> PublishReelsAsync(string videoPath, string caption, PublishOptions? options = null, Progress<SocialMedia.UploadProgress> progress = null)
    {
        ValidateReelsFile(videoPath);
        ValidateCaption(caption);

        return await ChunkedVideoUploadAsync(videoPath, caption, options, isReel: true);
    }

    private async Task<FacebookApiResponse> ChunkedVideoUploadAsync(string videoPath, string caption, PublishOptions? options, bool isReel)
    {
        var endpoint = isReel ? "video_reels" : "videos";
        var url = $"{GraphApiBase}/{_pageId}/{endpoint}";
        var fileInfo = new FileInfo(videoPath);

        try
        {
            // 阶段1：开始上传会话
            var startPayload = new Dictionary<string, object>
            {
                ["upload_phase"] = "start",
                ["file_size"] = fileInfo.Length,
                ["access_token"] = _pageAccessToken
            };

            if (isReel)
            {
                startPayload["description"] = caption;
            }
            else
            {
                startPayload["description"] = caption;
            }

            var startResponse = await PostJsonAsync<FacebookApiResponse>(url, startPayload);

            if (!string.IsNullOrEmpty(startResponse.Error?.Message))
                throw new FacebookApiException($"Start upload failed: {startResponse.Error.Message}");

            var videoId = startResponse.VideoId;
            var uploadUrl = startResponse.UploadUrl;

            if (string.IsNullOrEmpty(videoId) || string.IsNullOrEmpty(uploadUrl))
                throw new FacebookApiException("Invalid response from start upload phase");

            // 阶段2：分块上传
            await UploadChunksAsync(videoPath, uploadUrl);

            // 阶段3：完成上传
            var finishPayload = new Dictionary<string, object>
            {
                ["upload_phase"] = "finish",
                ["video_id"] = videoId,
                ["access_token"] = _pageAccessToken
            };

            if (isReel)
            {
                finishPayload["description"] = caption;
            }
            else
            {
                finishPayload["description"] = caption;
            }

            AddOptionalParameters(finishPayload, options);

            return await PostJsonAsync<FacebookApiResponse>(url, finishPayload);
        }
        catch (Exception ex)
        {
            throw new FacebookApiException($"Failed to upload video: {ex.Message}", ex);
        }
    }

    private async Task UploadChunksAsync(string filePath, string uploadUrl)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        var buffer = new byte[_chunkSize];
        int bytesRead;
        long startOffset = 0;

        while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
        {
            var chunk = bytesRead < _chunkSize ? buffer[..bytesRead] : buffer;

            using var chunkContent = new ByteArrayContent(chunk);
            chunkContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // Facebook分块上传需要指定Content-Range头部
            var endOffset = startOffset + bytesRead - 1;
            var fileSize = fileStream.Length;
            chunkContent.Headers.Add("Content-Range", $"bytes {startOffset}-{endOffset}/{fileSize}");

            var response = await _httpClient.PostAsync(uploadUrl, chunkContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new FacebookApiException($"Chunk upload failed: {response.StatusCode} - {errorContent}");
            }

            startOffset += bytesRead;
        }
    }

    #endregion

    #region 5. 获取内容列表接口（新增）

    /// <summary>
    /// 获取动态流内容列表
    /// </summary>
    public async Task<FacebookApiResponse> GetFeedAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();

        var defaultFields = "id,message,story,created_time,updated_time,permalink_url," +
                            "full_picture,attachments{type,url,media,title,description}," +
                            "likes.summary(true),comments.summary(true),shares";

        var fields = options.Fields ?? defaultFields;

        var url = $"{GraphApiBase}/{_pageId}/feed";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    /// <summary>
    /// 获取图片列表
    /// </summary>
    public async Task<FacebookApiResponse> GetPhotosAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();

        var defaultFields = "id,name,created_time,link,picture,images";
        var fields = options.Fields ?? defaultFields;

        var url = $"{GraphApiBase}/{_pageId}/photos";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    /// <summary>
    /// 获取视频列表
    /// </summary>
    public async Task<FacebookApiResponse> GetVideosAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();

        var defaultFields = "id,title,description,created_time,permalink_url,length,source,thumbnails";
        var fields = options.Fields ?? defaultFields;

        var url = $"{GraphApiBase}/{_pageId}/videos";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    /// <summary>
    /// 获取 Reels 列表
    /// </summary>
    public async Task<FacebookApiResponse> GetReelsAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();

        var defaultFields = "id,caption,created_time,permalink_url,length,source,media_type";
        var fields = options.Fields ?? defaultFields;

        var url = $"{GraphApiBase}/{_pageId}/video_reels";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    /// <summary>
    /// 获取所有类型的内容（混合）
    /// </summary>
    public async Task<FacebookApiResponse> GetAllContentAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();

        var defaultFields = "id,created_time,updated_time,message,story,type,permalink_url";
        var fields = options.Fields ?? defaultFields;

        var url = $"{GraphApiBase}/{_pageId}/published_posts";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    /// <summary>
    /// 根据ID获取特定内容详情
    /// </summary>
    public async Task<FacebookApiResponse> GetContentByIdAsync(string contentId, string? fields = null)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));

        var defaultFields = "id,message,story,created_time,updated_time,permalink_url," +
                            "full_picture,attachments,likes.summary(true),comments.summary(true),shares";

        fields ??= defaultFields;

        var url = $"{GraphApiBase}/{contentId}";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = fields,
            ["access_token"] = _pageAccessToken
        };

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    /// <summary>
    /// 获取内容统计信息
    /// </summary>
    public async Task<FacebookApiResponse> GetInsightsAsync(string contentId, List<string> metrics)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));

        if (metrics == null || metrics.Count == 0)
            metrics = new List<string> { "post_impressions", "post_engaged_users", "post_clicks" };

        var url = $"{GraphApiBase}/{contentId}/insights";
        var queryParams = new Dictionary<string, string>
        {
            ["metric"] = string.Join(",", metrics),
            ["access_token"] = _pageAccessToken
        };

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    #endregion

    #region 6. 分页加载辅助方法

    /// <summary>
    /// 获取下一页内容
    /// </summary>
    public async Task<FacebookApiResponse> GetNextPageAsync(FacebookApiResponse currentResponse)
    {
        if (string.IsNullOrEmpty(currentResponse.Paging?.Next))
            throw new ArgumentException("No next page available");

        return await GetJsonAsync<FacebookApiResponse>(currentResponse.Paging.Next);
    }

    /// <summary>
    /// 获取上一页内容
    /// </summary>
    public async Task<FacebookApiResponse> GetPreviousPageAsync(FacebookApiResponse currentResponse)
    {
        if (string.IsNullOrEmpty(currentResponse.Paging?.Previous))
            throw new ArgumentException("No previous page available");

        return await GetJsonAsync<FacebookApiResponse>(currentResponse.Paging.Previous);
    }

    /// <summary>
    /// 获取所有内容（自动分页）
    /// </summary>
    public async Task<List<FacebookPost>> GetAllPostsAsync(int maxPages = 10)
    {
        var allPosts = new List<FacebookPost>();
        FacebookApiResponse? currentResponse = null;
        int pageCount = 0;

        do
        {
            if (currentResponse == null)
            {
                currentResponse = await GetFeedAsync(new ContentQueryOptions { Limit = 25 });
            }
            else if (!string.IsNullOrEmpty(currentResponse.Paging?.Next))
            {
                currentResponse = await GetNextPageAsync(currentResponse);
            }
            else
            {
                break;
            }

            if (currentResponse.Data != null)
            {
                allPosts.AddRange(currentResponse.Data);
            }

            pageCount++;

        } while (!string.IsNullOrEmpty(currentResponse.Paging?.Next) && pageCount < maxPages);

        return allPosts;
    }

    #endregion

    #region 7. 内容管理方法

    /// <summary>
    /// 删除内容
    /// </summary>
    public async Task<FacebookApiResponse> DeleteContentAsync(string contentId)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));

        var url = $"{GraphApiBase}/{contentId}";
        var queryParams = new Dictionary<string, string>
        {
            ["access_token"] = _pageAccessToken
        };

        try
        {
            var response = await _httpClient.DeleteAsync(BuildUrlWithParams(url, queryParams));
            return await HandleResponseAsync<FacebookApiResponse>(response);
        }
        catch (Exception ex)
        {
            throw new FacebookApiException($"Failed to delete content: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 更新内容
    /// </summary>
    public async Task<FacebookApiResponse> UpdateContentAsync(string contentId, string newMessage)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));

        ValidateMessage(newMessage);

        var url = $"{GraphApiBase}/{contentId}";
        var payload = new Dictionary<string, object>
        {
            ["message"] = newMessage,
            ["access_token"] = _pageAccessToken
        };

        try
        {
            return await PostJsonAsync<FacebookApiResponse>(url, payload);
        }
        catch (Exception ex)
        {
            throw new FacebookApiException($"Failed to update content: {ex.Message}", ex);
        }
    }

    #endregion

    #region 8. 辅助方法

    private Dictionary<string, string> BuildQueryParams(ContentQueryOptions options, string fields)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["limit"] = options.Limit.ToString(),
            ["fields"] = fields,
            ["access_token"] = _pageAccessToken
        };

        if (!string.IsNullOrEmpty(options.Since))
            queryParams["since"] = options.Since;

        if (!string.IsNullOrEmpty(options.Until))
            queryParams["until"] = options.Until;

        if (!string.IsNullOrEmpty(options.Order))
            queryParams["order"] = options.Order;

        if (options.AdditionalParams != null)
        {
            foreach (var param in options.AdditionalParams)
            {
                queryParams[param.Key] = param.Value;
            }
        }

        return queryParams;
    }

    private void ValidateMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        if (message.Length > 63206) // Facebook 字符限制
            throw new ArgumentException($"Message too long ({message.Length} > 63206 chars)", nameof(message));
    }

    private void ValidateCaption(string caption)
    {
        if (string.IsNullOrWhiteSpace(caption))
            throw new ArgumentException("Caption cannot be empty", nameof(caption));

        if (caption.Length > 2200) // Instagram/Facebook 标题限制
            throw new ArgumentException($"Caption too long ({caption.Length} > 2200 chars)", nameof(caption));
    }

    private void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (description.Length > 5000) // Facebook 视频描述限制
            throw new ArgumentException($"Description too long ({description.Length} > 5000 chars)", nameof(description));
    }

    private void ValidateFile(string filePath, string[] allowedExtensions)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException($"Unsupported file type: {extension}. Allowed: {string.Join(", ", allowedExtensions)}");

        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
            throw new ArgumentException($"File is empty: {filePath}");
    }

    private void ValidateReelsFile(string videoPath)
    {
        ValidateFile(videoPath, new[] { ".mp4" });

        var fileInfo = new FileInfo(videoPath);
        const long maxReelsSize = 1024 * 1024 * 1024; // 1GB
        if (fileInfo.Length > maxReelsSize)
            throw new ArgumentException($"Reels file too large ({fileInfo.Length} > {maxReelsSize} bytes)");
    }

    private async Task<ByteArrayContent> CreateFileContent(string filePath, string formName)
    {
        var bytes = await File.ReadAllBytesAsync(filePath);
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        var fileContent = new ByteArrayContent(bytes);

        if (MimeTypes.TryGetValue(extension, out var mimeType))
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
        }
        else
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        }

        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = formName,
            FileName = Path.GetFileName(filePath)
        };

        return fileContent;
    }

    private void AddOptionalParameters(Dictionary<string, object> payload, PublishOptions? options)
    {
        if (options == null) return;

        if (!string.IsNullOrEmpty(options.Link))
            payload["link"] = options.Link;

        if (options.Published.HasValue)
            payload["published"] = options.Published.Value;

        if (options.ScheduledPublish.HasValue && options.ScheduledPublish.Value &&
            options.ScheduledPublishTime.HasValue)
        {
            payload["scheduled_publish_time"] = options.ScheduledPublishTime.Value;
        }

        if (options.AdditionalParams != null)
        {
            foreach (var param in options.AdditionalParams)
            {
                payload[param.Key] = param.Value;
            }
        }
    }

    private void AddOptionalFormParameters(MultipartFormDataContent form, PublishOptions? options)
    {
        if (options == null) return;

        if (!string.IsNullOrEmpty(options.Link))
            form.Add(new StringContent(options.Link), "link");

        if (options.Published.HasValue)
            form.Add(new StringContent(options.Published.Value.ToString().ToLower()), "published");

        if (options.AdditionalParams != null)
        {
            foreach (var param in options.AdditionalParams)
            {
                form.Add(new StringContent(param.Value), param.Key);
            }
        }
    }

    #endregion

    #region HTTP 请求处理

    private async Task<T> GetJsonAsync<T>(string url, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var fullUrl = BuildUrlWithParams(url, queryParams);
            var response = await _httpClient.GetAsync(fullUrl);
            return await HandleResponseAsync<T>(response);
        }
        catch (Exception ex)
        {
            throw new FacebookApiException($"GET request failed: {ex.Message}", ex);
        }
    }

    private async Task<T> PostJsonAsync<T>(string url, Dictionary<string, object> payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            return await HandleResponseAsync<T>(response);
        }
        catch (Exception ex)
        {
            throw new FacebookApiException($"POST request failed: {ex.Message}", ex);
        }
    }

    private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<FacebookApiResponse>(content, _jsonOptions);
                if (errorResponse?.Error != null)
                {
                    throw new FacebookApiException(
                        $"Facebook API Error: {errorResponse.Error.Message}",
                        errorResponse.Error.Code);
                }
            }
            catch (JsonException)
            {
                // 如果不是有效的JSON错误响应，抛出原始错误
            }

            throw new FacebookApiException($"HTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            if (result == null)
                throw new FacebookApiException("Failed to deserialize response");

            return result;
        }
        catch (JsonException ex)
        {
            throw new FacebookApiException($"Failed to parse response: {ex.Message}", ex);
        }
    }

    private string BuildUrlWithParams(string url, Dictionary<string, string>? queryParams)
    {
        if (queryParams == null || queryParams.Count == 0)
            return url;

        var queryString = string.Join("&", queryParams.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        return $"{url}?{queryString}";
    }

    #endregion

    #region 9. 实用工具方法

    /// <summary>
    /// 获取Page基本信息
    /// </summary>
    public async Task<FacebookApiResponse> GetPageInfoAsync()
    {
        var url = $"{GraphApiBase}/{_pageId}";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = "id,name,about,category,followers_count,fan_count,link,picture",
            ["access_token"] = _pageAccessToken
        };

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    /// <summary>
    /// 测试API连接
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var url = $"{GraphApiBase}/me/accounts";
            var queryParams = new Dictionary<string, string>
            {
                ["access_token"] = _pageAccessToken
            };

            await GetJsonAsync<FacebookApiResponse>(url, queryParams);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 批量发布内容
    /// </summary>
    public async Task<List<FacebookApiResponse>> BatchPublishAsync(List<BatchPublishItem> items)
    {
        var results = new List<FacebookApiResponse>();

        foreach (var item in items)
        {
            try
            {
                FacebookApiResponse response;

                switch (item.Type)
                {
                    case PublishType.Feed:
                        response = await PublishFeedAsync(item.Message, item.Options);
                        break;
                    case PublishType.Photo:
                        response = await PublishPhotoAsync(item.FilePath!, item.Caption!, item.Options);
                        break;
                    case PublishType.Video:
                        response = await PublishVideoAsync(item.FilePath!, item.Description!, item.Options);
                        break;
                    case PublishType.Reels:
                        response = await PublishReelsAsync(item.FilePath!, item.Caption!, item.Options);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported publish type: {item.Type}");
                }

                results.Add(response);
            }
            catch (Exception ex)
            {
                results.Add(new FacebookApiResponse
                {
                    Success = false,
                    Error = new FacebookError { Message = ex.Message }
                });
            }
        }

        return results;
    }

    #endregion

    #region IDisposable 实现

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}

/// <summary>
/// 发布类型
/// </summary>
public enum PublishType
{
    Feed,
    Photo,
    Video,
    Reels
}

/// <summary>
/// 批量发布项
/// </summary>
public class BatchPublishItem
{
    public PublishType Type { get; set; }
    public string? Message { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public PublishOptions? Options { get; set; }
}

#endregion
