using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartMedia.AtqCore.SocialMedia;

#region 通用数据模型

/// <summary>
/// 社交媒体API响应基类
/// </summary>
public class SocialMediaApiResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("error")]
    public ApiError? Error { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}

/// <summary>
/// API错误信息
/// </summary>
public class ApiError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("error_subcode")]
    public int ErrorSubcode { get; set; }
}

/// <summary>
/// 媒体类型枚举
/// </summary>
public enum MediaType
{
    Image,
    Video,
    Carousel,
    Story,
    Reel
}

/// <summary>
/// 发布状态
/// </summary>
public enum PublishStatus
{
    Draft,
    Published,
    Scheduled,
    Failed
}

/// <summary>
/// 上传进度信息
/// </summary>
public class UploadProgress
{
    public long BytesUploaded { get; set; }
    public long TotalBytes { get; set; }
    public double PercentComplete => TotalBytes > 0 ? (double)BytesUploaded / TotalBytes * 100 : 0;
    public string? CurrentPhase { get; set; }
}

#endregion

#region 异常类

/// <summary>
/// 社交媒体API异常基类
/// </summary>
public class SocialMediaApiException : Exception
{
    public int? ErrorCode { get; }
    public string? Platform { get; }

    public SocialMediaApiException(string message, string? platform = null) 
        : base(message) 
    {
        Platform = platform;
    }

    public SocialMediaApiException(string message, Exception innerException, string? platform = null)
        : base(message, innerException)
    {
        Platform = platform;
    }

    public SocialMediaApiException(string message, int errorCode, string? platform = null, Exception? innerException = null)
        : base($"{message} (Error Code: {errorCode})", innerException)
    {
        ErrorCode = errorCode;
        Platform = platform;
    }
}

#endregion

#region 抽象基类

/// <summary>
/// 社交媒体发布器抽象基类
/// </summary>
public abstract class SocialMediaPublisherBase : IDisposable
{
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonOptions;
    protected readonly int _chunkSize = 5 * 1024 * 1024; // 5MB 默认分块大小
    
