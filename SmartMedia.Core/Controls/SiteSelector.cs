using SmartMedia.Core.Comm;
using SmartMedia.Core.SitesBase; 
using SmartMedia.Modules.PushContent; 
using System.ComponentModel;
using System.Data; 

namespace SmartMedia.Core.Controls;

public partial class SiteSelector : UserControl
{
    public SiteSelector()
    {
        dicSettingWindows = new Dictionary<string, PushSettingBase>();
        InitializeComponent();

        fPanelSelPlugins.AutoScroll = true;
        fPanelSelPlugins.BackColor = Color.White;
        fPanelSelPlugins.Controls.Clear();

        this.Load += SiteSelector_Load;


    }

    private void SiteSelector_Load(object? sender, EventArgs e)
    {
        BindSites();
    }

    private Dictionary<string, Dictionary<string, string>> _SiteSettingValues;

    /// <summary>
    /// 修改发布内容时初始化站点配置信息
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Dictionary<string, Dictionary<string, string>> SiteSettingValues {            
        set
        {
            _SiteSettingValues = value;
            int iIndex = 0;
            foreach (var site in _SiteSettingValues)
            {
                var pushPlugin = GetByClasName(site.Key);
                if (pushPlugin != null) {
                    AddSelSiteToPanel(pushPlugin);
                     
                    if (!dicSettingWindows.ContainsKey(site.Key))
                    { 
                        var setWin = pushPlugin.NewSettingWin;
                        if (iIndex == 0)
                        {
                            pnSettings.Controls.Clear();
                            pnSettings.Controls.Add(setWin);
                        }
                        iIndex++;

                        setWin.IsSelected = true;
                        setWin.InitCtrs(_SiteSettingValues[site.Key]);
                        dicSettingWindows.Add(site.Key, setWin);
                        

                    }
                }
                
            }
        }
    }
    public PushBase GetByClasName(string ClassName)
    {
        var values = GetPushPlugins();
        foreach (PushBase t in values)
        {
            if (t.ClassName == ClassName)
                return t;
        }
        return null;
    }
    /// <summary>
    /// 获取站点配置
    /// </summary>
    /// <returns>(错误信息-为空表示没有错误,站点配置)</returns> 
    public (string, Dictionary<string, Dictionary<string, string>>?) GetValueSettings()
    {
        var selPlugins = new Dictionary<string, PushSettingBase>();
        foreach (var SetWin in dicSettingWindows)
        {   
            var w = SetWin.Value;
            if (SetWin.Value.IsSelected)
            {
                var ddd = w.GetSettings();
                selPlugins.Add(SetWin.Key, SetWin.Value);
            }
               
        }

        if (selPlugins.Count < 1)
        {
            return ("请选择发布平台", null);
        }
        var dicSites = new Dictionary<string, Dictionary<string, string>>();
        foreach (var plugin in selPlugins)
        {
            (string err, Dictionary<string, string> config) = plugin.Value.GetSettings();
            if (!string.IsNullOrEmpty(err))
            {
                return (err, null);

            }
            dicSites.Add(plugin.Key, config);
            //plugin.SaveCf();
        }
        return ("", dicSites);

    }


    private ImgListItemBll<PushBase> sites; 
    private void BindSites()
    {
        // 在设计时直接返回，不执行任何实际代码
        if (DesignMode || IsAncestorSiteInDesignMode)
        {                
            return;
        }

        //siteSel = new ImgListItemBll<PushBase>(this.lvSiteSels);
        sites = new ImgListItemBll<PushBase>(this.lvSites);
        sites.OnMouseClick += Sites_Click;

        //获取模块
        var modules = GetPushPlugins();
        foreach (var plugin in modules)
        {
            //plugin.SettingWindow.OnEnable += OnPluginEnable;
            sites.Add(plugin.PluginName, plugin.IcoName, plugin);
        }

        sites.Bind();

    }

    virtual protected List<PushBase> GetPushPlugins()
    {
        //获取所有发布插件
        return PluginUtils.VideoPushList.Cast<PushBase>().ToList(); 
        
    }

