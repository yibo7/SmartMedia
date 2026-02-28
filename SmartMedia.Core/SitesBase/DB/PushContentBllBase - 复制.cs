//using Newtonsoft.Json.Linq;
//using SmartMedia.Core.Controls;
//using System.Diagnostics;
//using System.Security.Cryptography;
//using System.Text;
//using XS.Core2.XsExtensions;
//using ZstdSharp.Unsafe;

//namespace SmartMedia.Core.SitesBase.DB;

//abstract public class PushContentBllBase : LiteDbInt<PushInfo>
//{
//    abstract public int IType { get; }
//    abstract public string Title { get; }
//    /// <summary>
//    /// 打开添加窗口
//    /// </summary>
//    abstract public void OnOpenAdd(long Id); 
//    abstract public Task StartPush(long Id, Action<string, int, int> CallBack);

//    virtual public string ImportData(List<ImportModel> importModels) {

//        var sbErr = new StringBuilder();
//        foreach (var item in importModels) {
//            var model = new PushInfo();

//            model.Title = item.Title.Trim();
//            model.Tags = item.TagsString;
//            //model.Status = 0;
//            model.Original = item.Original;
//            model.Info = string.IsNullOrWhiteSpace(item.Description)?"": item.Description;

//            model.FilePath = item.FilePath;
//            model.ImgPath = item.ImgPath;
            
//            model.ClassId = item.ClassId;   

//            model.Sites = item.Sites;

//            model.PublishTimeStamp = item.PublishTimeStamp;
             
//            var rz = this.AddData(model);
//            if (!string.IsNullOrEmpty(rz.Item1))
//            {
//                sbErr.AppendLine($"添加【{model.Title}】发生错误：{rz.Item1}");
//            }
//        }
//        return sbErr.ToString();
//    }

//    abstract public SiteSelector NewSiteSelector();
//    public List<PushInfo> Search(int status = -1, string keyword = "",int ClassId=0)
//    {
//        int limit = 1000;
//        string query = $"IType = {IType}";

//        if (!string.IsNullOrEmpty(keyword))
//        {
//            limit = 10000;
//            query += $" AND Title.contains('{keyword}')";
//        }

//        if (status > -1)
//        {
//            query += $" AND Status = {status}";
//        }

//        if (ClassId > 0)
//        {
//            query += $" AND ClassId = {ClassId}";
//        }

//        // 按位置传参：query, limit, orderBy, descending
//        return Find(query, limit, null, false);
//    }

//    /// <summary>
//    /// 获取所有定时发布的任务
//    /// </summary>
//    /// <param name="limit"></param>
//    /// <returns></returns>
//    public List<PushInfo> GetScheduledContents(int limit = 100)
//    {
//        // 查询条件：状态为0（未发布）且publish_time > 0（已设置定时时间）
//        string query = "Status = 0 AND PublishTimeStamp > 0";

//        // 按发布时间升序排序（最先发布的排前面）
//        string orderBy = "PublishTimeStamp";

//        // 使用你的Find方法，传入查询条件、限制数量、排序字段
//        return Find(query, limit, orderBy, false);
//    }

//    protected async Task PushTask<T>(long Id, Action<string, int, int> CallBack) where T : PushBase
//    {
//        await Task.Delay(1000);

//        var model = GetEntity(Id);
//        int pushStatus = -1;
//        StringBuilder sbPushLogs = new StringBuilder();
//        try
//        {
//            var siteSettings = model.ConverSiteSettingToDict();

//            int iMax = siteSettings.Count;

//            CallBack($"开始上传【{model.Title}】...", iMax, 0);
//            await Task.Delay(1000);
//            int CurrentCount = 0;
            
//            foreach (var site in siteSettings)
//            {
//                var pushPlugin = PluginUtils.GetByClasName<T>(site.Key);

//                if (Equals(pushPlugin, null))
//                {

//                    CallBack($"【{site.Key}】插件不存在！", iMax, CurrentCount);

//                    sbPushLogs.AppendLine($"【{site.Key}】插件不存在！");

//                    await Task.Delay(5000);
//                    continue;
//                }

//                //pushPlugin.CustomConfigs = site.Value;

