//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SmartMedia.AtqCore;

//public static class PictureBoxExtensions
//{
//    /// <summary>
//    /// 异步加载网络图片到PictureBox
//    /// </summary>
//    public static async Task LoadImageFromUrlAsync(this PictureBox pictureBox, string imageUrl,
//        Image placeholder = null, bool showError = true)
//    {
//        // 确保在UI线程上执行
//        if (pictureBox.InvokeRequired)
//        {
//            pictureBox.Invoke(new Action(async () =>
//            {
//                await LoadImageFromUrlAsync(pictureBox, imageUrl, placeholder, showError);
//            }));
//            return;
//        }

//        try
//        {
//            // 显示占位图
//            if (placeholder != null)
//            {
//                pictureBox.Image?.Dispose(); // 释放旧图片
//                pictureBox.Image = placeholder;
//            }

//            // 加载网络图片
//            var image = await ImageLoader.LoadFromUrlAsync(imageUrl);

//            if (image != null)
//            {
//                pictureBox.Image?.Dispose(); // 释放占位图
//                pictureBox.Image = image;
//            }
//            else if (showError)
//            {
//                pictureBox.Image = CreateErrorImage(pictureBox);
//            }
//        }
//        catch (Exception)
//        {
//            if (showError)
//            {
//                pictureBox.Image = CreateErrorImage(pictureBox);
//            }
//        }
//    }

//    /// <summary>
//    /// 创建错误提示图片
//    /// </summary>
//    private static Image CreateErrorImage(PictureBox pictureBox)
//    {
//        int width = Math.Max(pictureBox.Width, 100);
//        int height = Math.Max(pictureBox.Height, 100);

//        var bitmap = new Bitmap(width, height);
//        using (var g = Graphics.FromImage(bitmap))
//        {
//            g.Clear(Color.LightGray);
//            using (var font = new Font("Microsoft YaHei", 12))
//            using (var brush = new SolidBrush(Color.DarkRed))
//            {
//                var text = "图片加载失败";
//                var size = g.MeasureString(text, font);
//                g.DrawString(text, font, brush,
//                    (width - size.Width) / 2,
//                    (height - size.Height) / 2);
//            }
//        }
//        return bitmap;
//    }
//}

//public static class ImageLoader
//{
//    private static readonly HttpClient _httpClient = new HttpClient();

//    static ImageLoader()
//    {
//        // 初始化HttpClient
//        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
//        _httpClient.Timeout = TimeSpan.FromSeconds(30);
//    }

//    /// <summary>
//    /// 异步加载网络图片
//    /// </summary>
//    /// <param name="imageUrl">图片URL</param>
//    /// <returns>Image对象或null</returns>
//    public static async Task<Image> LoadFromUrlAsync(string imageUrl)
//    {
//        if (string.IsNullOrWhiteSpace(imageUrl))
//            return null;

//        try
//        {
//            var bytes = await _httpClient.GetByteArrayAsync(imageUrl);
//            using (var ms = new MemoryStream(bytes))
//            {
//                return Image.FromStream(ms);
//            }
//        }
//        catch (Exception ex)
//        {
//            // 可以记录日志
//            Console.WriteLine($"加载图片失败: {imageUrl}, 错误: {ex.Message}");
//            return null;
//        }
//    }

//    /// <summary>
//    /// 同步加载网络图片（谨慎使用，会阻塞UI）
//    /// </summary>
//    public static Image LoadFromUrl(string imageUrl)
//    {
//        if (string.IsNullOrWhiteSpace(imageUrl))
//            return null;

//        try
//        {
//            var bytes = _httpClient.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();
//            using (var ms = new MemoryStream(bytes))
//            {
//                return Image.FromStream(ms);
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"加载图片失败: {imageUrl}, 错误: {ex.Message}");
//            return null;
//        }
//    }
//}