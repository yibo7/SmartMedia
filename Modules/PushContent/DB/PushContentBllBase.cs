using SmartMedia.Controls;
using SmartMedia.MCore;
using SmartMedia.Plugins;
using SmartMedia.Plugins.AutoPush;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using XS.Core2.XsExtensions;
using ZstdSharp.Unsafe;

namespace SmartMedia.Modules.PushContent.DB
{
    abstract public class PushContentBllBase : LiteDbInt<PushInfo>
    {
        abstract protected int IType { get; }
        abstract public string Title { get; }
        /// <summary>
        /// 打开添加窗口
        /// </summary>
        abstract public void OnOpenAdd(long Id);
        //abstract  protected Task PushOneTask(long Id, Action<string,int,int> CallBack);
        abstract public Task StartPush(long Id, Action<string, int, int> CallBack);

        virtual public string ImportData(List<ImportModel> importModels) {

            var sbErr = new StringBuilder();
            foreach (var item in importModels) {
                var model = new PushInfo();

                model.Title = item.Title.Trim();
                model.Tags = item.TagsString;
                //model.Status = 0;
                model.Original = item.Original;
                model.Info = string.IsNullOrWhiteSpace(item.Description)?"": item.Description;

                model.FilePath = item.FilePath;
                model.ImgPath = item.ImgPath;
                 

                model.Sites = item.Sites;

                model.PublishTimeStamp = item.PublishTimeStamp;
                 
                var rz = this.AddData(model);
                if (!string.IsNullOrEmpty(rz.Item1))
                {
                    sbErr.AppendLine($"添加【{model.Title}】发生错误：{rz.Item1}");
                }
            }
            return sbErr.ToString();
        }

        abstract public SiteSelector NewSiteSelector();
        public List<PushInfo> Search(int status = -1, string keyword = "")
        {
            int limit = 1000;
            string query = $"IType = {IType}";

            if (!string.IsNullOrEmpty(keyword))
            {
                limit = 10000;
                query += $" AND Title.contains('{keyword}')";
            }

            if (status > -1)
            {
                query += $" AND Status = {status}";
            }

            // 按位置传参：query, limit, orderBy, descending
            return Find(query, limit, null, false);
        }

        /// <summary>
        /// 获取所有定时发布的任务
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<PushInfo> GetScheduledContents(int limit = 100)
        {
            // 查询条件：状态为0（未发布）且publish_time > 0（已设置定时时间）
            string query = "Status = 0 AND PublishTimeStamp > 0";

            // 按发布时间升序排序（最先发布的排前面）
            string orderBy = "PublishTimeStamp";

            // 使用你的Find方法，传入查询条件、限制数量、排序字段
            return Find(query, limit, orderBy, false);
        }

        protected async Task PushTask<T>(long Id, Action<string, int, int> CallBack) where T : PushBase
        {
            await Task.Delay(1000);

            var model = GetEntity(Id);
            int pushStatus = -1;
            StringBuilder sbPushLogs = new StringBuilder();
            try
            {
                var siteSettings = model.ConverSiteSettingToDict();

                int iMax = siteSettings.Count;

                CallBack($"开始上传【{model.Title}】...", iMax, 0);
                await Task.Delay(1000);
                int CurrentCount = 0;
                
                foreach (var site in siteSettings)
                {
                    var pushPlugin = PluginUtils.GetByClasName<T>(site.Key);

                    if (Equals(pushPlugin, null))
                    {

                        CallBack($"【{site.Key}】插件不存在！", iMax, CurrentCount);

                        sbPushLogs.AppendLine($"【{site.Key}】插件不存在！");

                        await Task.Delay(5000);
                        continue;
                    }

                    //pushPlugin.CustomConfigs = site.Value;

                    pushPlugin.SetCtrValueToPush(site.Value); //更新站点的发布参数，方便实现插件获取最新数据。

                    CallBack($"【{pushPlugin.PluginName}】上传中...", iMax, CurrentCount);
                    string err = await pushPlugin.Start(model, (msg) => {
                        CallBack(msg, iMax, CurrentCount);
                    });
                    CurrentCount++;
                    if (string.IsNullOrEmpty(err))
                    {
                        pushStatus = 1;
                        CallBack($"【{pushPlugin.PluginName}】上传完毕！", iMax, CurrentCount);
                        sbPushLogs.AppendLine($"【{pushPlugin.PluginName}】上传完毕！");

                    }
                    else
                    {
                        CallBack($"【{pushPlugin.PluginName}】上传有错误：{err}！", iMax, CurrentCount);
                        sbPushLogs.AppendLine($"【{pushPlugin.PluginName}】上传有错误：{err}！");
                        await Task.Delay(10000);
                    }

                }
            }
            catch (Exception ex)
            {               
                sbPushLogs.AppendLine($"发布发生异常：{ex.Message}！");
                Debug.WriteLine(sbPushLogs);
            }

            model.Status = pushStatus;
            model.PublishLog = sbPushLogs.ToString();

            Update(model);
        }


        private string GetVideoMd5(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                using (var crypto = MD5.Create())
                {
                    var md5Hash = crypto.ComputeHash(fs);
                    return BitConverter.ToString(md5Hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        public void UpdateStatusToPush(PushInfo model)
        {
            model.Status = 1;
            base.Update(model);
        }
        public bool IsHave(string video_path)
        {
            string md5 = GetVideoMd5(video_path);
            return base.ExistsWhere($"md_wu='{md5}'");
        }
        public (string, long) AddData(PushInfo model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return ("标题不能为空", 0);
            }

            string sMd5 = ""; 
            if (string.IsNullOrWhiteSpace(model.FilePath) && (IType == 1 || IType == 2 || IType == 4))
            {
                sMd5 = model.FilePath.Md5();
                string tip = "路径不能为空";
                switch (IType)
                {
                    case 1:
                        tip = "视频路径不能为空";
                        break;
                    case 2:
                        tip = "音频路径不能为空";
                        break;
                    case 4:
                        tip = "需要添加图片集";
                        break;
                    default:
                        break;
                }
                return (tip, 0);
            }
            else 
            {
                sMd5  = model.Title.Md5();
            }

            //if (string.IsNullOrWhiteSpace(model.Tags) && IType == 1)
            //    return ("至少要填写一个标签", 0);

            model.IType = this.IType;

            model.Tags = model.Tags.Replace("＃", "#"); 

            model.CreateTime = DateTime.Now;
            model.CreateTimeInt = XS.Core2.SqlDateTimeInt.GetMilliSecond();

            model.MdWu = sMd5;// $"{model.FilePath}{model.Sites}".Md5();


            if (model.Id > 0)
            {
                this.Update(model);
            }
            else
            {
                if (this.ExistsWhere($"md_wu='{model.MdWu}'"))
                    return ("已经存在相同记录", 0);
                model.Status = 0;
                long Id = this.Add(model);
                model.Id = Id;
            }


            return ("", model.Id);
        }
    }
}
