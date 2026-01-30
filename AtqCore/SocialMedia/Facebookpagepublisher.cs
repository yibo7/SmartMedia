using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartMedia.AtqCore.SocialMedia;

#region Facebook特定数据模型

/// <summary>
/// Facebook API 响应模型
/// </summary>
public class FacebookApiResponse : SocialMediaApiResponse
{
    [JsonPropertyName("post_id")]
    public string? PostId { get; set; }

    [JsonPropertyName("video_id")]
    public string? VideoId { get; set; }

    [JsonPropertyName("upload_url")]
    public string? UploadUrl { get; set; }

    [JsonPropertyName("data")]
    public new List<FacebookPost>? Data { get; set; }

    [JsonPropertyName("paging")]
    public PagingInfo? Paging { get; set; }
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

public class AttachmentsData
{
    [JsonPropertyName("data")]
    public List<Attachment>? Data { get; set; }
}

public class Attachment
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("media")]
    public MediaData? Media { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class MediaData
{
    [JsonPropertyName("image")]
    public MediaImage? Image { get; set; }
}

public class MediaImage
{
    [JsonPropertyName("src")]
    public string? Src { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}

public class LikesSummary
{
    [JsonPropertyName("data")]
    public List<object>? Data { get; set; }

    [JsonPropertyName("summary")]
    public Summary? Summary { get; set; }
}

public class CommentsSummary
{
    [JsonPropertyName("data")]
    public List<object>? Data { get; set; }

    [JsonPropertyName("summary")]
    public Summary? Summary { get; set; }
}

public class SharesSummary
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class Summary
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("can_like")]
    public bool CanLike { get; set; }

    [JsonPropertyName("has_liked")]
    public bool HasLiked { get; set; }
}

public class PagingInfo
{
    [JsonPropertyName("cursors")]
    public Cursors? Cursors { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public string? Previous { get; set; }
}

public class Cursors
{
    [JsonPropertyName("before")]
    public string? Before { get; set; }

