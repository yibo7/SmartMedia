 
using SmartMedia.Core; 
using XS.Core2;

namespace SmartMedia
{
    public partial class PluginConfigs : XS.WinFormsTools.WinMiniformBase
    {
        private PluginBase currentPlugin;
        public PluginConfigs(PluginBase plugin)
        {
            currentPlugin = plugin;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            gvSettings.Columns.Add("Key", "配置名称");
            gvSettings.Columns.Add("Value", "值");
            gvSettings.AllowUserToAddRows = false;
            gvSettings.CellEndEdit += gvSettings_CellEndEdit;

            gvSettings.Columns[0].ReadOnly = true;
            gvSettings.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.Text = currentPlugin.PluginName;

            
        }
        //private async Task InitWebView()
        //{
        //    await webViewHelp.EnsureCoreWebView2Async();
        //    // 等待页面加载完成
        //    webViewHelp.CoreWebView2.NavigationCompleted += (sender, args) =>
        //    {
        //        if (args.IsSuccess)
        //        {
        //            // 使用 JavaScript 给所有超链接添加 target="_blank"
        //            webViewHelp.CoreWebView2.ExecuteScriptAsync("document.querySelectorAll('a').forEach(function(a) { a.target='_blank'; });");
        //        }
        //    };
        //}


        private void ConfigBind()
        {
            gvSettings.Rows.Clear();
            toolMainBar.Items.Clear();

            if (!Equals(currentPlugin, null))
            {
                var lbTitle = new ToolStripLabel();
                lbTitle.Text = $"{currentPlugin.PluginName}自定义配置";
                toolMainBar.Items.Add(lbTitle);

                var cf = currentPlugin.CustomConfigs;
                if (cf != null && cf.Count > 1)
                {
                    foreach (var item in cf)
                    {
                        if (item.Key == "IsEnable")
                        {
                            continue;
                        }

                        gvSettings.Rows.Add(item.Key, item.Value);

                    }
                }

                var reversedBtns = currentPlugin.BtnActionNames.Select(kvp => kvp)
                         .Reverse()
                         .ToList();
                foreach (var item in reversedBtns)
                {

                    // 创建ToolStripButton
                    ToolStripButton toolBtn = new ToolStripButton();
                    toolBtn.Alignment = ToolStripItemAlignment.Right; // 设置按钮右对齐 
                    toolBtn.Text = item.Key;
                    var ico = Resource.ResourceManager.GetObject(item.Value);
                    if (ico != null)
                    {
                        toolBtn.Image = (Bitmap)ico;

                    }

                    //toolBtn.Name = item;
                    toolBtn.Click += new EventHandler((object sender, EventArgs e) =>
                    {
                        // 将sender转换为ToolStripButton类型
                        //ToolStripButton clickedButton = sender as ToolStripButton;
                        currentPlugin.OnBtnActionAsync(item.Key);
                    });
                    toolMainBar.Items.Add(toolBtn);
                }

            }

        }


        private void gvSettings_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var CustomConfigs = currentPlugin.CustomConfigs;
            foreach (DataGridViewRow row in gvSettings.Rows)
            {
                if (!row.IsNewRow)
                {
                    try
                    {
                        // 获取当前行的K和V列的值
                        string key = XsUtils.ObjectToStr(row.Cells["Key"].Value);
                        string value = XsUtils.ObjectToStr(row.Cells["Value"].Value);
                        if (!string.IsNullOrEmpty(value))
                            CustomConfigs[key] = value.Trim();
                    }
                    catch (Exception)
                    {

                    }

                }
            }
            currentPlugin.SaveCf();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (item.Checked)
            //{
            //    plugin.CustomConfigs["IsEnable"] = "true";

            //}
            //else
            //{
            //    plugin.CustomConfigs["IsEnable"] = "false";
            //}
            currentPlugin.SaveCf();
            this.Close();
        }

        private void PluginConfigs_Load(object sender, EventArgs e)
        {
            ConfigBind();
        }
    }
}
