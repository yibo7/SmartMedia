using System.Text.Json.Serialization;
namespace SmartMedia.AtqCore.SocialMedia;

#region Instagram特定数据模型

/// <summary>
/// Instagram API 响应模型
/// </summary>
public class InstagramApiResponse : SocialMediaApiResponse
{
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }

    [JsonPropertyName("data")]
    public new List<InstagramMedia>? Data { get; set; }

    [JsonPropertyName("paging")]
    public InstagramPaging? Paging { get; set; }

    [JsonPropertyName("container_id")]
    public string? ContainerId { get; set; }

    [JsonPropertyName("status_code")]
    public string? StatusCode { get; set; }
}

/// <summary>
/// Instagram 媒体模型
/// </summary>
public class InstagramMedia
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; } // IMAGE, VIDEO, CAROUSEL_ALBUM

    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }

    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("like_count")]
    public int? LikeCount { get; set; }

    [JsonPropertyName("comments_count")]
    public int? CommentsCount { get; set; }

    [JsonPropertyName("is_story")]
    public bool? IsStory { get; set; }

    [JsonPropertyName("children")]
    public InstagramChildren? Children { get; set; }
}

public class InstagramChildren
{
    [JsonPropertyName("data")]
    public List<InstagramMedia>? Data { get; set; }
}

public class InstagramPaging
{
    [JsonPropertyName("cursors")]
    public InstagramCursors? Cursors { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public string? Previous { get; set; }
}

public class InstagramCursors
{
    [JsonPropertyName("before")]
    public string? Before { get; set; }

    [JsonPropertyName("after")]
    public string? After { get; set; }
}

/// <summary>
/// Instagram Insights 数据
/// </summary>
public class InstagramInsights
{
    [JsonPropertyName("impressions")]
    public int? Impressions { get; set; }

    [JsonPropertyName("reach")]
    public int? Reach { get; set; }

    [JsonPropertyName("engagement")]
    public int? Engagement { get; set; }

    [JsonPropertyName("saved")]
    public int? Saved { get; set; }
}

#endregion

#region Instagram配置类

/// <summary>
/// Instagram 发布选项
/// </summary>
public class InstagramPublishOptions
{
    public string? LocationId { get; set; }
    public List<string>? UserTags { get; set; }
    public string? ProductTags { get; set; }
    public bool? ShareToFeed { get; set; } = true;
    public string? CoverUrl { get; set; }
    public string? VideoTitle { get; set; }
    public bool? IsCarouselItem { get; set; }
    public Dictionary<string, string>? AdditionalParams { get; set; }
}

/// <summary>
/// Instagram Carousel 项
/// </summary>
public class CarouselItem
{
    public string FilePath { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public bool IsVideo => MediaType == MediaType.Video;
}

/// <summary>
/// Instagram Story 选项
/// </summary>
public class StoryOptions
{
    public string? Link { get; set; }
    public string? LinkText { get; set; }
    public List<string>? Mentions { get; set; }
    public string? Location { get; set; }
    public Dictionary<string, string>? AdditionalParams { get; set; }
}

/// <summary>
/// Instagram 内容查询选项
/// </summary>
public class InstagramQueryOptions
{
    public int Limit { get; set; } = 25;
    public string? Fields { get; set; }
    public string? Since { get; set; }
    public string? Until { get; set; }
    public Dictionary<string, string>? AdditionalParams { get; set; }
}

#endregion

/// <summary>
/// Instagram 内容发布器
/// </summary>
public class InstagramPublisher : SocialMediaPublisherBase
{
    private readonly string _instagramBusinessAccountId;
    private readonly string _accessToken;

    private const string GraphApiVersion = "v19.0";
    protected override string ApiBaseUrl => $"https://graph.facebook.com/{GraphApiVersion}";
    public override string PlatformName => "Instagram";

    // Instagram特定限制
    private const int MaxCaptionLength = 2200;
    private const int MaxHashtags = 30;
    private const long MaxImageSize = 8 * 1024 * 1024; // 8MB
    private const long MaxVideoSize = 100 * 1024 * 1024; // 100MB
    private const long MaxStoryVideoSize = 4 * 1024 * 1024; // 4MB for stories
    private const int MaxCarouselItems = 10;

