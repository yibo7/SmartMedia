 
//namespace SmartMedia.AtqCore.WeChatArticle;

//// --- Usage Examples ---
//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        string appId = "YOUR_APP_ID"; // 替换为你的 AppID
//        string appSecret = "YOUR_APP_SECRET"; // 替换为你的 AppSecret

//        var publisher = new WeChatArticlePublisher(appId, appSecret);

//        // --- Example 1: Publish an Article using Draft ---
//        Console.WriteLine("--- Publishing Article via Draft ---");
//        string title = "测试图文消息 (草稿箱)";
//        string author = "Your Name";
//        string digest = "这是通过草稿箱发布的测试文章的摘要。";
//        string content = "<p>这是通过草稿箱发布的测试文章的正文内容。</p><p>支持HTML格式。</p>";
//        string contentSourceUrl = "https://www.example.com/article/new_draft_123"; // 原文链接
//        string thumbImagePath = @"C:\path\to\your\cover_image_new.jpg"; // 替换为你本地封面图片的路径

//        string result = await publisher.PublishArticleAsync(
//            title: title,
//            thumbMediaPath: thumbImagePath, // 提供本地图片路径，代码会自动上传
//            author: author,
//            digest: digest,
//            content: content,
//            contentSourceUrl: contentSourceUrl,
//            isToAll: true // 设置为 true 表示群发给所有用户
//        );
//        Console.WriteLine(result);

//        Console.WriteLine("\n--- Getting Published Articles List ---");
//        // --- Example 2: Get Published Articles List ---
//        try
//        {
//            var listResponse = await publisher.GetPublishedListAsync(offset: 0, count: 5); // 获取前5篇已发布文章

//            Console.WriteLine( $"共找到 {listResponse.TotalCount} 篇已发布文章，本次返回 {listResponse.ItemCount} 篇。");

//            foreach (var item in listResponse.Items)
//            {
//                // 每个 item.Content.NewsItem 是一个数组，即使只有一篇文章
//                var article = item.Content.NewsItem[0];
//                Console.WriteLine( $"\nPublish ID: {item.PublishId}");
//                Console.WriteLine( $"Article ID (for Mass Send): {item.ArticleId}");
//                Console.WriteLine( $"Title: {article.Title}");
//                Console.WriteLine( $"Author: {article.Author}");
//                Console.WriteLine( $"Digest: {article.Digest}");
//                Console.WriteLine( $"Create Time: {new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(item.CreateTimeUnixTimestamp).ToLocalTime()}");
//                Console.WriteLine( $"Update Time: {new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(item.UpdateTimeUnixTimestamp).ToLocalTime()}");
//                Console.WriteLine( $"URL: {article.Url}");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine( $"获取已发布文章列表失败: {ex.Message}");
//        }
//    }
//}