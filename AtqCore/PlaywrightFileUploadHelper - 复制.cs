//namespace SmartMedia.AtqCore;
//using Microsoft.Playwright;
//using System.Diagnostics;
//using System.Text;
///// <summary>/// 
//// 使用视频上传  await uploadHelper.UploadVideoFile("C:\\videos\\demo.mp4", "#video-upload-btn");
//// 使用音频上传  await uploadHelper.UploadAudioFile("C:\\audio\\music.mp3", "#audio-upload-btn");
//// 上传文件   await uploadHelper.UploadFile("test.jpg", "image", ".upload-button");
///// </summary>
//public class PlaywrightFileUploadHelper
//{
//    private readonly IPage _page; 

//    public PlaywrightFileUploadHelper(IPage page)
//    {
//        _page = page; 
//    }
//    private void DebugLog(string log)
//    {
//        Debug.WriteLine(log);
//    }

//    // ================ 新增的视频上传方法 ================

//    /// <summary>
//    /// 专门上传视频文件的方法
//    /// </summary>
//    /// <param name="videoFilePath">视频文件路径</param>
//    /// <param name="preClickSelector">触发元素选择器（可选）</param>
//    /// <param name="maxRetries">最大重试次数</param>
//    /// <returns>上传是否成功</returns>
//    public async Task<bool> UploadVideoFile(string videoFilePath, string preClickSelector = null, int maxRetries = 2)
//    {
//        try
//        {
//            DebugLog($"开始上传视频文件: {Path.GetFileName(videoFilePath)}");

//            // 验证文件
//            if (!File.Exists(videoFilePath))
//            {
//                DebugLog($"视频文件不存在: {videoFilePath}");
//                return false;
//            }

//            // 验证文件格式
//            var validVideoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm", ".wmv", ".flv", ".m4v" };
//            var extension = Path.GetExtension(videoFilePath).ToLower();
//            if (!validVideoExtensions.Contains(extension))
//            {
//                DebugLog($"不支持的视频格式: {extension}");
//            }

//            // 获取文件大小
//            var fileInfo = new FileInfo(videoFilePath);
//            DebugLog($"视频文件大小: {fileInfo.Length / 1024 / 1024} MB");

//            // 如果需要，先点击触发元素
//            if (!string.IsNullOrEmpty(preClickSelector))
//            {
//                await ClickTriggerElement(preClickSelector);
//                await _page.WaitForTimeoutAsync(3000); // 视频上传可能需要更长的等待时间
//            }

//            // 查找支持视频的上传控件
//            var retryCount = 0;
//            while (retryCount < maxRetries)
//            {
//                DebugLog($"尝试查找视频上传控件 (重试 {retryCount + 1}/{maxRetries})");

//                // 查找所有上传控件
//                var allInputs = await _page.QuerySelectorAllAsync("input[type='file'], [role='upload'], [class*='upload'], [class*='video']");

//                // 优先查找视频相关的上传控件
//                var videoSpecificSelectors = new[]
//                {
//                    "input[accept*='video']",
//                    "[class*='video-upload']",
//                    "[class*='videoUpload']",
//                    "[data-testid*='video']",
//                    "[role='video-upload']",
//                    ".video-upload-area",
//                    ".video-drop-zone",
//                    "input[accept*='mp4']",
//                    "input[accept*='mov']",
//                    "input[accept*='avi']"
//                };

//                IElementHandle videoInput = null;

//                foreach (var selector in videoSpecificSelectors)
//                {
//                    var elements = await _page.QuerySelectorAllAsync(selector);
//                    if (elements.Count > 0)
//                    {
//                        videoInput = elements.First();
//                        DebugLog($"找到视频专用上传控件: {selector}");
//                        break;
//                    }
//                }

//                // 如果没有找到视频专用控件，使用通用文件上传控件
//                if (videoInput == null)
//                {
//                    foreach (var input in allInputs)
//                    {
//                        var accept = await input.GetAttributeAsync("accept");
//                        if (string.IsNullOrEmpty(accept) || accept.Contains("video") || accept.Contains("*/*"))
//                        {
//                            videoInput = input;
//                            DebugLog($"使用通用文件上传控件");
//                            break;
//                        }
//                    }
//                }

//                if (videoInput != null)
//                {
//                    try
//                    {
//                        DebugLog($"开始上传视频...");

