using System.ComponentModel;

namespace SmartMedia.Core.Controls;

public partial class ImageList : UserControl
{
    private List<PictureBox> imageBoxes = new List<PictureBox>();
    private OpenFileDialog openFileDialog;

    public ImageList()
    {
        InitializeComponent();
        InitializeFileDialog();
        //AutoSize = true;
        InitializeContainer();
    }

    private void InitializeContainer()
    {
        // 确保 imageContainer 是 FlowLayoutPanel
        if (imageContainer is FlowLayoutPanel flowPanel)
        {
            flowPanel.FlowDirection = FlowDirection.LeftToRight;
            flowPanel.WrapContents = false; // 关键：不换行
            flowPanel.AutoSize = false;
            flowPanel.AutoScroll = true; // 启用滚动条
            flowPanel.HorizontalScroll.Enabled = true;
            flowPanel.HorizontalScroll.Visible = true;
            flowPanel.VerticalScroll.Enabled = false;
            flowPanel.VerticalScroll.Visible = false;
            flowPanel.Height = 136; // 固定高度，容纳图片+边距
            flowPanel.Padding = new Padding(5);
        }
    }
    // 新增属性：获取和设置图片路径数组
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string?[] ImgPathValues
    {
        get
        {
            // 返回当前所有图片的路径数组
            return imageBoxes
                .Select(pb => pb.Tag as string)
                .Where(path => !string.IsNullOrEmpty(path))
                .ToArray();
        }
        set
        {
            ClearImages();

            if (value != null)
            {
                List<string> failedPaths = new List<string>();

                foreach (var imagePath in value)
                {
                    if (string.IsNullOrEmpty(imagePath))
                        continue;

                    try
                    {
                        if (File.Exists(imagePath))
                        {
                            AddImage(imagePath);
                        }
                        else
                        {
                            failedPaths.Add(imagePath);
                        }
                    }
                    catch (Exception)
                    {
                        failedPaths.Add(imagePath);
                    }
                }

                 
            }
        }
    }

    private void InitializeFileDialog()
    {
        openFileDialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*",
            Title = "选择图片"
        };
    }

    public List<string> ImagePaths => imageBoxes
        .Select(pb => pb.Tag as string)
        .Where(path => !string.IsNullOrEmpty(path))
        .ToList();

   

    public void AddImage(string imagePath)
    {
        try
        {
            var pictureBox = CreatePictureBox(imagePath);
            imageContainer.Controls.Add(pictureBox);
            imageBoxes.Add(pictureBox);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法加载图片: {ex.Message}", "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void AddImage(Image image)
    {
        try
        {
            var pictureBox = CreatePictureBox(image);
            imageContainer.Controls.Add(pictureBox);
            imageBoxes.Add(pictureBox);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法添加图片: {ex.Message}", "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private PictureBox CreatePictureBox(string imagePath)
    {
        var image = Image.FromFile(imagePath);
        return CreatePictureBox(image, imagePath);
    }

    private PictureBox CreatePictureBox(Image image, string imagePath = null)
    {
        var pictureBox = new PictureBox
        {
            Size = new Size(100, 100), // 固定大小
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(5),
            BackColor = Color.White,
            Cursor = Cursors.Hand,
            Tag = imagePath
        };

        pictureBox.Image = image;
        AddContextMenu(pictureBox);
        pictureBox.Click += PictureBox_Click;
        pictureBox.DoubleClick += PictureBox_DoubleClick;

        return pictureBox;
    }

    private void AddContextMenu(PictureBox pictureBox)
    {
        var contextMenu = new ContextMenuStrip();

        var menuDelete = new ToolStripMenuItem("删除");
        menuDelete.Click += (s, e) => RemoveImage(pictureBox);

        var menuView = new ToolStripMenuItem("查看大图");
        menuView.Click += (s, e) => ViewImage(pictureBox.Image);

        var menuSave = new ToolStripMenuItem("另存为");
        menuSave.Click += (s, e) => SaveImage(pictureBox.Image);

        contextMenu.Items.AddRange(new ToolStripItem[] { menuView, menuSave, menuDelete });
        pictureBox.ContextMenuStrip = contextMenu;
    }

    private void PictureBox_Click(object sender, EventArgs e)
    {
        var pictureBox = sender as PictureBox;
        if (pictureBox != null)
        {
            pictureBox.BorderStyle = pictureBox.BorderStyle == BorderStyle.FixedSingle
                ? BorderStyle.Fixed3D
                : BorderStyle.FixedSingle;
        }
    }

    private void PictureBox_DoubleClick(object sender, EventArgs e)
    {
        var pictureBox = sender as PictureBox;
        if (pictureBox?.Image != null)
        {
            ViewImage(pictureBox.Image);
        }
    }

    private void ViewImage(Image image)
    {
        var form = new Form
        {
            Text = "查看图片",
            StartPosition = FormStartPosition.CenterScreen,
            Size = new Size(800, 600),
            BackColor = Color.Black
        };

        var viewPictureBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = image
        };

        form.Controls.Add(viewPictureBox);
        form.ShowDialog();
    }

    private void SaveImage(Image image)
    {
        using (var saveDialog = new SaveFileDialog())
        {
            saveDialog.Filter = "JPEG 图片|*.jpg|PNG 图片|*.png|BMP 图片|*.bmp";
            saveDialog.Title = "保存图片";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                image.Save(saveDialog.FileName);
            }
        }
    }

    public void RemoveImage(PictureBox pictureBox)
    {
        if (imageContainer.Controls.Contains(pictureBox))
        {
            imageContainer.Controls.Remove(pictureBox);
            imageBoxes.Remove(pictureBox);

            pictureBox.Image?.Dispose();
            pictureBox.Dispose();
        }
    }

    public void ClearImages()
    {
        foreach (var pictureBox in imageBoxes)
        {
            pictureBox.Image?.Dispose();
            pictureBox.Dispose();
        }

        imageContainer.Controls.Clear();
        imageBoxes.Clear();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        // 调整容器宽度，跟随控件宽度变化
        if (imageContainer != null)
        {
            imageContainer.Width = this.ClientSize.Width;
        }
    }

    private void btnAddImage_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            foreach (var filePath in openFileDialog.FileNames)
            {
                AddImage(filePath);
            }
        }
    }
}