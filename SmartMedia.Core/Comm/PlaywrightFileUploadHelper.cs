namespace SmartMedia.Core.Comm;

using Microsoft.Playwright;
using System.Diagnostics;
using System.Text;
/// <summary>/// 
// 使用视频上传  await uploadHelper.UploadVideoFile("C:\\videos\\demo.mp4", "#video-upload-btn");
// 使用音频上传  await uploadHelper.UploadAudioFile("C:\\audio\\music.mp3", "#audio-upload-btn");
// 上传文件   await uploadHelper.UploadImgFile("test.jpg", "image", ".upload-button");
/// </summary>
public class PlaywrightFileUploadHelper
{
    private readonly IPage _page;

    public PlaywrightFileUploadHelper(IPage page)
    {
        _page = page;
    }

    private void DebugLog(string log)
    {
        Debug.WriteLine($"[PlaywrightFileUploadHelper] {DateTime.Now:HH:mm:ss.fff} - {log}");
    }

    // ================ 路径辅助方法 ================

    /// <summary>
    /// 修复文件路径，转换为绝对路径
    /// </summary>
    private string FixFilePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        // 如果已经是绝对路径，直接返回
        if (Path.IsPathRooted(path))
            return path;

        // 清理路径格式（移除 .\ 和双反斜杠）
        path = path.Replace(".\\", "").Replace("./", "")
                   .Replace("\\\\", "\\").Replace("//", "/");

