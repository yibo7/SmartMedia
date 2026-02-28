using SmartMedia.Core.Controls;
using XS.WinFormsTools;

namespace SmartMedia.Modules.VideoManageModule
{
    partial class ImportDatas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSave = new XsButton();
            splitContainer1 = new SplitContainer();
            pan1 = new Panel();
            classCombox = new ClassCombox();
            lbErrInfo = new Label();
            lbShowJsonMap = new LinkLabel();
            cbIsOriginal = new XS.WinFormsTools.Controls.CheckBox();
            publishTimes = new PublishTimes();
            label3 = new Label();
            lbDataInfo = new Label();
            btnSelDataPath = new XsButton();
            label10 = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            pan1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.DeepSkyBlue;
            btnSave.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
            btnSave.ConerRadius = 5;
            btnSave.Font = new Font("Segoe UI", 9F);
            btnSave.Location = new Point(147, 400);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(115, 42);
            btnSave.TabIndex = 16;
            btnSave.Text = "开始导入";
            btnSave.TextColor = Color.White;
            btnSave.Click += btnSave_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = Color.WhiteSmoke;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(pan1);
            splitContainer1.Size = new Size(1134, 643);
            splitContainer1.SplitterDistance = 440;
            splitContainer1.TabIndex = 42;
            // 
            // pan1
            // 
            pan1.BackColor = Color.White;
            pan1.Controls.Add(classCombox);
            pan1.Controls.Add(lbErrInfo);
            pan1.Controls.Add(btnSave);
            pan1.Controls.Add(lbShowJsonMap);
            pan1.Controls.Add(cbIsOriginal);
            pan1.Controls.Add(publishTimes);
            pan1.Controls.Add(label3);
            pan1.Controls.Add(lbDataInfo);
            pan1.Controls.Add(btnSelDataPath);
            pan1.Dock = DockStyle.Fill;
            pan1.Location = new Point(0, 0);
            pan1.Name = "pan1";
            pan1.Size = new Size(440, 643);
            pan1.TabIndex = 0;
            // 
            // classCombox
            // 
            classCombox.Location = new Point(250, 80);
            classCombox.Name = "classCombox";
            classCombox.Size = new Size(169, 38);
            classCombox.TabIndex = 77;
            // 
            // lbErrInfo
            // 
            lbErrInfo.ForeColor = Color.Red;
            lbErrInfo.Location = new Point(82, 471);
            lbErrInfo.Name = "lbErrInfo";
            lbErrInfo.Size = new Size(229, 99);
            lbErrInfo.TabIndex = 57;
            // 
            // lbShowJsonMap
            // 
            lbShowJsonMap.AutoSize = true;
            lbShowJsonMap.Location = new Point(303, 23);
            lbShowJsonMap.Name = "lbShowJsonMap";
            lbShowJsonMap.Size = new Size(116, 17);
            lbShowJsonMap.TabIndex = 56;
            lbShowJsonMap.TabStop = true;
            lbShowJsonMap.Text = "查看导入数据包格式";
            lbShowJsonMap.LinkClicked += lbShowJsonMap_LinkClicked;
            // 
            // cbIsOriginal
            // 
            cbIsOriginal.AutoSize = true;
            cbIsOriginal.Font = new Font("Segoe UI", 12F);
            cbIsOriginal.Location = new Point(27, 96);
            cbIsOriginal.Name = "cbIsOriginal";
            cbIsOriginal.Size = new Size(99, 20);
            cbIsOriginal.TabIndex = 55;
            cbIsOriginal.Text = "是否原创";
            cbIsOriginal.UseVisualStyleBackColor = true;
            // 
            // publishTimes
            // 
            publishTimes.BackColor = Color.White;
            publishTimes.Location = new Point(24, 134);
            publishTimes.Name = "publishTimes";
            publishTimes.Size = new Size(399, 208);
            publishTimes.TabIndex = 54;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ControlDark;
            label3.Location = new Point(27, 356);
            label3.Name = "label3";
            label3.Size = new Size(200, 17);
            label3.TabIndex = 53;
            label3.Text = "请在右则选择与配置需要发布的平台";
            // 
            // lbDataInfo
            // 
            lbDataInfo.AutoSize = true;
            lbDataInfo.ForeColor = Color.MediumSeaGreen;
            lbDataInfo.Location = new Point(29, 62);
            lbDataInfo.Name = "lbDataInfo";
            lbDataInfo.Size = new Size(0, 17);
            lbDataInfo.TabIndex = 47;
            // 
            // btnSelDataPath
            // 
            btnSelDataPath.BackColor = Color.DeepSkyBlue;
            btnSelDataPath.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
            btnSelDataPath.ConerRadius = 5;
            btnSelDataPath.Font = new Font("Segoe UI", 9F);
            btnSelDataPath.Location = new Point(27, 14);
            btnSelDataPath.Name = "btnSelDataPath";
            btnSelDataPath.Size = new Size(113, 37);
            btnSelDataPath.TabIndex = 46;
            btnSelDataPath.Text = "选择数据包目录";
            btnSelDataPath.TextColor = Color.White;
            btnSelDataPath.Click += btnSelDataPath_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = Color.Red;
            label10.Location = new Point(536, 67);
            label10.Name = "label10";
            label10.Size = new Size(13, 17);
            label10.TabIndex = 41;
            label10.Text = "*";
            // 
            // ImportDatas
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1134, 643);
            Controls.Add(splitContainer1);
            Controls.Add(label10);
            Name = "ImportDatas";
            Text = "批量导入";
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            pan1.ResumeLayout(false);
            pan1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private SplitContainer splitContainer1;
        private XsButton btnSave;
        private Label label10;
        private Panel pan1;
        private XsButton btnSelDataPath;
        private XsButton btnSelCoverImg;
        private Label label15;
        private Label label16; 
        private FlowLayoutPanel fPanelSelPlugins;
        private Label lbDataInfo;
        private Label label3;
        private PublishTimes publishTimes;
        private XS.WinFormsTools.Controls.CheckBox cbIsOriginal;
        private LinkLabel lbShowJsonMap;
        private Label lbErrInfo;
        private ClassCombox classCombox;
    }
}