 
using XS.WinFormsTools.XsListView;

namespace SmartMedia.Core.UIForms;

partial class HelpMarkdownViewer
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
        webBox = new Microsoft.Web.WebView2.WinForms.WebView2();
        toolMainBar = new ToolStrip();
        lbTitle = new ToolStripLabel();
        ((System.ComponentModel.ISupportInitialize)webBox).BeginInit();
        toolMainBar.SuspendLayout();
        SuspendLayout();
        // 
        // webBox
        // 
        webBox.AllowExternalDrop = true;
        webBox.CreationProperties = null;
        webBox.DefaultBackgroundColor = Color.White;
        webBox.Dock = DockStyle.Fill;
        webBox.Location = new Point(0, 33);
        webBox.Name = "webBox";
        webBox.Size = new Size(951, 509);
        webBox.TabIndex = 3;
        webBox.ZoomFactor = 1D;
        // 
        // toolMainBar
        // 
        toolMainBar.GripMargin = new Padding(3);
        toolMainBar.Items.AddRange(new ToolStripItem[] { lbTitle });
        toolMainBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
        toolMainBar.Location = new Point(0, 0);
        toolMainBar.Name = "toolMainBar";
        toolMainBar.Size = new Size(951, 33);
        toolMainBar.TabIndex = 2;
        toolMainBar.Text = "toolStrip1";
        // 
        // lbTitle
        // 
        lbTitle.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
        lbTitle.Name = "lbTitle";
        lbTitle.Size = new Size(57, 30);
        lbTitle.Text = "标题";
        // 
        // HelpMarkdownViewer
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(951, 542);
        Controls.Add(webBox);
        Controls.Add(toolMainBar);
        Name = "HelpMarkdownViewer";
        Text = "TestModule";
        ((System.ComponentModel.ISupportInitialize)webBox).EndInit();
        toolMainBar.ResumeLayout(false);
        toolMainBar.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private Microsoft.Web.WebView2.WinForms.WebView2 webBox;
    private ToolStrip toolMainBar;
    private ToolStripLabel lbTitle;
}