//                        // 设置上传超时（视频文件较大，需要更长的时间）
//                        await videoInput.SetInputFilesAsync(videoFilePath);

//                        // 等待上传完成（根据文件大小动态调整等待时间）
//                        var waitTime = Math.Min(fileInfo.Length / (1024 * 1024) * 1000, 30000); // 每MB等待1秒，最多30秒
//                        DebugLog($"等待上传完成，预计等待时间: {waitTime}ms");
//                        await _page.WaitForTimeoutAsync(3000); // 基础等待时间

//                        // 检查上传是否成功（可以通过页面元素变化或网络请求判断）
//                        var success = await CheckVideoUploadSuccess(videoFilePath);

//                        if (success)
//                        {
//                            DebugLog("视频上传成功！");
//                            return true;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        DebugLog($"视频上传失败: {ex.Message}");
//                    }
//                }

//                retryCount++;
//                if (retryCount < maxRetries)
//                {
//                    DebugLog($"等待2秒后重试...");
//                    await _page.WaitForTimeoutAsync(2000);
//                }
//            }

//            DebugLog("视频上传失败，所有重试次数已用完");
//            return false;
//        }
//        catch (Exception ex)
//        {
//            DebugLog($"视频上传过程中发生错误: {ex.Message}");
//            return false;
//        }
//    }

//    // ================ 新增的音频上传方法 ================

//    /// <summary>
//    /// 专门上传音频文件的方法
//    /// </summary>
//    /// <param name="audioFilePath">音频文件路径</param>
//    /// <param name="preClickSelector">触发元素选择器（可选）</param>
//    /// <returns>上传是否成功</returns>
//    public async Task<bool> UploadAudioFile(string audioFilePath, string preClickSelector = null)
//    {
//        try
//        {
//            DebugLog($"开始上传音频文件: {Path.GetFileName(audioFilePath)}");

//            // 验证文件
//            if (!File.Exists(audioFilePath))
//            {
//                DebugLog($"音频文件不存在: {audioFilePath}");
//                return false;
//            }

//            // 验证文件格式
//            var validAudioExtensions = new[] { ".mp3", ".wav", ".ogg", ".aac", ".flac", ".m4a", ".wma" };
//            var extension = Path.GetExtension(audioFilePath).ToLower();
//            if (!validAudioExtensions.Contains(extension))
//            {
//                DebugLog($"不支持的音频格式: {extension}");
//            }

//            // 如果需要，先点击触发元素
//            if (!string.IsNullOrEmpty(preClickSelector))
//            {
//                await ClickTriggerElement(preClickSelector);
//                await _page.WaitForTimeoutAsync(2000);
//            }

//            // 查找支持音频的上传控件
//            var audioSpecificSelectors = new[]
//            {
//                "input[accept*='audio']",
//                "input[accept*='mp3']",
//                "input[accept*='wav']",
//                "[class*='audio-upload']",
//                "[class*='audioUpload']",
//                "[data-testid*='audio']",
//                "[role='audio-upload']",
//                ".audio-upload-area",
//                ".audio-drop-zone"
//            };

//            IElementHandle audioInput = null;

//            // 优先查找音频相关的上传控件
//            foreach (var selector in audioSpecificSelectors)
//            {
//                var elements = await _page.QuerySelectorAllAsync(selector);
//                if (elements.Count > 0)
//                {
//                    audioInput = elements.First();
//                    DebugLog($"找到音频专用上传控件: {selector}");
//                    break;
//                }
//            }

//            // 如果没有找到音频专用控件，使用通用文件上传控件
//            if (audioInput == null)
//            {
//                DebugLog("未找到音频专用上传控件，尝试使用通用文件上传控件");
//                var allInputs = await _page.QuerySelectorAllAsync("input[type='file'], [role='upload']");

//                foreach (var input in allInputs)
//                {
//                    var accept = await input.GetAttributeAsync("accept");
//                    if (string.IsNullOrEmpty(accept) || accept.Contains("audio") || accept.Contains("*/*"))
//                    {
//                        audioInput = input;
//                        DebugLog($"使用通用文件上传控件");
//                        break;
//                    }
//                }
//            }

//            if (audioInput != null)
//            {
//                try
//                {
//                    DebugLog($"开始上传音频...");
//                    await audioInput.SetInputFilesAsync(audioFilePath);

