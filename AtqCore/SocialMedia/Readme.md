# 社交媒体发布器 - 架构文档

## 📋 目录

1. [概述](#概述)
2. [架构设计](#架构设计)
3. [核心组件](#核心组件)
4. [使用指南](#使用指南)
5. [最佳实践](#最佳实践)
6. [常见问题](#常见问题)

---

## 概述

这是一个企业级的社交媒体内容发布解决方案，采用分层架构设计，支持Facebook和Instagram平台的内容发布和管理。

### ✨ 主要特性

- ✅ **多平台支持**: Facebook Page 和 Instagram Business
- ✅ **跨平台发布**: 一次发布到多个平台
- ✅ **多种内容类型**: 文字、图片、视频、Reels、Stories、轮播
- ✅ **分块上传**: 支持大文件分块上传
- ✅ **进度追踪**: 实时上传进度反馈
- ✅ **批量发布**: 支持批量和定时发布
- ✅ **错误处理**: 完善的异常处理和重试机制
- ✅ **统计分析**: 发布统计和数据洞察

---

## 架构设计

### 🏗️ 分层架构

```
┌─────────────────────────────────────────────────────────┐
│              CrossPlatformPublisher                      │
│          (跨平台发布协调器 - 应用层)                       │
├─────────────────────────────────────────────────────────┤
│    FacebookPagePublisher   │   InstagramPublisher        │
│      (平台特定实现层)         │    (平台特定实现层)          │
├─────────────────────────────────────────────────────────┤
│           SocialMediaPublisherBase                       │
│              (抽象基类 - 基础设施层)                       │
│  • HTTP通信  • 文件处理  • 验证  • 重试                   │
└─────────────────────────────────────────────────────────┘
```

### 🎯 设计原则

1. **单一职责原则 (SRP)**: 每个类专注于一个平台的功能
2. **开闭原则 (OCP)**: 对扩展开放，对修改封闭
3. **里氏替换原则 (LSP)**: 所有发布器都可以替换基类
4. **接口隔离原则 (ISP)**: 平台特定功能独立
5. **依赖倒置原则 (DIP)**: 依赖抽象而非具体实现

### 📦 组件关系

```
SocialMediaPublisherBase (抽象基类)
    ├── FacebookPagePublisher (继承)
    ├── InstagramPublisher (继承)
    └── CrossPlatformPublisher (组合)
            ├── 聚合 FacebookPagePublisher
            └── 聚合 InstagramPublisher
```

---

## 核心组件

### 1️⃣ SocialMediaPublisherBase (抽象基类)

**职责**: 提供所有发布器的通用功能

**核心功能**:
- HTTP请求处理 (GET, POST, DELETE)
- 文件上传和分块上传
- 通用验证逻辑
- 重试机制
- 错误处理

**使用场景**: 不直接使用，作为其他发布器的基类

```csharp
// 基类提供的通用方法
protected async Task<T> GetJsonAsync<T>(string url, Dictionary<string, string>? queryParams = null)
protected async Task<T> PostJsonAsync<T>(string url, object payload)
protected async Task UploadFileInChunksAsync(string filePath, string uploadUrl, IProgress<UploadProgress>? progress)
protected void ValidateText(string text, int maxLength, string fieldName)
protected void ValidateFile(string filePath, string[] allowedExtensions, string fileType)
```

---

### 2️⃣ FacebookPagePublisher

**职责**: Facebook Page内容发布和管理

**支持的内容类型**:
- ✅ 纯文字帖子 (Feed)
- ✅ 图片帖子
- ✅ 视频帖子
- ✅ Facebook Reels
- ✅ 计划发布

**主要方法**:

```csharp
// 发布
public async Task<FacebookApiResponse> PublishFeedAsync(string message, PublishOptions? options = null)
public async Task<FacebookApiResponse> PublishPhotoAsync(string imagePath, string caption, PublishOptions? options = null)
public async Task<FacebookApiResponse> PublishVideoAsync(string videoPath, string description, PublishOptions? options = null, IProgress<UploadProgress>? progress = null)
public async Task<FacebookApiResponse> PublishReelsAsync(string videoPath, string caption, PublishOptions? options = null, IProgress<UploadProgress>? progress = null)

// 查询
public async Task<FacebookApiResponse> GetFeedAsync(ContentQueryOptions? options = null)
public async Task<FacebookApiResponse> GetPhotosAsync(ContentQueryOptions? options = null)
public async Task<FacebookApiResponse> GetVideosAsync(ContentQueryOptions? options = null)
public async Task<FacebookApiResponse> GetReelsAsync(ContentQueryOptions? options = null)

// 管理
public async Task<FacebookApiResponse> DeleteContentAsync(string contentId)
public async Task<FacebookApiResponse> UpdateContentAsync(string contentId, string newMessage)
```

**使用示例**:

```csharp
using var fbPublisher = new FacebookPagePublisher(pageId, accessToken);

// 发布图片
var result = await fbPublisher.PublishPhotoAsync(
    "path/to/image.jpg",
    "Amazing photo! #Photography"
);

// 发布Reels
var progress = new Progress<UploadProgress>(p => 
    Console.WriteLine($"{p.PercentComplete:F1}% - {p.CurrentPhase}"));

var reelsResult = await fbPublisher.PublishReelsAsync(
    "path/to/reel.mp4",
    "Quick tutorial! #Reels",
    progress: progress
);
```

---

### 3️⃣ InstagramPublisher

**职责**: Instagram Business账户内容发布和管理

**支持的内容类型**:
- ✅ 图片帖子
- ✅ 视频帖子
- ✅ Instagram Reels
- ✅ Stories
- ✅ 轮播 (Carousel)

**主要方法**:

```csharp
// 发布
public async Task<InstagramApiResponse> PublishPhotoAsync(string imagePath, string caption, InstagramPublishOptions? options = null, IProgress<UploadProgress>? progress = null)
public async Task<InstagramApiResponse> PublishVideoAsync(string videoPath, string caption, InstagramPublishOptions? options = null, IProgress<UploadProgress>? progress = null)
public async Task<InstagramApiResponse> PublishReelAsync(string videoPath, string caption, InstagramPublishOptions? options = null, IProgress<UploadProgress>? progress = null)
public async Task<InstagramApiResponse> PublishStoryAsync(string mediaPath, bool isVideo, StoryOptions? options = null, IProgress<UploadProgress>? progress = null)
public async Task<InstagramApiResponse> PublishCarouselAsync(List<CarouselItem> items, string caption, InstagramPublishOptions? options = null, IProgress<UploadProgress>? progress = null)

// 查询
public async Task<InstagramApiResponse> GetMediaAsync(InstagramQueryOptions? options = null)
public async Task<InstagramApiResponse> GetStoriesAsync()

// 分析
public async Task<InstagramApiResponse> GetMediaInsightsAsync(string mediaId)
public async Task<InstagramApiResponse> GetAccountInsightsAsync(string metric, string period)
```

**使用示例**:

```csharp
using var igPublisher = new InstagramPublisher(businessAccountId, accessToken);

// 发布Reels
var reelResult = await igPublisher.PublishReelAsync(
    "path/to/reel.mp4",
    "Tutorial time! 🎬 #Tutorial #Instagram",
    options: new InstagramPublishOptions
    {
        ShareToFeed = true,
        CoverUrl = "https://example.com/cover.jpg"
    }
);

// 发布轮播
var carouselItems = new List<CarouselItem>
{
    new CarouselItem { FilePath = "image1.jpg", MediaType = MediaType.Image },
    new CarouselItem { FilePath = "image2.jpg", MediaType = MediaType.Image },
    new CarouselItem { FilePath = "video.mp4", MediaType = MediaType.Video }
};

var carouselResult = await igPublisher.PublishCarouselAsync(
    carouselItems,
    "Swipe to see more! ➡️ #Carousel"
);
```

---

### 4️⃣ CrossPlatformPublisher

**职责**: 跨平台发布协调和统一接口

**核心功能**:
- 同时发布到多个平台
- 批量发布管理
- 发布统计和报告
- 错误处理和容错

**主要方法**:

```csharp
// 跨平台发布
public async Task<CrossPlatformPublishResult> PublishPhotoAsync(string imagePath, string caption, List<TargetPlatform> platforms, ...)
public async Task<CrossPlatformPublishResult> PublishVideoAsync(string videoPath, string caption, List<TargetPlatform> platforms, ...)
public async Task<CrossPlatformPublishResult> PublishReelsAsync(string videoPath, string caption, List<TargetPlatform> platforms, ...)

// 批量发布
public async Task<List<CrossPlatformPublishResult>> BatchPublishAsync(List<BatchPublishItem> items, IProgress<UploadProgress>? progress = null)

// 统计
public PublishStatistics GetStatistics(List<CrossPlatformPublishResult> results)
```

**使用示例**:

```csharp
using var fbPublisher = new FacebookPagePublisher(fbPageId, fbToken);
using var igPublisher = new InstagramPublisher(igAccountId, igToken);
using var crossPublisher = new CrossPlatformPublisher(fbPublisher, igPublisher);

// 同时发布到两个平台
var result = await crossPublisher.PublishPhotoAsync(
    "path/to/image.jpg",
    "Amazing photo! 📸 #CrossPlatform",
    platforms: new List<TargetPlatform> 
    { 
        TargetPlatform.Facebook, 
        TargetPlatform.Instagram 
    }
);

// 检查结果
if (result.OverallSuccess)
{
    Console.WriteLine("✅ Published to all platforms!");
}
else
{
    Console.WriteLine($"⚠️ Published to {result.SuccessCount}/{result.PlatformResults.Count} platforms");
    foreach (var failed in result.FailedPlatforms)
    {
        Console.WriteLine($"❌ {failed}: {result.PlatformResults[failed].ErrorMessage}");
    }
}
```

---

## 使用指南

### 前置要求

1. **Facebook要求**:
   - Facebook Page ID
   - Page Access Token (需要以下权限):
     - `pages_manage_posts`
     - `pages_read_engagement`
     - `pages_manage_metadata`

2. **Instagram要求**:
   - Instagram Business Account ID
   - Access Token (需要以下权限):
     - `instagram_basic`
     - `instagram_content_publish`
     - `instagram_manage_insights`

3. **媒体文件托管**:
   - Instagram需要公开可访问的媒体URL
   - 需要实现 `UploadToHostingAsync` 方法

### 快速开始

#### 1. 基础配置

```csharp
// appsettings.json
{
  "SocialMedia": {
    "Facebook": {
      "PageId": "your-page-id",
      "AccessToken": "your-access-token"
    },
    "Instagram": {
      "BusinessAccountId": "your-business-account-id",
      "AccessToken": "your-access-token"
    }
  }
}
```

#### 2. 初始化发布器

```csharp
// 单平台使用
var fbPublisher = new FacebookPagePublisher(
    configuration["SocialMedia:Facebook:PageId"],
    configuration["SocialMedia:Facebook:AccessToken"]
);

// 跨平台使用
var crossPublisher = new CrossPlatformPublisher(
    new FacebookPagePublisher(fbPageId, fbToken),
    new InstagramPublisher(igAccountId, igToken)
);
```

#### 3. 发布内容

```csharp
// 简单发布
await fbPublisher.PublishPhotoAsync(
    "photo.jpg",
    "Hello World!"
);

// 带进度追踪
var progress = new Progress<UploadProgress>(p =>
    Console.WriteLine($"{p.PercentComplete:F1}%")
);

await igPublisher.PublishReelAsync(
    "reel.mp4",
    "My first reel!",
    progress: progress
);

// 跨平台发布
await crossPublisher.PublishPhotoAsync(
    "photo.jpg",
    "Cross-platform post!",
    new List<TargetPlatform> { TargetPlatform.Facebook, TargetPlatform.Instagram }
);
```

---

## 最佳实践

### 1. 🔐 安全性

```csharp
// ❌ 不要硬编码凭据
var publisher = new FacebookPagePublisher("123456", "hardcoded-token");

// ✅ 使用配置或环境变量
var publisher = new FacebookPagePublisher(
    Environment.GetEnvironmentVariable("FB_PAGE_ID"),
    Environment.GetEnvironmentVariable("FB_ACCESS_TOKEN")
);
```

### 2. 📊 错误处理

```csharp
try
{
    var result = await publisher.PublishPhotoAsync("photo.jpg", "Caption");
}
catch (FileNotFoundException ex)
{
    // 文件不存在
    logger.LogError($"File not found: {ex.Message}");
}
catch (SocialMediaApiException ex)
{
    // API错误
    logger.LogError($"API Error ({ex.Platform}): {ex.Message}");
    if (ex.ErrorCode.HasValue)
    {
        // 根据错误码处理
        switch (ex.ErrorCode.Value)
        {
            case 190: // Token过期
                await RefreshTokenAsync();
                break;
            case 100: // 参数错误
                // 记录并通知
                break;
        }
    }
}
```

### 3. 🔄 资源管理

```csharp
// ✅ 使用 using 语句确保资源释放
using (var publisher = new FacebookPagePublisher(pageId, token))
{
    await publisher.PublishPhotoAsync("photo.jpg", "Caption");
}

// ✅ 或者在依赖注入中注册为 Scoped
services.AddScoped<FacebookPagePublisher>(sp => 
    new FacebookPagePublisher(pageId, token));
```

### 4. 📈 性能优化

```csharp
// ✅ 批量发布使用并行
var config = new CrossPlatformPublishConfig
{
    ParallelPublish = true,  // 并行发布到多个平台
    ContinueOnError = true   // 一个失败不影响其他
};

// ✅ 复用 HttpClient
var httpClient = new HttpClient();
var fb = new FacebookPagePublisher(pageId, token, httpClient);
var ig = new InstagramPublisher(accountId, token, httpClient);

// ✅ 使用进度反馈改善用户体验
var progress = new Progress<UploadProgress>(p =>
{
    progressBar.Value = p.PercentComplete;
    statusLabel.Text = p.CurrentPhase;
});
```

### 5. 📝 日志记录

```csharp
public class LoggingFacebookPublisher : FacebookPagePublisher
{
    private readonly ILogger _logger;

    public LoggingFacebookPublisher(
        string pageId, 
        string token, 
        ILogger logger) 
        : base(pageId, token)
    {
        _logger = logger;
    }

    protected override void LogInfo(string message)
    {
        _logger.LogInformation(message);
    }

    protected override void LogError(string message, Exception? ex = null)
    {
        _logger.LogError(ex, message);
    }
}
```

---

## 常见问题

### Q1: Instagram为什么需要公开URL？

**A**: Instagram API要求媒体文件必须通过公开可访问的URL提供。你需要：

1. 将文件上传到云存储（AWS S3, Azure Blob, Google Cloud Storage）
2. 实现 `UploadToHostingAsync` 方法
3. 确保URL在Instagram处理期间保持可访问

```csharp
private async Task<string> UploadToHostingAsync(string filePath, IProgress<UploadProgress>? progress)
{
    // 实现示例 - AWS S3
    var s3Client = new AmazonS3Client();
    var request = new PutObjectRequest
    {
        BucketName = "your-bucket",
        Key = $"instagram/{Guid.NewGuid()}/{Path.GetFileName(filePath)}",
        FilePath = filePath,
        CannedACL = S3CannedACL.PublicRead
    };

    await s3Client.PutObjectAsync(request);
    return $"https://your-bucket.s3.amazonaws.com/{request.Key}";
}
```

### Q2: 如何处理Token过期？

**A**: 实现Token刷新机制：

```csharp
public class TokenManager
{
    private string _currentToken;
    private DateTime _tokenExpiry;

    public async Task<string> GetValidTokenAsync()
    {
        if (DateTime.Now >= _tokenExpiry.AddMinutes(-5))
        {
            _currentToken = await RefreshTokenAsync();
            _tokenExpiry = DateTime.Now.AddHours(2); // Facebook tokens通常2小时
        }
        return _currentToken;
    }

    private async Task<string> RefreshTokenAsync()
    {
        // 实现Token刷新逻辑
        // Facebook: 使用长期Token或OAuth流程
        // Instagram: 通过Facebook Graph API刷新
    }
}
```

### Q3: 大文件上传超时怎么办？

**A**: 调整超时和分块大小：

```csharp
var httpClient = new HttpClient 
{ 
    Timeout = TimeSpan.FromMinutes(60) // 增加超时
};

var publisher = new FacebookPagePublisher(pageId, token, httpClient);

// 或者修改分块大小（在基类中）
// _chunkSize = 10 * 1024 * 1024; // 10MB chunks
```

### Q4: 如何实现定时发布？

**A**: 使用任务调度：

```csharp
// 方案1: 使用Facebook原生计划发布
var options = new PublishOptions
{
    Published = false,
    ScheduledPublish = true,
    ScheduledPublishTime = DateTimeOffset.Now.AddHours(2).ToUnixTimeSeconds()
};

await fbPublisher.PublishPhotoAsync("photo.jpg", "Scheduled post", options);

// 方案2: 使用后台任务（如Hangfire）
BackgroundJob.Schedule(
    () => PublishContentAsync("photo.jpg", "Caption"),
    TimeSpan.FromHours(2)
);
```

### Q5: 如何批量导入历史内容？

**A**: 使用批量发布功能：

```csharp
// 读取历史内容
var historicalPosts = await LoadHistoricalPostsAsync();

// 转换为批量发布项
var batchItems = historicalPosts.Select(post => new BatchPublishItem
{
    Config = new CrossPlatformPublishConfig
    {
        Content = post.Caption,
        FilePath = post.MediaPath,
        MediaType = DetermineMediaType(post.MediaPath),
        TargetPlatforms = new List<TargetPlatform> { TargetPlatform.Instagram }
    },
    ScheduledTime = post.OriginalPublishDate
}).ToList();

// 分批发布（避免API限流）
var batches = batchItems.Chunk(10); // 每批10个
foreach (var batch in batches)
{
    await crossPublisher.BatchPublishAsync(batch.ToList());
    await Task.Delay(TimeSpan.FromMinutes(1)); // 间隔1分钟
}
```

---

## 扩展开发

### 添加新平台

1. **创建新的发布器类**:

```csharp
public class TwitterPublisher : SocialMediaPublisherBase
{
    public override string PlatformName => "Twitter";
    protected override string ApiBaseUrl => "https://api.twitter.com/2";

    public override async Task<bool> TestConnectionAsync()
    {
        // 实现Twitter连接测试
    }

    public override async Task<object> GetAccountInfoAsync()
    {
        // 实现获取账户信息
    }

    // 实现Twitter特定功能...
}
```

2. **集成到CrossPlatformPublisher**:

```csharp
public enum TargetPlatform
{
    Facebook,
    Instagram,
    Twitter  // 新增
}

// 在CrossPlatformPublisher中添加支持
private TwitterPublisher? _twitterPublisher;
```

---

## 总结

这个架构提供了：

✅ **可维护性**: 清晰的分层和职责分离  
✅ **可扩展性**: 易于添加新平台和功能  
✅ **可复用性**: 基类提供通用功能  
✅ **健壮性**: 完善的错误处理和验证  
✅ **灵活性**: 支持单平台和跨平台使用  

通过这个架构，你可以轻松地管理多个社交媒体平台的内容发布，同时保持代码的整洁和可维护性。
