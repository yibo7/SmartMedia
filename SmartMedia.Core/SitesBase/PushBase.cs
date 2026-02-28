using Microsoft.Playwright;
using SmartMedia.Core.Comm;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Modules.PushContent; 
using XS.Core2;

namespace SmartMedia.Core.SitesBase;

/// <summary>
/// 发布内容分类模型
/// </summary>
public class CategoryItem
{
    public CategoryItem(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
    public List<CategoryItem> SubItems { get; set; }
}

///// <summary>
///// 可用控件类型
///// </summary>
//public enum CtrlType
//{
//    CoverImage, TextBox, ComboBox, CheckBox, Categorie
//}

//public class SettingItem
//{
//    SettingCtrBase ucc = null;
//    public SettingItem(SettingCtrBase ucc, string tip, string title = "", string value = "")
//    {         
        
//    } 

//}

abstract public class PushBase : PluginBase
{
    /// <summary>
    /// 是否从API发布内容，像YuoTube支持API发布的站点，会采用API发布内容
    /// 如果重写UploadPage与DataPage为空，将认为当前站点采用API发布内容
    /// </summary>
    public bool IsPushFromApi => string.IsNullOrWhiteSpace(UploadPage) && string.IsNullOrWhiteSpace(DataPage);
    virtual public bool IsAllowPush => true; // 默认都是支持发布的，但有些平台只支持账号管理，可以重写这个变量为false
    virtual public int OrderIndex => 1000;
    virtual public string CategoryFileName => "";
    virtual public List<string> GetCategoryList()
    {
        string sClassNames = GetCtrValue("ClassName");
        if (!string.IsNullOrEmpty(sClassNames))
        {
            return sClassNames.Split(',').ToList();
        }

        return new List<string>();
    }

    protected T BuildCtr<T>(string title="", string tips = "", string defaultValue="",string initData="") where T: SettingCtrBase, new()
    {
        var ctr = new T();

        ctr.InitPushBase(this);

        if (!string.IsNullOrEmpty(title)) ctr.SetTitle(title);
        if (!string.IsNullOrEmpty(tips)) ctr.SetTips(tips);
        if (!string.IsNullOrEmpty(initData)) ctr.InitData(initData);

        if (!string.IsNullOrEmpty(defaultValue)) ctr.SetValue(defaultValue);

        return ctr;
    }

    /// <summary>
    /// 配置所有可用的控件
    /// Dictionary<控件的ID-具有唯一性, 控件实例>
    /// </summary>
    virtual protected Dictionary<string, SettingCtrBase> SiteCtrls 
    {
        get
        {
            return new Dictionary<string, SettingCtrBase>() 
            {
                { "ClassName",BuildCtr<SettingCategories>("发布分类",initData:CategoryFileName)},
                { "SpecialName",BuildCtr<SettingTextBox>("合集名称","可为空，你在此平台上创建的合集名称") }
            };
        }
    }

    virtual public Dictionary<string, SettingCtrBase> GetSiteCtrls()
    {
        return SiteCtrls;
    }

    ///// <summary>
    ///// 设置自动定义配置，key为键值，value为默认值
    ///// </summary>
    ///// <returns>(控件名称，默认值)</returns> 
    //virtual public Dictionary<string, string> CustomCtrls() => new Dictionary<string, string>() {
    //    { "发布分类",""},{ "合集名称",""}
    //};



    //virtual public Dictionary<string, string> CustomCtrls() => new Dictionary<string, string>() { };

    //更新站点的发布参数，只为发布时使用
    private Dictionary<string, string> CtrValues;
    /// <summary>
    /// 更新站点的发布参数，只为发布时使用，方便获取站点控件的值
    /// </summary>
    /// <param name="ctrValues">当前发布任务站点的发布参数</param>
    public void SetCtrValueToPush(Dictionary<string, string> ctrValues)
    {
        CtrValues = ctrValues;
    }

    public string GetCtrValue(string key)
    {
        var cf = CtrValues;// GetCustomConfigs();
        if (cf.ContainsKey(key))
        {
            return cf[key];
        }
        return "";
    }


