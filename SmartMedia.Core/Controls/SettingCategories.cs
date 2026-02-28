using Newtonsoft.Json.Linq;
using SmartMedia.Core.SitesBase;
using System.Data;
using System.Diagnostics;

namespace SmartMedia.Core.Controls;

public partial class SettingCategories : SettingCtrBase
{
    private string[] SelItems = new string[3];
    private string CategoryFileName;
    private bool _isSettingValue = false;

    public SettingCategories()
    {
        InitializeComponent();
        this.Load += SettingCategories_Load;
        //CategoryFileName = categoryFileName;

        //if (!DesignMode)
        //{
        //    InitCategorys();
        //}
    }

    override public void InitData(string categoryFileName)
    {
        CategoryFileName = categoryFileName;

        if (!DesignMode)
        {
            InitCategorys();
        }
    }

    private void SettingCategories_Load(object? sender, EventArgs e)
    {
        InitCategorys();
    }

    override public void SetValue(string value)
    {
        try
        {
            _isSettingValue = true;

            // 清空之前的选中项
            for (int i = 0; i < SelItems.Length; i++)
            {
                SelItems[i] = null;
            }

            // 如果有多个值用逗号分隔
            if (!string.IsNullOrEmpty(value))
            {
                string[] values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                // 根据值的数量设置相应的分类级别
                switch (values.Length)
                {
                    case 1:
                        // 只有一级分类，如："博客与人物"
                        SelItems[0] = values[0].Trim();
                        if (cbCategory1.Items.Count > 0)
                        {
                            cbCategory1.SelectedValue = values[0].Trim();
                            cbCategory2.Visible = false;
                            cbCategory3.Visible = false;
                            SelItems[1] = null;
                            SelItems[2] = null;
                        }
                        break;

                    case 2:
                        // 只有二级分类，如："军事,国防动态"
                        SetSecondLevelCategory(values[0].Trim(), values[1].Trim());
                        break;

                    case 3:
                        // 三级分类，如："生活,日常,手工"
                        SetThirdLevelCategory(values[0].Trim(), values[1].Trim(), values[2].Trim());
                        break;

                    default:
                        Debug.WriteLine($"不支持的分类格式：{value}");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"设置分类控件出错：{ex.Message}");
        }
        finally
        {
            _isSettingValue = false;
        }
    }

    /// <summary>
    /// 设置二级分类
    /// </summary>
    private void SetSecondLevelCategory(string firstLevel, string secondLevel)
    {
        SelItems[0] = firstLevel;
        SelItems[1] = secondLevel;
        SelItems[2] = null; // 清空第三级

        // 确保UI已初始化
        if (cbCategory1.Items.Count == 0)
            return;

        try
        {
            // 先设置一级分类
            cbCategory1.SelectedValue = firstLevel;

            // 等待一级分类设置完成
            this.BeginInvoke((Action)(() =>
            {
                try
                {
                    // 检查一级分类是否有二级子项
                    if (cbCategory2.Visible && cbCategory2.Items.Count > 0)
                    {
                        // 设置二级分类
                        cbCategory2.SelectedValue = secondLevel;
                        SelItems[1] = secondLevel; // 确保更新
                        cbCategory3.Visible = false;
                        cbCategory3.SelectedIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"设置二级分类失败：{secondLevel}, 错误：{ex.Message}");
                }
            }));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"设置一级分类失败：{firstLevel}, 错误：{ex.Message}");
        }
    }

