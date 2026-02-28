using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SmartMedia.Core.Comm;

public class AudioPlayer : IDisposable
{
    private IWavePlayer _waveOutDevice;
    private WaveStream _waveStream;
    private bool _isDisposed = false;

    // 播放状态属性
    public bool IsPlaying
    {
        get
        {
            if (_waveOutDevice == null) return false;

            // 检查 WaveOutEvent 的播放状态
            // PlaybackState 是枚举类型：Stopped, Playing, Paused
            return _waveOutDevice.PlaybackState == PlaybackState.Playing;
        }
    }

    // 其他状态属性
    public bool IsPaused
    {
        get
        {
            if (_waveOutDevice == null) return false;
            return _waveOutDevice.PlaybackState == PlaybackState.Paused;
        }
    }

    public bool IsStopped
    {
        get
        {
            if (_waveOutDevice == null) return true;
            return _waveOutDevice.PlaybackState == PlaybackState.Stopped;
        }
    }

    // 播放状态变更事件
    public event EventHandler<PlaybackState> PlaybackStateChanged;

    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"音频文件不存在: {filePath}");

        DisposeCurrentResources();

        try
        {
            // 优先使用 MediaFoundation
            _waveStream = new MediaFoundationReader(filePath);
            _waveOutDevice = new WaveOutEvent();

            // 订阅播放状态变更事件
            _waveOutDevice.PlaybackStopped += OnPlaybackStopped;

            _waveOutDevice.Init(_waveStream);
        }
        catch (COMException)
        {
            // MediaFoundation 失败，回退到 AudioFileReader
            try
            {
                _waveStream = new AudioFileReader(filePath);
                _waveOutDevice = new WaveOutEvent();

                // 订阅播放状态变更事件
                _waveOutDevice.PlaybackStopped += OnPlaybackStopped;

                _waveOutDevice.Init(_waveStream);
            }
            catch (MmException ex)
            {
                throw new Exception($"无法播放音频文件：{ex.Message}\n请确保已安装音频编解码器", ex);
            }
        }

        // 初始状态变更
        OnPlaybackStateChanged();
    }

    private void OnPlaybackStopped(object sender, StoppedEventArgs e)
    {
        // 播放自然结束时的处理
        OnPlaybackStateChanged();

        // 如果有异常，可以在这里处理
        if (e.Exception != null)
        {
            Console.WriteLine($"播放错误: {e.Exception.Message}");
        }
    }

    private void OnPlaybackStateChanged()
    {
        PlaybackStateChanged?.Invoke(this,
            _waveOutDevice?.PlaybackState ?? PlaybackState.Stopped);
    }

    public void Play()
    {
        if (_waveOutDevice == null || _waveStream == null)
            throw new InvalidOperationException("请先加载音频文件");

        _waveOutDevice?.Play();
        OnPlaybackStateChanged();
    }

    public void Pause()
    {
        _waveOutDevice?.Pause();
        OnPlaybackStateChanged();
    }

    public void Stop()
    {
        _waveOutDevice?.Stop();
        if (_waveStream != null)
        {
            _waveStream.Position = 0;
        }
        OnPlaybackStateChanged();
    }

    // 切换播放/暂停
    public void TogglePlayPause()
    {
        if (IsPlaying)
        {
            Pause();
        }
        else if (IsPaused || IsStopped)
        {
            Play();
        }
    }

    private void DisposeCurrentResources()
    {
        if (_waveOutDevice != null)
        {
            _waveOutDevice.PlaybackStopped -= OnPlaybackStopped;
            _waveOutDevice.Stop();
            _waveOutDevice.Dispose();
        }

        _waveStream?.Dispose();

        _waveOutDevice = null;
        _waveStream = null;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            DisposeCurrentResources();
            _isDisposed = true;
        }
    }

    // 其他有用的属性
    public TimeSpan CurrentTime
    {
        get => _waveStream?.CurrentTime ?? TimeSpan.Zero;
        set
        {
            if (_waveStream != null)
                _waveStream.CurrentTime = value;
        }
    }

    public TimeSpan TotalTime => _waveStream?.TotalTime ?? TimeSpan.Zero;

    public float Volume
    {
        get => _waveOutDevice?.Volume ?? 1.0f;
        set
        {
            if (_waveOutDevice != null)
                _waveOutDevice.Volume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }

    // 获取播放进度百分比 (0.0 - 1.0)
    public double PlaybackProgress
    {
        get
        {
            if (_waveStream == null || _waveStream.Length == 0)
                return 0.0;

            try
            {
                return (double)_waveStream.Position / _waveStream.Length;
            }
            catch
            {
                return 0.0;
            }
        }
        set
        {
            if (_waveStream != null && _waveStream.Length > 0)
            {
                long position = (long)(_waveStream.Length * Math.Clamp(value, 0.0, 1.0));
                _waveStream.Position = position;
            }
        }
    }
}