//                pushPlugin.SetCtrValueToPush(site.Value); //更新站点的发布参数，方便实现插件获取最新数据。

//                CallBack($"【{pushPlugin.PluginName}】上传中...", iMax, CurrentCount);
//                string err = await pushPlugin.Start(model, (msg) => {
//                    Debug.WriteLine(msg);
//                    CallBack(msg, iMax, CurrentCount);
//                });
//                CurrentCount++;
//                if (string.IsNullOrEmpty(err))
//                {
//                    pushStatus = 1;
//                    CallBack($"【{pushPlugin.PluginName}】上传完毕！", iMax, CurrentCount);
//                    sbPushLogs.AppendLine($"【{pushPlugin.PluginName}】上传完毕！");

//                }
//                else
//                {
//                    CallBack($"【{pushPlugin.PluginName}】上传有错误：{err}！", iMax, CurrentCount);
//                    sbPushLogs.AppendLine($"【{pushPlugin.PluginName}】上传有错误：{err}！");
//                    await Task.Delay(10000);
//                }

//            }
//        }
//        catch (Exception ex)
//        {               
//            sbPushLogs.AppendLine($"发布发生异常：{ex.Message}！");
//            Debug.WriteLine(sbPushLogs);
//        }

//        model.Status = pushStatus;
//        model.PublishLog = sbPushLogs.ToString();

//        Update(model);
//    }


//    private string GetVideoMd5(string path)
//    {
//        using (FileStream fs = File.OpenRead(path))
//        {
//            using (var crypto = MD5.Create())
//            {
//                var md5Hash = crypto.ComputeHash(fs);
//                return BitConverter.ToString(md5Hash).Replace("-", "").ToLowerInvariant();
//            }
//        }
//    }
//    public void UpdateStatusToPush(PushInfo model)
//    {
//        model.Status = 1;
//        base.Update(model);
//    }
//    public bool IsHave(string video_path)
//    {
//        string md5 = GetVideoMd5(video_path);
//        return base.ExistsWhere($"md_wu='{md5}'");
//    }
//    public (string, long) AddData(PushInfo model)
//    {
//        if (string.IsNullOrWhiteSpace(model.Title))
//        {
//            return ("标题不能为空", 0);
//        }

//        string sMd5 = ""; 
//        if (string.IsNullOrWhiteSpace(model.FilePath) && (IType == 1 || IType == 2 || IType == 4))
//        {
//            sMd5 = model.FilePath.Md5();
//            string tip = "路径不能为空";
//            switch (IType)
//            {
//                case 1:
//                    tip = "视频路径不能为空";
//                    break;
//                case 2:
//                    tip = "音频路径不能为空";
//                    break;
//                case 4:
//                    tip = "需要添加图片集";
//                    break;
//                default:
//                    break;
//            }
//            return (tip, 0);
//        }
//        else 
//        {
//            sMd5  = model.Title.Md5();
//        }

//        //if (string.IsNullOrWhiteSpace(model.Tags) && IType == 1)
//        //    return ("至少要填写一个标签", 0);

//        model.IType = this.IType;

//        model.Tags = model.Tags.Replace("＃", "#"); 

//        model.CreateTime = DateTime.Now;
//        model.CreateTimeInt = XS.Core2.SqlDateTimeInt.GetMilliSecond();

//        model.MdWu = sMd5;// $"{model.FilePath}{model.Sites}".Md5();

//        CopyModelFiles(model);
//        if (model.Id > 0)
//        {
//            this.Update(model);
//        }
//        else
//        {
//            if (this.ExistsWhere($"md_wu='{model.MdWu}'"))
//                return ("已经存在相同记录", 0);
//            model.Status = 0;
//            long Id = this.Add(model);
//            model.Id = Id;
//        }

       

//        return ("", model.Id);
//    }

//    private PushInfo CopyModelFiles(PushInfo model)
//    {

//        if (Settings.Instance.IsCopyFiles == 1)
//        {
//            // 主逻辑代码 - 调用非常简洁 
//            // 
//            if (model.IType == 4)
//            {
//                // 处理多个图片文件路径
//                model.FilePath = ProcessMultipleImageFiles(model.FilePath, model.ClassId);
//            }
//            else
//            {
//                model.FilePath = CopyFileToSubDirectory(model.FilePath, $"Media", model.ClassId, "文件");
//            }
//            model.ImgPath = CopyFileToSubDirectory(model.ImgPath, "Images", model.ClassId, "图片文件");


