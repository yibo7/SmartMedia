using XS.WinFormsTools;

namespace SmartMedia.Core.UIForms;

partial class AddArticle
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
        htmlEditor = new XS.HtmlEditor.HtmlEditor();
        txtTitle = new XsTextBox();
        panel2 = new Panel();
        publishTimer = new SmartMedia.Core.Controls.PublishTimer();
        classCombox = new SmartMedia.Core.Controls.ClassCombox();
        cbIsRc = new XS.WinFormsTools.Controls.CheckBox();
        groupBox1 = new GroupBox();
        linkBarDelete = new LinkLabel();
        picCover = new XS.WinFormsTools.Controls.PictureBox();
        ucSplitLine_h1 = new XS.WinFormsTools.Forms.UCSplitLine_H();
        label4 = new Label();
        label6 = new Label();
        txtTags = new XsTextBox();
        siteSelector = new SmartMedia.Core.Controls.SiteSelectorArticle();
        label10 = new Label();
        panel1.SuspendLayout();
        panelCenter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        panel2.SuspendLayout();
        groupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picCover).BeginInit();
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
        splitContainer1.Location = new Point(0, 0);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(htmlEditor);
        splitContainer1.Panel1.Controls.Add(txtTitle);
        splitContainer1.Panel1.Controls.Add(panel2);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(siteSelector);
        splitContainer1.Size = new Size(1290, 734);
        splitContainer1.SplitterDistance = 748;
        splitContainer1.TabIndex = 42;
        // 
        // htmlEditor
        // 
        htmlEditor.Dock = DockStyle.Fill;
        htmlEditor.Location = new Point(0, 31);
        htmlEditor.Name = "htmlEditor";
        htmlEditor.ServerUrl = null;
        htmlEditor.Size = new Size(748, 490);
        htmlEditor.TabIndex = 4;
        // 
        // txtTitle
        // 
        txtTitle.BackColor = SystemColors.Window;
        txtTitle.BorderColor = Color.Transparent;
        txtTitle.BorderFocusColor = Color.Transparent;
        txtTitle.BorderSize = 2;
        txtTitle.ConerRadius = 2;
        txtTitle.Dock = DockStyle.Top;
        txtTitle.Font = new Font("Microsoft Sans Serif", 9.5F);
        txtTitle.ForeColor = Color.Gray;
        txtTitle.Location = new Point(0, 0);
        txtTitle.Margin = new Padding(4);
        txtTitle.Multiline = false;
        txtTitle.Name = "txtTitle";
        txtTitle.Padding = new Padding(7);
        txtTitle.PasswordChar = false;
        txtTitle.PlaceholderText = "标题";
        txtTitle.ScrollBar = ScrollBars.None;
        txtTitle.Size = new Size(748, 31);
        txtTitle.TabIndex = 2;
        txtTitle.Texts = "标题";
        txtTitle.UnderlinedStyle = false;
        // 
        // panel2
        // 
        panel2.BackColor = Color.White;
        panel2.Controls.Add(publishTimer);
        panel2.Controls.Add(classCombox);
        panel2.Controls.Add(cbIsRc);
        panel2.Controls.Add(groupBox1);
        panel2.Controls.Add(ucSplitLine_h1);
        panel2.Controls.Add(label4);
        panel2.Controls.Add(label6);
        panel2.Controls.Add(txtTags);
        panel2.Dock = DockStyle.Bottom;
        panel2.Location = new Point(0, 521);
        panel2.Name = "panel2";
        panel2.Size = new Size(748, 213);
        panel2.TabIndex = 3;
        // 
        // publishTimer
        // 
        publishTimer.CheckBoxText = "是否定时发布";
        publishTimer.Location = new Point(228, 80);
        publishTimer.Name = "publishTimer";
        publishTimer.Size = new Size(233, 84);
        publishTimer.TabIndex = 78;
        // 
        // classCombox
        // 
        classCombox.Location = new Point(14, 120);
        classCombox.Name = "classCombox";
        classCombox.Size = new Size(169, 38);
        classCombox.TabIndex = 77;
        // 
        // cbIsRc
        // 
        cbIsRc.AutoSize = true;
        cbIsRc.Font = new Font("Segoe UI", 12F);
        cbIsRc.Location = new Point(14, 86);
        cbIsRc.Name = "cbIsRc";
        cbIsRc.Size = new Size(99, 20);
        cbIsRc.TabIndex = 74;
        cbIsRc.Text = "是否原创";
        cbIsRc.UseVisualStyleBackColor = true;
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(linkBarDelete);
        groupBox1.Controls.Add(picCover);
        groupBox1.Location = new Point(496, 20);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(200, 161);
        groupBox1.TabIndex = 73;
        groupBox1.TabStop = false;
        groupBox1.Text = "默认封面图";
        // 
        // linkBarDelete
        // 
        linkBarDelete.AutoSize = true;
        linkBarDelete.LinkColor = Color.IndianRed;
        linkBarDelete.Location = new Point(138, 129);
        linkBarDelete.Name = "linkBarDelete";
        linkBarDelete.Size = new Size(32, 17);
        linkBarDelete.TabIndex = 73;
        linkBarDelete.TabStop = true;
        linkBarDelete.Text = "删除";
        linkBarDelete.LinkClicked += linkBarDelete_LinkClicked;
        // 
        // picCover
        // 
        picCover.ErrorColor = Color.FromArgb(255, 240, 240);
        picCover.LoadedColor = Color.FromArgb(245, 245, 245);
        picCover.LoadingColor = Color.FromArgb(240, 240, 240);
        picCover.Location = new Point(25, 26);
        picCover.Name = "picCover";
        picCover.ShowLoadingIndicator = true;
        picCover.Size = new Size(151, 89);
        picCover.TabIndex = 72;
        picCover.TabStop = false;
        picCover.Click += picCover_Click;
        // 
        // ucSplitLine_h1
        // 
        ucSplitLine_h1.BackColor = Color.FromArgb(232, 232, 232);
        ucSplitLine_h1.Dock = DockStyle.Top;
        ucSplitLine_h1.Location = new Point(0, 0);
        ucSplitLine_h1.Name = "ucSplitLine_h1";
        ucSplitLine_h1.Size = new Size(748, 1);
        ucSplitLine_h1.TabIndex = 70;
        ucSplitLine_h1.TabStop = false;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(13, 10);
        label4.Name = "label4";
        label4.Size = new Size(61, 17);
        label4.TabIndex = 48;
        label4.Text = "标签/话题";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.ForeColor = SystemColors.AppWorkspace;
        label6.Location = new Point(80, 10);
        label6.Name = "label6";
        label6.Size = new Size(112, 17);
        label6.TabIndex = 58;
        label6.Text = "选填，多个用#分开";
        // 
        // txtTags
        // 
        txtTags.BackColor = SystemColors.Window;
        txtTags.BorderColor = Color.Gainsboro;
        txtTags.BorderFocusColor = Color.DodgerBlue;
        txtTags.BorderSize = 2;
        txtTags.ConerRadius = 2;
        txtTags.Font = new Font("Microsoft Sans Serif", 9.5F);
        txtTags.ForeColor = Color.Gray;
        txtTags.Location = new Point(14, 35);
        txtTags.Margin = new Padding(4);
        txtTags.Multiline = false;
        txtTags.Name = "txtTags";
        txtTags.Padding = new Padding(7);
        txtTags.PasswordChar = false;
        txtTags.PlaceholderText = "";
        txtTags.ScrollBar = ScrollBars.None;
        txtTags.Size = new Size(447, 31);
        txtTags.TabIndex = 47;
        txtTags.Texts = "";
        txtTags.UnderlinedStyle = false;
        // 
        // siteSelector
        // 
        siteSelector.Dock = DockStyle.Fill;
        siteSelector.Location = new Point(0, 0);
        siteSelector.Name = "siteSelector";
        siteSelector.Size = new Size(538, 734);
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
        // AddArticle
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1290, 808);
        Controls.Add(splitContainer1);
        Controls.Add(label10);
        Controls.Add(panel1);
        Name = "AddArticle";
        Text = "添加图文";
        panel1.ResumeLayout(false);
        panelCenter.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picCover).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel panel1;
    private SplitContainer splitContainer1;
    private XsButton btnSave;
    private Label label10;
    private Label label15;
    private Label label16; 
    private Panel panelCenter;
    private XsButton btnPush;
    private Controls.SiteSelectorArticle siteSelector;
    private XsTextBox txtTitle;
    private Panel panel2;
    private XS.WinFormsTools.Forms.UCSplitLine_H ucSplitLine_h1;
    private Label label4;
    private Label label6;
    private XsTextBox txtTags;
    private GroupBox groupBox1;
    private LinkLabel linkBarDelete;
    private XS.WinFormsTools.Controls.PictureBox picCover;
    private Controls.PublishTimer publishTimer;
    private XS.WinFormsTools.Controls.CheckBox cbIsRc;
    private XS.HtmlEditor.HtmlEditor htmlEditor;
    private Controls.ClassCombox classCombox;
}