//                    // 等待上传完成
//                    await _page.WaitForTimeoutAsync(2000);

//                    // 检查上传是否成功
//                    var success = await CheckAudioUploadSuccess(audioFilePath);

//                    if (success)
//                    {
//                        DebugLog("音频上传成功！");
//                        return true;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    DebugLog($"音频上传失败: {ex.Message}");
//                    return false;
//                }
//            }
//            else
//            {
//                DebugLog("未找到可用的音频上传控件");
//            }

//            return false;
//        }
//        catch (Exception ex)
//        {
//            DebugLog($"音频上传过程中发生错误: {ex.Message}");
//            return false;
//        }
//    }

    
//    // ================ 新增的媒体文件检查方法 ================

//    /// <summary>
//    /// 检查视频上传是否成功
//    /// </summary>
//    private async Task<bool> CheckVideoUploadSuccess(string videoFilePath)
//    {
//        try
//        {
//            var fileName = Path.GetFileName(videoFilePath);

//            // 方法1: 检查上传进度条是否消失
//            var progressBars = await _page.QuerySelectorAllAsync(".progress-bar, .upload-progress, .progress");
//            if (progressBars.Count > 0)
//            {
//                // 等待进度条消失
//                await _page.WaitForTimeoutAsync(5000);
//                var remainingBars = await _page.QuerySelectorAllAsync(".progress-bar, .upload-progress, .progress:not([style*='display: none'])");
//                if (remainingBars.Count == 0)
//                {
//                    DebugLog("上传进度条已消失，上传可能已完成");
//                    return true;
//                }
//            }

//            // 方法2: 检查是否显示成功消息
//            var successSelectors = new[]
//            {
//                ".upload-success",
//                ".success-message",
//                ".toast-success",
//                "[class*='success']",
//                "text='上传成功'",
//                "text='upload successful'"
//            };

//            foreach (var selector in successSelectors)
//            {
//                try
//                {
//                    var element = await _page.QuerySelectorAsync(selector);
//                    if (element != null && await element.IsVisibleAsync())
//                    {
//                        DebugLog("检测到成功消息");
//                        return true;
//                    }
//                }
//                catch { }
//            }

//            // 方法3: 检查文件名是否显示在页面上
//            var fileNameElements = await _page.QuerySelectorAllAsync($"text={fileName}");
//            if (fileNameElements.Count > 0)
//            {
//                DebugLog($"检测到文件名显示: {fileName}");
//                return true;
//            }

//            // 方法4: 检查是否有预览元素
//            var previewElements = await _page.QuerySelectorAllAsync("video, [class*='video-preview'], [class*='preview']");
//            if (previewElements.Count > 0)
//            {
//                DebugLog("检测到预览元素");
//                return true;
//            }

//            DebugLog("未能确定上传状态，假设上传成功");
//            return true; // 如果没有明确的失败迹象，假设成功
//        }
//        catch (Exception ex)
//        {
//            DebugLog($"检查视频上传状态时出错: {ex.Message}");
//            return false;
//        }
//    }

//    /// <summary>
//    /// 检查音频上传是否成功
//    /// </summary>
//    private async Task<bool> CheckAudioUploadSuccess(string audioFilePath)
//    {
//        try
//        {
//            var fileName = Path.GetFileName(audioFilePath);

//            // 检查是否显示成功消息
//            var successSelectors = new[]
//            {
//                ".upload-success",
//                ".success-message",
//                ".toast-success",
//                "[class*='success']"
//            };

//            foreach (var selector in successSelectors)
//            {
//                try
//                {
//                    var element = await _page.QuerySelectorAsync(selector);
//                    if (element != null && await element.IsVisibleAsync())
//                    {
//                        DebugLog("检测到成功消息");
//                        return true;
//                    }
//                }
//                catch { }
//            }

//            // 检查文件名是否显示在页面上
//            var fileNameElements = await _page.QuerySelectorAllAsync($"text={fileName}");
//            if (fileNameElements.Count > 0)
//            {
//                DebugLog($"检测到文件名显示: {fileName}");
//                return true;
//            }

//            // 检查是否有音频预览元素
//            var audioElements = await _page.QuerySelectorAllAsync("audio, [class*='audio-preview'], [class*='audio-player']");
//            if (audioElements.Count > 0)
//            {
//                DebugLog("检测到音频预览元素");
//                return true;
//            }