//            // 处理 Sites 中的图片
//            if (!string.IsNullOrEmpty(model.Sites))
//            {
//                try
//                {
//                    // 解析 JSON
//                    var sitesJson = JObject.Parse(model.Sites);

//                    // 遍历所有的平台（如 YouTube、Bilibili 等）
//                    foreach (var site in sitesJson)
//                    {
//                        var platformName = site.Key;
//                        var platformData = site.Value as JObject;

//                        if (platformData != null)
//                        {
//                            // 获取所有属性
//                            var properties = platformData.Properties().ToList();

//                            foreach (var property in properties)
//                            {
//                                // 检查属性名是否以 "picCoverImage" 开头
//                                if (property.Name.StartsWith("picCoverImage"))
//                                {
//                                    string imagePath = property.Value?.ToString();

//                                    if (!string.IsNullOrEmpty(imagePath))
//                                    {
//                                        try
//                                        {
//                                            // 复制图片文件并获取新路径
//                                            string newImagePath = CopyFileToSubDirectory(
//                                                imagePath,
//                                                "Images", model.ClassId,
//                                                $"{platformName}_{property.Name}"
//                                            );

//                                            // 更新 JSON 中的路径
//                                            platformData[property.Name] = newImagePath;

//                                            //Debug.WriteLine($"已更新 {platformName} 的 {property.Name}: {newImagePath}");
//                                        }
//                                        catch (Exception ex)
//                                        {
//                                            Debug.WriteLine($"处理 {platformName} 的 {property.Name} 时出错: {ex.Message}");
//                                            // 可以选择保留原路径或设置为空
//                                            // platformData[property.Name] = null;
//                                        }
//                                    }
//                                }
//                                // 检查属性名是否等于 "LrcSrt"
//                                else if (property.Name == "LrcSrt")
//                                {
//                                    string lrcSrtPath = property.Value?.ToString();

//                                    if (!string.IsNullOrEmpty(lrcSrtPath))
//                                    {
//                                        try
//                                        {
//                                            // 复制字幕/歌词文件并获取新路径
//                                            string newLrcSrtPath = CopyFileToSubDirectory(
//                                                lrcSrtPath,
//                                                "Lrc", model.ClassId,
//                                                $"{platformName}_{property.Name}"
//                                            );

//                                            // 更新 JSON 中的路径
//                                            platformData[property.Name] = newLrcSrtPath;

//                                            Debug.WriteLine($"已更新 {platformName} 的 {property.Name}: {newLrcSrtPath}");
//                                        }
//                                        catch (Exception ex)
//                                        {
//                                            Debug.WriteLine($"处理 {platformName} 的 {property.Name} 时出错: {ex.Message}");
//                                            // 可以选择保留原路径或设置为空
//                                            // platformData[property.Name] = null;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    // 更新 model.Sites 为修改后的 JSON
//                    model.Sites = sitesJson.ToString();
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine($"解析或处理 Sites JSON 时出错: {ex.Message}");
//                }
//            }


//        }

//        return model;
//    }

//    // 处理多个图片文件路径的方法
//    private string ProcessMultipleImageFiles(string filePaths, int SpecialId)
//    {
//        if (string.IsNullOrEmpty(filePaths))
//            return filePaths;

//        // 按换行符分割路径（支持 \n 或 \r\n）
//        var pathArray = filePaths.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

//        if (pathArray.Length == 0)
//            return filePaths;

//        // 处理每个路径
//        var newPaths = new List<string>();

//        foreach (var originalPath in pathArray)
//        {
//            string trimmedPath = originalPath.Trim();

//            if (!string.IsNullOrEmpty(trimmedPath))
//            {
//                // 复制图片到 Images 目录
//                string newPath = CopyFileToSubDirectory(trimmedPath, "Images", SpecialId, "多图片文件");
//                newPaths.Add(newPath);
//            }
//        }

