using global::SmartMedia.AtqCore.SocialMedia; 
using System.Text.Json.Serialization; 

namespace SmartMedia.AtqCore.TikTok;

#region TikTok 数据模型

public class TikTokApiResponse : SocialMediaApiResponse
{
    [JsonPropertyName("publish_id")]
    public string? PublishId { get; set; }

    [JsonPropertyName("upload_url")]
    public string? UploadUrl { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }  // PROCESSING, PUBLISH_COMPLETE, FAILED

    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    [JsonPropertyName("publicaly_available_post_id")]
    public List<string>? PublicPostIds { get; set; }

    [JsonPropertyName("share_url")]
    public string? ShareUrl { get; set; }
}

public class TikTokPublishOptions
{
    public string? Title { get; set; }
    public TikTokPrivacyLevel PrivacyLevel { get; set; } = TikTokPrivacyLevel.PublicToEveryone;
    public bool DisableComment { get; set; } = false;
    public bool DisableDuet { get; set; } = false;
    public bool DisableStitch { get; set; } = false;
    public int? VideoCoverTimestampMs { get; set; }
    public bool AutoAddMusic { get; set; } = false;
    public bool BrandedContent { get; set; } = false;
    public bool YourBrand { get; set; } = false;
}

public enum TikTokPrivacyLevel
{
    PublicToEveryone,
    MutualFollowFriends,
    SelfOnly
}

public enum TikTokUploadSource
{
    FileUpload,
    PullFromUrl
}

#endregion

public class TikTokPublisher : SocialMediaPublisherBase
{
    private readonly string _openId;
    private readonly string _accessToken;

    protected override string ApiBaseUrl => "https://open.tiktokapis.com";
    public override string PlatformName => "TikTok";

    // TikTok 限制
    private const int MaxTitleLength = 2200;
    private const long MaxVideoSize = 4L * 1024 * 1024 * 1024; // 4GB
    private const long MaxPhotoSize = 20 * 1024 * 1024; // 20MB
    private const int MaxPhotosPerPost = 35;

    private static readonly string[] VideoExtensions = { ".mp4", ".webm", ".mpeg" };
    private static readonly string[] PhotoExtensions = { ".jpg", ".jpeg", ".webp" };

    public TikTokPublisher(
        string openId,
        string accessToken,
        HttpClient? httpClient = null)
        : base(httpClient)
    {
        if (string.IsNullOrWhiteSpace(openId))
            throw new ArgumentException("OpenId cannot be empty", nameof(openId));

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access Token cannot be empty", nameof(accessToken));

        _openId = openId;
        _accessToken = accessToken;
    }

    #region 实现抽象方法

