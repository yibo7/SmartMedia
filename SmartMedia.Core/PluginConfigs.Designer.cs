namespace SmartMedia.Core;

partial class PluginConfigs
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
        btnSave = new XS.WinFormsTools.XsButton();
        toolMainBar = new ToolStrip();
        toolStripLabel1 = new ToolStripLabel();
        gvSettings = new DataGridView();
        toolMainBar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gvSettings).BeginInit();
        SuspendLayout();
        // 
        // btnSave
        // 
        btnSave.BackColor = Color.DeepSkyBlue;
        btnSave.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnSave.ConerRadius = 5;
        btnSave.Font = new Font("Segoe UI", 9F);
        btnSave.Location = new Point(212, 305);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(110, 38);
        btnSave.TabIndex = 0;
        btnSave.Text = "保存配置";
        btnSave.TextColor = Color.White;
        btnSave.Click += btnSave_Click;
        // 
        // toolMainBar
        // 
        toolMainBar.ImageScalingSize = new Size(32, 32);
        toolMainBar.Items.AddRange(new ToolStripItem[] { toolStripLabel1 });
        toolMainBar.Location = new Point(0, 0);
        toolMainBar.Name = "toolMainBar";
        toolMainBar.Size = new Size(560, 25);
        toolMainBar.TabIndex = 0;
        toolMainBar.Text = "toolStrip1";
        // 
        // toolStripLabel1
        // 
        toolStripLabel1.Name = "toolStripLabel1";
        toolStripLabel1.Size = new Size(68, 22);
        toolStripLabel1.Text = "自定义配置";
        // 
        // gvSettings
        // 
        gvSettings.BorderStyle = BorderStyle.None;
        gvSettings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gvSettings.Location = new Point(0, 28);
        gvSettings.Name = "gvSettings";
        gvSettings.Size = new Size(563, 255);
        gvSettings.TabIndex = 1;
        // 
        // PluginConfigs
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(560, 374);
        Controls.Add(btnSave);
        Controls.Add(gvSettings);
        Controls.Add(toolMainBar);
        Name = "PluginConfigs";
        Text = "config";
        Load += PluginConfigs_Load;
        toolMainBar.ResumeLayout(false);
        toolMainBar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gvSettings).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private XS.WinFormsTools.XsButton btnSave;
    private ToolStrip toolMainBar;
    private ToolStripLabel toolStripLabel1;
    private DataGridView gvSettings;
}