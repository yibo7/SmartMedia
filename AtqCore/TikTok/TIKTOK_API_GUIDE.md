# TikTok Content Posting API 完整指南

## 📋 目录

1. [TikTok API 概述](#tiktok-api-概述)
2. [支持的功能](#支持的功能)
3. [前置要求](#前置要求)
4. [凭据获取](#凭据获取)
5. [API 限制](#api-限制)
6. [发布流程](#发布流程)
7. [代码实现](#代码实现)
8. [集成到现有架构](#集成到现有架构)
9. [常见问题](#常见问题)

---

## TikTok API 概述

### ✅ 好消息：TikTok 提供官方 Content Posting API

TikTok 于 2023 年推出了 **Content Posting API**，允许第三方应用直接发布内容到用户的 TikTok 账户。

### 🎯 两种发布模式

| 模式 | 说明 | 用途 |
|------|------|------|
| **Direct Post** | 直接发布到 TikTok | 内容立即发布到用户的 TikTok 主页 |
| **Upload as Draft** | 上传为草稿 | 内容保存为草稿，用户可在 TikTok App 中编辑后发布 |

---

## 支持的功能

### ✅ 支持的内容类型

- ✅ **视频发布**（主要功能）
  - 标准视频帖子
  - 支持多种格式：MP4 + H.264, WebM, MPEG
  
- ✅ **图片发布**（较新功能）
  - 单张图片
  - 图片轮播（最多 35 张）
  - 支持格式：JPG, JPEG, WEBP（**不支持 PNG**）

### 📊 视频要求

| 参数 | 要求 |
|------|------|
| **最小时长** | 3 秒 |
| **最大时长** | 10 分钟（普通用户）<br>60 分钟（认证用户） |
| **文件大小** | 最大 4GB |
| **分辨率** | 最小 360p，推荐 720p 或 1080p |
| **宽高比** | 支持 9:16（竖屏）、16:9（横屏）、1:1（方形） |
| **格式** | MP4 (H.264), WebM, MPEG |

### 📊 图片要求

| 参数 | 要求 |
|------|------|
| **格式** | JPG, JPEG, WEBP（**不支持 PNG**） |
| **文件大小** | 每张最大 20MB |
| **数量** | 1-35 张 |
| **分辨率** | 最小 720p，推荐 1080p |

### 🎨 发布设置

- ✅ 标题/描述（最多 2,200 字符）
- ✅ Hashtags 标签
- ✅ 提及用户 (@用户名)
- ✅ 隐私设置（公开、好友、仅自己）
- ✅ 评论开关
- ✅ Duet 开关
- ✅ Stitch 开关
- ✅ 视频封面选择
- ✅ 品牌内容标记
- ✅ AI 生成内容标记
- ❌ **不支持定时发布**（目前）
- ❌ **不支持音乐选择**（可自动添加）

---

## 前置要求

### 1. TikTok 开发者账户

🔗 注册地址：https://developers.tiktok.com/

### 2. TikTok 账户类型

| 账户类型 | 是否支持 |
|---------|---------|
| 个人账户 | ✅ 支持 |
| 商业账户 | ✅ 支持 |
| 创作者账户 | ✅ 支持 |

**注意**：与 Instagram 不同，TikTok API **不要求**商业账户！

### 3. 应用审核要求

⚠️ **重要**：
- **测试阶段**：所有通过 API 发布的内容将被设为**私密可见**（仅自己可见）
- **生产阶段**：需要通过 TikTok 的应用审核才能发布**公开内容**
- **审核内容**：需要证明应用符合 TikTok 的服务条款和社区准则

---

## 凭据获取

### 第一步：创建 TikTok App

#### 1. 访问 TikTok 开发者控制台

```
1. 访问：https://developers.tiktok.com/
2. 登录你的 TikTok 账户
3. 点击 "Manage Apps" → "Create an App"
```

#### 2. 填写应用信息

```
App Name: Social Media Manager
App Description: 社交媒体内容发布管理工具
Category: Content & Publishing
```

#### 3. 记录凭据

创建成功后，在应用详情页面记录：

```
Client Key: awxxxxxxxxxx      ← 类似 App ID
Client Secret: xxxxxxxxxx     ← 保密！
```

---

### 第二步：配置 Content Posting API

#### 1. 添加 Content Posting API 产品

```
应用控制台:
    └── Products
        └── Content Posting API
            └── [Apply] 或 [Add]
```

#### 2. 配置重定向 URI

```
Redirect URI: https://your-domain.com/tiktok/callback
              http://localhost:8080/tiktok/callback (测试用)
```

#### 3. 启用 Direct Post 功能

```
Content Posting API 设置:
    └── Direct Post Configuration
        └── [Enable]
```

---

### 第三步：申请权限 (Scopes)

#### 必需权限

| 权限名称 | 用途 | 审核要求 |
|---------|------|---------|
| `user.info.basic` | 获取用户基本信息 | ✅ 需要审核 |
| `video.publish` | 发布视频 | ✅ 需要审核 |

#### 可选权限

| 权限名称 | 用途 | 审核要求 |
|---------|------|---------|
| `video.list` | 获取视频列表 | ✅ 需要审核 |
| `user.info.profile` | 获取用户详细信息 | ✅ 需要审核 |
| `user.info.stats` | 获取用户统计数据 | ✅ 需要审核 |

---

### 第四步：获取 Access Token

#### OAuth 2.0 授权流程

**1. 构建授权 URL**

```csharp
public string GetAuthorizationUrl(string clientKey, string redirectUri)
{
    var csrfState = Guid.NewGuid().ToString(); // 防 CSRF 攻击
    
    var authUrl = "https://www.tiktok.com/v2/auth/authorize" +
                  $"?client_key={clientKey}" +
                  $"&scope=user.info.basic,video.publish" +
                  $"&response_type=code" +
                  $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                  $"&state={csrfState}";
    
    return authUrl;
}
```

**2. 用户授权后，交换 Code 获取 Token**

```csharp
public async Task<TikTokTokenResponse> GetAccessTokenAsync(
    string code,
    string clientKey,
    string clientSecret,
    string redirectUri)
{
    var url = "https://open.tiktokapis.com/v2/oauth/token/";
    
    var payload = new
    {
        client_key = clientKey,
        client_secret = clientSecret,
        code = code,
        grant_type = "authorization_code",
        redirect_uri = redirectUri
    };
    
    var content = new StringContent(
        JsonSerializer.Serialize(payload),
        Encoding.UTF8,
        "application/json");
    
    var response = await httpClient.PostAsync(url, content);
    var jsonResponse = await response.Content.ReadAsStringAsync();
    
    return JsonSerializer.Deserialize<TikTokTokenResponse>(jsonResponse);
}

// 响应模型
public class TikTokTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }  // ← 这就是你需要的
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }  // 有效期（秒）
    
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }  // 用于刷新
    
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }
    
    [JsonPropertyName("open_id")]
    public string OpenId { get; set; }  // ← TikTok 用户 ID
    
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}
```

**3. Token 刷新**

```csharp
public async Task<TikTokTokenResponse> RefreshAccessTokenAsync(
    string refreshToken,
    string clientKey)
{
    var url = "https://open.tiktokapis.com/v2/oauth/token/";
    
    var payload = new
    {
        client_key = clientKey,
        grant_type = "refresh_token",
        refresh_token = refreshToken
    };
    
    // 发送请求...
}
```

#### Token 有效期

| Token 类型 | 有效期 |
|-----------|--------|
| Access Token | 24 小时 |
| Refresh Token | 长期有效（建议定期刷新）|

---

## API 限制

### ⚠️ 重要限制

| 限制类型 | 限制值 |
|---------|--------|
| **每分钟发布数** | 6 个视频/图片 |
| **每天发布数** | 15 个视频/图片 |
| **每用户请求速率** | 30 请求/分钟 |
| **初始化请求速率** | 6 请求/分钟 |

### 📊 处理时间

| 阶段 | 平均时间 |
|------|---------|
| 视频上传 | 取决于文件大小 |
| 视频处理 | 1-3 分钟 |
| 内容审核 | 1 分钟（可能几小时）|
| 总计 | 通常 2-5 分钟 |

---

## 发布流程

### 完整发布流程图

```
1. 初始化发布请求
   └── POST /v2/post/publish/video/init/
       └── 返回 publish_id 和 upload_url

2. 上传视频文件
   └── PUT 到 upload_url
       └── 支持分块上传

3. 轮询状态或等待 Webhook
   └── POST /v2/post/publish/status/fetch/
       └── 状态: PROCESSING → PUBLISH_COMPLETE → FAILED

4. 获取最终结果
   └── 成功: 返回 post_id 和 share_url
   └── 失败: 返回 fail_reason
```

---

## 代码实现

### TikTok Publisher 实现

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartMedia.AtqCore.SocialMedia;

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
```

---

## 集成到现有架构

### 添加 TikTok 到跨平台发布器

```csharp
// 1. 更新枚举
public enum TargetPlatform
{
    Facebook,
    Instagram,
    TikTok  // 新增
}

// 2. 在 CrossPlatformPublisher 中添加
public class CrossPlatformPublisher
{
    private readonly TikTokPublisher? _tiktokPublisher;
    
    public CrossPlatformPublisher(
        FacebookPagePublisher? facebookPublisher = null,
        InstagramPublisher? instagramPublisher = null,
        TikTokPublisher? tiktokPublisher = null)  // 新增
    {
        _facebookPublisher = facebookPublisher;
        _instagramPublisher = instagramPublisher;
        _tiktokPublisher = tiktokPublisher;  // 新增
    }
    
    // 在 PublishToPlatformAsync 中添加 TikTok 分支
    private async Task<PlatformPublishResult> PublishToPlatformAsync(...)
    {
        switch (platform)
        {
            case TargetPlatform.TikTok:
                var ttResult = await PublishToTikTokAsync(config, progress);
                result.MediaId = ttResult.PublishId;
                result.Permalink = ttResult.ShareUrl;
                break;
        }
    }
}
```

---

## 常见问题

### Q1: 为什么发布的内容是私密的？

**A**: 应用还在**测试模式**。需要完成以下步骤：

```
1. 充分测试应用功能
2. 准备审核材料：
   - 应用演示视频
   - 使用场景说明
   - 隐私政策
   - 服务条款
3. 提交审核申请
4. 通过审核后，内容可以公开发布
```

### Q2: 如何验证域名/URL？

**A**: 对于 `PULL_FROM_URL` 模式，需要验证域名：

```
1. 登录 TikTok 开发者控制台
2. 进入应用设置
3. 找到 "Domain Verification" 或 "URL Prefix"
4. 添加你的域名
5. 按照指引完成验证（通常是添加 DNS 记录或上传验证文件）
```

### Q3: 为什么图片上传失败？

**可能原因**：
1. ❌ 使用了 PNG 格式（TikTok **不支持** PNG）
2. ❌ 文件大小超过 20MB
3. ❌ 超过 35 张图片限制

**解决方案**：
```csharp
// 转换 PNG 到 JPEG
using var image = Image.Load(pngPath);
await image.SaveAsJpegAsync(jpegPath, new JpegEncoder { Quality = 90 });
```

### Q4: 如何处理速率限制？

**A**: 实现智能队列系统：

```csharp
public class TikTokPublishQueue
{
    private Queue<PublishTask> _queue = new();
    private int _publishedToday = 0;
    private DateTime _dayStart = DateTime.Today;
    
    public async Task<bool> CanPublishAsync()
    {
        // 重置每日计数
        if (DateTime.Today > _dayStart)
        {
            _publishedToday = 0;
            _dayStart = DateTime.Today;
        }
        
        // 检查限制
        if (_publishedToday >= 15)
        {
            return false; // 达到每日限制
        }
        
        return true;
    }
    
    public async Task QueuePublishAsync(PublishTask task)
    {
        if (!await CanPublishAsync())
        {
            // 延迟到明天
            task.ScheduledTime = DateTime.Today.AddDays(1);
        }
        
        _queue.Enqueue(task);
    }
}
```

### Q5: 能否添加音乐到视频？

**A**: 目前 API 有限支持：

- ✅ 可以设置 `auto_add_music = true` 让 TikTok 自动添加推荐音乐
- ❌ **不能**通过 API 指定特定音乐
- ✅ 用户可以在发布后在 TikTok App 中更改音乐

---

## 📚 参考资源

- 📖 [TikTok 开发者文档](https://developers.tiktok.com/)
- 📖 [Content Posting API 概览](https://developers.tiktok.com/products/content-posting-api/)
- 📖 [API 参考](https://developers.tiktok.com/doc/content-posting-api-reference-direct-post)
- 🛠️ [TikTok 开发者控制台](https://developers.tiktok.com/apps)

---

## 总结

### ✅ TikTok API 的优势

1. **官方支持** - 由 TikTok 官方提供和维护
2. **简单集成** - 标准 OAuth 2.0 流程
3. **功能丰富** - 支持视频和图片，多种发布选项
4. **不需要商业账户** - 个人账户即可使用

### ⚠️ TikTok API 的限制

1. **严格的速率限制** - 每天最多 15 个帖子
2. **需要审核** - 公开发布需要应用审核
3. **处理时间较长** - 通常需要几分钟
4. **功能限制** - 不支持定时发布、音乐选择

### 🎯 最佳使用场景

- ✅ 内容创作工具集成 TikTok 发布
- ✅ 社交媒体管理平台
- ✅ 批量发布工具
- ❌ 高频实时发布（受限于速率限制）

通过集成 TikTok API，您的社交媒体发布系统将支持当前最热门的短视频平台！🚀
