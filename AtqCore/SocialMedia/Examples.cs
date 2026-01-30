//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SmartMedia.AtqCore.SocialMedia;

//namespace SmartMedia.AtqCore.Examples;

///// <summary>
///// 社交媒体发布器使用示例
///// </summary>
//public class SocialMediaPublisherExamples
//{
//    // 配置信息（实际使用时从配置文件或环境变量读取）
//    private const string FacebookPageId = "your-facebook-page-id";
//    private const string FacebookAccessToken = "your-facebook-access-token";
//    private const string InstagramBusinessAccountId = "your-instagram-business-account-id";
//    private const string InstagramAccessToken = "your-instagram-access-token";

//    #region 示例1：Facebook单独使用

//    public static async Task Example1_FacebookBasicUsage()
//    {
//        Console.WriteLine("=== Example 1: Facebook Basic Usage ===\n");

//        using var fbPublisher = new FacebookPagePublisher(FacebookPageId, FacebookAccessToken);

//        // 测试连接
//        var connected = await fbPublisher.TestConnectionAsync();
//        Console.WriteLine($"Facebook connection test: {(connected ? "Success" : "Failed")}");

//        if (!connected) return;

//        // 1. 发布纯文字
//        Console.WriteLine("\n1. Publishing text post...");
//        var textResult = await fbPublisher.PublishFeedAsync(
//            "Hello from the new Social Media Publisher! 🚀");
//        Console.WriteLine($"   Posted! ID: {textResult.Id}");

//        // 2. 发布图片
//        Console.WriteLine("\n2. Publishing photo...");
//        var photoResult = await fbPublisher.PublishPhotoAsync(
//            imagePath: "path/to/image.jpg",
//            caption: "Check out this amazing photo! #SocialMedia #API");
//        Console.WriteLine($"   Posted! ID: {photoResult.Id}");

//        // 3. 发布视频
//        Console.WriteLine("\n3. Publishing video...");
//        var progress = new Progress<UploadProgress>(p =>
//            Console.WriteLine($"   Progress: {p.PercentComplete:F1}% - {p.CurrentPhase}"));

//        var videoResult = await fbPublisher.PublishVideoAsync(
//            videoPath: "path/to/video.mp4",
//            description: "Amazing video content! 🎥",
//            progress: progress);
//        Console.WriteLine($"   Posted! ID: {videoResult.VideoId}");

//        // 4. 发布Reels
//        Console.WriteLine("\n4. Publishing Reels...");
//        var reelsResult = await fbPublisher.PublishReelsAsync(
//            videoPath: "path/to/reel.mp4",
//            caption: "Quick Reel! #Reels #ShortVideo",
//            progress: progress);
//        Console.WriteLine($"   Posted! ID: {reelsResult.VideoId}");

//        // 5. 获取最近的帖子
//        Console.WriteLine("\n5. Fetching recent posts...");
//        var feed = await fbPublisher.GetFeedAsync(new ContentQueryOptions { Limit = 5 });
//        Console.WriteLine($"   Found {feed.Data?.Count ?? 0} posts");
//        if (feed.Data != null)
//        {
//            foreach (var post in feed.Data)
//            {
//                Console.WriteLine($"   - {post.Id}: {post.Message?.Substring(0, Math.Min(50, post.Message.Length ?? 0))}...");
//            }
//        }
//    }

//    #endregion

//    #region 示例2：Instagram单独使用

//    public static async Task Example2_InstagramBasicUsage()
//    {
//        Console.WriteLine("=== Example 2: Instagram Basic Usage ===\n");

//        using var igPublisher = new InstagramPublisher(InstagramBusinessAccountId, InstagramAccessToken);

//        // 测试连接
//        var connected = await igPublisher.TestConnectionAsync();
//        Console.WriteLine($"Instagram connection test: {(connected ? "Success" : "Failed")}");

//        if (!connected) return;

