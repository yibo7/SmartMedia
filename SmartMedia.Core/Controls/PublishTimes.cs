using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartMedia.Core.Controls
{
    public partial class PublishTimes : UserControl
    {
        public PublishTimes()
        {
            InitializeComponent();

            gbTimer.Enabled = cbIsOpenTimer.Checked;
            InitCtrls();
        }

        private void InitCtrls()
        {
            for (int i = 1; i < 50; i++)
            {
                cbPushNumber.Items.Add($"更新{i}期");
            }
            cbPushNumber.SelectedIndex = 1;


            for (int i = 0; i < 24; i++)
            {
                cbStartHour.Items.Add($"{i}点");
                cbEndHour.Items.Add($"{i}点");
            }
            cbStartHour.SelectedIndex = 7;
            cbEndHour.SelectedIndex = 9;

            cbWeek.Items.Add("每天");
            cbWeek.Items.Add("每周一");
            cbWeek.Items.Add("每周二");
            cbWeek.Items.Add("每周三");
            cbWeek.Items.Add("每周四");
            cbWeek.Items.Add("每周五");
            cbWeek.Items.Add("每周六");
            cbWeek.Items.Add("每周日");

            cbWeek.SelectedIndex = 0;


        }

        private void cbIsOpenTimer_CheckedChanged(object sender, EventArgs e)
        {
            gbTimer.Enabled = cbIsOpenTimer.Checked;
        }

        /// <summary>
        /// 为数据生成发布时间字典（按时间顺序排序）
        /// </summary>
        /// <param name="totalRecords">需要导入的数据总数（已排好序）</param>
        /// <returns>发布时间字典，键为记录索引（从0开始），值为按时间顺序排列的发布时间</returns>
        public Dictionary<int, DateTime> GeneratePublishTimes(int totalRecords)
        {
            var publishTimes = new Dictionary<int, DateTime>();

            if (totalRecords <= 0 || !cbIsOpenTimer.Checked)
                return publishTimes;

            // 获取配置参数
            DateTime startDate = dtPushTime.Value.Date; // 开始日期
            int recordsPerDay = GetRecordsPerDay(); // 每天发布的期数
            DayOfWeek[] allowedDays = GetAllowedDays(); // 允许发布的星期
            int startHour = cbStartHour.SelectedIndex; // 开始小时
            int endHour = cbEndHour.SelectedIndex; // 结束小时
            bool isFixedTime = (startHour == endHour); // 是否为固定时间点发布

            // 生成随机数对象
            Random random = new Random();

            // 用于存储每天的时间点列表
            List<DateTime> dailyTimes = new List<DateTime>();

            // 计算总共需要的天数
            int daysNeeded = (int)Math.Ceiling((double)totalRecords / recordsPerDay);

            // 找到符合条件的发布日期列表
            List<DateTime> publishDates = new List<DateTime>();
            DateTime currentDate = startDate;

            while (publishDates.Count < daysNeeded)
            {
                if (IsDayAllowed(currentDate, allowedDays))
                {
                    publishDates.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }

            // 为每天生成发布时间
            foreach (var publishDate in publishDates)
            {
                // 生成当天的所有时间点（按时间顺序）
                List<DateTime> dayTimes = GenerateDayTimes(publishDate, recordsPerDay, startHour, endHour, isFixedTime, random);
                dailyTimes.AddRange(dayTimes);
            }

            // 只取前totalRecords个时间点（防止多生成）
            dailyTimes = dailyTimes.Take(totalRecords).ToList();

            // 按时间排序
            dailyTimes.Sort();

            // 分配给每条记录
            for (int i = 0; i < dailyTimes.Count && i < totalRecords; i++)
            {
                publishTimes[i] = dailyTimes[i];
            }

            return publishTimes;
        }

        /// <summary>
        /// 生成一天内的时间点（按时间顺序）
        /// </summary>
        private List<DateTime> GenerateDayTimes(DateTime date, int recordsPerDay, int startHour, int endHour, bool isFixedTime, Random random)
        {
            var times = new List<DateTime>();

            if (isFixedTime)
            {
                // 固定时间点发布，所有记录都在同一时间（这不合理，应该分散）
                // 改为在固定时间点生成多个时间（相同小时，不同分钟）
                for (int i = 0; i < recordsPerDay; i++)
                {
                    int minute = random.Next(0, 60);
                    int second = random.Next(0, 60);
                    times.Add(date.AddHours(startHour).AddMinutes(minute).AddSeconds(second));
                }
            }
            else
            {
                // 在时间范围内生成recordsPerDay个随机时间点
                for (int i = 0; i < recordsPerDay; i++)
                {
                    // 在开始和结束时间之间均匀分布
                    double hourStep = (endHour - startHour) / (double)Math.Max(1, recordsPerDay - 1);
                    double hour = startHour + (hourStep * i);

                    int baseHour = (int)hour;
                    int minute = random.Next(0, 60);
                    int second = random.Next(0, 60);

                    // 确保时间不会超过结束时间
                    if (baseHour > endHour) baseHour = endHour;

                    times.Add(date.AddHours(baseHour).AddMinutes(minute).AddSeconds(second));
                }
            }

            // 按时间排序
            times.Sort();
            return times;
        }

        /// <summary>
        /// 获取每天发布的期数
        /// </summary>
        private int GetRecordsPerDay()
        {
            // 从 "更新2期" 这样的字符串中提取数字
            if (cbPushNumber.SelectedItem != null)
            {
                string text = cbPushNumber.SelectedItem.ToString();
                if (int.TryParse(text.Replace("更新", "").Replace("期", ""), out int number))
                {
                    return number;
                }
            }
            return 1; // 默认值
        }

        /// <summary>
        /// 获取允许发布的星期
        /// </summary>
        private DayOfWeek[] GetAllowedDays()
        {
            if (cbWeek.SelectedItem == null)
                return new DayOfWeek[0];

            string selectedWeek = cbWeek.SelectedItem.ToString();

            if (selectedWeek == "每天")
            {
                // 每天都可以发布
                return new DayOfWeek[]
                {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
                };
            }
            else
            {
                // 根据选择的星期返回对应的DayOfWeek
                Dictionary<string, DayOfWeek> weekMapping = new Dictionary<string, DayOfWeek>
        {
            { "每周一", DayOfWeek.Monday },
            { "每周二", DayOfWeek.Tuesday },
            { "每周三", DayOfWeek.Wednesday },
            { "每周四", DayOfWeek.Thursday },
            { "每周五", DayOfWeek.Friday },
            { "每周六", DayOfWeek.Saturday },
            { "每周日", DayOfWeek.Sunday }
        };

                if (weekMapping.TryGetValue(selectedWeek, out DayOfWeek day))
                {
                    return new DayOfWeek[] { day };
                }
            }

            return new DayOfWeek[0];
        }

        /// <summary>
        /// 检查日期是否符合允许的星期
        /// </summary>
        private bool IsDayAllowed(DateTime date, DayOfWeek[] allowedDays)
        {
            return allowedDays.Contains(date.DayOfWeek);
        }
    }
}
