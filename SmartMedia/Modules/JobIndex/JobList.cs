using SmartMedia.Core.SitesBase.BLL;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms;
using SmartMedia.MCore;
using SmartMedia.Modules.PushContent.DB;
using System.Diagnostics;
using System.Timers; 
using XS.WinFormsTools;

namespace SmartMedia.Modules.Job;

public partial class JobList : XsDockContent //, IModules 作为首页，不用在模块中加载
{
    private System.Timers.Timer _timer; // 定时器
    private bool _isTaskRunning = false; // 标记任务是否正在执行
    private readonly object _lockObj = new object(); // 线程锁对象

    public string Title => "任务管理器";//要实现模块名称
    public System.Drawing.Image Ico => Resource.books; //要实现模块图标 

    public JobList()
    {
        CloseButtonVisible = false; // 隐藏关闭按钮 
        InitializeComponent();

        //lvData.AddColum("", 42);
        lvData.AddColum("Id", 0);
        lvData.AddColum("任务名称", -100);
        lvData.AddColum("内容类别", 80);
        lvData.AddColum("发布时间", 120);
        lvData.AddColum("发布倒计时", 150);
         

        lvData.SelectedIndexChanged += lvData_SelectedIndexChanged;

        // 初始化定时器
        InitTimer();

        BindData();
         
    }

    /// <summary>
    /// 初始化定时器
    /// </summary>
    private void InitTimer()
    {
        // 如果定时器已存在，先停止并释放
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Dispose();
            _timer = null;
        }

        // 创建新的定时器，设置间隔为1分钟（60000毫秒）
        _timer = new System.Timers.Timer(60000);
        _timer.Elapsed += Timer_Elapsed;
        _timer.AutoReset = true; // 设置为自动重置，使定时器持续触发

        // 根据设置决定是否启动定时器
        if (Settings.Instance.IsTimerStart == 1)
        {
            _timer.Start();
            btnPassAll.Image = Resource.spass;
            btnPassAll.Text = "暂停任务";
        }
        else
        {
            btnPassAll.Image = Resource.start;
            btnPassAll.Text = "开启任务";
        }
    }


    private void AddLogItem(string log)
    {
        // 添加新日志
        lbLogs.Items.Add(log);

        // 如果超过300条，删除最旧的（索引0）
        while (lbLogs.Items.Count > 100)
        {
            lbLogs.Items.RemoveAt(0);
        }

        // 可选：自动滚动到底部，显示最新日志
        lbLogs.TopIndex = lbLogs.Items.Count - 1;
    }
    /// <summary>
    /// 定时器事件处理
    /// </summary>
    private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        // 检查是否有任务正在执行
        lock (_lockObj)
        {
            if (_isTaskRunning)
            { 

                AddLogItem($"[{DateTime.Now}] 上次任务尚未完成，跳过本次执行");

                return;
            }
            _isTaskRunning = true;
        }
         

        try
        {
            Debug.WriteLine($"[{DateTime.Now}] 开始执行定时任务");
            var dataBll = new VideoBll();
            var lst = dataBll.GetScheduledContents(1);
            if (lst.Count < 1)
            {
                RefushData();
                return;
            }

            var model = lst[0];

            long currentTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            if (model.PublishTimeStamp > currentTimestamp)
            {
                return;
            }

            AddLogItem($"------【{model.Title}】------");
            
            // 执行异步任务
            await TimerTodoAsync(model);           

            AddLogItem($"-----------------------------");

            RefushData();

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{DateTime.Now}] 定时任务执行出错: {ex.Message}");
            // 这里可以添加错误日志记录
        }
        finally
        {
            // 任务执行完毕，重置标记
            lock (_lockObj)
            {
                _isTaskRunning = false;
            }
        }
        
    }

    private void RefushData()
    {
        // 任务执行完成后，在UI线程刷新数据
        this.Invoke(new Action(() =>
        {
            try
            {
                // 可以添加其他UI更新操作
                // 例如：更新状态栏信息
                if (lvData.Items.Count > 0)
                {
                    lbStatus.Text = $"最后更新: {DateTime.Now:HH:mm:ss} | 任务数(只展示最近100条): {lvData.Items.Count}";
                }
            }
            catch (Exception ex)
            {
                AddLogItem($"刷新数据出错: {ex.Message}");
            }
        }));
    }

    /// <summary>
    /// 定时执行的任务 - 保留原有的执行程序
    /// </summary>
    private async Task TimerTodoAsync(PushInfo model)
    {

        try
        {
            var dataBll = model.GetBll();

            await dataBll.StartPush(model.Id, (tips, imax, icurrent) =>
            {
                AddLogItem(tips);
                lbStatus.Text = $"发布中:{tips}";

            });
        }
        catch (Exception ex)
        {
            AddLogItem($"发布失败:{ex.Message}"); 
        }
    }

    private void AddItem(PushInfo model, int iIndex)
    {
        lvData.AddItem(model.Id.ToString(), $"{iIndex}、{model.Title}", model.ContentTypeName, model.PublishDateTime, model.FormatTimeRemaining());
    }

    private void BindData()
    {
        lvData.Items.Clear();
        var lst = new VideoBll().GetScheduledContents();
        int iIndex = 1;
        foreach (var model in lst)
        {
            AddItem(model, iIndex);
            iIndex++;
        }

        if (lvData.Items.Count > 0)
        {
            lvData.SelectedItem(0);// 选中第一行
        }
    }

    private void lvData_SelectedIndexChanged(object sender, EventArgs e)
    {
        // 检查是否有选中项
        if (lvData.SelectedItems.Count > 0)
        {
            string GetSelJobJd = lvData.GetSelectItemValue(0);
        }
    }

    private void btnRefesh_Click(object sender, EventArgs e)
    {
        this.BindData();
    }
     

    /// <summary>
    /// 开关定时器
    /// </summary>
    private void btnPassAll_Click(object sender, EventArgs e)
    {
        if (Dialogs.ConfirmDialog(Settings.Instance.IsTimerStart == 0 ? "确定要开启定时任务吗？" : "确定要暂停定时任务吗？"))
        {
            if (Settings.Instance.IsTimerStart == 1)
            {
                Settings.Instance.IsTimerStart = 0;
                _timer?.Stop(); // 停止定时器
                btnPassAll.Image = Resource.start;
                btnPassAll.Text = "开启任务";
            }
            else
            {
                Settings.Instance.IsTimerStart = 1;
                _timer?.Start(); // 启动定时器
                btnPassAll.Image = Resource.spass;
                btnPassAll.Text = "暂停任务";
            }

            this.BindData();
            Settings.Instance.Save();
        }
    }

    /// <summary>
    /// 窗体关闭时释放资源
    /// </summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Dispose();
            _timer = null;
        }
        base.OnFormClosing(e);
    }
}