    //virtual public Dictionary<string, SettingItem> ConfigCtrls => new Dictionary<string, SettingItem>() {
    //    { "发布分类",new SettingItem(CtrlType.Categorie,"")},{ "合集名称",new SettingItem(CtrlType.TextBox,"可为空，你在此平台上创建的合集名称")},{ "相关话题",new SettingItem(CtrlType.TextBox,"可为空，多个用逗号分开")}
    //};


    //override public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
    //    { "发布分类",""},{ "合集名称",""},{ "相关话题",""}
    //};
    /// <summary>
    /// 验证分类是否选择用，要求分类必须是几级，0表示当前平台不需要分类
    /// </summary>
    virtual public int CategoryLeve => 0;
    virtual public PushSettingBase NewSettingWin => new PushSettingBase(this);
    /// <summary>
    /// 发布整个流程的动作
    /// </summary>
    /// <returns>返回空值表示成功，否则表示有错误</returns>
    abstract protected Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string>? CallBack);
    virtual public Image IcoName => Resource.inindex;

    abstract public Task<string> LoginAsync();


    /// <summary>
    /// 上传页面的URL
    /// </summary>
    abstract protected string UploadPage { get; }
    /// <summary>
    /// 查看数据报表的页面URL
    /// </summary>
    abstract public string DataPage { get; }

    /// <summary>
    /// 登录状态JSON的URL
    /// </summary>
    private string StatePath = string.Empty;

    //public PushSettingBase SettingWindow;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="statePath">在使用前要先登录网站获取登录信息</param>
    public PushBase()
    {
        ClassName = GetType().Name;

        StatePath = Path.Combine(Application.StartupPath, $"SiteState/{ClassName}.json");

        //SettingWindow = NewSettingWin;

        //CustomConfigs["IsEnable"] = "true"; // 暂时都是启动状态，后期如果站点多了，只选择登录后的网站可通过这个设置
    }


    /// <summary>
    /// 获取站点JSon字符，可以用来判断是否登录过
    /// </summary>
    /// <returns>返回空表示还没有登录过</returns>
    public string GetStateJsonStr()
    {
        if (File.Exists(StatePath))
            return XS.Core2.FSO.FObject.ReadFile(StatePath);
        return "";
    }

    /// <summary>
    /// 开始上传视频
    /// </summary>
    /// <param name="model">视频信息</param>
    /// <returns>返回空值表示上传成功，否则有错误信息</returns>
    public async Task<string> Start(PushInfo? model, Action<string> CallBack)
    {
        if (IsPushFromApi) // 直接采用API发布
        {
            return await ActionsAsync(model,null, CallBack);
        }

        try
        {
            string stateInfo = await PlaywrightUtils.GetState(StatePath);
            if (string.IsNullOrWhiteSpace(stateInfo))
                return "还没有登录平台，点击右上角【登录网站】";


            if (Equals(model, null))
                return "信息实体不能为空";

            bool IsOpenBorwn = Settings.Instance.IsOpenBrowser == 1;

            await using var context = await PlaywrightUtils.CreatePlaywrightAsync(StatePath, !IsOpenBorwn);

            var page = await context.NewPageAsync();
            page.SetDefaultTimeout(0); // 永远不超时,一些大文件上传会超时
            //停一下再获取内容，是因为异步加载的内容有时没有那么快出来
            //await page.WaitForLoadStateAsync(LoadState.NetworkIdle); // 等待页面加载完毕 

            // 导航到B站上传页面
            await page.GotoAsync(UploadPage);
            await page.WaitForTimeoutAsync(2000); // 等待页面加载

            //model.special = GetCf("合集名称"); // 统一获取合集名称

            return await ActionsAsync(model, page, CallBack);

        }
        catch (Exception ex)
        {
            var err = $"{ClassName}上传发生异常：{ex.Message}";
            LogHelper.Error<PushBase>(err);
            return err;
        }

    }


