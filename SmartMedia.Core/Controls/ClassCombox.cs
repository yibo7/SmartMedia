using SmartMedia.Modules.PushContent.DB; 
using System.ComponentModel; 
using XS.Core2.XsExtensions;

namespace SmartMedia.Core.Controls;

public partial class ClassCombox : UserControl
{
    public ClassCombox()
    {
        InitializeComponent(); 
    }



    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Value
    {
        get
        {
            if (cbClass.SelectedValue == null || cbClass.SelectedIndex < 0)
                return 0;
            return cbClass.SelectedValue.ToString().ToInt();
        }
        set
        {
            // 等待控件初始化完成
            if (!cbClass.IsHandleCreated)
            {
                cbClass.HandleCreated += (s, e) => SetSelectedValue(value);
                return;
            }

            SetSelectedValue(value);
        }
    }

    private void SetSelectedValue(int value)
    {
        // 直接使用SelectedValue可能失败，添加回退逻辑
        cbClass.SelectedValue = value;

        // 验证是否设置成功
        if (cbClass.SelectedValue?.ToString().ToInt() != value)
        {
            // 遍历查找对应的项
            for (int i = 0; i < cbClass.Items.Count; i++)
            {
                if (cbClass.Items[i] is PushContentClass item && item.Id == value)
                {
                    cbClass.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    public void BindClass(int itype)
    {
        var lst = PushContentClassBll.Instance.FindByTypeId(itype);

        // 添加默认分类
        var defaultItem = new PushContentClass { Id = 0, ClassName = "选择自定义分类" };
        var bllClass = new PushContentClassBll();
        var classList = bllClass.FindByTypeId(itype);
        // 绑定到ToolStripComboBox
        cbClass.DataSource = new List<PushContentClass>(classList)
            .Prepend(defaultItem)  // 将默认项添加到开头
            .ToList();
         
        cbClass.DisplayMember = "ClassName"; // 显示产品名称
        cbClass.ValueMember = "Id";     // 值对应产品ID
        cbClass.SelectedIndex = 0;
    }
}