//        // 获取账户信息
//        Console.WriteLine("\nGetting account info...");
//        var accountInfo = await igPublisher.GetAccountInfoAsync();
//        Console.WriteLine($"Account info retrieved: {accountInfo}");

//        // 1. 发布图片
//        Console.WriteLine("\n1. Publishing photo...");
//        var progress = new Progress<UploadProgress>(p =>
//            Console.WriteLine($"   Progress: {p.PercentComplete:F1}% - {p.CurrentPhase}"));

//        var photoResult = await igPublisher.PublishPhotoAsync(
//            imagePath: "path/to/image.jpg",
//            caption: "Beautiful moment captured! 📸 #Photography #Instagram",
//            options: new InstagramPublishOptions
//            {
//                ShareToFeed = true,
//                LocationId = "123456789" // Optional
//            },
//            progress: progress);
//        Console.WriteLine($"   Posted! Media ID: {photoResult.MediaId}");
//        Console.WriteLine($"   Permalink: {photoResult.Permalink}");

//        // 2. 发布Reels
//        Console.WriteLine("\n2. Publishing Reel...");
//        var reelResult = await igPublisher.PublishReelAsync(
//            videoPath: "path/to/reel.mp4",
//            caption: "Quick tutorial! 🎬 #Reels #Tutorial #Instagram",
//            options: new InstagramPublishOptions
//            {
//                ShareToFeed = true,
//                CoverUrl = "https://example.com/cover.jpg" // Optional
//            },
//            progress: progress);
//        Console.WriteLine($"   Posted! Media ID: {reelResult.MediaId}");

//        // 3. 发布Story
//        Console.WriteLine("\n3. Publishing Story...");
//        var storyResult = await igPublisher.PublishStoryAsync(
//            mediaPath: "path/to/story-image.jpg",
//            isVideo: false,
//            options: new StoryOptions
//            {
//                Link = "https://example.com",
//                LinkText = "Learn More"
//            },
//            progress: progress);
//        Console.WriteLine($"   Posted! Media ID: {storyResult.MediaId}");

//        // 4. 发布轮播（Carousel）
//        Console.WriteLine("\n4. Publishing Carousel...");
//        var carouselItems = new List<CarouselItem>
//        {
//            new CarouselItem { FilePath = "path/to/image1.jpg", MediaType = MediaType.Image },
//            new CarouselItem { FilePath = "path/to/image2.jpg", MediaType = MediaType.Image },
//            new CarouselItem { FilePath = "path/to/video.mp4", MediaType = MediaType.Video }
//        };

//        var carouselResult = await igPublisher.PublishCarouselAsync(
//            items: carouselItems,
//            caption: "Swipe to see more! ➡️ #Carousel #MultiPost",
//            progress: progress);
//        Console.WriteLine($"   Posted! Media ID: {carouselResult.MediaId}");

//        // 5. 获取最近的媒体
//        Console.WriteLine("\n5. Fetching recent media...");
//        var media = await igPublisher.GetMediaAsync(new InstagramQueryOptions { Limit = 5 });
//        Console.WriteLine($"   Found {media.Data?.Count ?? 0} media items");
//        if (media.Data != null)
//        {
//            foreach (var item in media.Data)
//            {
//                Console.WriteLine($"   - {item.Id} ({item.MediaType}): {item.Caption?.Substring(0, Math.Min(50, item.Caption.Length ?? 0))}...");
//            }
//        }
//    }

//    #endregion

//    #region 示例3：跨平台发布

//    public static async Task Example3_CrossPlatformPublishing()
//    {
//        Console.WriteLine("=== Example 3: Cross-Platform Publishing ===\n");

//        using var fbPublisher = new FacebookPagePublisher(FacebookPageId, FacebookAccessToken);
//        using var igPublisher = new InstagramPublisher(InstagramBusinessAccountId, InstagramAccessToken);
//        using var crossPublisher = new CrossPlatformPublisher(fbPublisher, igPublisher);

