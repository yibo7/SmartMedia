using System.ComponentModel;

namespace SmartMedia
{
    public partial class frmSplash : Form
    {
        public frmSplash()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.ShowInTaskbar = false;

            lvVerson.BackColor = Color.Transparent;
            lvVerson.FlatStyle = FlatStyle.Flat;
            lvVerson.UseMnemonic = false;
            lvVerson.AutoEllipsis = true;
            picStart.Controls.Add(lvVerson);


            // 设置lbStatus基于picStart背景透明
            lbStatus.BackColor = Color.Transparent;
            lbStatus.FlatStyle = FlatStyle.Flat;
            lbStatus.UseMnemonic = false;
            lbStatus.AutoEllipsis = true;
            picStart.Controls.Add(lbStatus);





        }
        private string _StatusInfo = "";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StatusInfo
        {
            set
            {
                _StatusInfo = value;
                ChangeStatusText();
            }
            get
            {
                return _StatusInfo;
            }
        }

        public void ChangeStatusText()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(this.ChangeStatusText));
                    return;
                }

                lbStatus.Text = _StatusInfo;
            }
            catch (Exception e)
            {
                //    异常处理
            }
        }
    }
}