//            DebugLog("未能确定上传状态，假设上传成功");
//            return true;
//        }
//        catch (Exception ex)
//        {
//            DebugLog($"检查音频上传状态时出错: {ex.Message}");
//            return false;
//        }
//    }

     

//    /// <summary>
//    /// 获取推荐的媒体文件上传选择器
//    /// </summary>
//    public async Task<List<string>> GetMediaUploadSelectors()
//    {
//        var selectors = new List<string>();

//        // 查找所有可能的媒体上传控件
//        var mediaSelectors = new[]
//        {
//            "input[accept*='video']",
//            "input[accept*='audio']",
//            "[class*='media-upload']",
//            "[class*='video-upload']",
//            "[class*='audio-upload']",
//            "[data-testid*='media']",
//            "[role='media-upload']",
//            ".media-upload-area",
//            ".video-upload-area",
//            ".audio-upload-area"
//        };

//        foreach (var selector in mediaSelectors)
//        {
//            var elements = await _page.QuerySelectorAllAsync(selector);
//            if (elements.Count > 0)
//            {
//                selectors.Add(selector);
//            }
//        }

//        DebugLog($"找到 {selectors.Count} 个媒体上传选择器");
//        return selectors;
//    }

//    /// <summary>
//    /// 通用方法：上传文件到页面
//    /// </summary>
//    public async Task<bool> UploadImgFile(string filePath, string fileTypeHint = null, string preClickSelector = null)
//    {
//        try
//        {
//            DebugLog($"开始上传文件: {Path.GetFileName(filePath)}");

//            // 1. 如果需要，先点击触发元素
//            if (!string.IsNullOrEmpty(preClickSelector))
//            {
//                await ClickTriggerElement(preClickSelector);
//                await _page.WaitForTimeoutAsync(2000);
//            }

//            // 2. 查找所有文件上传控件
//            var fileInputs = await FindAllFileInputs();
//            DebugLog($"找到 {fileInputs.Count} 个文件上传控件");

//            // 3. 根据文件类型筛选合适的控件
//            var suitableInputs = FilterSuitableInputs(fileInputs, filePath, fileTypeHint);

//            if (suitableInputs.Count == 0)
//            {
//                DebugLog("未找到合适的文件上传控件，尝试使用所有可用的控件");
//                suitableInputs = fileInputs;
//            }

//            // 4. 尝试上传文件
//            foreach (var input in suitableInputs)
//            {
//                if (await TryUploadToInput(input, filePath))
//                {
//                    DebugLog($"文件成功上传到控件");
//                    return true;
//                }
//            }

//            DebugLog("所有上传尝试都失败了");
//            return false;
//        }
//        catch (Exception ex)
//        {
//            DebugLog($"文件上传过程中发生错误: {ex.Message}");
//            return false;
//        }
//    }

//    /// <summary>
//    /// 查找页面上所有的文件上传控件
//    /// </summary>
//    public async Task<List<FileInputInfo>> FindAllFileInputs()
//    {
//        var inputs = new List<FileInputInfo>();

//        // 1. 查找所有input元素
//        var allElements = await _page.QuerySelectorAllAsync("input, [role='upload'], [data-upload]");

//        foreach (var element in allElements)
//        {
//            try
//            {
//                var info = await AnalyzeElement(element);
//                if (info != null)
//                {
//                    inputs.Add(info);
//                }
//            }
//            catch (Exception ex)
//            {
//                DebugLog($"分析元素时出错: {ex.Message}");
//            }
//        }

//        // 2. 记录调试信息
//        LogFileInputs(inputs);

//        return inputs;
//    }

//    /// <summary>
//    /// 详细分析页面上的文件上传控件（调试用）
//    /// </summary>
//    public async Task<string> AnalyzeUploadControls()
//    {
//        var sb = new StringBuilder();

//        sb.AppendLine("=== 文件上传控件分析报告 ===");
//        sb.AppendLine($"分析时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

//        // 1. 查找所有input元素
//        var allInputs = await _page.QuerySelectorAllAsync("input");
//        sb.AppendLine($"\n1. 找到 {allInputs.Count} 个input元素");

