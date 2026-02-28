namespace SmartMedia.Core.Controls;

partial class SiteSelector
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
        splitContainer1 = new SplitContainer();
        fPanelSelPlugins = new Panel();
        splitContainer2 = new SplitContainer();
        pnSettings = new Panel();
        ucSplitLine_v1 = new XS.WinFormsTools.Forms.UCSplitLine_V();
        lvSites = new ListView();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
        splitContainer2.Panel1.SuspendLayout();
        splitContainer2.Panel2.SuspendLayout();
        splitContainer2.SuspendLayout();
        pnSettings.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.FixedPanel = FixedPanel.Panel1;
        splitContainer1.Location = new Point(0, 0);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(fPanelSelPlugins);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(splitContainer2);
        splitContainer1.Size = new Size(449, 375);
        splitContainer1.SplitterDistance = 57;
        splitContainer1.TabIndex = 0;
        // 
        // fPanelSelPlugins
        // 
        fPanelSelPlugins.BorderStyle = BorderStyle.None;
        fPanelSelPlugins.Dock = DockStyle.Fill;
        fPanelSelPlugins.Location = new Point(0, 0);
        fPanelSelPlugins.Name = "fPanelSelPlugins";
        fPanelSelPlugins.Size = new Size(57, 375);
        fPanelSelPlugins.TabIndex = 2; 
        // 
        // splitContainer2
        // 
        splitContainer2.Dock = DockStyle.Fill;
        splitContainer2.Location = new Point(0, 0);
        splitContainer2.Name = "splitContainer2";
        // 
        // splitContainer2.Panel1
        // 
        splitContainer2.Panel1.Controls.Add(pnSettings);
        // 
        // splitContainer2.Panel2
        // 
        splitContainer2.Panel2.Controls.Add(lvSites);
        splitContainer2.Size = new Size(388, 375);
        splitContainer2.SplitterDistance = 292;
        splitContainer2.TabIndex = 0;
        // 
        // pnSettings
        // 
        pnSettings.BackColor = Color.White;
        pnSettings.Controls.Add(ucSplitLine_v1);
        pnSettings.Dock = DockStyle.Fill;
        pnSettings.Location = new Point(0, 0);
        pnSettings.Name = "pnSettings";
        pnSettings.Size = new Size(292, 375);
        pnSettings.TabIndex = 2;
        // 
        // ucSplitLine_v1
        // 
        ucSplitLine_v1.BackColor = Color.FromArgb(232, 232, 232);
        ucSplitLine_v1.Dock = DockStyle.Left;
        ucSplitLine_v1.Location = new Point(0, 0);
        ucSplitLine_v1.Name = "ucSplitLine_v1";
        ucSplitLine_v1.Size = new Size(1, 375);
        ucSplitLine_v1.TabIndex = 47;
        ucSplitLine_v1.TabStop = false;
        // 
        // lvSites
        // 
        lvSites.BorderStyle = BorderStyle.None;
        lvSites.Dock = DockStyle.Fill;
        lvSites.Location = new Point(0, 0);
        lvSites.Name = "lvSites";
        lvSites.Size = new Size(92, 375);
        lvSites.TabIndex = 1;
        lvSites.UseCompatibleStateImageBehavior = false;
        // 
        // SiteSelector
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(splitContainer1);
        Name = "SiteSelector";
        Size = new Size(449, 375);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        splitContainer2.Panel1.ResumeLayout(false);
        splitContainer2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
        splitContainer2.ResumeLayout(false);
        pnSettings.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private Panel pnSettings;
    private XS.WinFormsTools.Forms.UCSplitLine_V ucSplitLine_v1;
    private Panel fPanelSelPlugins;
    private ListView lvSites;
}
