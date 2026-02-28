namespace SmartMedia.Core.Controls;

partial class ImageList
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
         if (disposing)
        {
            ClearImages();
            openFileDialog?.Dispose();
            imageContainer?.Dispose();
        }
        base.Dispose(disposing);

        base.Dispose(disposing);
    }

    #region 组件设计器生成的代码

    /// <summary> 
    /// 设计器支持所需的方法 - 不要修改
    /// 使用代码编辑器修改此方法的内容。
    /// </summary>
    private void InitializeComponent()
    {
        imageContainer = new FlowLayoutPanel();
        btnAddImage = new XS.WinFormsTools.XsButton();
        SuspendLayout();
        // 
        // imageContainer
        // 
        imageContainer.Location = new Point(3, 41);
        imageContainer.Name = "imageContainer";
        imageContainer.Size = new Size(402, 121);
        imageContainer.TabIndex = 1;
        // 
        // btnAddImage
        // 
        btnAddImage.BackColor = Color.DeepSkyBlue;
        btnAddImage.ButtonType = XS.WinFormsTools.HelperCore.ButtonType.Default;
        btnAddImage.ConerRadius = 5;
        btnAddImage.Font = new Font("Segoe UI", 9F);
        btnAddImage.Location = new Point(3, 3);
        btnAddImage.Name = "btnAddImage";
        btnAddImage.Size = new Size(98, 32);
        btnAddImage.TabIndex = 2;
        btnAddImage.Text = "添加图片集";
        btnAddImage.TextColor = Color.White;
        btnAddImage.Click += btnAddImage_Click;
        // 
        // ImageList
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        Controls.Add(btnAddImage);
        Controls.Add(imageContainer);
        Name = "ImageList";
        Size = new Size(409, 169);
        ResumeLayout(false);
    }

    #endregion
    private FlowLayoutPanel imageContainer;
    private XS.WinFormsTools.XsButton btnAddImage;
}