        // 组合绝对路径
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(basePath, path);
    }

    /// <summary>
    /// 验证文件是否存在并返回绝对路径
    /// </summary>
    private async Task<string> ValidateAndGetPath(string filePath)
    {
        string fixedPath = FixFilePath(filePath);
        DebugLog($"原始路径: {filePath}");
        DebugLog($"修复后路径: {fixedPath}");

        if (!File.Exists(fixedPath))
        {
            DebugLog($"文件不存在: {fixedPath}");

            // 尝试查找当前目录下的文件
            string fileName = Path.GetFileName(fixedPath);
            string currentDir = Directory.GetCurrentDirectory();
            string altPath = Path.Combine(currentDir, fileName);

            if (File.Exists(altPath))
            {
                DebugLog($"在当前目录找到文件: {altPath}");
                return altPath;
            }

            return null;
        }

        return fixedPath;
    }

    // ================ 视频上传方法 ================

    /// <summary>
    /// 专门上传视频文件的方法
    /// </summary>
    public async Task<bool> UploadVideoFile(string videoFilePath, string preClickSelector = null, int maxRetries = 2)
    {
        try
        {
            // 验证文件路径
            string validPath = await ValidateAndGetPath(videoFilePath);
            if (validPath == null)
            {
                return false;
            }

            DebugLog($"开始上传视频文件: {Path.GetFileName(validPath)}");

            // 验证文件格式
            var validVideoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm", ".wmv", ".flv", ".m4v" };
            var extension = Path.GetExtension(validPath).ToLower();
            if (!validVideoExtensions.Contains(extension))
            {
                DebugLog($"不支持的视频格式: {extension}");
            }

            // 获取文件大小
            var fileInfo = new FileInfo(validPath);
            DebugLog($"视频文件大小: {fileInfo.Length / 1024 / 1024} MB");

            // 如果需要，先点击触发元素
            if (!string.IsNullOrEmpty(preClickSelector))
            {
                await ClickTriggerElement(preClickSelector);
                await _page.WaitForTimeoutAsync(3000);
            }

            // 构建视频上传选择器
            string selector = BuildVideoSelector();
            DebugLog($"使用选择器: {selector}");

            // 重试机制
            var retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    DebugLog($"尝试上传视频 (重试 {retryCount + 1}/{maxRetries})");

                    var locator = _page.Locator(selector);

                    // 等待元素出现并稳定
                    await locator.WaitForAsync(new LocatorWaitForOptions
                    {
                        State = WaitForSelectorState.Attached,
                        Timeout = 10000
                    });

                    await _page.WaitForTimeoutAsync(1000);

                    // 上传文件
                    await locator.SetInputFilesAsync(validPath);

                    // 等待上传开始
                    await _page.WaitForTimeoutAsync(3000);

                    // 检查上传是否成功
                    var success = await CheckVideoUploadSuccess(validPath);

                    if (success)
                    {
                        DebugLog("视频上传成功！");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    DebugLog($"视频上传失败: {ex.Message}");
                }

                retryCount++;
                if (retryCount < maxRetries)
                {
                    DebugLog($"等待2秒后重试...");
                    await _page.WaitForTimeoutAsync(2000);
                }
            }

            DebugLog("视频上传失败，所有重试次数已用完");
            return false;
        }
        catch (Exception ex)
        {
            DebugLog($"视频上传过程中发生错误: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 构建视频上传选择器
    /// </summary>
    private string BuildVideoSelector()
    {
        return "input[type='file'][accept*='video'], input[accept*='mp4'], input[accept*='mov']";
    }

    // ================ 音频上传方法 ================

    /// <summary>
    /// 专门上传音频文件的方法
    /// </summary>
    public async Task<bool> UploadAudioFile(string audioFilePath, string preClickSelector = null)
    {
        try
        {
            // 验证文件路径
            string validPath = await ValidateAndGetPath(audioFilePath);
            if (validPath == null)
            {
                return false;
            }

            DebugLog($"开始上传音频文件: {Path.GetFileName(validPath)}");

            // 验证文件格式
            var validAudioExtensions = new[] { ".mp3", ".wav", ".ogg", ".aac", ".flac", ".m4a", ".wma" };
            var extension = Path.GetExtension(validPath).ToLower();
            if (!validAudioExtensions.Contains(extension))
            {
                DebugLog($"不支持的音频格式: {extension}");
            }

            // 如果需要，先点击触发元素
            if (!string.IsNullOrEmpty(preClickSelector))
            {
                await ClickTriggerElement(preClickSelector);
                await _page.WaitForTimeoutAsync(2000);
            }

            // 构建音频上传选择器
            string selector = BuildAudioSelector();
            DebugLog($"使用选择器: {selector}");

            var locator = _page.Locator(selector);

            // 等待元素出现并稳定
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = 10000
            });

            await _page.WaitForTimeoutAsync(1000);

            // 上传文件
            await locator.SetInputFilesAsync(validPath);

            // 等待上传完成
            await _page.WaitForTimeoutAsync(3000);

            DebugLog("音频上传成功！");
            return true;
        }
        catch (Exception ex)
        {
            DebugLog($"音频上传过程中发生错误: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 构建音频上传选择器
    /// </summary>
    private string BuildAudioSelector()
    {
        return "input[type='file'][accept*='audio'], input[accept*='mp3'], input[accept*='wav']";
    }

    // ================ 图片上传方法（核心修复） ================

    /// <summary>
    /// 通用方法：上传文件到页面（修复版本）
    /// </summary>
    public async Task<bool> UploadImgFile(string filePath, string fileTypeHint = null, string preClickSelector = null)
    {
        try
        {
            string validPath = filePath;
            // 验证文件路径
            //string validPath = await ValidateAndGetPath(filePath);
            //if (validPath == null)
            //{
            //    return false;
            //}

            DebugLog($"开始上传文件: {Path.GetFileName(validPath)}");

            // 1. 如果需要，先点击触发元素
            if (!string.IsNullOrEmpty(preClickSelector))
            {
                await ClickTriggerElement(preClickSelector);
                await _page.WaitForTimeoutAsync(2000);
            }

            // 2. 构建合适的选择器
            string selector = BuildFileInputSelector(validPath, fileTypeHint);
            DebugLog($"使用选择器: {selector}");

            // 3. 使用 Locator 并等待元素稳定（关键修复）
            var fileInputLocator = _page.Locator(selector);

            await fileInputLocator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = 10000
            });

            // 确保元素完全稳定
            await _page.WaitForTimeoutAsync(1000);

            // 4. 上传文件（Locator 会自动处理元素状态）
            await fileInputLocator.SetInputFilesAsync(validPath);

            // 5. 等待上传完成
            await _page.WaitForTimeoutAsync(3000);

            DebugLog("文件上传成功");
            return true;
        }
        catch (Exception ex)
        {
            DebugLog($"文件上传过程中发生错误: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 根据文件类型构建合适的选择器
    /// </summary>
    private string BuildFileInputSelector(string filePath, string fileTypeHint)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        string selector = "input[type='file']";

        // 根据文件类型提示添加过滤
        if (!string.IsNullOrEmpty(fileTypeHint))
        {
            switch (fileTypeHint.ToLower())
            {
                case "image":
                    selector = "input[type='file'][accept*='image']";
                    break;
                case "video":
                    selector = "input[type='file'][accept*='video']";
                    break;
                case "audio":
                    selector = "input[type='file'][accept*='audio']";
                    break;
            }
        }

        // 根据具体扩展名进一步精确
        if (extension == ".jpg" || extension == ".jpeg")
        {
            selector = "input[accept='image/jpeg,image/jpg,image/png'], " + selector;
        }
        else if (extension == ".png")
        {
            selector = "input[accept*='image/png'], " + selector;
        }
        else if (extension == ".mp4")
        {
            selector = "input[accept*='video/mp4'], " + selector;
        }
        else if (extension == ".mp3")
        {
            selector = "input[accept*='audio/mp3'], input[accept*='audio/mpeg'], " + selector;
        }

        return selector;
    }

    // ================ 媒体文件检查方法 ================

    private async Task<bool> CheckVideoUploadSuccess(string videoFilePath)
    {
        try
        {
            var fileName = Path.GetFileName(videoFilePath);

            // 检查进度条
            var progressBars = await _page.QuerySelectorAllAsync(".progress-bar, .upload-progress, .progress");
            if (progressBars.Count > 0)
            {
                await _page.WaitForTimeoutAsync(5000);
                var remainingBars = await _page.QuerySelectorAllAsync(".progress-bar, .upload-progress, .progress:not([style*='display: none'])");
                if (remainingBars.Count == 0)
                {
                    DebugLog("上传进度条已消失");
                    return true;
                }
            }

            // 检查成功消息
            var successSelectors = new[]
            {
                ".upload-success",
                ".success-message",
                ".toast-success",
                "[class*='success']"
            };

            foreach (var selector in successSelectors)
            {
                var element = await _page.QuerySelectorAsync(selector);
                if (element != null && await element.IsVisibleAsync())
                {
                    DebugLog("检测到成功消息");
                    return true;
                }
            }

            // 检查预览元素
            var previewElements = await _page.QuerySelectorAllAsync("video, [class*='video-preview'], [class*='preview']");
            if (previewElements.Count > 0)
            {
                DebugLog("检测到预览元素");
                return true;
            }

            return true;
        }
        catch (Exception ex)
        {
            DebugLog($"检查视频上传状态时出错: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> CheckAudioUploadSuccess(string audioFilePath)
    {
        try
        {
            var fileName = Path.GetFileName(audioFilePath);

            var successSelectors = new[]
            {
                ".upload-success",
                ".success-message",
                ".toast-success",
                "[class*='success']"
            };

            foreach (var selector in successSelectors)
            {
                var element = await _page.QuerySelectorAsync(selector);
                if (element != null && await element.IsVisibleAsync())
                {
                    DebugLog("检测到成功消息");
                    return true;
                }
            }

            var audioElements = await _page.QuerySelectorAllAsync("audio, [class*='audio-preview'], [class*='audio-player']");
            if (audioElements.Count > 0)
            {
                DebugLog("检测到音频预览元素");
                return true;
            }

            return true;
        }
        catch (Exception ex)
        {
            DebugLog($"检查音频上传状态时出错: {ex.Message}");
            return false;
        }
    }

    // ================ 辅助方法 ================

    private async Task ClickTriggerElement(string selector)
    {
        DebugLog($"点击触发元素: {selector}");

        var element = _page.Locator(selector);

        // 等待元素可见
        await element.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });

        await element.ClickAsync();
        DebugLog("触发元素点击完成");
    }

    private string GetMimeType(string fileExtension)
    {
        return fileExtension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            ".avi" => "video/x-msvideo",
            ".mkv" => "video/x-matroska",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".ogg" => "audio/ogg",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };
    }

    // ================ 保留的原有调试方法（可选） ================

    public async Task<string> AnalyzeUploadControls()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== 文件上传控件分析报告 ===");
        sb.AppendLine($"分析时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        var allInputs = await _page.QuerySelectorAllAsync("input");
        sb.AppendLine($"\n1. 找到 {allInputs.Count} 个input元素");

        for (int i = 0; i < allInputs.Count; i++)
        {
            var input = allInputs[i];
            var type = await input.GetAttributeAsync("type");
            var accept = await input.GetAttributeAsync("accept");
            var id = await input.GetAttributeAsync("id");
            var visible = await input.IsVisibleAsync();

            sb.AppendLine($"\n  Input #{i + 1}:");
            sb.AppendLine($"    类型: {type}");
            sb.AppendLine($"    Accept: {accept}");
            sb.AppendLine($"    ID: {id}");
            sb.AppendLine($"    可见: {visible}");
        }

        return sb.ToString();
    }
}