//        for (int i = 0; i < allInputs.Count; i++)
//        {
//            var input = allInputs[i];
//            var tagName = await input.EvaluateAsync<string>("el => el.tagName");
//            var type = await input.GetAttributeAsync("type");
//            var accept = await input.GetAttributeAsync("accept");
//            var id = await input.GetAttributeAsync("id");
//            var classAttr = await input.GetAttributeAsync("class");
//            var visible = await input.IsVisibleAsync();

//            sb.AppendLine($"\n  Input #{i + 1}:");
//            sb.AppendLine($"    标签: {tagName}");
//            sb.AppendLine($"    类型: {type}");
//            sb.AppendLine($"    Accept: {accept}");
//            sb.AppendLine($"    ID: {id}");
//            sb.AppendLine($"    Class: {classAttr}");
//            sb.AppendLine($"    可见: {visible}");

//            // 获取更多属性
//            var name = await input.GetAttributeAsync("name");
//            var placeholder = await input.GetAttributeAsync("placeholder");
//            var ariaLabel = await input.GetAttributeAsync("aria-label");

//            if (!string.IsNullOrEmpty(name)) sb.AppendLine($"    Name: {name}");
//            if (!string.IsNullOrEmpty(placeholder)) sb.AppendLine($"    Placeholder: {placeholder}");
//            if (!string.IsNullOrEmpty(ariaLabel)) sb.AppendLine($"    Aria-label: {ariaLabel}");
//        }

//        // 2. 查找可能的文件上传区域
//        sb.AppendLine("\n\n2. 可能的文件上传区域:");
//        var uploadAreaSelectors = new[]
//        {
//            "[class*='upload']",
//            "[class*='drop']",
//            "[class*='file']",
//            "[data-testid*='upload']",
//            "[data-testid*='file']",
//            "[role='upload']",
//            ".upload-area",
//            ".drop-zone",
//            ".file-upload"
//        };

//        foreach (var selector in uploadAreaSelectors)
//        {
//            var elements = await _page.QuerySelectorAllAsync(selector);
//            if (elements.Count > 0)
//            {
//                sb.AppendLine($"\n  选择器 '{selector}' 找到 {elements.Count} 个元素:");
//                foreach (var element in elements.Take(3)) // 只显示前3个
//                {
//                    var tag = await element.EvaluateAsync<string>("el => el.tagName");
//                    var text = await element.TextContentAsync();
//                    var cls = await element.GetAttributeAsync("class");
//                    var textPreview = text?.Trim().Length > 50 ? text.Trim().Substring(0, 50) + "..." : text?.Trim();
//                    sb.AppendLine($"    - {tag} (类: {cls}): {textPreview}");
//                }
//            }
//        }

//        // 3. 检查隐藏的文件上传控件 - 修复了异步lambda问题
//        sb.AppendLine("\n\n3. 检查隐藏的文件上传控件:");
//        var hiddenFileInputs = new List<IElementHandle>();

//        foreach (var input in allInputs)
//        {
//            var type = await input.GetAttributeAsync("type");
//            var isVisible = await input.IsVisibleAsync();

//            if (type == "file" && !isVisible)
//            {
//                hiddenFileInputs.Add(input);
//            }
//        }

//        if (hiddenFileInputs.Count > 0)
//        {
//            sb.AppendLine($"找到 {hiddenFileInputs.Count} 个隐藏的文件上传控件");
//        }

//        // 4. 网络请求分析提示
//        sb.AppendLine("\n\n4. 网络请求分析提示:");
//        sb.AppendLine("   可以使用浏览器开发者工具的Network面板查看文件上传请求");
//        sb.AppendLine("   常见的上传端点可能包含: upload, file, image, media等关键词");

//        return sb.ToString();
//    }

//    /// <summary>
//    /// 智能推荐最佳的上传控件
//    /// </summary>
//    public async Task<FileInputInfo> RecommendBestUploadControl(string filePath, string fileTypeHint = null)
//    {
//        var inputs = await FindAllFileInputs();

//        // 根据文件类型和accept属性进行评分
//        var scoredInputs = new List<(FileInputInfo Input, int Score)>();

//        foreach (var input in inputs)
//        {
//            var score = 0;
//            var fileExtension = Path.GetExtension(filePath).ToLower();
//            var mimeType = GetMimeType(fileExtension);