    /// <summary>
    /// 设置三级分类
    /// </summary>
    private void SetThirdLevelCategory(string firstLevel, string secondLevel, string thirdLevel)
    {
        SelItems[0] = firstLevel;
        SelItems[1] = secondLevel;
        SelItems[2] = thirdLevel;

        // 确保UI已初始化
        if (cbCategory1.Items.Count == 0)
            return;

        try
        {
            // 先设置一级分类
            cbCategory1.SelectedValue = firstLevel;

            // 等待一级分类设置完成
            this.BeginInvoke((Action)(() =>
            {
                try
                {
                    // 设置二级分类
                    if (cbCategory2.Visible && cbCategory2.Items.Count > 0)
                    {
                        cbCategory2.SelectedValue = secondLevel;
                        SelItems[1] = secondLevel; // 确保更新

                        // 等待二级分类设置完成
                        this.BeginInvoke((Action)(() =>
                        {
                            try
                            {
                                // 设置三级分类
                                if (cbCategory3.Visible && cbCategory3.Items.Count > 0)
                                {
                                    cbCategory3.SelectedValue = thirdLevel;
                                    SelItems[2] = thirdLevel; // 确保更新
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"设置三级分类失败：{thirdLevel}, 错误：{ex.Message}");
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"设置二级分类失败：{secondLevel}, 错误：{ex.Message}");
                }
            }));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"设置一级分类失败：{firstLevel}, 错误：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取或设置分类，多个分类用逗号分开
    /// </summary>
    override public string Text
    {
        get
        {
            string[] aClassNames = GetComboBoxValues();
            if (aClassNames != null && aClassNames.Length > 0)
            {
                return string.Join(",", aClassNames);
            }
            return "";
        }
        set
        {
            string sCategroy = value;
            if (!string.IsNullOrEmpty(sCategroy))
            {
                string[] aCategroy = sCategroy.Split(',');
                int iIndex = 0;
                foreach (var item in aCategroy)
                {
                    if (iIndex < SelItems.Length)
                    {
                        SelItems[iIndex] = item;
                    }
                    iIndex++;
                }
            }
        }
    }

    override public string GetValue()
    {
         string[] aClassNames = GetComboBoxValues();
            if (aClassNames != null && aClassNames.Length > 0)
            {
                return string.Join(",", aClassNames);
            }
            return "";
    }

    //[Category("设置Tips文本")]
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    //public string Tips
    //{
    //    get { return lbTips.Text; }
    //    set
    //    {
    //        lbTips.Text = value;
    //    }
    //}
    //[Category("设置Title文本")]
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    //public string Title
    //{
    //    get { return lbTitle.Text; }
    //    set
    //    {
    //        if(!string.IsNullOrWhiteSpace(value))
    //            lbTitle.Text = value;
    //    }
    //}

    override public void SetTitle(string value) {
        if(!string.IsNullOrWhiteSpace(value))
                lbTitle.Text = value;
    }
    override public void SetTips(string value) {
        if (!string.IsNullOrWhiteSpace(value))
            lbTips.Text = value;
    }

    #region 平台分类处理
    /// <summary>
    /// 当前发布平台上的分类
    /// </summary>
    public List<CategoryItem> CategoryItems;
    public void InitCategorys()
    {
        if (!Equals(CategoryItems, null))
            return;
        CategoryItems = new List<CategoryItem>();
        if (!string.IsNullOrEmpty(CategoryFileName))
        {
            string CategoryFilePath = Path.Combine(Application.StartupPath, $"Datas\\Categorys\\{CategoryFileName}");

            if (File.Exists(CategoryFilePath))
            {
                string sJson = File.ReadAllText(CategoryFilePath);
                if (!string.IsNullOrEmpty(sJson))
                {
                    CategoryItems = ParseJson(sJson);
                    BindCategories();
                }
            }

        }
    }
    private List<CategoryItem> ParseJson(string json)
    {
        var result = new List<CategoryItem>();
        var jObject = JObject.Parse(json);

        foreach (var property in jObject.Properties())
        {
            var category = new CategoryItem(property.Name);
            if (property.Value.Type == JTokenType.Null)
            {
                // 一级分类
                result.Add(category);
            }
            else if (property.Value.Type == JTokenType.Array)
            {
                // 二级分类
                category.SubItems = property.Value.ToObject<List<string>>()
                    .Select(item => new CategoryItem(item))
                    .ToList();
                result.Add(category);
            }
            else if (property.Value.Type == JTokenType.Object)
            {
                // 二级分类
                category.SubItems = new List<CategoryItem>();
                foreach (var subProperty in property.Value.Children<JProperty>())
                {
                    var subCategory = new CategoryItem(subProperty.Name);
                    if (subProperty.Value.Type == JTokenType.Array)
                    {
                        // 三级分类
                        subCategory.SubItems = subProperty.Value.ToObject<List<string>>()
                            .Select(item => new CategoryItem(item))
                            .ToList();
                    }
                    else if (subProperty.Value.Type == JTokenType.Null)
                    {
                        // 二级分类没有子项
                        subCategory.SubItems = null;
                    }
                    category.SubItems.Add(subCategory);
                }
                result.Add(category);
            }
        }

        return result;
    }
    #endregion
    public string[] GetComboBoxValues()
    {
        // 如果正在设置值，直接返回SelItems
        if (_isSettingValue)
        {
            var val = SelItems.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
            return val;
        }

        // 否则从UI控件获取当前选择
        string[] currentSelections = new string[3];

        // 从一级分类获取
        if (cbCategory1.SelectedItem is CategoryItem item1)
        {
            currentSelections[0] = item1.Name;
        }

        // 从二级分类获取
        if (cbCategory2.Visible && cbCategory2.SelectedItem is CategoryItem item2)
        {
            currentSelections[1] = item2.Name;
        }

        // 从三级分类获取
        if (cbCategory3.Visible && cbCategory3.SelectedItem is CategoryItem item3)
        {
            currentSelections[2] = item3.Name;
        }

        var filteredItems = currentSelections.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
        return filteredItems;
    }

    private void BindCategories()
    {
        if (CategoryItems != null && CategoryItems.Count > 0)
        {
            BindCategories(CategoryItems);
        }
    }

    private void BindCategories(List<CategoryItem> categories)
    {
        // 绑定一级分类
        cbCategory1.DataSource = categories;
        cbCategory1.DisplayMember = "Name";
        cbCategory1.ValueMember = "Name";
        cbCategory1.SelectedIndex = -1;

        // 初始时隐藏二级和三级分类
        cbCategory2.Visible = false;
        cbCategory3.Visible = false;

        // 添加一级分类选择变化事件
        cbCategory1.SelectedIndexChanged += CbCategory1_SelectedIndexChanged;

        if (SelItems.Length > 0 && !string.IsNullOrEmpty(SelItems[0]))
            cbCategory1.SelectedValue = SelItems[0];
    }

    private void CbCategory1_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var selItem = cbCategory1.SelectedItem as CategoryItem;
        if (selItem != null)
        {
            SelItems[0] = selItem.Name;

            if (selItem.SubItems != null && selItem.SubItems.Any())
            {
                // 有二级分类，显示并绑定二级分类
                cbCategory2.Visible = true;
                cbCategory2.DataSource = selItem.SubItems;
                cbCategory2.DisplayMember = "Name";
                cbCategory2.ValueMember = "Name";
                cbCategory2.SelectedIndex = -1;

                // 隐藏三级分类
                cbCategory3.Visible = false;
                cbCategory3.SelectedIndex = -1;
                SelItems[2] = null;

                if (!string.IsNullOrEmpty(SelItems[1]))
                    cbCategory2.SelectedValue = SelItems[1];
            }
            else
            {
                // 没有二级分类，隐藏二级和三级
                cbCategory2.Visible = false;
                cbCategory3.Visible = false;
                cbCategory2.SelectedIndex = -1;
                cbCategory3.SelectedIndex = -1;
                SelItems[1] = null;
                SelItems[2] = null;
            }
        }
    }

    private void cbCategory2_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selItem = cbCategory2.SelectedItem as CategoryItem;

        if (selItem == null)
            return;

        SelItems[1] = selItem.Name;

        if (selItem.SubItems != null && selItem.SubItems.Any())
        {
            // 有三级分类，显示并绑定三级分类
            cbCategory3.Visible = true;
            cbCategory3.DataSource = selItem.SubItems;
            cbCategory3.DisplayMember = "Name";
            cbCategory3.ValueMember = "Name";
            cbCategory3.SelectedIndex = -1;

            if (!string.IsNullOrEmpty(SelItems[2]))
                cbCategory3.SelectedValue = SelItems[2];
        }
        else
        {
            // 没有三级分类，隐藏三级
            cbCategory3.Visible = false;
            cbCategory3.SelectedIndex = -1;
            SelItems[2] = null;
        }
    }

    private void cbCategory3_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selItem = cbCategory3.SelectedItem as CategoryItem;
        if (selItem == null)
            return;
        SelItems[2] = selItem.Name;
    }
}