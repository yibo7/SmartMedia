using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMedia.AtqCore.SocialMedia;

#region 跨平台发布模型

/// <summary>
/// 跨平台发布结果
/// </summary>
public class CrossPlatformPublishResult
{
    public bool OverallSuccess { get; set; }
    public Dictionary<string, PlatformPublishResult> PlatformResults { get; set; } = new();
    public int SuccessCount => PlatformResults.Count(p => p.Value.Success);
    public int FailureCount => PlatformResults.Count(p => !p.Value.Success);
    public List<string> SuccessfulPlatforms => PlatformResults.Where(p => p.Value.Success).Select(p => p.Key).ToList();
    public List<string> FailedPlatforms => PlatformResults.Where(p => !p.Value.Success).Select(p => p.Key).ToList();
}

/// <summary>
/// 单个平台发布结果
/// </summary>
public class PlatformPublishResult
{
    public string Platform { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? MediaId { get; set; }
    public string? Permalink { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public DateTime PublishedAt { get; set; }
}

/// <summary>
/// 跨平台发布配置
/// </summary>
public class CrossPlatformPublishConfig
{
    public string Content { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public MediaType MediaType { get; set; }
    public List<TargetPlatform> TargetPlatforms { get; set; } = new();
    public FacebookPublishSettings? FacebookSettings { get; set; }
    public InstagramPublishSettings? InstagramSettings { get; set; }
    public bool ContinueOnError { get; set; } = true;
    public bool ParallelPublish { get; set; } = false;
}

/// <summary>
/// 目标平台枚举
/// </summary>
public enum TargetPlatform
{
    Facebook,
    Instagram
}

/// <summary>
/// Facebook发布设置
/// </summary>
public class FacebookPublishSettings
{
    public PublishOptions? Options { get; set; }
    public bool AsReel { get; set; } = false;
}

/// <summary>
/// Instagram发布设置
/// </summary>
public class InstagramPublishSettings
{
    public InstagramPublishOptions? Options { get; set; }
    public bool AsReel { get; set; } = false;
    public bool AsStory { get; set; } = false;
    public StoryOptions? StoryOptions { get; set; }
}

/// <summary>
/// 批量发布项
/// </summary>
public class BatchPublishItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public CrossPlatformPublishConfig Config { get; set; } = new();
    public DateTime ScheduledTime { get; set; } = DateTime.Now;
}

#endregion

/// <summary>
/// 跨平台发布协调器
/// 支持同时发布到Facebook和Instagram
/// </summary>
public class CrossPlatformPublisher : IDisposable
{
    private readonly FacebookPagePublisher? _facebookPublisher;
    private readonly InstagramPublisher? _instagramPublisher;
    private readonly bool _disposePublishers;

    public CrossPlatformPublisher(
        FacebookPagePublisher? facebookPublisher = null,
        InstagramPublisher? instagramPublisher = null,
        bool disposePublishers = true)
    {
        _facebookPublisher = facebookPublisher;
        _instagramPublisher = instagramPublisher;
        _disposePublishers = disposePublishers;

        if (_facebookPublisher == null && _instagramPublisher == null)
        {
            throw new ArgumentException(
                "At least one publisher (Facebook or Instagram) must be provided");
        }
    }

    #region 单内容跨平台发布

    /// <summary>
    /// 发布纯文字内容到多个平台
    /// </summary>
    public async Task<CrossPlatformPublishResult> PublishTextAsync(
        string message,
        List<TargetPlatform> platforms,
        FacebookPublishSettings? facebookSettings = null,
        bool continueOnError = true)
    {
        var config = new CrossPlatformPublishConfig
        {
            Content = message,
            MediaType = MediaType.Image, // Text posts
            TargetPlatforms = platforms,
            FacebookSettings = facebookSettings,
            ContinueOnError = continueOnError
        };

        return await PublishAsync(config);
    }

    /// <summary>
    /// 发布图片到多个平台
    /// </summary>
    public async Task<CrossPlatformPublishResult> PublishPhotoAsync(
        string imagePath,
        string caption,
        List<TargetPlatform> platforms,
        FacebookPublishSettings? facebookSettings = null,
        InstagramPublishSettings? instagramSettings = null,
        bool continueOnError = true,
        IProgress<UploadProgress>? progress = null)
    {
        var config = new CrossPlatformPublishConfig
        {
            Content = caption,
            FilePath = imagePath,
            MediaType = MediaType.Image,
            TargetPlatforms = platforms,
            FacebookSettings = facebookSettings,
            InstagramSettings = instagramSettings,
            ContinueOnError = continueOnError
        };

        return await PublishAsync(config, progress);
    }

    /// <summary>
    /// 发布视频到多个平台
    /// </summary>
    public async Task<CrossPlatformPublishResult> PublishVideoAsync(
        string videoPath,
        string caption,
        List<TargetPlatform> platforms,
        FacebookPublishSettings? facebookSettings = null,
        InstagramPublishSettings? instagramSettings = null,
        bool continueOnError = true,
        IProgress<UploadProgress>? progress = null)
    {
        var config = new CrossPlatformPublishConfig
        {
            Content = caption,
            FilePath = videoPath,
            MediaType = MediaType.Video,
            TargetPlatforms = platforms,
            FacebookSettings = facebookSettings,
            InstagramSettings = instagramSettings,
            ContinueOnError = continueOnError
        };

        return await PublishAsync(config, progress);
    }

    /// <summary>
    /// 发布Reels到多个平台
    /// </summary>
    public async Task<CrossPlatformPublishResult> PublishReelsAsync(
        string videoPath,
        string caption,
        List<TargetPlatform> platforms,
        FacebookPublishSettings? facebookSettings = null,
        InstagramPublishSettings? instagramSettings = null,
        bool continueOnError = true,
        IProgress<UploadProgress>? progress = null)
    {
        var config = new CrossPlatformPublishConfig
        {
            Content = caption,
            FilePath = videoPath,
            MediaType = MediaType.Reel,
            TargetPlatforms = platforms,
            FacebookSettings = facebookSettings ?? new FacebookPublishSettings { AsReel = true },
            InstagramSettings = instagramSettings ?? new InstagramPublishSettings { AsReel = true },
            ContinueOnError = continueOnError
        };

        return await PublishAsync(config, progress);
    }

    #endregion

    #region 通用发布方法

    /// <summary>
    /// 通用发布方法
    /// </summary>
    public async Task<CrossPlatformPublishResult> PublishAsync(
        CrossPlatformPublishConfig config,
        IProgress<UploadProgress>? progress = null)
    {
        var result = new CrossPlatformPublishResult();

        if (config.ParallelPublish)
        {
            // 并行发布
            var tasks = new List<Task<PlatformPublishResult>>();

            foreach (var platform in config.TargetPlatforms)
            {
                tasks.Add(PublishToPlatformAsync(platform, config, progress));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var platformResult in results)
            {
                result.PlatformResults[platformResult.Platform] = platformResult;
            }
        }
        else
        {
            // 顺序发布
            foreach (var platform in config.TargetPlatforms)
            {
                var platformResult = await PublishToPlatformAsync(platform, config, progress);
                result.PlatformResults[platform.ToString()] = platformResult;

                // 如果不继续执行且失败，则停止
                if (!config.ContinueOnError && !platformResult.Success)
                {
                    break;
                }
            }
        }

        result.OverallSuccess = result.FailureCount == 0;

        return result;
    }

    #endregion

    #region 批量发布

    /// <summary>
    /// 批量发布多个内容
    /// </summary>
    public async Task<List<CrossPlatformPublishResult>> BatchPublishAsync(
        List<BatchPublishItem> items,
        IProgress<UploadProgress>? progress = null)
    {
        var results = new List<CrossPlatformPublishResult>();

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            progress?.Report(new UploadProgress
            {
                CurrentPhase = $"Publishing item {i + 1}/{items.Count} (ID: {item.Id})"
            });

            try
            {
                // 如果是计划发布且时间未到，跳过
                if (item.ScheduledTime > DateTime.Now)
                {
                    continue;
                }

                var result = await PublishAsync(item.Config, progress);
                results.Add(result);
            }
            catch (Exception ex)
            {
                results.Add(new CrossPlatformPublishResult
                {
                    OverallSuccess = false,
                    PlatformResults = item.Config.TargetPlatforms.ToDictionary(
                        p => p.ToString(),
                        p => new PlatformPublishResult
                        {
                            Platform = p.ToString(),
                            Success = false,
                            ErrorMessage = $"Batch publish failed: {ex.Message}",
                            Exception = ex
                        })
                });
            }
        }

        return results;
    }

    #endregion

    #region 平台特定发布逻辑

    private async Task<PlatformPublishResult> PublishToPlatformAsync(
        TargetPlatform platform,
        CrossPlatformPublishConfig config,
        IProgress<UploadProgress>? progress = null)
    {
        var result = new PlatformPublishResult
        {
            Platform = platform.ToString(),
            PublishedAt = DateTime.Now
        };

        try
        {
            switch (platform)
            {
                case TargetPlatform.Facebook:
                    var fbResult = await PublishToFacebookAsync(config, progress);
                    result.Success = fbResult.Success;
                    result.MediaId = fbResult.Id ?? fbResult.PostId;
                    result.ErrorMessage = fbResult.Error?.Message;
                    break;

                case TargetPlatform.Instagram:
                    var igResult = await PublishToInstagramAsync(config, progress);
                    result.Success = igResult.Success;
                    result.MediaId = igResult.MediaId;
                    result.Permalink = igResult.Permalink;
                    result.ErrorMessage = igResult.Error?.Message;
                    break;

                default:
                    throw new ArgumentException($"Unsupported platform: {platform}");
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.Exception = ex;
        }

        return result;
    }

    private async Task<FacebookApiResponse> PublishToFacebookAsync(
        CrossPlatformPublishConfig config,
        IProgress<UploadProgress>? progress = null)
    {
        if (_facebookPublisher == null)
            throw new InvalidOperationException("Facebook publisher not configured");

        var fbSettings = config.FacebookSettings ?? new FacebookPublishSettings();

        switch (config.MediaType)
        {
            case MediaType.Image:
                if (string.IsNullOrEmpty(config.FilePath))
                {
                    // 纯文字
                    return await _facebookPublisher.PublishFeedAsync(
                        config.Content,
                        fbSettings.Options);
                }
                else
                {
                    // 图片
                    return await _facebookPublisher.PublishPhotoAsync(
                        config.FilePath,
                        config.Content,
                        fbSettings.Options);
                }

            case MediaType.Video:
                if (fbSettings.AsReel)
                {
                    return await _facebookPublisher.PublishReelsAsync(
                        config.FilePath!,
                        config.Content,
                        fbSettings.Options,
                        progress);
                }
                else
                {
                    return await _facebookPublisher.PublishVideoAsync(
                        config.FilePath!,
                        config.Content,
                        fbSettings.Options,
                        progress);
                }

            case MediaType.Reel:
                return await _facebookPublisher.PublishReelsAsync(
                    config.FilePath!,
                    config.Content,
                    fbSettings.Options,
                    progress);

            default:
                throw new ArgumentException($"Unsupported media type for Facebook: {config.MediaType}");
        }
    }

    private async Task<InstagramApiResponse> PublishToInstagramAsync(
        CrossPlatformPublishConfig config,
        IProgress<UploadProgress>? progress = null)
    {
        if (_instagramPublisher == null)
            throw new InvalidOperationException("Instagram publisher not configured");

        var igSettings = config.InstagramSettings ?? new InstagramPublishSettings();

        // Story优先
        if (igSettings.AsStory && !string.IsNullOrEmpty(config.FilePath))
        {
            var isVideo = config.MediaType == MediaType.Video || config.MediaType == MediaType.Reel;
            return await _instagramPublisher.PublishStoryAsync(
                config.FilePath,
                isVideo,
                igSettings.StoryOptions,
                progress);
        }

        switch (config.MediaType)
        {
            case MediaType.Image:
                return await _instagramPublisher.PublishPhotoAsync(
                    config.FilePath!,
                    config.Content,
                    igSettings.Options,
                    progress);

            case MediaType.Video:
                if (igSettings.AsReel)
                {
                    return await _instagramPublisher.PublishReelAsync(
                        config.FilePath!,
                        config.Content,
                        igSettings.Options,
                        progress);
                }
                else
                {
                    return await _instagramPublisher.PublishVideoAsync(
                        config.FilePath!,
                        config.Content,
                        igSettings.Options,
                        progress);
                }

            case MediaType.Reel:
                return await _instagramPublisher.PublishReelAsync(
                    config.FilePath!,
                    config.Content,
                    igSettings.Options,
                    progress);

            default:
                throw new ArgumentException($"Unsupported media type for Instagram: {config.MediaType}");
        }
    }

    #endregion

    #region 内容同步

    /// <summary>
    /// 从Facebook同步到Instagram
    /// </summary>
    public async Task<InstagramApiResponse> SyncFacebookToInstagramAsync(
        string facebookPostId,
        InstagramPublishSettings? settings = null)
    {
        if (_facebookPublisher == null || _instagramPublisher == null)
            throw new InvalidOperationException("Both Facebook and Instagram publishers must be configured");

        // 获取Facebook内容
        var fbContent = await _facebookPublisher.GetContentByIdAsync(facebookPostId);

        if (fbContent?.Data == null)
            throw new InvalidOperationException($"Facebook post not found: {facebookPostId}");

        // 注意：实际实现需要下载媒体文件并重新上传
        // 这里只是演示流程
        throw new NotImplementedException(
            "Content sync requires downloading media from Facebook and re-uploading to Instagram. " +
            "Implement media download and re-upload logic based on your requirements.");
    }

    #endregion

    #region 统计和报告

    /// <summary>
    /// 获取跨平台发布统计
    /// </summary>
    public PublishStatistics GetStatistics(List<CrossPlatformPublishResult> results)
    {
        var stats = new PublishStatistics();

        foreach (var result in results)
        {
            stats.TotalPublishes++;

            if (result.OverallSuccess)
                stats.SuccessfulPublishes++;
            else
                stats.FailedPublishes++;

            foreach (var platformResult in result.PlatformResults.Values)
            {
                if (!stats.PlatformStats.ContainsKey(platformResult.Platform))
                {
                    stats.PlatformStats[platformResult.Platform] = new PlatformStatistics
                    {
                        Platform = platformResult.Platform
                    };
                }

                var platformStats = stats.PlatformStats[platformResult.Platform];
                platformStats.TotalAttempts++;

                if (platformResult.Success)
                    platformStats.SuccessfulPublishes++;
                else
                    platformStats.FailedPublishes++;
            }
        }

        return stats;
    }

    #endregion

    #region IDisposable实现

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && _disposePublishers)
            {
                _facebookPublisher?.Dispose();
                _instagramPublisher?.Dispose();
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

#region 统计模型

/// <summary>
/// 发布统计
/// </summary>
public class PublishStatistics
{
    public int TotalPublishes { get; set; }
    public int SuccessfulPublishes { get; set; }
    public int FailedPublishes { get; set; }
    public double SuccessRate => TotalPublishes > 0 
        ? (double)SuccessfulPublishes / TotalPublishes * 100 
        : 0;

    public Dictionary<string, PlatformStatistics> PlatformStats { get; set; } = new();
}

/// <summary>
/// 平台统计
/// </summary>
public class PlatformStatistics
{
    public string Platform { get; set; } = string.Empty;
    public int TotalAttempts { get; set; }
    public int SuccessfulPublishes { get; set; }
    public int FailedPublishes { get; set; }
    public double SuccessRate => TotalAttempts > 0 
        ? (double)SuccessfulPublishes / TotalAttempts * 100 
        : 0;
}

#endregion