    protected string GetSpecialName(string name = "SpecialName")
    {
        return GetCtrValue(name); // 统一获取合集名称
    }
    /// <summary>
    /// 点击某个按钮或元素上传文件
    /// </summary>
    /// <param name="ClickObj">点击元素的选择器</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="page">当前页面</param>
    /// <returns></returns>
    protected async Task<string> UploadFileAsync(string ClickObj, string filePath, IPage page)
    {
        try
        {
            // 开始等待文件选择器
            var waitForFileChooserTask = page.WaitForFileChooserAsync();
            // 如果您有一个文件输入元素，需要设置文件，可以这样做：
            await page.Locator(ClickObj).ClickAsync();
            // 等待文件选择器出现
            var fileChooser = await waitForFileChooserTask;
            // 选择文件
            await fileChooser.SetFilesAsync(filePath);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "";
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="login_url">登录地址</param>
    /// <param name="waitfor">等待出现的元素</param>
    /// <returns>返回空值 表示登录成功，否则错误</returns>
    protected async Task<string> LoginCustom(string LoginPage, string waitfor, WaitForSelectorState? VisibleState = WaitForSelectorState.Visible)
    {
        try
        {
            await using var context = await PlaywrightUtils.CreatePlaywrightAsync();
            var page = await context.NewPageAsync();
            page.SetDefaultTimeout(0);
            await page.GotoAsync(LoginPage);
            //停一下再获取内容，是因为异步加载的内容有时没有那么快出来
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle); // 等待页面加载完毕

            await page.WaitForSelectorAsync(waitfor, new()
            {
                Timeout = 60000 * 10, // 10分钟内登录完成
                State = VisibleState // 默认确保元素可见
            });

            var storageState = await context.StorageStateAsync();
            PlaywrightUtils.SaveState(storageState, StatePath);
        }
        catch (Exception ex)
        {

            return $"登录Playwright发生异常：{ex.Message}";
        }

        return "";

    }
    /// <summary>
    /// 滚动到页面底部
    /// </summary>
    /// <param name="page"></param>
    protected async Task ScrollToBottom(IPage page)
    {
        await page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight);");
        await page.WaitForTimeoutAsync(2000);
    }


    #region 专为API管理的平台提供方法
    /// <summary>
    /// 通过API获取频道信息
    /// </summary>
    /// <returns>返回MarkDown格式的文本</returns>
    //virtual public Task<(string, ChannelInfo)> GetChannelInfo() => null;
    //abstract protected Task<string> ActionsAsync(PushInfo? model,Action<string>? CallBack);
    virtual public Task<string> GetUserInfo() => null;

    /// <summary>
    /// 下载视频分类信息
    /// </summary>
    /// <returns>错误信息</returns>
    virtual public Task<string> UpdateClassData()
    {
        return Task.FromResult("");
    }
    /// <summary>
    /// 获取在线平台上的内容
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    virtual public Task<List<ContentFromSite>> GetDataList(int limit = 30)
    {
        return Task.FromResult(new List<ContentFromSite>());
    }
    /// <summary>
    /// 获取分类列表
    /// </summary>
    /// <returns>Dictionary<分类名称,分类ID></returns>
    virtual public Dictionary<string, string> GetClassData()
    {
        return new Dictionary<string, string>();
    }


