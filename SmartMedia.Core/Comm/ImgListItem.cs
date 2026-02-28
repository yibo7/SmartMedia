namespace SmartMedia.Core.Comm;


public class ImgListItem<T>
{
    public T DockContentObj { get; set; } //DockContent
    public string Name { get; set; }

    public Image Ico { get; set; }

}
public class ImgListItemBll<T>
{
    private ImageList imageList = new ImageList();
    public List<ImgListItem<T>> Items = new List<ImgListItem<T>>();
    private System.Windows.Forms.ListView lv;
    public ImgListItemBll(ListView _lv)
    {
        lv = _lv;
    }

    public void Add(string sName, Image icoImg, T dc)
    {

        Items.Add(new ImgListItem<T>() { Name = sName, Ico = icoImg, DockContentObj = dc });
        imageList.Images.Add(icoImg);
        imageList.ImageSize = new Size(32, 32);// 设置行高 20 //分别是宽和高  
    }
    //public void Add(string sName, Image icoImg)
    //{

    //    Items.Add(new ImgListItem() { Name = sName, Ico = icoImg});
    //    imageList.Images.Add(icoImg);
    //    imageList.ImageSize = new Size(32, 32);// 设置行高 20 //分别是宽和高  
    //}
    public Action<ImgListItem<T>> OnMouseDoubleClick { get; set; }
    public Action<ImgListItem<T>> OnMouseClick { get; set; }
    private void lv_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (!Equals(OnMouseDoubleClick, null))
        {
            if (lv.SelectedItems.Count > 0)
            {
                int index = (int)lv.SelectedItems[0].Tag;
                ImgListItem<T> dcImgListItem = Items[index];
                OnMouseDoubleClick(dcImgListItem);
            }

        }
    }
    public void ClearItems()
    {
        //
        // 清空LargeImageList中的所有图像
        if (lv.LargeImageList != null)
        {
            lv.LargeImageList.Images.Clear();
        }

        // 清空ListView中的所有项
        lv.Items.Clear();

        // 刷新ListView以确保显示更新
        lv.Refresh();
        Items.Clear();
    }
    public void Bind(bool IsShowTitle = true)
    {

        lv.View = View.LargeIcon;
        lv.Click += Lv_Click;
        lv.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_MouseDoubleClick);
        lv.LargeImageList = imageList;

        lv.BeginUpdate();

        for (int i = 0; i < Items.Count; i++)
        {
            ImgListItem<T> lli = Items[i];
            ListViewItem lvi = new ListViewItem();
            lvi.ImageIndex = i;
            if (IsShowTitle)
                lvi.Text = lli.Name;
            lvi.Tag = i;
            lv.Items.Add(lvi);

        }

        lv.EndUpdate();
        lv.Refresh();
    }

    private void Lv_Click(object? sender, EventArgs e)
    {
        if (!Equals(OnMouseClick, null))
        {
            if (lv.SelectedItems.Count > 0)
            {
                int index = (int)lv.SelectedItems[0].Tag;
                ImgListItem<T> dcImgListItem = Items[index];
                OnMouseClick(dcImgListItem);
            }

        }
    }
}
