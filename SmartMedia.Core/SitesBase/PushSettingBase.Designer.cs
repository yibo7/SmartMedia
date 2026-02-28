namespace SmartMedia.Modules.PushContent
{
    partial class PushSettingBase
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            ucSplitLine_v1 = new XS.WinFormsTools.Forms.UCSplitLine_V();
            lbTitle = new Label();
            picSiteIco = new PictureBox();
            hLineControl1 = new XS.WinFormsTools.HLineControl();
            label13 = new Label();
            swIsEnable = new XS.WinFormsTools.Controls.Switch();
            panelPrams = new Panel();
            flpPanel = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)picSiteIco).BeginInit();
            panelPrams.SuspendLayout();
            SuspendLayout();
            // 
            // ucSplitLine_v1
            // 
            ucSplitLine_v1.BackColor = Color.FromArgb(232, 232, 232);
            ucSplitLine_v1.Dock = DockStyle.Left;
            ucSplitLine_v1.Location = new Point(0, 0);
            ucSplitLine_v1.Name = "ucSplitLine_v1";
            ucSplitLine_v1.Size = new Size(1, 518);
            ucSplitLine_v1.TabIndex = 56;
            ucSplitLine_v1.TabStop = false;
            // 
            // lbTitle
            // 
            lbTitle.AutoSize = true;
            lbTitle.Font = new Font("Microsoft YaHei UI", 12F);
            lbTitle.Location = new Point(79, 17);
            lbTitle.Name = "lbTitle";
            lbTitle.Size = new Size(74, 21);
            lbTitle.TabIndex = 59;
            lbTitle.Text = "站点名称";
            // 
            // picSiteIco
            // 
            picSiteIco.Location = new Point(8, 3);
            picSiteIco.Name = "picSiteIco";
            picSiteIco.Size = new Size(64, 64);
            picSiteIco.TabIndex = 58;
            picSiteIco.TabStop = false;
            // 
            // hLineControl1
            // 
            hLineControl1.Dock = DockStyle.Top;
            hLineControl1.LineColor = Color.WhiteSmoke;
            hLineControl1.LineSize = 1;
            hLineControl1.Location = new Point(1, 0);
            hLineControl1.Name = "hLineControl1";
            hLineControl1.Size = new Size(844, 75);
            hLineControl1.TabIndex = 57;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(79, 47);
            label13.Name = "label13";
            label13.Size = new Size(56, 17);
            label13.TabIndex = 65;
            label13.Text = "启用发布";
            // 
            // swIsEnable
            // 
            swIsEnable.AutoSize = true;
            swIsEnable.Location = new Point(141, 45);
            swIsEnable.Name = "swIsEnable";
            swIsEnable.Size = new Size(40, 20);
            swIsEnable.TabIndex = 64;
            swIsEnable.Text = "switch1";
            swIsEnable.UseVisualStyleBackColor = true;
            swIsEnable.CheckStateChanged += swIsEnable_CheckStateChanged;
            // 
            // panelPrams
            // 
            panelPrams.Controls.Add(flpPanel);
            panelPrams.Dock = DockStyle.Fill;
            panelPrams.Location = new Point(1, 75);
            panelPrams.Name = "panelPrams";
            panelPrams.Size = new Size(844, 443);
            panelPrams.TabIndex = 66;
            // 
            // flpPanel
            // 
            flpPanel.Dock = DockStyle.Fill;
            flpPanel.Location = new Point(0, 0);
            flpPanel.Name = "flpPanel";
            flpPanel.Size = new Size(844, 443);
            flpPanel.TabIndex = 71;
            // 
            // PushSettingBase
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panelPrams);
            Controls.Add(label13);
            Controls.Add(swIsEnable);
            Controls.Add(lbTitle);
            Controls.Add(picSiteIco);
            Controls.Add(hLineControl1);
            Controls.Add(ucSplitLine_v1);
            Name = "PushSettingBase";
            Size = new Size(845, 518);
            ((System.ComponentModel.ISupportInitialize)picSiteIco).EndInit();
            panelPrams.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private XS.WinFormsTools.Forms.UCSplitLine_V ucSplitLine_v1;
        private Label lbTitle;
        private PictureBox picSiteIco;
        private XS.WinFormsTools.HLineControl hLineControl1;
        private Label label13;
        private XS.WinFormsTools.Controls.Switch swIsEnable;
        private Panel panelPrams;
        private FlowLayoutPanel flpPanel;
    }
}
