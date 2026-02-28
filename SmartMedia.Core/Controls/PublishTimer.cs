using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SmartMedia.Core.Controls;

public partial class PublishTimer : UserControl
{
    private bool _validateTime = true;
    private DateTime _lastValidValue; // 记录最后有效的时间值

    public PublishTimer()
    {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        // 设置DateTimePicker格式
        dtPushTime.Format = DateTimePickerFormat.Custom;
        dtPushTime.CustomFormat = "yyyy-MM-dd HH:mm";
        dtPushTime.ShowUpDown = true;

        // 设置初始值
        _lastValidValue = DateTime.Now.AddHours(1);
        dtPushTime.Value = _lastValidValue;

        // 根据验证状态设置MinDate
        UpdateMinDate();

        // 初始控件状态
        UpdateControlsState();

        // 订阅事件
        cbPushFromTime.CheckedChanged += CbPushFromTime_CheckedChanged;
        dtPushTime.ValueChanged += DtPushTime_ValueChanged;
    }

    #region 事件处理
    private void CbPushFromTime_CheckedChanged(object sender, EventArgs e)
    {
        dtPushTime.Enabled = cbPushFromTime.Checked;
    }

    private void DtPushTime_ValueChanged(object sender, EventArgs e)
    {
        // 只有当启用验证且控件启用时才进行验证
        if (_validateTime && dtPushTime.Enabled)
        {
            // 如果选择了过去的时间，恢复到上次有效值
            if (dtPushTime.Value < DateTime.Now)
            {
                // 使用BeginInvoke确保在UI更新后执行
                BeginInvoke(new Action(() =>
                {
                    dtPushTime.Value = _lastValidValue;
                }));
            }
            else
            {
                // 更新最后有效值
                _lastValidValue = dtPushTime.Value;
            }
        }
    }
    #endregion

    #region 属性
    /// <summary>
    /// 是否启用定时发布
    /// </summary>
    [Description("是否启用定时发布功能")]
    [Category("定时发布")]
    [DefaultValue(false)]
    public bool ScheduleEnabled
    {
        get => cbPushFromTime.Checked;
        set
        {
            if (cbPushFromTime.Checked != value)
            {
                cbPushFromTime.Checked = value;
                UpdateControlsState();
            }
        }
    }

    /// <summary>
    /// 获取或设置发布时间（DateTime格式）
    /// </summary>
    [Description("定时发布时间（DateTime格式）")]
    [Category("定时发布")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DateTime? DateTimeValue
    {
        get
        {
            if (!ScheduleEnabled) return null;
            return dtPushTime.Value;
        }
        set
        {
            if (value.HasValue)
            {
                dtPushTime.Value = value.Value;
                _lastValidValue = value.Value;
                ScheduleEnabled = true;
            }
            else
            {
                ScheduleEnabled = false;
            }
        }
    }

    /// <summary>
    /// 获取发布时间戳（Unix秒级时间戳）
    /// 如果未启用定时，返回0
    /// </summary>
    [Description("发布时间戳（Unix秒级时间戳，未定时为0）")]
    [Category("定时发布")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public long DateTimestamp
    {
        get
        {
            if (!ScheduleEnabled)
                return 0;

            return new DateTimeOffset(dtPushTime.Value.ToUniversalTime())
                .ToUnixTimeSeconds();
        }
        set
        {
            if (value <= 0)
            {
                ScheduleEnabled = false;
            }
            else
            {
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
                DateTimeValue = dateTime.ToLocalTime();
            }
        }
    }

    /// <summary>
    /// 是否验证时间必须晚于当前时间
    /// 如果为true，用户无法选择过去的时间
    /// </summary>
    [Description("是否验证发布时间必须晚于当前时间")]
    [Category("验证")]
    [DefaultValue(true)]
    public bool ValidateTime
    {
        get => _validateTime;
        set
        {
            if (_validateTime != value)
            {
                _validateTime = value;
                UpdateMinDate();
            }
        }
    }

    /// <summary>
    /// 时间显示格式
    /// </summary>
    [Description("时间显示格式")]
    [Category("外观")]
    [DefaultValue("yyyy-MM-dd HH:mm")]
    public string DateTimeFormat
    {
        get => dtPushTime.CustomFormat;
        set => dtPushTime.CustomFormat = value;
    }

    /// <summary>
    /// CheckBox显示的文本
    /// </summary>
    [Description("启用定时的标签文本")]
    [Category("外观")]
    [DefaultValue("启用定时发布")]
    public string CheckBoxText
    {
        get => cbPushFromTime.Text;
        set => cbPushFromTime.Text = value;
    }
    #endregion

    #region 私有方法
    private void UpdateControlsState()
    {
        dtPushTime.Enabled = cbPushFromTime.Checked;
    }

    private void UpdateMinDate()
    {
        if (_validateTime)
        {
            //dtPushTime.MinDate = DateTime.Now;

            // 如果当前值小于最小时间，调整到最小时间
            if (dtPushTime.Value < DateTime.Now)
            {
                dtPushTime.Value = DateTime.Now;
                _lastValidValue = DateTime.Now;
            }
        }
        else
        {
            dtPushTime.MinDate = DateTime.MinValue;
        }
    }
    #endregion

    #region 验证方法
    /// <summary>
    /// 验证时间是否有效
    /// </summary>
    /// <returns>验证结果和错误信息</returns>
    public (bool IsValid, string ErrorMessage) ValidateScheduleTime()
    {
        if (!ScheduleEnabled)
            return (true, string.Empty);

        if (!DateTimeValue.HasValue)
            return (false, "请选择发布时间");

        // 由于控件已阻止选择无效时间，这里可以简化验证
        // 或者仍然保留验证逻辑作为双重保障
        if (_validateTime && DateTimeValue.Value < DateTime.Now)
            return (false, "发布时间必须晚于当前时间");

        return (true, string.Empty);
    }

    /// <summary>
    /// 获取所有发布数据
    /// </summary>
    /// <returns>包含启用状态、时间、时间戳的元组</returns>
    public (bool Enabled, DateTime? DateTime, long Timestamp) GetPublishData()
    {
        return (ScheduleEnabled, DateTimeValue, DateTimestamp);
    }
    #endregion
}