//            // 检查accept属性是否匹配
//            if (!string.IsNullOrEmpty(input.Accept))
//            {
//                if (input.Accept.Contains(mimeType)) score += 10;
//                if (input.Accept.Contains(fileExtension.Replace(".", ""))) score += 8;
//                if (input.Accept.Contains("image/*") && mimeType.StartsWith("image/")) score += 6;
//                if (input.Accept.Contains("*/*")) score += 4;
//            }
//            else
//            {
//                score += 3; // 没有accept属性也有基本分
//            }

//            // 检查可见性
//            if (input.IsVisible) score += 5;

//            // 检查是否启用
//            if (!input.IsDisabled) score += 2;

//            // 根据元素名称或类名猜测
//            if (fileTypeHint != null && !string.IsNullOrEmpty(input.ClassName))
//            {
//                if (input.ClassName.Contains(fileTypeHint, StringComparison.OrdinalIgnoreCase)) score += 7;
//            }

//            if (score > 0)
//            {
//                scoredInputs.Add((input, score));
//            }
//        }

//        scoredInputs = scoredInputs.OrderByDescending(x => x.Score).ToList();

//        DebugLog($"推荐结果：");
//        foreach (var item in scoredInputs.Take(3))
//        {
//            DebugLog($"  Score {item.Score}: {item.Input}");
//        }

//        return scoredInputs.FirstOrDefault().Input;
//    }

//    // 私有辅助方法
//    private async Task ClickTriggerElement(string selector)
//    {
//        DebugLog($"点击触发元素: {selector}");

//        var element = _page.Locator(selector);
//        if (await element.CountAsync() == 0)
//        {
//            throw new Exception($"未找到触发元素: {selector}");
//        }

//        await element.ClickAsync();
//        await _page.WaitForTimeoutAsync(1000);
//    }

//    private async Task<FileInputInfo> AnalyzeElement(IElementHandle element)
//    {
//        var tagName = await element.EvaluateAsync<string>("el => el.tagName");
//        var type = await element.GetAttributeAsync("type");

//        // 如果不是input或者不是file类型，检查是否是上传区域
//        if (tagName.ToLower() != "input" || type != "file")
//        {
//            // 检查是否有上传相关的属性
//            var role = await element.GetAttributeAsync("role");
//            var dataUpload = await element.GetAttributeAsync("data-upload");
//            var classNameNew = await element.GetAttributeAsync("class");

//            if (role == "upload" || !string.IsNullOrEmpty(dataUpload) ||
//                (classNameNew?.Contains("upload", StringComparison.OrdinalIgnoreCase) == true))
//            {
//                return new FileInputInfo
//                {
//                    Element = element,
//                    TagName = tagName,
//                    Type = "upload-area",
//                    IsVisible = await element.IsVisibleAsync(),
//                    IsDisabled = await element.IsDisabledAsync(),
//                    ClassName = classNameNew,
//                    Accept = "unknown" // 上传区域没有明确的accept
//                };
//            }

//            return null;
//        }

//        // 是file类型的input
//        var accept = await element.GetAttributeAsync("accept");
//        var id = await element.GetAttributeAsync("id");
//        var className = await element.GetAttributeAsync("class");
//        var name = await element.GetAttributeAsync("name");
//        var isVisible = await element.IsVisibleAsync();
//        var isDisabled = await element.IsDisabledAsync();

//        return new FileInputInfo
//        {
//            Element = element,
//            TagName = tagName,
//            Type = type,
//            Accept = accept,
//            Id = id,
//            ClassName = className,
//            Name = name,
//            IsVisible = isVisible,
//            IsDisabled = isDisabled
//        };
//    }

//    private void LogFileInputs(List<FileInputInfo> inputs)
//    {
//        DebugLog($"=== 文件上传控件分析 ===");
//        DebugLog($"总共找到 {inputs.Count} 个控件");

//        for (int i = 0; i < inputs.Count; i++)
//        {
//            var input = inputs[i];
//            DebugLog($"[{i + 1}] {input.TagName}.{input.Type}" +
//                       $" (accept: {input.Accept ?? "无"}, " +
//                       $"visible: {input.IsVisible}, " +
//                       $"disabled: {input.IsDisabled})" +
//                       $" {input}");
//        }
//    }

//    private List<FileInputInfo> FilterSuitableInputs(List<FileInputInfo> inputs, string filePath, string fileTypeHint)
//    {
//        var fileExtension = Path.GetExtension(filePath).ToLower();
//        var mimeType = GetMimeType(fileExtension);

