
using SmartMedia.Core.Controls;
using SmartMedia.Sites.Videos.Youtube;
namespace SmartMedia.Plugins.AutoPush.Video.Youtube
{
    public partial class YoutubePlaylist : SettingCtrBase
    {
        public YoutubePlaylist()
        {
            InitializeComponent();
            this.Load += YoutubePlaylist_Load;
        }

        private string DefaultValue = "";
        /// <summary>
        /// DownPlaylist是异步加载，所以先执行SetValue后执行DownPlaylist，这里只记录要赋值的数据，DownPlaylist加载完成后再设置DefaultValue
        /// </summary>
        /// <param name="value"></param>
        override public void SetValue(string value)
        {
            DefaultValue = value; 
        }

        override public string GetValue()
        {
            string selectedId = cbCategory1.SelectedValue?.ToString();
            return selectedId;

        }
        private void YoutubePlaylist_Load(object? sender, EventArgs e)
        {
            _ = DownPlaylist(false);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            btnUpdate.Enabled = false;
            _ = DownPlaylist(true);
        } 
        private async Task DownPlaylist(bool IsRefesh)
        {
            YouTube pushBll = PushInstance as YouTube;
            var dicPlayList = await pushBll.GetPlayListAsync(IsRefesh);
            if (dicPlayList == null || dicPlayList.Count == 0)
            { 
                cbCategory1.Items.Clear();
                MessageBox.Show("没有播放列表");
                return;
            }

            // 方法1.1：转换为KeyValuePair列表

            var dataPlayList = dicPlayList.ToList();
            cbCategory1.DataSource = dataPlayList;
            cbCategory1.DisplayMember = "Value";
            cbCategory1.ValueMember = "Key";
            
            cbCategory1.SelectedValue = DefaultValue;

            btnUpdate.Enabled = true;
        }
    }
}
