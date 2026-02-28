
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Diagnostics;

namespace SmartMedia.Sites.Videos.Youtube;

/// <summary>
/// YouTube视频上传工具类
/// </summary>
public class YouTubeUploadService
{
    private YouTubeService _youtubeService;
    private UserCredential _credential;
    private readonly string[] _scopes = { YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.Youtube };

    /// <summary>
    /// 上传进度事件
    /// </summary>
    public event EventHandler<UploadProgressEventArgs> UploadProgressChanged;

    private string _ClientSecretsPath = string.Empty;
    private string _UserName = string.Empty;
    private string _AppName = string.Empty;
    public YouTubeUploadService(string clientSecretsPath, string userName = "user", string appName = "SmartMedia")
    {
        _ClientSecretsPath = clientSecretsPath;
        _UserName = userName;
        _AppName = appName;
    }

    /// <summary>
    /// 授权登录
    /// </summary>
    /// <param name="clientSecretsPath">client_secrets.json文件路径</param>
    /// <param name="userName">用户标识符（用于存储凭据）</param>
    /// <summary>
    /// 授权登录（如果已有有效令牌，不会打开浏览器）
    /// </summary>
    public async Task<bool> AuthorizeAsync()
    {
        try
        {
            // 这个调用会自动：
            // 1. 检查本地是否有有效令牌
            // 2. 有则使用，无则打开浏览器
            // 3. 令牌过期会自动刷新
            using (var stream = new FileStream(_ClientSecretsPath, FileMode.Open, FileAccess.Read))
            {
                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    _scopes,
                    _UserName,
                    CancellationToken.None
                );

                _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = _AppName
                });

                Debug.WriteLine($"授权成功！用户: {_credential.UserId}");
                Debug.WriteLine($"令牌过期时间: {DateTime.Now.AddSeconds(_credential.Token.ExpiresInSeconds ?? 0)}");

                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"授权失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <returns></returns>
    public async Task<bool> EnsureAuthorizationAsync()
    {

        // 情况0: 如果凭证完全为null，直接进行完整授权（这是最清晰的逻辑）
        if (_credential == null)
        {
            return await AuthorizeAsync();
        }

        // 情况A: 当前凭证有效，直接返回
        if (!_credential.Token.IsStale)
        {
            return true;
        }

        // 情况B: 有凭证但已过期，尝试静默刷新
        if (_credential != null && _credential.Token.IsStale)
        {
            try
            {
                if (await _credential.RefreshTokenAsync(CancellationToken.None))
                {
                    Debug.WriteLine("访问令牌已静默刷新。");
                    // !!! 关键修复：刷新后必须重建服务实例 !!!
                    _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = _credential,
                        ApplicationName = _AppName
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"静默刷新失败: {ex.Message}，需要重新授权。");
                // 刷新失败，继续执行情况C
            }
        }

        // 情况C: 无有效凭证，或刷新失败，进行完整授权
        // AuthorizeAsync 内部会创建 _youtubeService，所以这里直接调用即可
        return await AuthorizeAsync();
    }

    /// <summary>
    /// 上传视频
    /// </summary>
    /// <param name="videoPath">视频文件路径</param>
    /// <param name="title">标题</param>
    /// <param name="description">描述</param>
    /// <param name="tags">标签,多个用英文逗号分开</param>
    /// <param name="categoryId">分类ID（默认22=人物与博客）</param>
    /// <param name="privacyStatus">隐私状态: private, unlisted, public</param>
    /// <returns>上传后的视频ID</returns>
    public async Task<string> UploadVideoAsync(
        string videoPath,
        string title,
        string description,
        string sTags = "",
        string categoryId = "22",
        string privacyStatus = "private")
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var tags = sTags.Split(',');

