 
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase.DB;
using SmartMedia.Core.UIForms;
using SmartMedia.Core.XsExtensions; 
using System.Diagnostics;
using System.Text.Json;
using XS.Core2.XsExtensions;
using XS.WinFormsTools; 

namespace SmartMedia.Modules.VideoManageModule;


public partial class ImportDatas : XsDockContent
{


    private SiteSelector siteSelector;
    private IPushContentBll pushContentBll;
    public ImportDatas(IPushContentBll bllData)
    {
        pushContentBll = bllData;
        siteSelector = pushContentBll.NewSiteSelector();
        InitializeComponent();

        splitContainer1.Panel2.Controls.Add(siteSelector);
        splitContainer1.Size = new Size(1290, 734);
        splitContainer1.SplitterDistance = 504;
        splitContainer1.TabIndex = 42;

        siteSelector.Dock = DockStyle.Fill;
        siteSelector.Location = new Point(0, 0);
        siteSelector.Name = "siteSelector";
        siteSelector.Size = new Size(782, 734);
        siteSelector.TabIndex = 0;
         
        this.Text = pushContentBll.Title;

        classCombox.BindClass(pushContentBll.IType);

    }



    private void btnSave_Click(object sender, EventArgs e)
    {
        btnSave.Enabled = false;
        Task.Run(() => {

            string err = SaveData();
            btnSave.Enabled = true;

            if (string.IsNullOrEmpty(err))
            {
                lbErrInfo.ForeColor = Color.Green;
                err = "导入成功!";
            }
            else
            {
                lbErrInfo.ForeColor = Color.Red;
            }

            lbErrInfo.Text = err;

        });
    }
     


    private string SaveData()
    {
        (string err, Dictionary<string, Dictionary<string, string>>? dicSites) = siteSelector.GetValueSettings();

        if (!string.IsNullOrWhiteSpace(err))
        {
            Tips(err, TipsState.Error);
            return err;
        }
        if (Equals(importModels, null)|| importModels.Count<1)
            return "没选择目录或目录下数据为空!";
        // 注意：这里应该是直接调用 GeneratePublishTimes 方法
        // 而不是 publishTimes.GeneratePublishTimes
        Dictionary<int, DateTime> publishTimeDict = publishTimes.GeneratePublishTimes(importModels.Count); 
         

        int iIndex = 0;
        foreach (var model in importModels)
        {
            // 检查字典中是否有当前索引的时间
            if (publishTimeDict.ContainsKey(iIndex))
            {
                // 使用你的 DateTimeExtensions 将 DateTime 转换为时间戳
                DateTime publishTime = publishTimeDict[iIndex];
                model.PublishTimeStamp = publishTime.ToUnixTimeSeconds(); // 使用扩展方法

                // 如果需要毫秒级时间戳，可以使用：
                // model.PublishTimeStamp = publishTime.ToUnixTimeMilliseconds();
            }
            else
            {
                // 如果没有设置发布时间，可以根据需求处理
                model.PublishTimeStamp = 0; // 或者设置为默认值
            }

            model.Sites = dicSites?.ToJsonString() ?? "{}"; // 添加空值检查
            model.Original = cbIsOriginal.Checked?1:0;
            model.ClassId = classCombox.Value;
            iIndex++; // 索引递增
        }

        // 调用导入方法
        return pushContentBll.ImportData(importModels);
         
    }

    #region 导入数据处理