    /// <summary>
    /// 从分类名称获取分类ID
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <param name="useCache">是否使用缓存（默认true）</param>
    /// <returns>分类ID，未找到返回null</returns>
    public string GetCategoryIdByName(string categoryName, bool useCache = true)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            PrintLog("警告：分类名称不能为空");
            return null;
        }

        try
        {
            // 获取分类字典（支持缓存优化）
            Dictionary<string, string> categoryDict;

            if (useCache && _categoryCache != null && _categoryCache.Count > 0)
            {
                categoryDict = _categoryCache;
            }
            else
            {
                categoryDict = GetClassData();
                if (useCache)
                {
                    _categoryCache = categoryDict; // 缓存结果
                }
            }

            if (categoryDict == null || categoryDict.Count == 0)
            {
                PrintLog("警告：分类字典为空，请先调用UpdateClassData生成数据");
                return null;
            }

            // 精确查找（区分大小写）
            if (categoryDict.TryGetValue(categoryName, out string categoryId))
            {
                //PrintLog($"找到分类：{categoryName} -> {categoryId}");
                return categoryId;
            }

            // 模糊查找（不区分大小写）
            var matchedEntry = categoryDict.FirstOrDefault(
                kvp => string.Equals(kvp.Key, categoryName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(matchedEntry.Key))
            {
                PrintLog($"模糊匹配找到分类：{matchedEntry.Key} -> {matchedEntry.Value}");
                return matchedEntry.Value;
            }

            // 包含查找
            var containsMatch = categoryDict.FirstOrDefault(
                kvp => kvp.Key.Contains(categoryName, StringComparison.OrdinalIgnoreCase) ||
                       categoryName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(containsMatch.Key))
            {
                PrintLog($"包含匹配找到分类：{containsMatch.Key} -> {containsMatch.Value}");
                PrintLog($"提示：您输入的是 '{categoryName}'，匹配到 '{containsMatch.Key}'");
                return containsMatch.Value;
            }

            PrintLog($"未找到分类：{categoryName}");
            return null;
        }
        catch (Exception ex)
        {
            PrintLog($"获取分类ID时发生错误：{ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 从分类ID获取分类名称
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <param name="useCache">是否使用缓存（默认true）</param>
    /// <returns>分类名称，未找到返回null</returns>
    public string GetCategoryNameById(string categoryId, bool useCache = true)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
        {
            PrintLog("警告：分类ID不能为空");
            return null;
        }

        try
        {
            // 获取分类字典
            Dictionary<string, string> categoryDict;

            if (useCache && _categoryCache != null && _categoryCache.Count > 0)
            {
                categoryDict = _categoryCache;
            }
            else
            {
                categoryDict = GetClassData();
                if (useCache)
                {
                    _categoryCache = categoryDict;
                }
            }

            if (categoryDict == null || categoryDict.Count == 0)
            {
                PrintLog("警告：分类字典为空，请先调用UpdateClassData生成数据");
                return null;
            }

            // 精确查找ID
            var matchedEntry = categoryDict.FirstOrDefault(
                kvp => string.Equals(kvp.Value, categoryId, StringComparison.Ordinal));

            if (!string.IsNullOrEmpty(matchedEntry.Key))
            {
                //PrintLog($"找到分类：{categoryId} -> {matchedEntry.Key}");
                return matchedEntry.Key;
            }

            // 如果未找到，检查是否需要处理null值
            if (categoryId == "null" || categoryId == "NULL")
            {
                // 查找值为null的分类
                var nullEntry = categoryDict.FirstOrDefault(kvp => kvp.Value == null);
                if (!string.IsNullOrEmpty(nullEntry.Key))
                {
                    PrintLog($"找到值为null的分类：{nullEntry.Key}");
                    return nullEntry.Key;
                }
            }

            PrintLog($"未找到分类ID：{categoryId}");
            return null;
        }
        catch (Exception ex)
        {
            PrintLog($"获取分类名称时发生错误：{ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取所有分类的逆向字典（ID->名称）
    /// </summary>
    /// <returns>ID到名称的字典</returns>
    public Dictionary<string, string> GetCategoryIdToNameDictionary()
    {
        var result = new Dictionary<string, string>();

        try
        {
            var categoryDict = GetClassData();

            if (categoryDict == null || categoryDict.Count == 0)
            {
                return result;
            }

            foreach (var kvp in categoryDict)
            {
                string categoryId = kvp.Value;
                string categoryName = kvp.Key;

                if (!string.IsNullOrEmpty(categoryId))
                {
                    // 处理重复的ID（虽然YouTube ID应该是唯一的）
                    if (result.ContainsKey(categoryId))
                    {
                        PrintLog($"警告：发现重复的分类ID：{categoryId}");
                        // 可以合并名称或跳过
                        result[categoryId] = $"{result[categoryId]}/{categoryName}";
                    }
                    else
                    {
                        result[categoryId] = categoryName;
                    }
                }
            }

            PrintLog($"生成逆向字典，共 {result.Count} 个有ID的分类");
        }
        catch (Exception ex)
        {
            PrintLog($"生成逆向字典时发生错误：{ex.Message}");
        }

        return result;
    }

    // 缓存字段（可选）
    private Dictionary<string, string> _categoryCache = null;


    #endregion

    /// <summary>
    /// 修复文件路径格式
    /// </summary>
    protected string GetFullPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        // 移除开头的 .\\
        if (path.StartsWith(".\\") || path.StartsWith("./"))
        {
            path = path.Substring(2);
        }
        else if (path.StartsWith(".\\\\") || path.StartsWith(".//"))
        {
            path = path.Substring(3);
        }

        // 替换双反斜杠为单反斜杠
        path = path.Replace("\\\\", "\\").Replace("//", "/");

        // 如果还不是绝对路径，转换为绝对路径
        if (!Path.IsPathRooted(path))
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(baseDir, path);
        }

        return path;
    } 
}