    [JsonPropertyName("after")]
    public string? After { get; set; }
}

#endregion

#region Facebook配置类

/// <summary>
/// 内容查询参数
/// </summary>
public class ContentQueryOptions
{
    public int Limit { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Since { get; set; }
    public string? Until { get; set; }
    public string? Order { get; set; }
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

/// <summary>
/// Facebook Page 内容发布器
/// </summary>
public class FacebookPagePublisher : SocialMediaPublisherBase
{
    private readonly string _pageId;
    private readonly string _pageAccessToken;

    private const string GraphApiVersion = "v19.0";
    protected override string ApiBaseUrl => $"https://graph.facebook.com/{GraphApiVersion}";
    public override string PlatformName => "Facebook";

    // Facebook特定限制
    private const int MaxMessageLength = 63206;
    private const int MaxCaptionLength = 2200;
    private const int MaxDescriptionLength = 5000;
    private const long MaxReelsSize = 1024L * 1024 * 1024; // 1GB

    private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private static readonly string[] VideoExtensions = { ".mp4", ".mov", ".avi" };
    private static readonly string[] ReelsExtensions = { ".mp4" };

    public FacebookPagePublisher(string pageId, string pageAccessToken, HttpClient? httpClient = null)
        : base(httpClient)
    {
        if (string.IsNullOrWhiteSpace(pageId))
            throw new ArgumentException("Page ID cannot be empty", nameof(pageId));

        if (string.IsNullOrWhiteSpace(pageAccessToken))
            throw new ArgumentException("Page Access Token cannot be empty", nameof(pageAccessToken));

        _pageId = pageId;
        _pageAccessToken = pageAccessToken;
    }

    #region 实现抽象方法

    public override async Task<bool> TestConnectionAsync()
    {
        try
        {
            var url = $"{ApiBaseUrl}/me/accounts";
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

    public override async Task<object> GetAccountInfoAsync()
    {
        return await GetPageInfoAsync();
    }

    #endregion

    #region 1. 发布纯文字/链接

    public async Task<FacebookApiResponse> PublishFeedAsync(string message, PublishOptions? options = null)
    {
        ValidateText(message, MaxMessageLength, "Message");

        var url = $"{ApiBaseUrl}/{_pageId}/feed";
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
            throw new SocialMediaApiException($"Failed to publish feed: {ex.Message}", ex, PlatformName);
        }
    }

    #endregion

    #region 2. 发布图片

    public async Task<FacebookApiResponse> PublishPhotoAsync(
        string imagePath, 
        string caption, 
        PublishOptions? options = null)
    {
        ValidateFile(imagePath, ImageExtensions, "Image");
        ValidateText(caption, MaxCaptionLength, "Caption");

        var url = $"{ApiBaseUrl}/{_pageId}/photos";

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(caption), "caption");
        form.Add(new StringContent(_pageAccessToken), "access_token");

        AddOptionalFormParameters(form, options);

        var fileContent = await CreateFileContentAsync(imagePath, "source");
        form.Add(fileContent);

        try
        {
            return await PostFormAsync<FacebookApiResponse>(url, form);
        }
        catch (Exception ex)
        {
            throw new SocialMediaApiException($"Failed to publish photo: {ex.Message}", ex, PlatformName);
        }
    }

    #endregion

    #region 3. 发布视频

    public async Task<FacebookApiResponse> PublishVideoAsync(
        string videoPath, 
        string description, 
        PublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        ValidateFile(videoPath, VideoExtensions, "Video");
        ValidateText(description, MaxDescriptionLength, "Description");

        var fileSize = new FileInfo(videoPath).Length;

        if (fileSize <= _chunkSize)
        {
            return await SimpleVideoUploadAsync(videoPath, description, options);
        }
        else
        {
            return await ChunkedVideoUploadAsync(videoPath, description, options, false, progress);
        }
    }

    private async Task<FacebookApiResponse> SimpleVideoUploadAsync(
        string videoPath, 
        string description, 
        PublishOptions? options)
    {
        var url = $"{ApiBaseUrl}/{_pageId}/videos";

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(description), "description");
        form.Add(new StringContent(_pageAccessToken), "access_token");

        AddOptionalFormParameters(form, options);

        var fileContent = await CreateFileContentAsync(videoPath, "source");
        form.Add(fileContent);

        return await PostFormAsync<FacebookApiResponse>(url, form);
    }

    #endregion

    #region 4. 发布Reels

    public async Task<FacebookApiResponse> PublishReelsAsync(
        string videoPath, 
        string caption, 
        PublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        ValidateFile(videoPath, ReelsExtensions, "Reels video");
        ValidateFileSize(videoPath, MaxReelsSize, "Reels");
        ValidateText(caption, MaxCaptionLength, "Caption");

        return await ChunkedVideoUploadAsync(videoPath, caption, options, true, progress);
    }

    private async Task<FacebookApiResponse> ChunkedVideoUploadAsync(
        string videoPath, 
        string caption, 
        PublishOptions? options, 
        bool isReel,
        IProgress<UploadProgress>? progress = null)
    {
        var endpoint = isReel ? "video_reels" : "videos";
        var url = $"{ApiBaseUrl}/{_pageId}/{endpoint}";
        var fileInfo = new FileInfo(videoPath);

        try
        {
            // 阶段1：开始上传
            progress?.Report(new UploadProgress { CurrentPhase = "Initializing" });

            var startPayload = new Dictionary<string, object>
            {
                ["upload_phase"] = "start",
                ["file_size"] = fileInfo.Length,
                ["description"] = caption,
                ["access_token"] = _pageAccessToken
            };

            var startResponse = await PostJsonAsync<FacebookApiResponse>(url, startPayload);

            if (!string.IsNullOrEmpty(startResponse.Error?.Message))
                throw new SocialMediaApiException(
                    $"Start upload failed: {startResponse.Error.Message}", 
                    PlatformName);

            var videoId = startResponse.VideoId;
            var uploadUrl = startResponse.UploadUrl;

            if (string.IsNullOrEmpty(videoId) || string.IsNullOrEmpty(uploadUrl))
                throw new SocialMediaApiException("Invalid response from start upload phase", PlatformName);

            // 阶段2：分块上传
            await UploadFileInChunksAsync(videoPath, uploadUrl, progress);

            // 阶段3：完成上传
            progress?.Report(new UploadProgress { CurrentPhase = "Finalizing" });

            var finishPayload = new Dictionary<string, object>
            {
                ["upload_phase"] = "finish",
                ["video_id"] = videoId,
                ["description"] = caption,
                ["access_token"] = _pageAccessToken
            };

            AddOptionalParameters(finishPayload, options);

            return await PostJsonAsync<FacebookApiResponse>(url, finishPayload);
        }
        catch (Exception ex) when (ex is not SocialMediaApiException)
        {
            throw new SocialMediaApiException($"Failed to upload video: {ex.Message}", ex, PlatformName);
        }
    }

    #endregion

    #region 5. 获取内容列表

    public async Task<FacebookApiResponse> GetFeedAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();

        var defaultFields = "id,message,story,created_time,updated_time,permalink_url," +
                            "full_picture,attachments{type,url,media,title,description}," +
                            "likes.summary(true),comments.summary(true),shares";

        var fields = options.Fields ?? defaultFields;
        var url = $"{ApiBaseUrl}/{_pageId}/feed";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    public async Task<FacebookApiResponse> GetPhotosAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();
        var defaultFields = "id,name,created_time,link,picture,images";
        var fields = options.Fields ?? defaultFields;

        var url = $"{ApiBaseUrl}/{_pageId}/photos";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    public async Task<FacebookApiResponse> GetVideosAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();
        var defaultFields = "id,title,description,created_time,permalink_url,length,source,thumbnails";
        var fields = options.Fields ?? defaultFields;

        var url = $"{ApiBaseUrl}/{_pageId}/videos";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    public async Task<FacebookApiResponse> GetReelsAsync(ContentQueryOptions? options = null)
    {
        options ??= new ContentQueryOptions();
        var defaultFields = "id,caption,created_time,permalink_url,length,source,media_type";
        var fields = options.Fields ?? defaultFields;

        var url = $"{ApiBaseUrl}/{_pageId}/video_reels";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    public async Task<FacebookApiResponse> GetContentByIdAsync(string contentId, string? fields = null)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));

