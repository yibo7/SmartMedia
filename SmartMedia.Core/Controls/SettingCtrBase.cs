

using SmartMedia.Core.SitesBase;

namespace SmartMedia.Core.Controls
{
    public partial class SettingCtrBase : UserControl
    {
        public SettingCtrBase()
        {
            InitializeComponent();
        }

        protected PushBase PushInstance = null;

        public void InitPushBase(PushBase pushBase)
        {
            PushInstance = pushBase;
        }

        virtual public void InitData() { }

        virtual public void SetValue(string value) { }

        virtual public string GetValue() => "";

        virtual public void SetTitle(string value) { }
        virtual public void SetTips(string value) { }

        virtual public void InitData(string value) { }
    }
}
