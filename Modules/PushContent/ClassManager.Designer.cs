namespace SmartMedia.Modules.PushContent;

partial class ClassManager
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
        toolStrip1 = new ToolStrip();
        txtClassName = new ToolStripTextBox();
        btnSave = new ToolStripButton();
        btnDel = new ToolStripButton();
        lbData = new ListBox();
        toolStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // toolStrip1
        // 
        toolStrip1.Items.AddRange(new ToolStripItem[] { txtClassName, btnSave, btnDel });
        toolStrip1.Location = new Point(0, 0);
        toolStrip1.Name = "toolStrip1";
        toolStrip1.Size = new Size(298, 25);
        toolStrip1.TabIndex = 0;
        toolStrip1.Text = "toolStrip1";
        // 
        // txtClassName
        // 
        txtClassName.Name = "txtClassName";
        txtClassName.Size = new Size(150, 25);
        // 
        // btnSave
        // 
        btnSave.Image = Resource.save;
        btnSave.ImageTransparentColor = Color.Magenta;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(52, 22);
        btnSave.Text = "保存";
        btnSave.Click += btnSave_Click;
        // 
        // btnDel
        // 
        btnDel.Alignment = ToolStripItemAlignment.Right;
        btnDel.Image = Resource.Delete;
        btnDel.ImageTransparentColor = Color.Magenta;
        btnDel.Name = "btnDel";
        btnDel.Size = new Size(52, 22);
        btnDel.Text = "删除";
        btnDel.Click += btnDel_Click;
        // 
        // lbData
        // 
        lbData.Dock = DockStyle.Fill;
        lbData.FormattingEnabled = true;
        lbData.Location = new Point(0, 25);
        lbData.Name = "lbData";
        lbData.Size = new Size(298, 288);
        lbData.TabIndex = 1;
        // 
        // ClassManager
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(298, 313);
        Controls.Add(lbData);
        Controls.Add(toolStrip1);
        Name = "ClassManager";
        Text = "分类管理";
        toolStrip1.ResumeLayout(false);
        toolStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ToolStrip toolStrip1;
    private ListBox lbData;
    private ToolStripTextBox txtClassName;
    private ToolStripButton btnSave;
    private ToolStripButton btnDel;
}