        var defaultFields = "id,message,story,created_time,updated_time,permalink_url," +
                            "full_picture,attachments,likes.summary(true),comments.summary(true),shares";

        fields ??= defaultFields;

        var url = $"{ApiBaseUrl}/{contentId}";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = fields,
            ["access_token"] = _pageAccessToken
        };

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

    #endregion

    #region 6. 分页处理

    public async Task<FacebookApiResponse> GetNextPageAsync(FacebookApiResponse currentResponse)
    {
        if (string.IsNullOrEmpty(currentResponse.Paging?.Next))
            throw new ArgumentException("No next page available");

        return await GetJsonAsync<FacebookApiResponse>(currentResponse.Paging.Next);
    }

    public async Task<FacebookApiResponse> GetPreviousPageAsync(FacebookApiResponse currentResponse)
    {
        if (string.IsNullOrEmpty(currentResponse.Paging?.Previous))
            throw new ArgumentException("No previous page available");

        return await GetJsonAsync<FacebookApiResponse>(currentResponse.Paging.Previous);
    }

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

    #region 7. 内容管理

    public async Task<FacebookApiResponse> DeleteContentAsync(string contentId)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));

        var url = $"{ApiBaseUrl}/{contentId}";
        var queryParams = new Dictionary<string, string>
        {
            ["access_token"] = _pageAccessToken
        };

        try
        {
            return await DeleteAsync<FacebookApiResponse>(url, queryParams);
        }
        catch (Exception ex)
        {
            throw new SocialMediaApiException($"Failed to delete content: {ex.Message}", ex, PlatformName);
        }
    }

    public async Task<FacebookApiResponse> UpdateContentAsync(string contentId, string newMessage)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));

        ValidateText(newMessage, MaxMessageLength, "Message");

        var url = $"{ApiBaseUrl}/{contentId}";
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
            throw new SocialMediaApiException($"Failed to update content: {ex.Message}", ex, PlatformName);
        }
    }

    #endregion

    #region 辅助方法

    public async Task<FacebookApiResponse> GetPageInfoAsync()
    {
        var url = $"{ApiBaseUrl}/{_pageId}";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = "id,name,about,category,followers_count,fan_count,link,picture",
            ["access_token"] = _pageAccessToken
        };

        return await GetJsonAsync<FacebookApiResponse>(url, queryParams);
    }

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
}
