using System;
using System.Collections.Generic;
using System.Text;

namespace SmartMedia.Plugins.AutoPush
{
    /// <summary>
    /// 通过API 获取平台频道信息
    /// </summary>
    public class ChannelInfo
    {
        /// <summary>
        /// 频道名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 频道图片地址
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 频道简介
        /// </summary>
        public string Info { get; set; }

    }
}