    // 视频时长限制
    private const double MinReelDuration = 3; // 3秒
    private const double MaxReelDuration = 90; // 90秒
    private const double MaxStoryDuration = 60; // 60秒
    private const double MaxIGTVDuration = 3600; // 60分钟

    private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png" };
    private static readonly string[] VideoExtensions = { ".mp4", ".mov" };

    public InstagramPublisher(
        string instagramBusinessAccountId, 
        string accessToken, 
        HttpClient? httpClient = null)
        : base(httpClient)
    {
        if (string.IsNullOrWhiteSpace(instagramBusinessAccountId))
            throw new ArgumentException(
                "Instagram Business Account ID cannot be empty", 
                nameof(instagramBusinessAccountId));

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access Token cannot be empty", nameof(accessToken));

        _instagramBusinessAccountId = instagramBusinessAccountId;
        _accessToken = accessToken;
    }

    #region 实现抽象方法

    public override async Task<bool> TestConnectionAsync()
    {
        try
        {
            await GetAccountInfoAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override async Task<object> GetAccountInfoAsync()
    {
        var url = $"{ApiBaseUrl}/{_instagramBusinessAccountId}";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = "id,username,name,profile_picture_url,followers_count,follows_count,media_count",
            ["access_token"] = _accessToken
        };

        return await GetJsonAsync<InstagramApiResponse>(url, queryParams);
    }

    #endregion

    #region 1. 发布单张图片

    public async Task<InstagramApiResponse> PublishPhotoAsync(
        string imagePath,
        string caption,
        InstagramPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        ValidateFile(imagePath, ImageExtensions, "Image");
        ValidateFileSize(imagePath, MaxImageSize, "Image");
        ValidateCaption(caption);

        progress?.Report(new UploadProgress { CurrentPhase = "Uploading image" });

        // 第一步：上传图片到Instagram服务器（这里需要先上传到可访问的URL）
        // 实际应用中，你需要先将图片上传到自己的服务器或云存储
        // 这里假设已有图片URL
        var imageUrl = await UploadToHostingAsync(imagePath, progress);

        // 第二步：创建媒体容器
        var containerId = await CreateMediaContainerAsync(imageUrl, caption, "IMAGE", options);

        // 第三步：发布媒体
        return await PublishMediaAsync(containerId);
    }

    #endregion

    #region 2. 发布视频

    public async Task<InstagramApiResponse> PublishVideoAsync(
        string videoPath,
        string caption,
        InstagramPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        ValidateFile(videoPath, VideoExtensions, "Video");
        ValidateFileSize(videoPath, MaxVideoSize, "Video");
        ValidateCaption(caption);

        progress?.Report(new UploadProgress { CurrentPhase = "Uploading video" });

        var videoUrl = await UploadToHostingAsync(videoPath, progress);
        var containerId = await CreateMediaContainerAsync(videoUrl, caption, "VIDEO", options);

        // 等待视频处理完成
        await WaitForMediaProcessingAsync(containerId, progress);

        return await PublishMediaAsync(containerId);
    }

    #endregion

    #region 3. 发布Reels

    public async Task<InstagramApiResponse> PublishReelAsync(
        string videoPath,
        string caption,
        InstagramPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        ValidateFile(videoPath, VideoExtensions, "Reel");
        ValidateFileSize(videoPath, MaxVideoSize, "Reel");
        ValidateCaption(caption);

        progress?.Report(new UploadProgress { CurrentPhase = "Uploading reel" });

        var videoUrl = await UploadToHostingAsync(videoPath, progress);

        var createUrl = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/media";
        var payload = new Dictionary<string, object>
        {
            ["media_type"] = "REELS",
            ["video_url"] = videoUrl,
            ["caption"] = caption,
            ["share_to_feed"] = options?.ShareToFeed ?? true,
            ["access_token"] = _accessToken
        };

        if (!string.IsNullOrEmpty(options?.CoverUrl))
            payload["cover_url"] = options.CoverUrl;

        AddOptionalParameters(payload, options);

        var containerResponse = await PostJsonAsync<InstagramApiResponse>(createUrl, payload);
        var containerId = containerResponse.Id;

        if (string.IsNullOrEmpty(containerId))
            throw new SocialMediaApiException("Failed to create reel container", PlatformName);

        await WaitForMediaProcessingAsync(containerId, progress);

        return await PublishMediaAsync(containerId);
    }

    #endregion

    #region 4. 发布Carousel（轮播）

    public async Task<InstagramApiResponse> PublishCarouselAsync(
        List<CarouselItem> items,
        string caption,
        InstagramPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Carousel must have at least one item");

        if (items.Count > MaxCarouselItems)
            throw new ArgumentException($"Carousel can have maximum {MaxCarouselItems} items");

        ValidateCaption(caption);

        var childrenIds = new List<string>();

        // 上传每个项目
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            progress?.Report(new UploadProgress 
            { 
                CurrentPhase = $"Uploading item {i + 1}/{items.Count}" 
            });

            var mediaUrl = await UploadToHostingAsync(item.FilePath, progress);
            var mediaType = item.IsVideo ? "VIDEO" : "IMAGE";

            var childOptions = new InstagramPublishOptions 
            { 
                IsCarouselItem = true 
            };

            var childId = await CreateMediaContainerAsync(mediaUrl, string.Empty, mediaType, childOptions);
            childrenIds.Add(childId);

            if (item.IsVideo)
            {
                await WaitForMediaProcessingAsync(childId, progress);
            }
        }

        // 创建轮播容器
        var createUrl = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/media";
        var payload = new Dictionary<string, object>
        {
            ["media_type"] = "CAROUSEL",
            ["caption"] = caption,
            ["children"] = childrenIds,
            ["access_token"] = _accessToken
        };

        AddOptionalParameters(payload, options);

        var carouselResponse = await PostJsonAsync<InstagramApiResponse>(createUrl, payload);
        var carouselId = carouselResponse.Id;

        if (string.IsNullOrEmpty(carouselId))
            throw new SocialMediaApiException("Failed to create carousel container", PlatformName);

        return await PublishMediaAsync(carouselId);
    }

