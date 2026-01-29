using XS.Core2.Json;

namespace SmartMedia.MCore
{
    public class JobSetModel : BaseJsonFile
    {
        public static readonly JobSetModel Instance = new JobSetModel();
        public Dictionary<string, string> Cron = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<string, string>> CustomConfigs = new Dictionary<string, Dictionary<string, string>>();

        private JobSetModel() : base("JobSetting/settings.json")
        {
            //string sPath = Path.Combine(Application.StartupPath, "JobSetting/settings.json");
            //if (!File.Exists(sPath))
            //{
            //   FObject.WriteFileUtf8(sPath,"{}");
            //}
        }
        public Dictionary<string, string> GetCustomConfigs(string jobId)
        {
            if (this.CustomConfigs.ContainsKey(jobId))
            {
                return this.CustomConfigs[jobId];
            }
            return null;
        }
        public void SaveCustomConfigs(string jobId, Dictionary<string, string> cf)
        {
            if (cf != null)
            {
                this.CustomConfigs[jobId] = cf;
                this.Save();

            }
        }
    }
    //public class JobSettings:Singleton<JobSettings>
    //{
    //    public JsonFile<JobSetModel> Config;
    //    public JobSettings() { 
    //        Config = new JsonFile<JobSetModel>($"{XS.WinFormsTools.Utils.GetCuurentPath()}\\JobSetting\\settings.json");
    //    }

    //    public Dictionary<string,string> GetCustomConfigs(string jobId)
    //    {
    //        if (this.Config.Inst.CustomConfigs.ContainsKey(jobId))
    //        {
    //            return Config.Inst.CustomConfigs[jobId];
    //        }
    //        return null;
    //    }
    //    public void SaveCustomConfigs(string jobId, Dictionary<string, string> cf)
    //    {
    //        if (cf!=null)
    //        {
    //            Config.Inst.CustomConfigs[jobId] = cf;
    //            Config.Save();  

    //        } 
    //    }
    //}
}
