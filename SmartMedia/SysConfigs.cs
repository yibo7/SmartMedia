namespace SmartMedia
{
    public partial class SysConfigs : XS.WinFormsTools.WinMiniformBase
    {
        public SysConfigs()
        {
            InitializeComponent();

            cbIsOpenBrowser.Checked = Settings.Instance.IsOpenBrowser==1;
            cbIsCopyFiles.Checked = Settings.Instance.IsCopyFiles==1;
        }

        private void btnSaveConfigs_Click(object sender, EventArgs e)
        {
            Settings.Instance.IsOpenBrowser = cbIsOpenBrowser.Checked?1:0;
            Settings.Instance.IsCopyFiles = cbIsCopyFiles.Checked ? 1 : 0;
            Settings.Instance.Save();   

            XS.WinFormsTools.FrmTips.ShowTipsSuccess(this, "保存成功");

        }
    }
}
