using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartMedia.Sites.Article.WeChatArticle;

public class WeChatArticlePublisher
{
    private static readonly HttpClient httpClient = new HttpClient();
    private const string TokenUrl = "https://api.weixin.qq.com/cgi-bin/token";
    private const string UploadImgUrl = "https://api.weixin.qq.com/cgi-bin/media/uploadimg";
    private const string UploadMaterialUrl = "https://api.weixin.qq.com/cgi-bin/material/add_material";
    private const string AddDraftUrl = "https://api.weixin.qq.com/cgi-bin/draft/add";
    private const string SubmitDraftUrl = "https://api.weixin.qq.com/cgi-bin/freepublish/submit";
    private const string MassSendAllUrl = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall";
    private const string BatchGetPublishedUrl = "https://api.weixin.qq.com/cgi-bin/freepublish/batchget";

    private readonly string appId;
    private readonly string appSecret;

    // AccessToken 缓存字段
    private string _cachedAccessToken;
    private DateTime _tokenExpireTime;

    public WeChatArticlePublisher(string appId, string appSecret)
    {
        this.appId = appId;
        this.appSecret = appSecret;
    }

    /// <summary>
    /// 发布图文消息的主要方法 (使用草稿箱)
    /// </summary>
    public async Task<string> PublishArticleAsync(string title, string thumbMediaId = "", string thumbMediaPath = "", string author = "", string digest = "", string content = "", string contentSourceUrl = "", bool isToAll = true)
    {
        try
        {
            // 1. 确保拥有有效的 access_token (使用缓存机制)
            string accessToken = await GetValidAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return "获取 access_token 失败";
            }

            // 2. 上传封面图片获取【永久素材media_id】
            string uploadedThumbMediaId = thumbMediaId;
            if (string.IsNullOrEmpty(uploadedThumbMediaId) && !string.IsNullOrEmpty(thumbMediaPath))
            {
                var uploadResult = await UploadPermanentImageAsync(accessToken, thumbMediaPath);
                if (!string.IsNullOrEmpty(uploadResult.MediaId))
                {
                    uploadedThumbMediaId = uploadResult.MediaId;
                    Debug.WriteLine($"封面图片上传成功，获得永久media_id: {uploadedThumbMediaId}");
                }
                else
                {
                    return $"上传封面图片失败: {uploadResult.ErrMsg}";
                }
            }
            else if (string.IsNullOrEmpty(uploadedThumbMediaId))
            {
                return "创建草稿失败: 必须提供有效的封面图片 media_id 或图片路径。";
            }

            // 3. 创建草稿
            var addDraftResult = await AddDraftAsync(accessToken, title, uploadedThumbMediaId, author, digest, content, contentSourceUrl);
            if (string.IsNullOrEmpty(addDraftResult.MediaId))
            {
                return $"创建草稿失败: {addDraftResult.ErrMsg}";
            }
            string draftMediaId = addDraftResult.MediaId;
            Debug.WriteLine($"草稿创建成功，draftMediaId: {draftMediaId}");

            // 4. 发布草稿
            var submitDraftResult = await SubmitDraftAsync(accessToken, draftMediaId);
            if (string.IsNullOrEmpty(submitDraftResult.PublishId))
            {
                return $"发布草稿失败: {submitDraftResult.ErrMsg}";
            }
            string publishId = submitDraftResult.PublishId;
            string articleMediaId = submitDraftResult.ArticleId;
            Debug.WriteLine($"草稿发布成功，发布任务ID: {publishId}, 文章MediaID: {articleMediaId}");

            // 5. 可选：群发
            if (isToAll)
            {
                var massSendResult = await MassSendArticleAsync(accessToken, articleMediaId, isToAll);
                if (massSendResult.ErrCode == 0)
                {
                    return $"图文消息发布并群发成功！MsgId: {massSendResult.MsgId}";
                }
                else
                {
                    return $"群发消息失败: {massSendResult.ErrCode}, {massSendResult.ErrMsg}";
                }
            }
            else
            {
                return $"图文消息发布成功，发布任务ID: {publishId}。未进行群发。";
            }
        }
        catch (Exception ex)
        {
            return $"发布过程中发生异常: {ex.Message}";
        }
    }

    /// <summary>
    /// 获取有效的 Access Token (带缓存和自动刷新)
    /// </summary>
    private async Task<string> GetValidAccessTokenAsync()
    {
        // 如果缓存为空，或者已过期（预留5分钟缓冲），则重新获取
        if (string.IsNullOrEmpty(_cachedAccessToken) || DateTime.Now >= _tokenExpireTime)
        {
            var tokenResult = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(tokenResult.AccessToken))
            {
                Debug.WriteLine($"[ERROR] 无法获取有效的 access_token: {tokenResult.ErrMsg}");
                return null;
            }
            _cachedAccessToken = tokenResult.AccessToken;
            // 设置过期时间，tokenResult.ExpiresIn 是7200秒，我们提前300秒刷新
            _tokenExpireTime = DateTime.Now.AddSeconds(tokenResult.ExpiresIn - 300);
            Debug.WriteLine($"[INFO] 已刷新 access_token，将于 {_tokenExpireTime} 过期");
        }
        return _cachedAccessToken;
    }

    /// <summary>
    /// 获取 Access Token (原始API调用)
    /// </summary>
    private async Task<AccessTokenResponse> GetAccessTokenAsync()
    {
        string url = $"{TokenUrl}?grant_type=client_credential&appid={appId}&secret={appSecret}";
        var response = await httpClient.GetStringAsync(url);
        return JsonConvert.DeserializeObject<AccessTokenResponse>(response);
    }

    /// <summary>
    /// 上传永久图片素材 (使用 HttpWebRequest 兼容微信服务器)
    /// </summary>
    private async Task<UploadPermanentImageResponse> UploadPermanentImageAsync(string accessToken, string imagePath)
    {
        string url = $"{UploadMaterialUrl}?access_token={accessToken}&type=image";
        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

        // 1. 读取图片数据
        byte[] fileData;
        string fileName;
        try
        {
            fileData = await File.ReadAllBytesAsync(imagePath);
            fileName = Path.GetFileName(imagePath);
            Debug.WriteLine($"[INFO] 读取文件成功: {imagePath}, 大小: {fileData.Length} 字节");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] 读取文件失败: {ex.Message}");
            throw;
        }

        // 2. 手动构建符合微信标准的 multipart/form-data 请求体
        using (var requestStream = new MemoryStream())
        {
            // 根据文件扩展名确定 Content-Type
            string fileExtension = Path.GetExtension(imagePath).ToLowerInvariant().Replace(".", "");
            string contentType = fileExtension switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
            Debug.WriteLine($"[INFO] 文件 {fileName} 的 Content-Type 设置为: {contentType}");

            // 构建请求体
            string boundaryStart = $"--{boundary}\r\n";
            string headerTemplate = $"Content-Disposition: form-data; name=\"media\"; filename=\"{fileName}\"\r\n" +
                                    $"Content-Type: {contentType}\r\n\r\n";
            string boundaryEnd = $"\r\n--{boundary}--\r\n";

            // 写入请求体
            var boundaryStartBytes = Encoding.UTF8.GetBytes(boundaryStart);
            var headerBytes = Encoding.UTF8.GetBytes(headerTemplate);
            var boundaryEndBytes = Encoding.UTF8.GetBytes(boundaryEnd);

            requestStream.Write(boundaryStartBytes, 0, boundaryStartBytes.Length);
            requestStream.Write(headerBytes, 0, headerBytes.Length);
            requestStream.Write(fileData, 0, fileData.Length);
            requestStream.Write(boundaryEndBytes, 0, boundaryEndBytes.Length);

            // 3. 创建 HttpWebRequest 并发送
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.ContentLength = requestStream.Length;

            // 发送请求
            using (Stream stream = request.GetRequestStream())
            {
                requestStream.Position = 0;
                await requestStream.CopyToAsync(stream);
            }

            // 4. 获取响应
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseContent = await reader.ReadToEndAsync();
                    Debug.WriteLine($"[DEBUG] 微信服务器响应: {responseContent}");
                    return JsonConvert.DeserializeObject<UploadPermanentImageResponse>(responseContent);
                }
            }
            catch (WebException ex)
            {
                // 处理网络异常，尝试读取错误响应
                if (ex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)ex.Response)
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorContent = await reader.ReadToEndAsync();
                        Debug.WriteLine($"[ERROR] 上传失败，服务器返回: {errorContent}");
                    }
                }
                throw;
            }
        }
    }

    /// <summary>
    /// 新增草稿
    /// </summary>
    private async Task<AddDraftResponse> AddDraftAsync(string accessToken, string title, string thumbMediaId, string author, string digest, string content, string contentSourceUrl)
    {
        var article = new
        {
            title = title,
            thumb_media_id = thumbMediaId,
            author = author,
            digest = digest,
            show_cover_pic = 0,
            content = content,
            content_source_url = contentSourceUrl
        };
        var requestData = new { articles = new[] { article } };
        string jsonBody = JsonConvert.SerializeObject(requestData);
        var contentData = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        string url = $"{AddDraftUrl}?access_token={accessToken}";
        var response = await httpClient.PostAsync(url, contentData);
        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<AddDraftResponse>(responseContent);
    }

    /// <summary>
    /// 发布草稿
    /// </summary>
    private async Task<SubmitDraftResponse> SubmitDraftAsync(string accessToken, string draftMediaId)
    {
        var requestData = new { media_id = draftMediaId };
        string jsonBody = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        string url = $"{SubmitDraftUrl}?access_token={accessToken}";
        var response = await httpClient.PostAsync(url, content);
        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SubmitDraftResponse>(responseContent);
    }

    /// <summary>
    /// 群发图文消息
    /// </summary>
    private async Task<MassSendResponse> MassSendArticleAsync(string accessToken, string articleMediaId, bool isToAll)
    {
        var filter = new { is_to_all = isToAll };
        var msgData = new
        {
            filter = filter,
            mpnews = new { media_id = articleMediaId },
            msgtype = "mpnews",
            send_ignore_reprint = 0
        };
        string jsonBody = JsonConvert.SerializeObject(msgData);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        string url = $"{MassSendAllUrl}?access_token={accessToken}";
        var response = await httpClient.PostAsync(url, content);
        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<MassSendResponse>(responseContent);
    }

    /// <summary>
    /// 获取已发布文章列表 (使用发布能力接口)
    /// </summary>
    public async Task<GetPublishedListResponse> GetPublishedListAsync(int offset = 0, int count = 20)
    {
        if (count < 1 || count > 20) throw new ArgumentException("Count must be between 1 and 20.", nameof(count));
        try
        {
            string accessToken = await GetValidAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("获取 access_token 失败");
            }

            var requestData = new { offset = offset, count = count, no_content = 0 };
            string jsonBody = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            string url = $"{BatchGetPublishedUrl}?access_token={accessToken}";
            var response = await httpClient.PostAsync(url, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<GetPublishedListResponse>(responseContent);
            if (result.ErrCode.HasValue && result.ErrCode.Value != 0)
            {
                throw new InvalidOperationException($"获取已发布列表失败: {result.ErrCode.Value}, {result.ErrMsg}");
            }
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"获取已发布文章列表过程中发生异常: {ex.Message}", ex);
        }
    }

    // ================ 数据模型定义区域 ================

    public class AccessTokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }
        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
        [JsonProperty("errcode")] public int? ErrCode { get; set; }
        [JsonProperty("errmsg")] public string ErrMsg { get; set; }
    }

    public class UploadPermanentImageResponse
    {
        [JsonProperty("media_id")] public string MediaId { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("errcode")] public int? ErrCode { get; set; }
        [JsonProperty("errmsg")] public string ErrMsg { get; set; }
    }

    public class AddDraftResponse
    {
        [JsonProperty("media_id")] public string MediaId { get; set; }
        [JsonProperty("errcode")] public int? ErrCode { get; set; }
        [JsonProperty("errmsg")] public string ErrMsg { get; set; }
    }

    public class SubmitDraftResponse
    {
        [JsonProperty("publish_id")] public string PublishId { get; set; }
        [JsonProperty("article_id")] public string ArticleId { get; set; }
        [JsonProperty("errcode")] public int? ErrCode { get; set; }
        [JsonProperty("errmsg")] public string ErrMsg { get; set; }
    }

    public class MassSendResponse
    {
        [JsonProperty("errcode")] public int ErrCode { get; set; }
        [JsonProperty("errmsg")] public string ErrMsg { get; set; }
        [JsonProperty("msg_id")] public long MsgId { get; set; }
        [JsonProperty("msg_data_id")] public long MsgDataId { get; set; }
    }

    // 获取已发布列表响应模型
    public class GetPublishedListResponse
    {
        [JsonProperty("total_count")] public int TotalCount { get; set; }
        [JsonProperty("item_count")] public int ItemCount { get; set; }
        [JsonProperty("item")] public List<PublishedItem> Items { get; set; }
        [JsonProperty("errcode")] public int? ErrCode { get; set; }
        [JsonProperty("errmsg")] public string ErrMsg { get; set; }
    }

    public class PublishedItem
    {
        [JsonProperty("publish_id")] public string PublishId { get; set; }
        [JsonProperty("article_id")] public string ArticleId { get; set; }
        [JsonProperty("content")] public PublishedContent Content { get; set; }
        [JsonProperty("create_time")] public long CreateTimeUnixTimestamp { get; set; }
        [JsonProperty("update_time")] public long UpdateTimeUnixTimestamp { get; set; }
    }

    public class PublishedContent
    {
        [JsonProperty("news_item")] public List<ArticleInfo> NewsItem { get; set; }
        [JsonProperty("create_time")] public long CreateTimeUnixTimestamp { get; set; }
        [JsonProperty("update_time")] public long UpdateTimeUnixTimestamp { get; set; }
    }

    public class ArticleInfo
    {
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("author")] public string Author { get; set; }
        [JsonProperty("digest")] public string Digest { get; set; }
        [JsonProperty("content")] public string Content { get; set; }
        [JsonProperty("content_source_url")] public string ContentSourceUrl { get; set; }
        [JsonProperty("thumb_media_id")] public string ThumbMediaId { get; set; }
        [JsonProperty("show_cover_pic")] public int ShowCoverPic { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("thumb_url")] public string ThumbUrl { get; set; }
        [JsonProperty("need_open_comment")] public int NeedOpenComment { get; set; }
        [JsonProperty("only_fans_can_comment")] public int OnlyFansCanComment { get; set; }
    }
}