//        var progress = new Progress<UploadProgress>(p =>
//            Console.WriteLine($"Progress: {p.PercentComplete:F1}% - {p.CurrentPhase}"));

//        // 1. 同时发布图片到Facebook和Instagram
//        Console.WriteLine("1. Publishing photo to both platforms...");
//        var photoResult = await crossPublisher.PublishPhotoAsync(
//            imagePath: "path/to/image.jpg",
//            caption: "Cross-platform post! 🌍 #Facebook #Instagram",
//            platforms: new List<TargetPlatform> { TargetPlatform.Facebook, TargetPlatform.Instagram },
//            progress: progress);

//        Console.WriteLine($"\n   Overall Success: {photoResult.OverallSuccess}");
//        Console.WriteLine($"   Successful Platforms: {string.Join(", ", photoResult.SuccessfulPlatforms)}");
//        Console.WriteLine($"   Failed Platforms: {string.Join(", ", photoResult.FailedPlatforms)}");

//        foreach (var platform in photoResult.PlatformResults)
//        {
//            Console.WriteLine($"\n   {platform.Key}:");
//            Console.WriteLine($"   - Success: {platform.Value.Success}");
//            Console.WriteLine($"   - Media ID: {platform.Value.MediaId}");
//            Console.WriteLine($"   - Permalink: {platform.Value.Permalink}");
//            if (!platform.Value.Success)
//                Console.WriteLine($"   - Error: {platform.Value.ErrorMessage}");
//        }

//        // 2. 同时发布Reels到两个平台
//        Console.WriteLine("\n\n2. Publishing Reel to both platforms...");
//        var reelResult = await crossPublisher.PublishReelsAsync(
//            videoPath: "path/to/reel.mp4",
//            caption: "Amazing Reel across platforms! 🎥 #Reels",
//            platforms: new List<TargetPlatform> { TargetPlatform.Facebook, TargetPlatform.Instagram },
//            facebookSettings: new FacebookPublishSettings { AsReel = true },
//            instagramSettings: new InstagramPublishSettings { AsReel = true, ShareToFeed = true },
//            progress: progress);

//        Console.WriteLine($"\n   Overall Success: {reelResult.OverallSuccess}");
//        Console.WriteLine($"   Success Count: {reelResult.SuccessCount}/{reelResult.PlatformResults.Count}");
//    }

//    #endregion

//    #region 示例4：批量发布

//    public static async Task Example4_BatchPublishing()
//    {
//        Console.WriteLine("=== Example 4: Batch Publishing ===\n");

//        using var fbPublisher = new FacebookPagePublisher(FacebookPageId, FacebookAccessToken);
//        using var igPublisher = new InstagramPublisher(InstagramBusinessAccountId, InstagramAccessToken);
//        using var crossPublisher = new CrossPlatformPublisher(fbPublisher, igPublisher);

//        // 准备批量发布项目
//        var batchItems = new List<BatchPublishItem>
//        {
//            new BatchPublishItem
//            {
//                Config = new CrossPlatformPublishConfig
//                {
//                    Content = "Post 1: Morning greeting! ☀️",
//                    FilePath = "path/to/morning.jpg",
//                    MediaType = MediaType.Image,
//                    TargetPlatforms = new List<TargetPlatform> { TargetPlatform.Facebook, TargetPlatform.Instagram }
//                },
//                ScheduledTime = DateTime.Now
//            },
//            new BatchPublishItem
//            {
//                Config = new CrossPlatformPublishConfig
//                {
//                    Content = "Post 2: Afternoon update! 🌤️",
//                    FilePath = "path/to/afternoon.jpg",
//                    MediaType = MediaType.Image,
//                    TargetPlatforms = new List<TargetPlatform> { TargetPlatform.Facebook, TargetPlatform.Instagram }
//                },
//                ScheduledTime = DateTime.Now.AddSeconds(5)
//            },
//            new BatchPublishItem
//            {
//                Config = new CrossPlatformPublishConfig
//                {
//                    Content = "Post 3: Evening wrap-up! 🌙",
//                    FilePath = "path/to/evening-reel.mp4",
//                    MediaType = MediaType.Reel,
//                    TargetPlatforms = new List<TargetPlatform> { TargetPlatform.Instagram },
//                    InstagramSettings = new InstagramPublishSettings { AsReel = true }
//                },
//                ScheduledTime = DateTime.Now.AddSeconds(10)
//            }
//        };

