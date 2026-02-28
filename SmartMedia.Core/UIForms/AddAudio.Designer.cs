using XS.WinFormsTools;

namespace SmartMedia.Core.UIForms;

partial class AddAudio
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
        panel1 = new Panel();
        panelCenter = new Panel();
        btnPush = new XsButton();
        btnSave = new XsButton();
        splitContainer1 = new SplitContainer();
        pan1 = new Panel();
        classCombox = new Controls.ClassCombox();
        btnPlay = new XsButton();
        publishTimer = new Controls.PublishTimer();
        lbAudioName = new Label();
        label12 = new Label();
        label6 = new Label();
        txtInfo = new XsRichTextBox();
        cbIsRc = new XS.WinFormsTools.Controls.CheckBox();
        label1 = new Label();
        txtTitle = new XsTextBox();
        label2 = new Label();
        label9 = new Label();
        btnSelVideo = new XsButton();
        txtTags = new XsTextBox();
        label4 = new Label();
        siteSelector = new Controls.SiteSelectorAudio();
        label10 = new Label();
        panel1.SuspendLayout();
        panelCenter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        pan1.SuspendLayout();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.BackColor = Color.AliceBlue;
        panel1.Controls.Add(panelCenter);
        panel1.Dock = DockStyle.Bottom;
        panel1.Location = new Point(0, 734);
        panel1.Name = "panel1";
        panel1.Size = new Size(1290, 74);
        panel1.TabIndex = 0;
        // 
        // panelCenter
        // 
        panelCenter.Controls.Add(btnPush);
        panelCenter.Controls.Add(btnSave);
        panelCenter.Location = new Point(445, 6);
        panelCenter.Name = "panelCenter";
        panelCenter.Size = new Size(301, 65);
        panelCenter.TabIndex = 17;
        // 
        // btnPush
        // 
        btnPush.BackColor = Color.Teal;
        btnPush.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnPush.ConerRadius = 5;
        btnPush.Enabled = false;
        btnPush.Font = new Font("Segoe UI", 9F);
        btnPush.Location = new Point(166, 18);
        btnPush.Name = "btnPush";
        btnPush.Size = new Size(99, 33);
        btnPush.TabIndex = 17;
        btnPush.Text = "立即发布";
        btnPush.TextColor = Color.White;
        btnPush.Click += btnPush_Click;
        // 
        // btnSave
        // 
        btnSave.BackColor = Color.DeepSkyBlue;
        btnSave.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnSave.ConerRadius = 5;
        btnSave.Font = new Font("Segoe UI", 9F);
        btnSave.Location = new Point(40, 18);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(99, 33);
        btnSave.TabIndex = 16;
        btnSave.Text = "保存";
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
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(siteSelector);
        splitContainer1.Size = new Size(1290, 734);
        splitContainer1.SplitterDistance = 504;
        splitContainer1.TabIndex = 42;
        // 
        // pan1
        // 
        pan1.BackColor = Color.White;
        pan1.Controls.Add(classCombox);
        pan1.Controls.Add(btnPlay);
        pan1.Controls.Add(publishTimer);
        pan1.Controls.Add(lbAudioName);
        pan1.Controls.Add(label12);
        pan1.Controls.Add(label6);
        pan1.Controls.Add(txtInfo);
        pan1.Controls.Add(cbIsRc);
        pan1.Controls.Add(label1);
        pan1.Controls.Add(txtTitle);
        pan1.Controls.Add(label2);
        pan1.Controls.Add(label9);
        pan1.Controls.Add(btnSelVideo);
        pan1.Controls.Add(txtTags);
        pan1.Controls.Add(label4);
        pan1.Dock = DockStyle.Fill;
        pan1.Location = new Point(0, 0);
        pan1.Name = "pan1";
        pan1.Size = new Size(504, 734);
        pan1.TabIndex = 0;
        // 
        // classCombox
        // 
        classCombox.Location = new Point(306, 438);
        classCombox.Name = "classCombox";
        classCombox.Size = new Size(169, 38);
        classCombox.TabIndex = 77;
        // 
        // btnPlay
        // 
        btnPlay.BackColor = Color.CadetBlue;
        btnPlay.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnPlay.ConerRadius = 5;
        btnPlay.Font = new Font("Segoe UI", 9F);
        btnPlay.Location = new Point(374, 12);
        btnPlay.Name = "btnPlay";
        btnPlay.Size = new Size(101, 30);
        btnPlay.TabIndex = 71;
        btnPlay.Text = "试听";
        btnPlay.TextColor = Color.White;
        btnPlay.Click += btnPlay_Click;
        // 
        // publishTimer
        // 
        publishTimer.CheckBoxText = "是否定时发布";
        publishTimer.Location = new Point(12, 470);
        publishTimer.Name = "publishTimer";
        publishTimer.Size = new Size(233, 84);
        publishTimer.TabIndex = 70;
        // 
        // lbAudioName
        // 
        lbAudioName.AutoSize = true;
        lbAudioName.ForeColor = SystemColors.ButtonShadow;
        lbAudioName.Location = new Point(23, 51);
        lbAudioName.Name = "lbAudioName";
        lbAudioName.Size = new Size(0, 17);
        lbAudioName.TabIndex = 67;
        // 
        // label12
        // 
        label12.AutoSize = true;
        label12.ForeColor = SystemColors.AppWorkspace;
        label12.Location = new Point(59, 229);
        label12.Name = "label12";
        label12.Size = new Size(236, 17);
        label12.TabIndex = 60;
        label12.Text = "有此平台不允许加连接，发布时将自动过滤";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.ForeColor = SystemColors.AppWorkspace;
        label6.Location = new Point(103, 152);
        label6.Name = "label6";
        label6.Size = new Size(88, 17);
        label6.TabIndex = 58;
        label6.Text = "多个用#号分开";
        // 
        // txtInfo
        // 
        txtInfo.BackColor = SystemColors.Window;
        txtInfo.BorderColor = Color.Gainsboro;
        txtInfo.BorderFocusColor = Color.DodgerBlue;
        txtInfo.BorderSize = 2;
        txtInfo.ConerRadius = 2;
        txtInfo.Font = new Font("Microsoft Sans Serif", 9.5F);
        txtInfo.ForeColor = Color.DimGray;
        txtInfo.Location = new Point(31, 253);
        txtInfo.Margin = new Padding(4);
        txtInfo.Name = "txtInfo";
        txtInfo.Padding = new Padding(7);
        txtInfo.ReadOnly = false;
        txtInfo.Size = new Size(444, 161);
        txtInfo.TabIndex = 54;
        txtInfo.Texts = "";
        txtInfo.UnderlinedStyle = false;
        // 
        // cbIsRc
        // 
        cbIsRc.AutoSize = true;
        cbIsRc.Font = new Font("Segoe UI", 12F);
        cbIsRc.Location = new Point(24, 438);
        cbIsRc.Name = "cbIsRc";
        cbIsRc.Size = new Size(99, 20);
        cbIsRc.TabIndex = 45;
        cbIsRc.Text = "是否原创";
        cbIsRc.UseVisualStyleBackColor = true;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(23, 78);
        label1.Name = "label1";
        label1.Size = new Size(32, 17);
        label1.TabIndex = 41;
        label1.Text = "标题";
        // 
        // txtTitle
        // 
        txtTitle.BackColor = SystemColors.Window;
        txtTitle.BorderColor = Color.Gainsboro;
        txtTitle.BorderFocusColor = Color.DodgerBlue;
        txtTitle.BorderSize = 2;
        txtTitle.ConerRadius = 2;
        txtTitle.Font = new Font("Microsoft Sans Serif", 9.5F);
        txtTitle.ForeColor = Color.DimGray;
        txtTitle.Location = new Point(26, 106);
        txtTitle.Margin = new Padding(4);
        txtTitle.Multiline = false;
        txtTitle.Name = "txtTitle";
        txtTitle.Padding = new Padding(7);
        txtTitle.PasswordChar = false;
        txtTitle.PlaceholderText = "";
        txtTitle.ScrollBar = ScrollBars.None;
        txtTitle.Size = new Size(445, 31);
        txtTitle.TabIndex = 40;
        txtTitle.Texts = "";
        txtTitle.UnderlinedStyle = false;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(27, 228);
        label2.Name = "label2";
        label2.Size = new Size(32, 17);
        label2.TabIndex = 43;
        label2.Text = "简介";
        // 
        // label9
        // 
        label9.AutoSize = true;
        label9.ForeColor = Color.Red;
        label9.Location = new Point(55, 80);
        label9.Name = "label9";
        label9.Size = new Size(13, 17);
        label9.TabIndex = 53;
        label9.Text = "*";
        // 
        // btnSelVideo
        // 
        btnSelVideo.BackColor = Color.DeepSkyBlue;
        btnSelVideo.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnSelVideo.ConerRadius = 5;
        btnSelVideo.Font = new Font("Segoe UI", 9F);
        btnSelVideo.Location = new Point(27, 14);
        btnSelVideo.Name = "btnSelVideo";
        btnSelVideo.Size = new Size(101, 30);
        btnSelVideo.TabIndex = 46;
        btnSelVideo.Text = "选择音频";
        btnSelVideo.TextColor = Color.White;
        btnSelVideo.Click += btnSelVideo_Click;
        // 
        // txtTags
        // 
        txtTags.BackColor = SystemColors.Window;
        txtTags.BorderColor = Color.Gainsboro;
        txtTags.BorderFocusColor = Color.DodgerBlue;
        txtTags.BorderSize = 2;
        txtTags.ConerRadius = 2;
        txtTags.Font = new Font("Microsoft Sans Serif", 9.5F);
        txtTags.ForeColor = Color.DimGray;
        txtTags.Location = new Point(27, 177);
        txtTags.Margin = new Padding(4);
        txtTags.Multiline = false;
        txtTags.Name = "txtTags";
        txtTags.Padding = new Padding(7);
        txtTags.PasswordChar = false;
        txtTags.PlaceholderText = "";
        txtTags.ScrollBar = ScrollBars.None;
        txtTags.Size = new Size(444, 31);
        txtTags.TabIndex = 47;
        txtTags.Texts = "";
        txtTags.UnderlinedStyle = false;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(26, 152);
        label4.Name = "label4";
        label4.Size = new Size(61, 17);
        label4.TabIndex = 48;
        label4.Text = "标签/话题";
        // 
        // siteSelector
        // 
        siteSelector.Dock = DockStyle.Fill;
        siteSelector.Location = new Point(0, 0);
        siteSelector.Name = "siteSelector";
        siteSelector.Size = new Size(782, 734);
        siteSelector.TabIndex = 0;
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
        // AddAudio
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1290, 808);
        Controls.Add(splitContainer1);
        Controls.Add(label10);
        Controls.Add(panel1);
        Name = "AddAudio";
        Text = "添加视频";
        panel1.ResumeLayout(false);
        panelCenter.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        pan1.ResumeLayout(false);
        pan1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel panel1;
    private SplitContainer splitContainer1;
    private XsButton btnSave;
    private Label label10;
    private Panel pan1;
    private Label label1;
    private XsTextBox txtTitle;
    private Label label2;
    private Label label9;
    private XsButton btnSelVideo;
    private XsTextBox txtTags;
    private Label label4;
    private XS.WinFormsTools.Controls.CheckBox cbIsRc;
    private XsRichTextBox txtInfo;
    private XsButton btnSelCoverImg;
    private Label label6;
    private Label label12;
    private Label label15;
    private Label label16; 
    private Panel panelCenter;
    private XsButton btnPush;
    private FlowLayoutPanel fPanelSelPlugins;
    private Controls.PublishTimer publishTimer;
    private Controls.SiteSelectorAudio siteSelector; 
    private Label lbAudioName;
    private XsButton btnPlay;
    private Controls.ClassCombox classCombox;
}