//        DebugLog($"文件信息: 扩展名={fileExtension}, MIME类型={mimeType}, 类型提示={fileTypeHint}");

//        var suitableInputs = new List<FileInputInfo>();

//        foreach (var input in inputs)
//        {
//            // 过滤掉不可用的控件
//            if (input.IsDisabled) continue;

//            // 如果没有accept属性，认为可以接受任何文件
//            if (string.IsNullOrEmpty(input.Accept))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }

//            var accept = input.Accept.ToLower();

//            // 检查MIME类型匹配
//            if (accept.Contains(mimeType))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }

//            // 检查文件扩展名匹配
//            var extensionWithoutDot = fileExtension.Replace(".", "");
//            if (accept.Contains(extensionWithoutDot))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }

//            // 检查通配符
//            if (accept.Contains("*/*"))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }

//            // 检查类型通配符
//            if (mimeType.StartsWith("image/") && accept.Contains("image/*"))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }

//            if (mimeType.StartsWith("video/") && accept.Contains("video/*"))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }

//            if (mimeType.StartsWith("audio/") && accept.Contains("audio/*"))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }

//            // 根据文件类型提示匹配
//            if (!string.IsNullOrEmpty(fileTypeHint) && accept.Contains(fileTypeHint))
//            {
//                suitableInputs.Add(input);
//                continue;
//            }
//        }

//        return suitableInputs;
//    }

//    private async Task<bool> TryUploadToInput(FileInputInfo inputInfo, string filePath)
//    {
//        try
//        {
//            DebugLog($"尝试上传到: {inputInfo}");

//            if (inputInfo.Element == null)
//            {
//                DebugLog("元素为空，跳过");
//                return false;
//            }

//            // 如果元素不可见，尝试使其可见（通过点击父元素等方式）
//            if (!inputInfo.IsVisible)
//            {
//                DebugLog("元素不可见，尝试使其可见");
//                await inputInfo.Element.EvaluateAsync("el => el.style.display = 'block'");
//                await inputInfo.Element.EvaluateAsync("el => el.style.visibility = 'visible'");
//                await _page.WaitForTimeoutAsync(500);
//            }

//            // 上传文件
//            await inputInfo.Element.SetInputFilesAsync(filePath);

//            // 检查是否上传成功
//            await _page.WaitForTimeoutAsync(1000);

//            DebugLog("上传成功");
//            return true;
//        }
//        catch (Exception ex)
//        {
//            DebugLog($"上传失败: {ex.Message}");
//            return false;
//        }
//    }

//    private string GetMimeType(string fileExtension)
//    {
//        return fileExtension.ToLower() switch
//        {
//            ".jpg" or ".jpeg" => "image/jpeg",
//            ".png" => "image/png",
//            ".gif" => "image/gif",
//            ".bmp" => "image/bmp",
//            ".webp" => "image/webp",
//            ".svg" => "image/svg+xml",
//            ".mp4" => "video/mp4",
//            ".mov" => "video/quicktime",
//            ".avi" => "video/x-msvideo",
//            ".mkv" => "video/x-matroska",
//            ".pdf" => "application/pdf",
//            ".doc" => "application/msword",
//            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//            ".xls" => "application/vnd.ms-excel",
//            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//            ".txt" => "text/plain",
//            ".csv" => "text/csv",
//            ".zip" => "application/zip",
//            ".rar" => "application/x-rar-compressed",
//            _ => "application/octet-stream"
//        };
//    }
//}

//// 文件输入信息类
//public class FileInputInfo
//{
//    public IElementHandle Element { get; set; }
//    public string TagName { get; set; }
//    public string Type { get; set; }
//    public string Accept { get; set; }
//    public string Id { get; set; }
//    public string ClassName { get; set; }
//    public string Name { get; set; }
//    public bool IsVisible { get; set; }
//    public bool IsDisabled { get; set; }

//    public override string ToString()
//    {
//        var info = $"{TagName}[type={Type}]";
//        if (!string.IsNullOrEmpty(Id)) info += $"#{Id}";
//        if (!string.IsNullOrEmpty(ClassName)) info += $".{ClassName}";
//        if (!string.IsNullOrEmpty(Name)) info += $"[name={Name}]";
//        return info;
//    }
//} 

 

