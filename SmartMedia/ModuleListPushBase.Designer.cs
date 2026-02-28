namespace SmartMedia;

partial class ModuleListPushBase
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
        btnPush = new XS.WinFormsTools.XsButton();
        panel1 = new Panel();
        ucSplitLine_h1 = new XS.WinFormsTools.Forms.UCSplitLine_H();
        btnAddVideo = new XS.WinFormsTools.XsButton();
        panel2 = new Panel();
        lvTools = new ListView();
        panel1.SuspendLayout();
        panel2.SuspendLayout();
        SuspendLayout();
        // 
        // btnPush
        // 
        btnPush.BackColor = Color.Teal;
        btnPush.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnPush.ConerRadius = 15;
        btnPush.Font = new Font("Segoe UI", 9F);
        btnPush.Location = new Point(110, 9);
        btnPush.Name = "btnPush";
        btnPush.Size = new Size(69, 30);
        btnPush.TabIndex = 5;
        btnPush.Text = "发布";
        btnPush.TextColor = Color.White;
        btnPush.Click += btnPush_Click;
        // 
        // panel1
        // 
        panel1.Controls.Add(btnPush);
        panel1.Controls.Add(ucSplitLine_h1);
        panel1.Controls.Add(btnAddVideo);
        panel1.Dock = DockStyle.Top;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(179, 60);
        panel1.TabIndex = 1;
        // 
        // ucSplitLine_h1
        // 
        ucSplitLine_h1.BackColor = Color.FromArgb(232, 232, 232);
        ucSplitLine_h1.Location = new Point(-14, 50);
        ucSplitLine_h1.Name = "ucSplitLine_h1";
        ucSplitLine_h1.Size = new Size(232, 1);
        ucSplitLine_h1.TabIndex = 4;
        ucSplitLine_h1.TabStop = false;
        // 
        // btnAddVideo
        // 
        btnAddVideo.BackColor = Color.DeepSkyBlue;
        btnAddVideo.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Primary;
        btnAddVideo.ConerRadius = 15;
        btnAddVideo.Font = new Font("Segoe UI", 9F);
        btnAddVideo.Location = new Point(12, 9);
        btnAddVideo.Name = "btnAddVideo";
        btnAddVideo.Size = new Size(84, 30);
        btnAddVideo.TabIndex = 0;
        btnAddVideo.Text = "管理插件";
        btnAddVideo.TextColor = Color.White;
        btnAddVideo.Click += btnAddVideo_Click;
        // 
        // panel2
        // 
        panel2.Controls.Add(lvTools);
        panel2.Dock = DockStyle.Fill;
        panel2.Location = new Point(0, 60);
        panel2.Name = "panel2";
        panel2.Size = new Size(179, 390);
        panel2.TabIndex = 2;
        // 
        // lvTools
        // 
        lvTools.BorderStyle = BorderStyle.None;
        lvTools.Dock = DockStyle.Fill;
        lvTools.Location = new Point(0, 0);
        lvTools.Name = "lvTools";
        lvTools.Size = new Size(179, 390);
        lvTools.TabIndex = 3;
        lvTools.UseCompatibleStateImageBehavior = false;
        // 
        // ModuleListPushBase
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(179, 450);
        Controls.Add(panel2);
        Controls.Add(panel1);
        Name = "ModuleListPushBase";
        Text = "ModuleList";
        panel1.ResumeLayout(false);
        panel2.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
    private Panel panel1;
    protected XS.WinFormsTools.XsButton btnAddVideo;
    private XS.WinFormsTools.Forms.UCSplitLine_H ucSplitLine_h1;
    private Panel panel2;
    protected ListView lvTools;
    private XS.WinFormsTools.XsButton btnPush;
}