    protected static readonly Dictionary<string, string> CommonMimeTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" },
        { ".mp4", "video/mp4" },
        { ".mov", "video/quicktime" },
        { ".avi", "video/x-msvideo" },
        { ".mkv", "video/x-matroska" }
    };

    /// <summary>
    /// 平台名称
    /// </summary>
    public abstract string PlatformName { get; }

    /// <summary>
    /// API基础URL
    /// </summary>
    protected abstract string ApiBaseUrl { get; }

    protected SocialMediaPublisherBase(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.Timeout = TimeSpan.FromMinutes(30); // 视频上传需要更长时间

        _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    #region 验证方法

    /// <summary>
    /// 验证文本长度
    /// </summary>
    protected void ValidateText(string text, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException($"{fieldName} cannot be empty", fieldName);

        if (text.Length > maxLength)
            throw new ArgumentException(
                $"{fieldName} too long ({text.Length} > {maxLength} chars)", 
                fieldName);
    }

    /// <summary>
    /// 验证文件存在性和类型
    /// </summary>
    protected void ValidateFile(string filePath, string[] allowedExtensions, string fileType = "File")
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException($"{fileType} path cannot be empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"{fileType} not found: {filePath}");

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException(
                $"Unsupported {fileType.ToLower()} type: {extension}. " +
                $"Allowed: {string.Join(", ", allowedExtensions)}");
        }

        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
            throw new ArgumentException($"{fileType} is empty: {filePath}");
    }

    /// <summary>
    /// 验证文件大小
    /// </summary>
    protected void ValidateFileSize(string filePath, long maxSizeBytes, string fileType = "File")
    {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length > maxSizeBytes)
        {
            var maxSizeMB = maxSizeBytes / (1024.0 * 1024.0);
            var actualSizeMB = fileInfo.Length / (1024.0 * 1024.0);
            throw new ArgumentException(
                $"{fileType} too large ({actualSizeMB:F2}MB > {maxSizeMB:F2}MB): {filePath}");
        }
    }

    /// <summary>
    /// 验证视频时长
    /// </summary>
    protected virtual void ValidateVideoDuration(string videoPath, double minSeconds, double maxSeconds)
    {
        // 子类可以实现具体的视频时长检测逻辑
        // 这里提供基础框架
    }

    #endregion

    #region 文件处理

    /// <summary>
    /// 创建文件内容
    /// </summary>
    protected async Task<ByteArrayContent> CreateFileContentAsync(string filePath, string formName)
    {
        var bytes = await File.ReadAllBytesAsync(filePath);
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        var fileContent = new ByteArrayContent(bytes);

        if (CommonMimeTypes.TryGetValue(extension, out var mimeType))
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
        }
        else
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        }

        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = formName,
            FileName = Path.GetFileName(filePath)
        };

        return fileContent;
    }

    /// <summary>
    /// 分块上传文件
    /// </summary>
    protected async Task UploadFileInChunksAsync(
        string filePath, 
        string uploadUrl, 
        IProgress<UploadProgress>? progress = null,
        Dictionary<string, string>? additionalHeaders = null)
    {
        using var fileStream = new FileStream(
            filePath, 
            FileMode.Open, 
            FileAccess.Read, 
            FileShare.Read, 
            4096, 
            useAsync: true);

        var fileSize = fileStream.Length;
        var buffer = new byte[_chunkSize];
        int bytesRead;
        long startOffset = 0;

        while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
        {
            var chunk = bytesRead < _chunkSize ? buffer[..bytesRead] : buffer;

            using var chunkContent = new ByteArrayContent(chunk);
            chunkContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // 添加Content-Range头部
            var endOffset = startOffset + bytesRead - 1;
            chunkContent.Headers.Add("Content-Range", $"bytes {startOffset}-{endOffset}/{fileSize}");

            // 添加额外的头部
            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    chunkContent.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.PostAsync(uploadUrl, chunkContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new SocialMediaApiException(
                    $"Chunk upload failed: {response.StatusCode} - {errorContent}",
                    PlatformName);
            }

            startOffset += bytesRead;

            // 报告进度
            progress?.Report(new UploadProgress
            {
                BytesUploaded = startOffset,
                TotalBytes = fileSize,
                CurrentPhase = "Uploading"
            });
        }
    }

    /// <summary>
    /// 获取文件的MIME类型
    /// </summary>
    protected string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return CommonMimeTypes.TryGetValue(extension, out var mimeType) 
            ? mimeType 
            : "application/octet-stream";
    }

    #endregion

    #region HTTP请求处理

    /// <summary>
    /// GET请求
    /// </summary>
    protected async Task<T> GetJsonAsync<T>(string url, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var fullUrl = BuildUrlWithParams(url, queryParams);
            var response = await _httpClient.GetAsync(fullUrl);
            return await HandleResponseAsync<T>(response);
        }
        catch (Exception ex) when (ex is not SocialMediaApiException)
        {
            throw new SocialMediaApiException(
                $"GET request failed: {ex.Message}", 
                ex, 
                PlatformName);
        }
    }

    /// <summary>
    /// POST JSON请求
    /// </summary>
    protected async Task<T> PostJsonAsync<T>(string url, object payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            return await HandleResponseAsync<T>(response);
        }
        catch (Exception ex) when (ex is not SocialMediaApiException)
        {
            throw new SocialMediaApiException(
                $"POST request failed: {ex.Message}", 
                ex, 
                PlatformName);
        }
    }

    /// <summary>
    /// POST Form请求
    /// </summary>
    protected async Task<T> PostFormAsync<T>(string url, MultipartFormDataContent form)
    {
        try
        {
            var response = await _httpClient.PostAsync(url, form);
            return await HandleResponseAsync<T>(response);
        }
        catch (Exception ex) when (ex is not SocialMediaApiException)
        {
            throw new SocialMediaApiException(
                $"POST form request failed: {ex.Message}", 
                ex, 
                PlatformName);
        }
    }

    /// <summary>
    /// DELETE请求
    /// </summary>
    protected async Task<T> DeleteAsync<T>(string url, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var fullUrl = BuildUrlWithParams(url, queryParams);
            var response = await _httpClient.DeleteAsync(fullUrl);
            return await HandleResponseAsync<T>(response);
        }
        catch (Exception ex) when (ex is not SocialMediaApiException)
        {
            throw new SocialMediaApiException(
                $"DELETE request failed: {ex.Message}", 
                ex, 
                PlatformName);
        }
    }

    /// <summary>
    /// 处理HTTP响应
    /// </summary>
    protected virtual async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, content);
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            if (result == null)
                throw new SocialMediaApiException("Failed to deserialize response", PlatformName);

            return result;
        }
        catch (JsonException ex)
        {
            throw new SocialMediaApiException(
                $"Failed to parse response: {ex.Message}", 
                ex, 
                PlatformName);
        }
    }

    /// <summary>
    /// 处理错误响应（子类可以重写以实现平台特定的错误处理）
    /// </summary>
    protected virtual async Task HandleErrorResponseAsync(HttpResponseMessage response, string content)
    {
        try
        {
            var errorResponse = JsonSerializer.Deserialize<SocialMediaApiResponse>(content, _jsonOptions);
            if (errorResponse?.Error != null)
            {
                throw new SocialMediaApiException(
                    $"{PlatformName} API Error: {errorResponse.Error.Message}",
                    errorResponse.Error.Code,
                    PlatformName);
            }
        }
        catch (JsonException)
        {
            // 如果不是有效的JSON错误响应，抛出原始错误
        }

        throw new SocialMediaApiException(
            $"HTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}",
            PlatformName);
    }

    /// <summary>
    /// 构建带查询参数的URL
    /// </summary>
    protected string BuildUrlWithParams(string url, Dictionary<string, string>? queryParams)
    {
        if (queryParams == null || queryParams.Count == 0)
            return url;

        var queryString = string.Join("&", queryParams.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        return url.Contains('?') ? $"{url}&{queryString}" : $"{url}?{queryString}";
    }

    #endregion

    #region 抽象方法 - 子类必须实现

    /// <summary>
    /// 测试API连接
    /// </summary>
    public abstract Task<bool> TestConnectionAsync();

    /// <summary>
    /// 获取账户信息
    /// </summary>
    public abstract Task<object> GetAccountInfoAsync();

    #endregion

    #region 工具方法

    /// <summary>
    /// 重试机制
    /// </summary>
    protected async Task<T> RetryAsync<T>(
        Func<Task<T>> operation, 
        int maxRetries = 3, 
        int delayMilliseconds = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                await Task.Delay(delayMilliseconds * (i + 1)); // 指数退避
            }
        }

        // 最后一次尝试，不捕获异常
        return await operation();
    }

    /// <summary>
    /// 日志记录（子类可以重写）
    /// </summary>
    protected virtual void LogInfo(string message)
    {
        Console.WriteLine($"[{PlatformName}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - INFO: {message}");
    }

    /// <summary>
    /// 错误日志记录（子类可以重写）
    /// </summary>
    protected virtual void LogError(string message, Exception? ex = null)
    {
        Console.WriteLine($"[{PlatformName}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - ERROR: {message}");
        if (ex != null)
        {
            Console.WriteLine($"Exception: {ex}");
        }
    }

    #endregion

    #region IDisposable实现

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
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

#endregion