    #endregion

    #region 5. 发布Story

    public async Task<InstagramApiResponse> PublishStoryAsync(
        string mediaPath,
        bool isVideo,
        StoryOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        var extensions = isVideo ? VideoExtensions : ImageExtensions;
        var maxSize = isVideo ? MaxStoryVideoSize : MaxImageSize;

        ValidateFile(mediaPath, extensions, "Story media");
        ValidateFileSize(mediaPath, maxSize, "Story media");

        progress?.Report(new UploadProgress { CurrentPhase = "Uploading story" });

        var mediaUrl = await UploadToHostingAsync(mediaPath, progress);
        var mediaType = isVideo ? "VIDEO" : "IMAGE";

        var createUrl = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/media";
        var payload = new Dictionary<string, object>
        {
            ["media_type"] = "STORIES",
            [isVideo ? "video_url" : "image_url"] = mediaUrl,
            ["access_token"] = _accessToken
        };

        if (options != null)
        {
            if (!string.IsNullOrEmpty(options.Link))
                payload["link"] = options.Link;

            if (options.Mentions != null && options.Mentions.Any())
                payload["user_tags"] = options.Mentions;

            if (options.AdditionalParams != null)
            {
                foreach (var param in options.AdditionalParams)
                {
                    payload[param.Key] = param.Value;
                }
            }
        }

        var containerResponse = await PostJsonAsync<InstagramApiResponse>(createUrl, payload);
        var containerId = containerResponse.Id;

        if (string.IsNullOrEmpty(containerId))
            throw new SocialMediaApiException("Failed to create story container", PlatformName);

        if (isVideo)
        {
            await WaitForMediaProcessingAsync(containerId, progress);
        }

        return await PublishMediaAsync(containerId);
    }

    #endregion

    #region 6. 获取媒体内容

    public async Task<InstagramApiResponse> GetMediaAsync(InstagramQueryOptions? options = null)
    {
        options ??= new InstagramQueryOptions();

        var defaultFields = "id,caption,media_type,media_url,permalink,thumbnail_url," +
                            "timestamp,username,like_count,comments_count";

        var fields = options.Fields ?? defaultFields;

        var url = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/media";
        var queryParams = BuildQueryParams(options, fields);

        return await GetJsonAsync<InstagramApiResponse>(url, queryParams);
    }