    public override async Task<bool> TestConnectionAsync()
    {
        try
        {
            await GetUserInfoAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override async Task<object> GetAccountInfoAsync()
    {
        return await GetUserInfoAsync();
    }

    #endregion

    #region 1. 发布视频

    public async Task<TikTokApiResponse> PublishVideoAsync(
        string videoPath,
        string title,
        TikTokPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        ValidateFile(videoPath, VideoExtensions, "Video");
        ValidateFileSize(videoPath, MaxVideoSize, "Video");
        ValidateTitle(title);

        options ??= new TikTokPublishOptions();

        // 步骤1: 初始化发布
        progress?.Report(new UploadProgress { CurrentPhase = "Initializing" });

        var initResponse = await InitializeVideoPublishAsync(
            videoPath,
            title,
            options,
            TikTokUploadSource.FileUpload);

        var publishId = initResponse.PublishId;
        var uploadUrl = initResponse.UploadUrl;

        if (string.IsNullOrEmpty(publishId) || string.IsNullOrEmpty(uploadUrl))
            throw new SocialMediaApiException("Failed to initialize video publish", PlatformName);

        // 步骤2: 上传视频
        progress?.Report(new UploadProgress { CurrentPhase = "Uploading video" });
        await UploadVideoAsync(videoPath, uploadUrl, progress);

        // 步骤3: 等待处理完成
        progress?.Report(new UploadProgress { CurrentPhase = "Processing" });
        return await WaitForPublishCompleteAsync(publishId, progress);
    }

    public async Task<TikTokApiResponse> PublishVideoFromUrlAsync(
        string videoUrl,
        string title,
        TikTokPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        ValidateTitle(title);
        options ??= new TikTokPublishOptions();

        // 使用 PULL_FROM_URL 模式
        var initResponse = await InitializeVideoPublishAsync(
            videoUrl,
            title,
            options,
            TikTokUploadSource.PullFromUrl);

        var publishId = initResponse.PublishId;

        if (string.IsNullOrEmpty(publishId))
            throw new SocialMediaApiException("Failed to initialize video publish", PlatformName);

        // TikTok 会自动从 URL 下载视频
        return await WaitForPublishCompleteAsync(publishId, progress);
    }

    #endregion

    #region 2. 发布图片

    public async Task<TikTokApiResponse> PublishPhotoAsync(
        string photoPath,
        string title,
        TikTokPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        return await PublishPhotosAsync(
            new List<string> { photoPath },
            title,
            options,
            progress);
    }

    public async Task<TikTokApiResponse> PublishPhotosAsync(
        List<string> photoPaths,
        string title,
        TikTokPublishOptions? options = null,
        IProgress<UploadProgress>? progress = null)
    {
        if (photoPaths == null || photoPaths.Count == 0)
            throw new ArgumentException("Must provide at least one photo");

        if (photoPaths.Count > MaxPhotosPerPost)
            throw new ArgumentException($"Maximum {MaxPhotosPerPost} photos per post");

        foreach (var path in photoPaths)
        {
            ValidateFile(path, PhotoExtensions, "Photo");
            ValidateFileSize(path, MaxPhotoSize, "Photo");
        }

        ValidateTitle(title);
        options ??= new TikTokPublishOptions();

        // 初始化图片发布
        progress?.Report(new UploadProgress { CurrentPhase = "Initializing" });

        var initResponse = await InitializePhotoPublishAsync(
            photoPaths.Count,
            title,
            options);

        var publishId = initResponse.PublishId;
        var uploadUrls = initResponse.Data as List<string>; // 每张图片一个 URL

        // 上传每张图片
        for (int i = 0; i < photoPaths.Count; i++)
        {
            progress?.Report(new UploadProgress
            {
                CurrentPhase = $"Uploading photo {i + 1}/{photoPaths.Count}"
            });

            await UploadPhotoAsync(photoPaths[i], uploadUrls[i]);
        }

        // 等待处理完成
        return await WaitForPublishCompleteAsync(publishId, progress);
    }

    #endregion

    #region 3. 查询功能

    public async Task<TikTokApiResponse> GetUserInfoAsync()
    {
        var url = $"{ApiBaseUrl}/v2/user/info/";
        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = "open_id,union_id,avatar_url,display_name,username"
        };

        // TikTok 使用 Bearer Token
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

        return await GetJsonAsync<TikTokApiResponse>(url, queryParams);
    }

    public async Task<TikTokApiResponse> GetPublishStatusAsync(string publishId)
    {
        var url = $"{ApiBaseUrl}/v2/post/publish/status/fetch/";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

        var payload = new { publish_id = publishId };

        return await PostJsonAsync<TikTokApiResponse>(url, payload);
    }

    #endregion

    #region 辅助方法

    private async Task<TikTokApiResponse> InitializeVideoPublishAsync(
        string videoPathOrUrl,
        string title,
        TikTokPublishOptions options,
        TikTokUploadSource source)
    {
        var url = $"{ApiBaseUrl}/v2/post/publish/video/init/";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

        var payload = new Dictionary<string, object>
        {
            ["post_info"] = new
            {
                title = title,
                privacy_level = GetPrivacyLevelString(options.PrivacyLevel),
                disable_comment = options.DisableComment,
                disable_duet = options.DisableDuet,
                disable_stitch = options.DisableStitch,
                video_cover_timestamp_ms = options.VideoCoverTimestampMs ?? 1000,
                brand_content_toggle = options.BrandedContent,
                brand_organic_toggle = options.YourBrand,
                auto_add_music = options.AutoAddMusic
            }
        };

        if (source == TikTokUploadSource.FileUpload)
        {
            var fileInfo = new FileInfo(videoPathOrUrl);
            payload["source_info"] = new
            {
                source = "FILE_UPLOAD",
                video_size = fileInfo.Length,
                chunk_size = _chunkSize,
                total_chunk_count = (int)Math.Ceiling((double)fileInfo.Length / _chunkSize)
            };
        }
        else
        {
            payload["source_info"] = new
            {
                source = "PULL_FROM_URL",
                video_url = videoPathOrUrl
            };
        }

        return await PostJsonAsync<TikTokApiResponse>(url, payload);
    }

    private async Task<TikTokApiResponse> InitializePhotoPublishAsync(
        int photoCount,
        string title,
        TikTokPublishOptions options)
    {
        var url = $"{ApiBaseUrl}/v2/post/publish/content/init/";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

        var payload = new
        {
            post_info = new
            {
                title = title,
                privacy_level = GetPrivacyLevelString(options.PrivacyLevel),
                disable_comment = options.DisableComment,
                auto_add_music = options.AutoAddMusic,
                brand_content_toggle = options.BrandedContent,
                brand_organic_toggle = options.YourBrand
            },
            post_mode = "DIRECT_POST",
            media_type = "PHOTO",
            source_info = new
            {
                source = "FILE_UPLOAD",
                photo_cover_index = 0,
                photo_count = photoCount
            }
        };

        return await PostJsonAsync<TikTokApiResponse>(url, payload);
    }

    private async Task UploadVideoAsync(
        string videoPath,
        string uploadUrl,
        IProgress<UploadProgress>? progress = null)
    {
        using var fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read);
        var fileSize = fileStream.Length;
        var buffer = new byte[_chunkSize];
        int bytesRead;
        long totalUploaded = 0;

        while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
        {
            var chunk = bytesRead < _chunkSize ? buffer[..bytesRead] : buffer;

            using var content = new ByteArrayContent(chunk);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4");

            var response = await _httpClient.PutAsync(uploadUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new SocialMediaApiException($"Upload failed: {error}", PlatformName);
            }

            totalUploaded += bytesRead;
            progress?.Report(new UploadProgress
            {
                BytesUploaded = totalUploaded,
                TotalBytes = fileSize,
                CurrentPhase = "Uploading"
            });
        }
    }

