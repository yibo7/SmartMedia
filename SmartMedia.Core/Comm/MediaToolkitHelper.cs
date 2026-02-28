using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SmartMedia.Core.Comm;

public class MediaToolkitHelper : IDisposable
{
    private bool _ffmpegChecked;
    private Engine _engine;

    public const string FFMPEG_EXECUTABLE = "ffmpeg.exe";
    public const string FFMPEG_DOWNLOAD_URL = "https://ffmpeg.org/download.html";

    public MediaToolkitHelper()
    {
    }

    /// <summary>
    /// 检测 FFmpeg 是否可用（基于 PATH）
    /// </summary>
    public string CheckFfmpegAvailability()
    {
        if (_ffmpegChecked)
            return null;

        try
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FFMPEG_EXECUTABLE,
                    Arguments = "-version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            p.Start();
            p.WaitForExit(3000);

            if (p.ExitCode != 0)
                return "未检测到 FFmpeg，请确认已加入 PATH";

            _ffmpegChecked = true;
            return null;
        }
        catch (Exception ex)
        {
            return $"FFmpeg 检测失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 获取 Engine（确保 FFmpeg 可用）
    /// </summary>
    public Engine GetEngine()
    {
        var error = CheckFfmpegAvailability();
        if (error != null)
            throw new InvalidOperationException(error);

        _engine ??= new Engine();
        return _engine;
    }

    /// <summary>
    /// 获取视频总时长（秒）
    /// </summary>
    /// <param name="videoPath">视频文件路径</param>
    /// <returns>时长秒，如果失败返回 -1</returns>
    public double GetVideoDurationSeconds(string videoPath)
    {
        if (!File.Exists(videoPath))
            return -1;

        try
        {
            var inputFile = new MediaFile(videoPath);

            using (var engine = new Engine()) // 不用单例
            {
                engine.GetMetadata(inputFile);
            }

            // TotalSeconds 可能返回 double
            return inputFile.Metadata?.Duration.TotalSeconds ?? -1;
        }
        catch
        {
            return -1;
        }
    }


    /// <summary>
    /// 截图
    /// </summary>
    /// <param name="videoPath">视频完整路径</param>
    /// <param name="time">时间点（默认3秒）可以这样设置，TimeSpan.FromSeconds(5)</param>
    /// <param name="outputImagePath">图片的保存路径，如果为空，将生成与视频同名的图片</param>
    /// <returns>(是否成功,提示信息)</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public (bool success, string messageOrPath) CaptureFrame(
    string videoPath,
    TimeSpan time = default,
    string outputImagePath = "")
    {
        try
        {
            if (!File.Exists(videoPath))
            {
                return (false, $"视频文件不存在: {videoPath}");
            }

            if (time == default || time < TimeSpan.Zero)
            {
                time = TimeSpan.FromSeconds(3);
            }

            if (string.IsNullOrWhiteSpace(outputImagePath))
            {
                string dir = Path.GetDirectoryName(videoPath) ?? Directory.GetCurrentDirectory();
                string name = Path.GetFileNameWithoutExtension(videoPath);
                outputImagePath = Path.Combine(dir, $"{name}.jpg");
            }
            else if (!Path.HasExtension(outputImagePath))
            {
                outputImagePath += ".jpg";
            }

            string outDir = Path.GetDirectoryName(outputImagePath);
            if (!string.IsNullOrWhiteSpace(outDir) && !Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            if (File.Exists(outputImagePath))
            {
                File.Delete(outputImagePath);
            }

            var ffmpegError = CheckFfmpegAvailability();
            if (ffmpegError != null)
            {
                return (false, ffmpegError);
            }

            var input = new MediaFile(videoPath);
            var output = new MediaFile(outputImagePath);

            var options = new ConversionOptions
            {
                Seek = time
            };

            // ⚠️ 关键：Engine 必须 Dispose
            using (var engine = GetEngine())
            {
                engine.GetThumbnail(input, output, options);
            }

            // 等待 ffmpeg 彻底释放文件（保险）
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (File.Open(outputImagePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(30);
                }
            }

            if (!File.Exists(outputImagePath))
            {
                return (false, "截图失败：未生成图片文件");
            }

            var info = new FileInfo(outputImagePath);
            if (info.Length == 0)
            {
                File.Delete(outputImagePath);
                return (false, "截图失败：生成的图片文件为空");
            }

            return (true, outputImagePath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"截图过程中发生错误: {ex.Message}");
            return (false, $"截图过程中发生错误: {ex.Message}");
        }
    }



    /// <summary>
    /// 异步截图
    /// </summary>
    public async Task<(bool, string)> CaptureFrameAsync(
        string videoPath,
        TimeSpan time = default,
        string outputImagePath = "")
    {
        return await Task.Run(() => CaptureFrame(videoPath, time, outputImagePath));
    }

    public void Dispose()
    {
        _engine?.Dispose();
        _engine = null;
    }
}