//        // 将新路径用换行符连接起来
//        return string.Join("\n", newPaths);
//    }
//    // 封装的函数：将文件复制到指定的子目录，返回新路径
//    private string CopyFileToSubDirectory(string sourceFilePath, string subFolder,int ClassId, string fileTypeDescription = "文件")
//    {
//        if (string.IsNullOrEmpty(sourceFilePath))
//            return sourceFilePath;

//        // 如果源文件不存在，直接返回原路径
//        if (!File.Exists(sourceFilePath))
//        {
//            Debug.WriteLine($"{fileTypeDescription}不存在: {sourceFilePath}");
//            return sourceFilePath;
//        }

//        try
//        {
//            string sClassId = Equals(ClassId,null)?"0":ClassId.ToString();

//            // 从配置中获取基础文件夹名称，并构建目标目录路径
//            string copyFolder = Settings.Instance.CopyFolder ?? "CopyFiles"; // 提供默认值
//            string targetDirectory = Path.Combine(".", copyFolder, subFolder, sClassId);

//            // 确保目标目录存在
//            Directory.CreateDirectory(targetDirectory);

//            // 获取文件名和扩展名
//            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
//            string extension = Path.GetExtension(sourceFilePath);

//            // 计算源文件的MD5值
//            string sourceFileMD5 = CalculateMD5(sourceFilePath);

//            // 构建初始目标路径
//            string targetFileName = fileName + extension;
//            string targetPath = Path.Combine(targetDirectory, targetFileName);

//            // 检查目标文件是否存在
//            if (File.Exists(targetPath))
//            {
//                // 计算已存在文件的MD5值
//                string existingFileMD5 = CalculateMD5(targetPath);

//                // 如果MD5相同，说明是同一个文件，直接返回现有路径
//                if (sourceFileMD5 == existingFileMD5)
//                {
//                    Debug.WriteLine($"{fileTypeDescription}已存在且内容相同，直接使用现有文件: {targetPath}");
//                    return targetPath;
//                }

//                // MD5不同，说明是不同文件，需要生成带数字序号的新文件名
//                Debug.WriteLine($"{fileTypeDescription}已存在但内容不同，将创建新版本");

//                int counter = 2; // 从2开始计数
//                bool fileExists;

//                do
//                {
//                    // 生成新文件名：原文件名_数字.扩展名
//                    targetFileName = $"{fileName}_{counter}{extension}";
//                    targetPath = Path.Combine(targetDirectory, targetFileName);

//                    // 如果文件存在，检查MD5是否相同
//                    if (File.Exists(targetPath))
//                    {
//                        string newFileMD5 = CalculateMD5(targetPath);

//                        // 如果MD5相同，直接返回这个现有文件
//                        if (sourceFileMD5 == newFileMD5)
//                        {
//                            Debug.WriteLine($"{fileTypeDescription}内容与现有文件相同: {targetPath}");
//                            return targetPath;
//                        }

//                        // 如果MD5不同，继续尝试下一个序号
//                        fileExists = true;
//                    }
//                    else
//                    {
//                        // 文件不存在，可以创建
//                        fileExists = false;
//                    }

//                    counter++;

//                    // 防止无限循环（理论上是不会的，但为了安全）
//                    if (counter > 1000) // 设置一个上限
//                    {
//                        Debug.WriteLine($"警告：已尝试{counter - 1}个序号，可能存在异常");
//                        break;
//                    }
//                } while (fileExists);
//            }

//            // 复制文件
//            File.Copy(sourceFilePath, targetPath);
//            Debug.WriteLine($"{fileTypeDescription}已复制到: {targetPath}");

//            // 返回新路径
//            return targetPath;
//        }
//        catch (Exception ex)
//        {
//            Debug.WriteLine($"复制{fileTypeDescription}时出错: {ex.Message}");
//            return sourceFilePath; // 出错时返回原路径
//        }
//    }

//    // 辅助函数：计算文件的MD5值
//    private string CalculateMD5(string filePath)
//    {
//        using (var md5 = System.Security.Cryptography.MD5.Create())
//        {
//            using (var stream = File.OpenRead(filePath))
//            {
//                byte[] hash = md5.ComputeHash(stream);
//                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
//            }
//        }
//    }
//}
