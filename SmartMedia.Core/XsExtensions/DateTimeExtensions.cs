using System;
using System.Collections.Generic;
using System.Text;

namespace SmartMedia.Core.XsExtensions;

// 添加一个扩展方法用于转换时间戳
public static class DateTimeExtensions
{
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        TimeSpan timeSpan = dateTime.ToUniversalTime() - UnixEpoch;
        return (long)timeSpan.TotalSeconds;
    }

    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        TimeSpan timeSpan = dateTime.ToUniversalTime() - UnixEpoch;
        return (long)timeSpan.TotalMilliseconds;
    }
}