    /// <summary>
    /// 从目录中读取并序列化所有数据包文件
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>按序号排序的ImportModel列表</returns>
    private List<ImportModel> LoadImportModelsFromDirectory(string directoryPath)
    {
        var importModels = new List<ImportModel>();

        if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
        {
            Tips("目录不存在或路径为空！");
            return importModels;
        }

        try
        {
            // 获取目录下所有JSON文件
            var jsonFiles = Directory.GetFiles(directoryPath, "*.json");

            if (jsonFiles.Length == 0)
            {
                Tips("目录中没有找到JSON文件！");
                return importModels;
            }

            // 按文件名排序（支持两种命名格式）
            var sortedFiles = jsonFiles
                .Select(file => new
                {
                    FilePath = file,
                    FileName = Path.GetFileName(file),
                    Order = ParseFileOrder(file)
                })
                .Where(item => item.Order.HasValue) // 只处理能解析出序号的文件
                .OrderBy(item => item.Order.Value)  // 按序号排序
                .Select(item => item.FilePath)
                .ToList();

            // 加载并解析每个JSON文件
            foreach (var filePath in sortedFiles)
            {
                try
                {
                    var model = LoadImportModelFromFile(filePath);
                    if (model != null)
                    {
                        importModels.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"解析文件 {Path.GetFileName(filePath)} 时出错: {ex.Message}");
                    // 可以选择记录错误但继续处理其他文件
                }
            }
             
            

            return importModels;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"读取目录时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return importModels;
        }
    }

    /// <summary>
    /// 从文件名解析排序序号
    /// </summary>
    private int? ParseFileOrder(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        // 匹配两种命名格式：
        // 1. 纯数字： "1", "2", "123"
        // 2. 数字.其他内容： "1.文件名", "2.文件名"

        // 找到第一个数字序列
        string orderPart = string.Empty;
        bool foundDigit = false;

        foreach (char c in fileName)
        {
            if (char.IsDigit(c))
            {
                orderPart += c;
                foundDigit = true;
            }
            else if (foundDigit && (c == '.' || c == ' ' || c == '-'))
            {
                // 遇到分隔符停止（支持 .、空格、- 作为分隔符）
                break;
            }
            else if (foundDigit)
            {
                // 已经找到数字但遇到非分隔符字符，停止
                break;
            }
        }

        if (!string.IsNullOrEmpty(orderPart) && int.TryParse(orderPart, out int order))
        {
            return order;
        }

        return null; // 无法解析序号
    }

    /// <summary>
    /// 从单个JSON文件加载ImportModel
    /// </summary>
    private ImportModel LoadImportModelFromFile(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);

        // 使用 System.Text.Json 进行反序列化
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        var model = JsonSerializer.Deserialize<ImportModel>(jsonContent, options); 
        return model;
    } 

    /// <summary>
    /// 更新数据信息显示
    /// </summary>
    private void UpdateDataInfoLabel(int count, string directoryPath)
    {
        lbDataInfo.Text = $"共有{count}个数据包，来自目录：{directoryPath}";
    }
    private List<ImportModel> importModels;
    // 在你的界面代码中添加这个方法调用
    private void btnSelDataPath_Click(object sender, EventArgs e)
    {
        var folderPath = Dialogs.OpenSelFolder();
        if (!string.IsNullOrEmpty(folderPath))
        {
            // 加载数据包
            importModels = LoadImportModelsFromDirectory(folderPath);
            UpdateDataInfoLabel(importModels.Count, folderPath); 
        }
    }

    #endregion


    private void lbShowJsonMap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        string jsonHelp = @"
你可以按照如下 JSON 示例来生成导入数据包json文件：

```json
{
  ""Title"": ""示例标题"",
  ""Tags"":   ""Tags"": [
    ""智慧解脱"",
    ""正念修行"",
    ""哲王对话""
  ],
  ""Description"": ""这是内容或简介信息。"",
  ""FilePath"": ""/path/to/video.mp4,/path/to/audio.mp3,/path/to/image.jpg"",
  ""ImgPath"": ""/path/to/cover.jpg""
}
```
### 说明：

- **Title**：字符串，直接对应。
- **Tags**：字符串数组。
- Description: 内容或简介，纯文本字符串。
- **FilePath**：多个路径用英文逗号 `,` 分隔。
- **ImgPath**：单个封面图片路径。
 
### 数据包命令规则：
数据包通过命名序号排序，所以你应该这样命名数据包文件名称：
>序号.***.json

比如
```
1.json
2.json
3.json
......

```
如果你想更详细的命名，也可以这样：
```
1.烟雨孤村赐能名.json
2.柴钱微薪赎兄妹.json
3.破壁残灯聚一家.json
......

```

";

        base.OpenHelpDockToMain($"批量导入数据包制作指南", jsonHelp);
    }
}