    public async Task<InstagramApiResponse> GetMediaByIdAsync(string mediaId, string? fields = null)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
            throw new ArgumentException("Media ID cannot be empty", nameof(mediaId));

        var defaultFields = "id,caption,media_type,media_url,permalink,thumbnail_url," +
                            "timestamp,username,like_count,comments_count,children{id,media_type,media_url}";

        fields ??= defaultFields;

        var url = $"{ApiBaseUrl}/{mediaId}";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = fields,
            ["access_token"] = _accessToken
        };

        return await GetJsonAsync<InstagramApiResponse>(url, queryParams);
    }

    public async Task<InstagramApiResponse> GetStoriesAsync()
    {
        var url = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/stories";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = "id,media_type,media_url,permalink,timestamp",
            ["access_token"] = _accessToken
        };

        return await GetJsonAsync<InstagramApiResponse>(url, queryParams);
    }

    #endregion

    #region 7. 内容管理

    public async Task<InstagramApiResponse> DeleteMediaAsync(string mediaId)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
            throw new ArgumentException("Media ID cannot be empty", nameof(mediaId));

        var url = $"{ApiBaseUrl}/{mediaId}";
        var queryParams = new Dictionary<string, string>
        {
            ["access_token"] = _accessToken
        };

        return await DeleteAsync<InstagramApiResponse>(url, queryParams);
    }

    public async Task<InstagramApiResponse> UpdateCaptionAsync(string mediaId, string newCaption)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
            throw new ArgumentException("Media ID cannot be empty", nameof(mediaId));

        ValidateCaption(newCaption);

        var url = $"{ApiBaseUrl}/{mediaId}";
        var payload = new Dictionary<string, object>
        {
            ["caption"] = newCaption,
            ["access_token"] = _accessToken
        };

        return await PostJsonAsync<InstagramApiResponse>(url, payload);
    }

    #endregion

    #region 8. 获取Insights（数据分析）

    public async Task<InstagramApiResponse> GetMediaInsightsAsync(string mediaId)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
            throw new ArgumentException("Media ID cannot be empty", nameof(mediaId));

        var url = $"{ApiBaseUrl}/{mediaId}/insights";
        var queryParams = new Dictionary<string, string>
        {
            ["metric"] = "impressions,reach,engagement,saved",
            ["access_token"] = _accessToken
        };

        return await GetJsonAsync<InstagramApiResponse>(url, queryParams);
    }

    public async Task<InstagramApiResponse> GetAccountInsightsAsync(
        string metric = "impressions,reach,profile_views",
        string period = "day",
        string? since = null,
        string? until = null)
    {
        var url = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/insights";
        var queryParams = new Dictionary<string, string>
        {
            ["metric"] = metric,
            ["period"] = period,
            ["access_token"] = _accessToken
        };

        if (!string.IsNullOrEmpty(since))
            queryParams["since"] = since;

        if (!string.IsNullOrEmpty(until))
            queryParams["until"] = until;

        return await GetJsonAsync<InstagramApiResponse>(url, queryParams);
    }

    #endregion

    #region 辅助方法

    private async Task<string> CreateMediaContainerAsync(
        string mediaUrl,
        string caption,
        string mediaType,
        InstagramPublishOptions? options)
    {
        var createUrl = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/media";
        var payload = new Dictionary<string, object>
        {
            ["access_token"] = _accessToken
        };

        if (options?.IsCarouselItem == true)
        {
            payload["is_carousel_item"] = true;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(caption))
                payload["caption"] = caption;
        }

        switch (mediaType.ToUpper())
        {
            case "IMAGE":
                payload["image_url"] = mediaUrl;
                break;
            case "VIDEO":
                payload["video_url"] = mediaUrl;
                payload["media_type"] = "VIDEO";
                break;
            default:
                throw new ArgumentException($"Unsupported media type: {mediaType}");
        }

        AddOptionalParameters(payload, options);

        var response = await PostJsonAsync<InstagramApiResponse>(createUrl, payload);

        if (string.IsNullOrEmpty(response.Id))
            throw new SocialMediaApiException("Failed to create media container", PlatformName);

        return response.Id;
    }

    private async Task<InstagramApiResponse> PublishMediaAsync(string containerId)
    {
        var publishUrl = $"{ApiBaseUrl}/{_instagramBusinessAccountId}/media_publish";
        var payload = new Dictionary<string, object>
        {
            ["creation_id"] = containerId,
            ["access_token"] = _accessToken
        };

        return await PostJsonAsync<InstagramApiResponse>(publishUrl, payload);
    }

    private async Task WaitForMediaProcessingAsync(
        string containerId, 
        IProgress<UploadProgress>? progress = null,
        int maxWaitSeconds = 120)
    {
        var checkUrl = $"{ApiBaseUrl}/{containerId}";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = "status_code",
            ["access_token"] = _accessToken
        };

        var startTime = DateTime.Now;
        var checkInterval = TimeSpan.FromSeconds(3);

        while ((DateTime.Now - startTime).TotalSeconds < maxWaitSeconds)
        {
            var response = await GetJsonAsync<InstagramApiResponse>(checkUrl, queryParams);

            if (response.StatusCode == "FINISHED")
            {
                progress?.Report(new UploadProgress { CurrentPhase = "Processing complete" });
                return;
            }
            else if (response.StatusCode == "ERROR")
            {
                throw new SocialMediaApiException("Media processing failed", PlatformName);
            }

            progress?.Report(new UploadProgress 
            { 
                CurrentPhase = $"Processing media... ({response.StatusCode})" 
            });

            await Task.Delay(checkInterval);
        }

        throw new SocialMediaApiException("Media processing timeout", PlatformName);
    }

    private async Task<string> UploadToHostingAsync(string filePath, IProgress<UploadProgress>? progress = null)
    {
        // 这是一个占位方法
        // 实际应用中，你需要：
        // 1. 将文件上传到你自己的服务器或云存储（如AWS S3, Azure Blob, Google Cloud Storage）
        // 2. 返回一个公开可访问的URL
        // 3. 确保URL在Instagram抓取文件期间保持可访问

        // 示例实现（需要根据实际情况修改）：
        throw new NotImplementedException(
            "You must implement UploadToHostingAsync to upload media files to a publicly accessible hosting service. " +
            "Instagram requires media URLs to be publicly accessible for processing.");

        // 示例代码：
        // var uploadedUrl = await YourCloudStorageService.UploadFileAsync(filePath);
        // return uploadedUrl;
    }

    private void ValidateCaption(string caption)
    {
        if (!string.IsNullOrWhiteSpace(caption))
        {
            if (caption.Length > MaxCaptionLength)
                throw new ArgumentException(
                    $"Caption too long ({caption.Length} > {MaxCaptionLength} chars)");

            // 计算hashtag数量
            var hashtags = caption.Split('#').Length - 1;
            if (hashtags > MaxHashtags)
                throw new ArgumentException(
                    $"Too many hashtags ({hashtags} > {MaxHashtags})");
        }
    }

    private Dictionary<string, string> BuildQueryParams(InstagramQueryOptions options, string fields)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["limit"] = options.Limit.ToString(),
            ["fields"] = fields,
            ["access_token"] = _accessToken
        };

        if (!string.IsNullOrEmpty(options.Since))
            queryParams["since"] = options.Since;

        if (!string.IsNullOrEmpty(options.Until))
            queryParams["until"] = options.Until;

        if (options.AdditionalParams != null)
        {
            foreach (var param in options.AdditionalParams)
            {
                queryParams[param.Key] = param.Value;
            }
        }

        return queryParams;
    }

    private void AddOptionalParameters(
        Dictionary<string, object> payload, 
        InstagramPublishOptions? options)
    {
        if (options == null) return;

        if (!string.IsNullOrEmpty(options.LocationId))
            payload["location_id"] = options.LocationId;

        if (options.UserTags != null && options.UserTags.Any())
            payload["user_tags"] = options.UserTags;

        if (!string.IsNullOrEmpty(options.ProductTags))
            payload["product_tags"] = options.ProductTags;

        if (options.AdditionalParams != null)
        {
            foreach (var param in options.AdditionalParams)
            {
                payload[param.Key] = param.Value;
            }
        }
    }

    #endregion
}
