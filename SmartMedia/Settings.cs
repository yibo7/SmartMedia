using XS.Core2;

namespace SmartMedia
{
    public class Settings
    {
        public readonly static Settings Instance = new Settings();
        private IniParser iniParser;

        private Settings()
        {
            string sPath = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            if (sPath.EndsWith("\\bin"))
                sPath = sPath.Replace("\\bin", "");
#endif
            iniParser = new IniParser(string.Concat(sPath, @"\conf\conf.ini"));

            //app 
            Email = iniParser.GetSetting("App", "Email"); 
            MaxLastReport = iniParser.GetSettingInt("App", "MaxLastReport");
            IsOpenBrowser = iniParser.GetSettingInt("App", "IsOpenBrowser");
            IsCopyFiles = iniParser.GetSettingInt("App", "IsCopyFiles");

            IsTimerStart = iniParser.GetSettingInt("App", "IsTimerStart");

        }

        public void Save()
        {
            //app 
            iniParser.AddSetting("App", "Email", Email);
            iniParser.AddSetting("App", "IsOpenBrowser", IsOpenBrowser);
            iniParser.AddSetting("App", "IsCopyFiles", IsCopyFiles);
            iniParser.AddSetting("App", "IsTimerStart", IsTimerStart);

            iniParser.SaveSettings();

        }


        //app 
        /// <summary>
        /// 汇报Email
        /// </summary>
        public string Email { get; set; }
       
        /// <summary>
        /// 最后报告保留多少条
        /// </summary>
        public int MaxLastReport = 30;

        public int IsOpenBrowser = 0;

        public int IsCopyFiles = 0;

        public string CopyFolder = "CopyFiles";

        public int IsTimerStart = 0;

    }
}