//        var progress = new Progress<UploadProgress>(p =>
//            Console.WriteLine($"Progress: {p.CurrentPhase}"));

//        Console.WriteLine("Starting batch publish...\n");
//        var results = await crossPublisher.BatchPublishAsync(batchItems, progress);

//        // 显示统计
//        var stats = crossPublisher.GetStatistics(results);
//        Console.WriteLine("\n=== Batch Publishing Statistics ===");
//        Console.WriteLine($"Total Publishes: {stats.TotalPublishes}");
//        Console.WriteLine($"Successful: {stats.SuccessfulPublishes}");
//        Console.WriteLine($"Failed: {stats.FailedPublishes}");
//        Console.WriteLine($"Success Rate: {stats.SuccessRate:F1}%");

//        Console.WriteLine("\n=== Platform Statistics ===");
//        foreach (var platformStat in stats.PlatformStats.Values)
//        {
//            Console.WriteLine($"\n{platformStat.Platform}:");
//            Console.WriteLine($"  Total Attempts: {platformStat.TotalAttempts}");
//            Console.WriteLine($"  Successful: {platformStat.SuccessfulPublishes}");
//            Console.WriteLine($"  Failed: {platformStat.FailedPublishes}");
//            Console.WriteLine($"  Success Rate: {platformStat.SuccessRate:F1}%");
//        }
//    }

//    #endregion

//    #region 示例5：高级功能

//    public static async Task Example5_AdvancedFeatures()
//    {
//        Console.WriteLine("=== Example 5: Advanced Features ===\n");

//        using var fbPublisher = new FacebookPagePublisher(FacebookPageId, FacebookAccessToken);
//        using var igPublisher = new InstagramPublisher(InstagramBusinessAccountId, InstagramAccessToken);

//        // 1. 定时发布（Facebook）
//        Console.WriteLine("1. Scheduling a post for future...");
//        var scheduledTime = DateTimeOffset.Now.AddHours(2).ToUnixTimeSeconds();
//        var scheduledResult = await fbPublisher.PublishFeedAsync(
//            "This post is scheduled for 2 hours from now! ⏰",
//            new PublishOptions
//            {
//                Published = false,
//                ScheduledPublish = true,
//                ScheduledPublishTime = scheduledTime
//            });
//        Console.WriteLine($"   Scheduled! ID: {scheduledResult.Id}");

//        // 2. 更新已发布的内容
//        Console.WriteLine("\n2. Updating published content...");
//        var updateResult = await fbPublisher.UpdateContentAsync(
//            contentId: "existing-post-id",
//            newMessage: "Updated content! This post has been edited. ✏️");
//        Console.WriteLine($"   Updated! Success: {updateResult.Success}");

//        // 3. 删除内容
//        Console.WriteLine("\n3. Deleting content...");
//        var deleteResult = await fbPublisher.DeleteContentAsync("post-id-to-delete");
//        Console.WriteLine($"   Deleted! Success: {deleteResult.Success}");

//        // 4. 获取内容洞察（Insights）
//        Console.WriteLine("\n4. Getting media insights...");
//        var insights = await igPublisher.GetMediaInsightsAsync("instagram-media-id");
//        Console.WriteLine($"   Insights retrieved successfully");

//        // 5. 分页获取所有帖子
//        Console.WriteLine("\n5. Fetching all posts with pagination...");
//        var allPosts = await fbPublisher.GetAllPostsAsync(maxPages: 3);
//        Console.WriteLine($"   Retrieved {allPosts.Count} posts across multiple pages");

