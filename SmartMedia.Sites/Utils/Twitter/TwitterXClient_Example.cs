//// ══════════════════════════════════════════════════════════
//// TwitterXClient 使用示例
//// ══════════════════════════════════════════════════════════

//using System;
//using System.Threading.Tasks;

//class Program
//{
//    static async Task Main()
//    {
//        // ① 初始化客户端
//        using var client = new TwitterXClient(
//            apiKey:       "your_api_key",
//            apiSecret:    "your_api_secret",
//            accessToken:  "your_access_token",
//            accessSecret: "your_access_token_secret"
//        );

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 获取我的用户信息 & ID
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var profile = await client.GetMyProfileAsync();
//        string myUserId = profile.GetProperty("data").GetProperty("id").GetString()!;
//        Console.WriteLine($"用户ID: {myUserId}");

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 发布纯文字推文
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var tweet = await client.PostTweetAsync("Hello from C# 🚀");
//        Console.WriteLine($"推文ID: {tweet.TweetId}");

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 发布图文推文
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var imageTweet = await client.PostTweetWithImagesAsync(
//            text:       "这是一条图文推文 🖼️",
//            imagePaths: new[] { "photo1.jpg", "photo2.png" }
//        );
//        Console.WriteLine($"图文推文ID: {imageTweet.TweetId}");

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 获取我发布的推文列表
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var myTweets = await client.GetUserTweetsAsync(
//            userId:       myUserId,
//            maxResults:   20,
//            onlyWithMedia: false  // true = 只获取含图片的推文
//        );

//        Console.WriteLine($"\n共获取 {myTweets.TotalCount} 条推文：");
//        foreach (var t in myTweets.Tweets)
//        {
//            Console.WriteLine($"[{t.CreatedAt}] {t.Text}");
//            Console.WriteLine($"  ❤️ {t.LikeCount}  🔁 {t.RetweetCount}  💬 {t.ReplyCount}");

//            // 打印关联的图片URL
//            foreach (var key in t.MediaKeys)
//            {
//                if (myTweets.MediaMap.TryGetValue(key, out var media))
//                    Console.WriteLine($"  🖼️ [{media.Type}] {media.Url}");
//            }
//        }

//        // 翻页
//        if (myTweets.NextPageToken != null)
//        {
//            var page2 = await client.GetUserTweetsAsync(
//                userId:          myUserId,
//                maxResults:      20,
//                paginationToken: myTweets.NextPageToken
//            );
//            Console.WriteLine($"第二页: {page2.TotalCount} 条");
//        }

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 获取账号统计数据
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var userMetrics = await client.GetUserMetricsAsync(myUserId);
//        Console.WriteLine($"\n📊 账号统计");
//        Console.WriteLine($"粉丝数:  {userMetrics.FollowersCount:N0}");
//        Console.WriteLine($"关注数:  {userMetrics.FollowingCount:N0}");
//        Console.WriteLine($"推文数:  {userMetrics.TweetCount:N0}");
//        Console.WriteLine($"列表数:  {userMetrics.ListedCount:N0}");

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 获取单条推文统计
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var tweetMetrics = await client.GetTweetMetricsAsync(tweet.TweetId);
//        Console.WriteLine($"\n📈 推文统计");
//        Console.WriteLine($"点赞:   {tweetMetrics.LikeCount}");
//        Console.WriteLine($"转推:   {tweetMetrics.RetweetCount}");
//        Console.WriteLine($"回复:   {tweetMetrics.ReplyCount}");
//        Console.WriteLine($"引用:   {tweetMetrics.QuoteCount}");
//        Console.WriteLine($"曝光量: {tweetMetrics.ImpressionCount}");

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 搜索推文
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var searchResult = await client.SearchTweetsAsync(
//            query:      "#AI lang:zh",
//            maxResults: 20
//        );
//        Console.WriteLine($"\n🔍 搜索到 {searchResult.TotalCount} 条推文");

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 回复推文
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        var reply = await client.ReplyToTweetAsync(
//            text:          "感谢你的分享！",
//            replyToTweetId: tweet.TweetId
//        );
//        Console.WriteLine($"回复成功: {reply.TweetId}");

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 点赞 & 转推
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        await client.LikeTweetAsync(myUserId, tweet.TweetId);
//        await client.RetweetAsync(myUserId, tweet.TweetId);

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 发送私信
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        await client.SendDirectMessageAsync(
//            targetUserId: "target_user_id",
//            text:         "你好，这是一条私信！"
//        );

//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        // 删除推文
//        // ━━━━━━━━━━━━━━━━━━━━━━━━
//        bool deleted = await client.DeleteTweetAsync(tweet.TweetId);
//        Console.WriteLine($"删除结果: {deleted}");
//    }
//}