        var video = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = title,
                Description = description,
                Tags = tags,
                CategoryId = categoryId
            },
            Status = new VideoStatus
            {
                PrivacyStatus = privacyStatus, // "private", "public", "unlisted"
                SelfDeclaredMadeForKids = false
            }
        };

        using (var fileStream = new FileStream(videoPath, FileMode.Open))
        {
            var videosInsertRequest = _youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

            videosInsertRequest.ProgressChanged += (progress) =>
            {
                UploadProgressChanged?.Invoke(this, new UploadProgressEventArgs
                {
                    BytesSent = progress.BytesSent,
                    Status = progress.Status.ToString()
                });

                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        Debug.WriteLine($"已上传 {progress.BytesSent} 字节");
                        break;
                    case UploadStatus.Failed:
                        Debug.WriteLine($"上传失败: {progress.Exception}");
                        break;
                }
            };

            videosInsertRequest.ResponseReceived += (video) =>
            {
                Debug.WriteLine($"视频上传成功！视频ID: {video.Id}");
            };

            var uploadResponse = await videosInsertRequest.UploadAsync();

            if (uploadResponse.Status == UploadStatus.Failed)
            {
                throw new Exception($"上传失败: {uploadResponse.Exception.Message}");
            }

            return videosInsertRequest.ResponseBody?.Id;
        }
    }

    /// <summary>
    /// 获取所有视频分类
    /// </summary>
    /// <param name="regionCode">地区代码，如 "US", "CN"</param>
    /// <param name="languageCode">语言代码，如 "zh-CN", "en-US"</param>
    /// <returns>分类列表</returns>
    public async Task<List<VideoCategory>> GetVideoCategoriesAsync(
        string regionCode = "CN",
        string languageCode = "zh-CN")  // 添加语言参数
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var categoriesRequest = _youtubeService.VideoCategories.List("snippet");
        categoriesRequest.RegionCode = regionCode;
        categoriesRequest.Hl = languageCode;  // 添加这一行！

        var categoriesResponse = await categoriesRequest.ExecuteAsync();
        return categoriesResponse.Items.ToList();
    }

    /// <summary>
    /// 获取我的频道信息
    /// </summary>
    public async Task<Google.Apis.YouTube.v3.Data.Channel> GetMyChannelAsync()
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var channelsRequest = _youtubeService.Channels.List("snippet,contentDetails,statistics");
        channelsRequest.Mine = true;

        var channelsResponse = await channelsRequest.ExecuteAsync();
        return channelsResponse.Items.FirstOrDefault();
    }

    /// <summary>
    /// 获取我上传的视频列表
    /// </summary>
    /// <param name="maxResults">最大结果数</param>
    public async Task<List<Video>> GetMyVideosAsync(long maxResults = 25)
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var channel = await GetMyChannelAsync();
        var uploadPlaylistId = channel?.ContentDetails?.RelatedPlaylists?.Uploads;

        if (string.IsNullOrEmpty(uploadPlaylistId))
            return new List<Video>();

        var playlistItemsRequest = _youtubeService.PlaylistItems.List("snippet");
        playlistItemsRequest.PlaylistId = uploadPlaylistId;
        playlistItemsRequest.MaxResults = maxResults;

        var playlistItemsResponse = await playlistItemsRequest.ExecuteAsync();

        var videos = new List<Video>();
        foreach (var item in playlistItemsResponse.Items)
        {
            var videoRequest = _youtubeService.Videos.List("snippet,statistics,status");
            videoRequest.Id = item.Snippet.ResourceId.VideoId;
            var videoResponse = await videoRequest.ExecuteAsync();
            videos.AddRange(videoResponse.Items);
        }

        return videos;
    }

    /// <summary>
    /// 更新视频信息
    /// </summary>
    public async Task<Video> UpdateVideoAsync(string videoId, string title = null, string description = null, List<string> tags = null)
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var videoRequest = _youtubeService.Videos.List("snippet,status");
        videoRequest.Id = videoId;
        var videoResponse = await videoRequest.ExecuteAsync();
        var video = videoResponse.Items.FirstOrDefault();

        if (video == null)
            throw new Exception("视频不存在");

        if (!string.IsNullOrEmpty(title))
            video.Snippet.Title = title;
        if (!string.IsNullOrEmpty(description))
            video.Snippet.Description = description;
        if (tags != null && tags.Count > 0)
            video.Snippet.Tags = tags;

        var updateRequest = _youtubeService.Videos.Update(video, "snippet,status");
        return await updateRequest.ExecuteAsync();
    }

    /// <summary>
    /// 删除视频
    /// </summary>
    public async Task<bool> DeleteVideoAsync(string videoId)
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        try
        {
            var deleteRequest = _youtubeService.Videos.Delete(videoId);
            await deleteRequest.ExecuteAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 设置视频缩略图
    /// </summary>
    /// <param name="videoId">视频Id</param>
    /// <param name="thumbnailPath">缩略图路径</param>
    /// <returns>错误信息，不为空表示有错误，为空表示成功</returns>
    public async Task<string> SetThumbnailAsync(string videoId, string thumbnailPath)
    {
        // 参数验证
        if (string.IsNullOrEmpty(videoId))
            return "视频ID不能为空。";

        if (string.IsNullOrEmpty(thumbnailPath))
            return "缩略图路径不能为空。";

        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            return "无法获得有效的API授权。";
        }

        try
        {
            // 验证文件是否存在
            if (!File.Exists(thumbnailPath))
                return $"缩略图文件不存在: {thumbnailPath}";

            // 验证文件扩展名
            string extension = Path.GetExtension(thumbnailPath).ToLowerInvariant();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

            if (!allowedExtensions.Contains(extension))
                return $"不支持的文件格式。支持的格式: {string.Join(", ", allowedExtensions)}";

            // 验证文件大小（YouTube限制：小于2MB）
            long fileSize = new FileInfo(thumbnailPath).Length;
            const long maxFileSize = 2 * 1024 * 1024; // 2MB

            if (fileSize > maxFileSize)
                return $"文件大小超过限制。最大: 2MB，当前: {FormatFileSize(fileSize)}";

            // 验证图像尺寸
            var imageInfo = await GetImageInfoAsync(thumbnailPath);
            if (imageInfo == null)
                return "无法读取图像信息，可能文件已损坏。";

            // 验证最小尺寸
            if (imageInfo.Width < 640 || imageInfo.Height < 480)
                return $"图像尺寸过小。最小要求: 640x480，当前: {imageInfo.Width}x{imageInfo.Height}";

            // 验证宽高比（推荐16:9，但非强制）
            double aspectRatio = (double)imageInfo.Width / imageInfo.Height;
            const double targetAspectRatio = 16.0 / 9.0;
            const double tolerance = 0.15; // 15%的容差

            if (Math.Abs(aspectRatio - targetAspectRatio) > tolerance)
            {
                Debug.WriteLine($"警告: 图像宽高比非标准16:9，实际: {aspectRatio:F2}");
                // 这里不返回错误，只是警告，因为YouTube会接受非16:9的图片
            }

            // 验证最大尺寸（虽然不是硬性限制，但建议合理范围）
            if (imageInfo.Width > 3840 || imageInfo.Height > 2160)
                Debug.WriteLine($"警告: 图像尺寸过大，建议控制在3840x2160以内。当前: {imageInfo.Width}x{imageInfo.Height}");

            // 获取正确的MIME类型
            string mimeType = GetMimeType(thumbnailPath);

            // 上传缩略图
            using (var fileStream = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read))
            {
                var thumbnailRequest = _youtubeService.Thumbnails.Set(videoId, fileStream, mimeType);
                await thumbnailRequest.UploadAsync();

                Debug.WriteLine($"缩略图设置成功！视频ID: {videoId}, 尺寸: {imageInfo.Width}x{imageInfo.Height}, 大小: {FormatFileSize(fileSize)}");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = ex.Message;

            // 提供更友好的错误信息
            if (ex.Message.Contains("quota", StringComparison.OrdinalIgnoreCase))
                errorMessage = "API配额不足，请稍后再试。";
            else if (ex.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
                errorMessage = "授权失效，请重新登录。";
            else if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                errorMessage = "视频不存在或已被删除。";
            else if (ex.Message.Contains("too large", StringComparison.OrdinalIgnoreCase))
                errorMessage = "文件过大，请确保小于2MB。";

            return $"缩略图上传失败：{errorMessage}";
        }
    }

    /// <summary>
    /// 获取图像信息（宽度、高度）
    /// </summary>
    private async Task<ImageInfo> GetImageInfoAsync(string imagePath)
    {
        try
        {
            // 使用System.Drawing（需要安装System.Drawing.Common包）
            using (var image = Image.FromFile(imagePath))
            {
                return new ImageInfo
                {
                    Width = image.Width,
                    Height = image.Height
                };
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"获取图像信息失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取MIME类型
    /// </summary>
    private string GetMimeType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "image/jpeg" // 默认
        };
    }

    /// <summary>
    /// 格式化文件大小显示
    /// </summary>
    private string FormatFileSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        int suffixIndex = 0;
        double size = bytes;

        while (size >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            size /= 1024;
            suffixIndex++;
        }

        return $"{size:0.##} {suffixes[suffixIndex]}";
    }

    /// <summary>
    /// 图像信息类
    /// </summary>
    private class ImageInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
    /// <summary>
    /// 获取当前登录用户的播放列表（合集）
    /// </summary>
    /// <param name="maxResults">返回结果的最大数量（默认50）</param>
    /// <returns>播放列表集合</returns>
    public async Task<List<Playlist>> GetMyPlaylistsAsync(long maxResults = 50)
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var playlistsRequest = _youtubeService.Playlists.List("snippet,contentDetails");
        playlistsRequest.Mine = true; // 仅获取当前授权账号的列表
        playlistsRequest.MaxResults = maxResults;

        var playlistsResponse = await playlistsRequest.ExecuteAsync();
        return playlistsResponse.Items.ToList();
    }

    /// <summary>
    /// 创建一个新的播放列表（合集）
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="description">描述</param>
    /// <param name="privacyStatus">隐私状态: public, private, unlisted</param>
    /// <returns>创建成功的播放列表对象</returns>
    public async Task<Playlist> CreatePlaylistAsync(string title, string description = "", string privacyStatus = "public")
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var newPlaylist = new Playlist
        {
            Snippet = new PlaylistSnippet
            {
                Title = title,
                Description = description
            },
            Status = new PlaylistStatus
            {
                PrivacyStatus = privacyStatus
            }
        };

        var insertRequest = _youtubeService.Playlists.Insert(newPlaylist, "snippet,status");
        return await insertRequest.ExecuteAsync();
    }

    /// <summary>
    /// 将视频添加到指定的播放列表
    /// </summary>
    /// <param name="videoId">视频ID</param>
    /// <param name="playlistId">播放列表ID</param>
    public async Task<PlaylistItem> AddVideoToPlaylistAsync(string videoId, string playlistId)
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var playlistItem = new PlaylistItem
        {
            Snippet = new PlaylistItemSnippet
            {
                PlaylistId = playlistId,
                ResourceId = new ResourceId
                {
                    Kind = "youtube#video",
                    VideoId = videoId
                }
            }
        };

        var insertRequest = _youtubeService.PlaylistItems.Insert(playlistItem, "snippet");
        return await insertRequest.ExecuteAsync();
    }
    /// <summary>
    /// 撤销授权
    /// </summary>
    public async Task RevokeAuthorizationAsync()
    {
        if (_credential != null)
        {
            await _credential.RevokeTokenAsync(CancellationToken.None);
            _credential = null;
            _youtubeService = null;
        }
    }

    /// <summary>
    /// 上传字幕文件
    /// </summary>
    /// <param name="videoId">视频ID</param>
    /// <param name="captionFilePath">字幕文件路径（支持SRT、VTT、SBV等格式）</param>
    /// <param name="language">语言代码（如 zh-CN, en, ja）</param>
    /// <param name="name">字幕名称（可选，如"中文（简体）"）</param>
    /// <param name="isDraft">是否为草稿（true=需要审核，false=直接发布）</param>
    /// <returns>上传的字幕ID</returns>
    public async Task<string> UploadCaptionAsync(
        string videoId,
        string captionFilePath,
        string language = "zh-CN",
        string name = null,
        bool isDraft = false)
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        if (!File.Exists(captionFilePath))
            throw new FileNotFoundException("字幕文件不存在", captionFilePath);

        // 创建字幕资源
        var caption = new Caption
        {
            Snippet = new CaptionSnippet
            {
                VideoId = videoId,
                Language = language,
                Name = name ?? GetLanguageName(language),
                IsDraft = isDraft
            }
        };

        // 读取字幕文件
        using (var fileStream = new FileStream(captionFilePath, FileMode.Open))
        {
            var captionInsertRequest = _youtubeService.Captions.Insert(
                caption,
                "snippet",
                fileStream,
                "*/*" // 支持所有字幕格式
            );

            var uploadedCaption = await captionInsertRequest.UploadAsync();

            if (uploadedCaption.Status == Google.Apis.Upload.UploadStatus.Failed)
            {
                throw new Exception($"字幕上传失败: {uploadedCaption.Exception?.Message}");
            }

            Console.WriteLine($"字幕上传成功！字幕ID: {captionInsertRequest.ResponseBody?.Id}");
            return captionInsertRequest.ResponseBody?.Id;
        }
    }

    /// <summary>
    /// 获取视频的所有字幕
    /// </summary>
    /// <param name="videoId">视频ID</param>
    /// <returns>字幕列表</returns>
    public async Task<List<Caption>> GetCaptionsAsync(string videoId)
    {
        // 确保授权
        if (!await EnsureAuthorizationAsync())
        {
            throw new InvalidOperationException("无法获得有效的API授权。");
        }

        var captionListRequest = _youtubeService.Captions.List("snippet", videoId);
        var captionListResponse = await captionListRequest.ExecuteAsync();

        return captionListResponse.Items.ToList();
    }
    /// <summary>
    /// 获取语言显示名称
    /// </summary>
    private string GetLanguageName(string languageCode)
    {
        var languageNames = new Dictionary<string, string>
            {
                { "zh-CN", "中文（简体）" },
                { "zh-TW", "中文（繁體）" },
                { "en", "English" },
                { "en-US", "English (US)" },
                { "en-GB", "English (UK)" },
                { "ja", "日本語" },
                { "ko", "한국어" },
                { "es", "Español" },
                { "fr", "Français" },
                { "de", "Deutsch" },
                { "ru", "Русский" },
                { "ar", "العربية" },
                { "pt", "Português" },
                { "it", "Italiano" },
                { "hi", "हिन्दी" },
                { "th", "ไทย" },
                { "vi", "Tiếng Việt" },
                { "id", "Bahasa Indonesia" }
            };

        return languageNames.ContainsKey(languageCode)
            ? languageNames[languageCode]
            : languageCode;
    }

}

/// <summary>
/// 上传进度事件参数
/// </summary>
public class UploadProgressEventArgs : EventArgs
{
    public long BytesSent { get; set; }
    public string Status { get; set; }
}