    private Dictionary<string, PushSettingBase> dicSettingWindows; 
    private void Sites_Click(ImgListItem<PushBase> item)
    {
        var plugin = item.DockContentObj;
        string pushPluginKey = plugin.ClassName;
        if (!dicSettingWindows.ContainsKey(pushPluginKey))
        {
            dicSettingWindows.Add(pushPluginKey, plugin.NewSettingWin);
        }

        PushSettingBase CurrentSettingWindow = dicSettingWindows[pushPluginKey];
         
        if (CurrentSettingWindow.OnEnable == null)
        {
            CurrentSettingWindow.OnEnable += OnPluginEnable;
        }
        pnSettings.Controls.Clear();
        pnSettings.Controls.Add(CurrentSettingWindow);

        if (_SiteSettingValues == null || !_SiteSettingValues.ContainsKey(pushPluginKey))
        {
            CurrentSettingWindow.InitCtrs();
        }

        //if (_SiteSettingValues != null && _SiteSettingValues.ContainsKey(pushPluginKey)) // 编辑记录
        //    CurrentSettingWindow.InitCtrs(_SiteSettingValues[pushPluginKey]);
        //else
        //    CurrentSettingWindow.InitCtrs();

    }

     

    int fPanelSelPluginsY = 10; // 起始位置

    private void AddSelSiteToPanel(PushBase plugin)
    {
        string sName = plugin.ClassName;
        PictureBox pb = new PictureBox();
        pb.Size = new Size(32, 32);
        pb.Image = plugin.IcoName;
        pb.SizeMode = PictureBoxSizeMode.StretchImage;
        pb.Name = sName;
        pb.Location = new Point(
            (fPanelSelPlugins.ClientSize.Width - 32) / 2,
            fPanelSelPluginsY
        );

        // 添加双击事件 - 删除该控件并重新布局
        pb.DoubleClick += (s, e) =>
        {
            if (XS.WinFormsTools.Dialogs.ConfirmDialog("确认要删除这个站点吗？"))
            {
                if(dicSettingWindows!=null && dicSettingWindows.ContainsKey(sName))
                {
                    PushSettingBase pl = dicSettingWindows[sName];
                    pl.IsSelected = false;
                } 

                fPanelSelPlugins.Controls.RemoveByKey(sName);
                RearrangePluginIcons(); // 重新排列剩余图标
            }

        };

        pb.Click += (s, e) => {

            pnSettings.Controls.Clear();
            pnSettings.Controls.Add(dicSettingWindows[sName]);
        };

        fPanelSelPlugins.Controls.Add(pb);
        fPanelSelPluginsY += 50; // 每个图标间距50像素
    }

    private void OnPluginEnable(bool IsEnable, PushBase plugin)
    {
        string Name = plugin.ClassName;
        bool IsHave = fPanelSelPlugins.Controls.ContainsKey(Name);

        if (IsEnable && !IsHave)
        {
            AddSelSiteToPanel(plugin);
        }
        else if (!IsEnable && IsHave)
        {
            fPanelSelPlugins.Controls.RemoveByKey(Name);
            RearrangePluginIcons(); // 重新排列剩余图标
        }
    }

    // 重新排列所有图标的方法
    private void RearrangePluginIcons()
    {
        if (fPanelSelPlugins.Controls.Count == 0)
        {
            fPanelSelPluginsY = 10; // 重置起始位置
            return;
        }

        // 按当前Y坐标排序控件
        var sortedControls = fPanelSelPlugins.Controls.Cast<Control>()
            .OrderBy(c => c.Location.Y)
            .ToList();

        // 重新设置位置
        int currentY = 10;
        foreach (var control in sortedControls)
        {
            control.Location = new Point(
                (fPanelSelPlugins.ClientSize.Width - 32) / 2,
                currentY
            );
            currentY += 50; // 保持相同间距
        }

        fPanelSelPluginsY = currentY; // 更新下一个添加位置
    }

}