    private async Task UploadPhotoAsync(string photoPath, string uploadUrl)
    {
        var bytes = await File.ReadAllBytesAsync(photoPath);

        using var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        var response = await _httpClient.PutAsync(uploadUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new SocialMediaApiException($"Photo upload failed: {error}", PlatformName);
        }
    }

    private async Task<TikTokApiResponse> WaitForPublishCompleteAsync(
        string publishId,
        IProgress<UploadProgress>? progress = null,
        int maxWaitMinutes = 10)
    {
        var startTime = DateTime.Now;
        var checkInterval = TimeSpan.FromSeconds(5);

        while ((DateTime.Now - startTime).TotalMinutes < maxWaitMinutes)
        {
            var status = await GetPublishStatusAsync(publishId);

            progress?.Report(new UploadProgress
            {
                CurrentPhase = $"Processing... ({status.Status})"
            });

            switch (status.Status?.ToUpper())
            {
                case "PUBLISH_COMPLETE":
                    progress?.Report(new UploadProgress { CurrentPhase = "Complete" });
                    return status;

                case "FAILED":
                    throw new SocialMediaApiException(
                        $"Publish failed: {status.FailReason}",
                        PlatformName);

                case "PROCESSING":
                    await Task.Delay(checkInterval);
                    break;

                default:
                    await Task.Delay(checkInterval);
                    break;
            }
        }

        throw new SocialMediaApiException("Publish timeout", PlatformName);
    }

    private string GetPrivacyLevelString(TikTokPrivacyLevel level)
    {
        return level switch
        {
            TikTokPrivacyLevel.PublicToEveryone => "PUBLIC_TO_EVERYONE",
            TikTokPrivacyLevel.MutualFollowFriends => "MUTUAL_FOLLOW_FRIENDS",
            TikTokPrivacyLevel.SelfOnly => "SELF_ONLY",
            _ => "PUBLIC_TO_EVERYONE"
        };
    }

    private void ValidateTitle(string title)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            if (title.Length > MaxTitleLength)
                throw new ArgumentException(
                    $"Title too long ({title.Length} > {MaxTitleLength} chars)");
        }
    }

    #endregion
}