//        // 6. 使用重试机制
//        Console.WriteLine("\n6. Publishing with retry mechanism...");
//        var retryPublisher = new FacebookPagePublisher(FacebookPageId, FacebookAccessToken);

//        // 使用基类的RetryAsync方法
//        // 注意：需要在子类中暴露或直接调用
//        Console.WriteLine("   Retry mechanism is built into the base class");
//    }

//    #endregion

//    #region 示例6：错误处理

//    public static async Task Example6_ErrorHandling()
//    {
//        Console.WriteLine("=== Example 6: Error Handling ===\n");

//        using var fbPublisher = new FacebookPagePublisher(FacebookPageId, FacebookAccessToken);

//        try
//        {
//            // 1. 处理文件不存在错误
//            Console.WriteLine("1. Testing file not found error...");
//            await fbPublisher.PublishPhotoAsync(
//                "non-existent-file.jpg",
//                "This will fail");
//        }
//        catch (FileNotFoundException ex)
//        {
//            Console.WriteLine($"   Caught FileNotFoundException: {ex.Message}");
//        }

//        try
//        {
//            // 2. 处理文本过长错误
//            Console.WriteLine("\n2. Testing text too long error...");
//            var longText = new string('A', 70000);
//            await fbPublisher.PublishFeedAsync(longText);
//        }
//        catch (ArgumentException ex)
//        {
//            Console.WriteLine($"   Caught ArgumentException: {ex.Message}");
//        }

//        try
//        {
//            // 3. 处理API错误
//            Console.WriteLine("\n3. Testing API error...");
//            var invalidPublisher = new FacebookPagePublisher("invalid-id", "invalid-token");
//            await invalidPublisher.TestConnectionAsync();
//        }
//        catch (SocialMediaApiException ex)
//        {
//            Console.WriteLine($"   Caught SocialMediaApiException:");
//            Console.WriteLine($"   Platform: {ex.Platform}");
//            Console.WriteLine($"   Error Code: {ex.ErrorCode}");
//            Console.WriteLine($"   Message: {ex.Message}");
//        }

//        // 4. 跨平台发布的错误处理
//        Console.WriteLine("\n4. Testing cross-platform error handling...");
//        using var crossPublisher = new CrossPlatformPublisher(fbPublisher, null);

//        var result = await crossPublisher.PublishPhotoAsync(
//            "path/to/image.jpg",
//            "Testing error handling",
//            new List<TargetPlatform> { TargetPlatform.Facebook },
//            continueOnError: true);

//        if (!result.OverallSuccess)
//        {
//            Console.WriteLine("   Some platforms failed:");
//            foreach (var failed in result.FailedPlatforms)
//            {
//                var platformResult = result.PlatformResults[failed];
//                Console.WriteLine($"   - {failed}: {platformResult.ErrorMessage}");
//            }
//        }
//    }

//    #endregion

//    #region Main示例运行器

//    public static async Task Main(string[] args)
//    {
//        Console.WriteLine("Social Media Publisher Examples\n");
//        Console.WriteLine("================================\n");

//        try
//        {
//            // 运行示例（取消注释想要运行的示例）

//            // await Example1_FacebookBasicUsage();
//            // await Example2_InstagramBasicUsage();
//            // await Example3_CrossPlatformPublishing();
//            // await Example4_BatchPublishing();
//            // await Example5_AdvancedFeatures();
//            await Example6_ErrorHandling();

//            Console.WriteLine("\n\nAll examples completed successfully!");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"\n\nExample failed with error:");
//            Console.WriteLine($"Type: {ex.GetType().Name}");
//            Console.WriteLine($"Message: {ex.Message}");
//            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
//        }

//        Console.WriteLine("\nPress any key to exit...");
//        Console.ReadKey();
//    }

//    #endregion
//}