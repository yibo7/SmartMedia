using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase; 
using System.ComponentModel;


namespace SmartMedia.Modules.PushContent
{
    public partial class PushSettingBase : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<bool, PushBase> OnEnable { get; set; }
        private PushBase PushModel;
        public PushSettingBase(PushBase pushModel)
        {
            PushModel = pushModel;
            InitializeComponent();


            this.Dock = DockStyle.Fill;

            picSiteIco.Image = PushModel.IcoName;
            lbTitle.Text = pushModel.PluginName;
            panelPrams.Enabled = swIsEnable.Checked;
            flpPanel.FlowDirection = FlowDirection.TopDown; // 设置布局方向为从上到下


        }
        bool IsCtrlInited = false;
        private Dictionary<string, SettingCtrBase> _dynamicControls = new Dictionary<string, SettingCtrBase>();

        /// <summary>
        /// 初始化控件
        /// </summary>
        /// <param name="keyValues">是否指定控件的值，编辑数据时可以指定</param>
        public void InitCtrs(Dictionary<string, string> keyValues = null)
        {
            if (IsCtrlInited)
                return;

            foreach (var ctrSeting in PushModel.GetSiteCtrls()) 
            { 
                string ctrKey = ctrSeting.Key;
                SettingCtrBase ucc = ctrSeting.Value;
                string newValue = "";
                if (!Equals(keyValues,null) && keyValues.ContainsKey(ctrKey))
                    newValue = keyValues[ctrKey];

                if (ucc != null)
                {
                    ucc.Name = ctrKey;
                    flpPanel.Controls.Add(ucc);
                    ucc.InitData();
                    ucc.SetValue(newValue);                     
                    // 保存对动态控件的引用
                    _dynamicControls[ctrKey] = ucc;
                }
                 

            }
            IsCtrlInited = true;
        } 
        public bool _IsSelected = false;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSelected
        {
            get {
                return _IsSelected;
            } 
            set{
                _IsSelected = value;
                swIsEnable.Checked = value; 
            }
        }
        private void swIsEnable_CheckStateChanged(object sender, EventArgs e)
        {
            if (!PushModel.IsAllowPush)
            {
                //swIsEnable.Checked = false;
                MessageBox.Show("暂不支持发布");
                return;
            }
            IsSelected = swIsEnable.Checked;
            panelPrams.Enabled = IsSelected;
            if (!Equals(OnEnable, null))
            {
                OnEnable(swIsEnable.CheckState == CheckState.Checked, PushModel);
            }
        }



        /// <summary>
        /// 获取发布插件-配置信息（用户保存发布内容时的操作）
        /// </summary>
        /// <returns></returns>
        virtual public (string, Dictionary<string, string>) GetSettings()
        {
            string sErr = ""; 

            Dictionary<string, string> CtrValues = new Dictionary<string, string>();

            //foreach (UserControl ctr in flpPanel.Controls)
            //{
            //    CtrValues[ctr.Name] = ctr.Text;
            //}
            // 使用保存的控件引用获取值
            foreach (var kvp in _dynamicControls)
            {
                var ctr = kvp.Value;
                // 使用控件特定的获取值方法，而不是通用的Text属性
                CtrValues[kvp.Key] = ctr.GetValue();
            }
            return (sErr, CtrValues);
        }

    }
}

