
using SmartMedia.Modules.PushContent.DB;
using System.Data;

namespace SmartMedia.Modules.PushContent;

public partial class ClassManager : XS.WinFormsTools.WinMiniformBase
{
    private int _itype;
    public ClassManager(int itype)
    {
        _itype = itype;
        InitializeComponent();

        BindClass();

        this.StartPosition = FormStartPosition.CenterScreen;
    }

    private void BindClass()
    {
        var lst = PushContentClassBll.Instance.FindByTypeId(_itype);

        lbData.DataSource = lst;
        lbData.DisplayMember = "ClassName"; // 显示产品名称
        lbData.ValueMember = "Id";     // 值对应产品ID

    }

    private void btnSave_Click(object sender, EventArgs e)
    {

        string sClassName = txtClassName.Text.Trim();

        if (string.IsNullOrWhiteSpace(sClassName))
        {
            MessageBox.Show("请输入分类名称！");
            return;
        }

        var model = new PushContentClass();

        model.ClassName = sClassName;
        model.IType = _itype;

        PushContentClassBll.Instance.Add(model);

        BindClass();

    }

    private void btnDel_Click(object sender, EventArgs e)
    {
        if (lbData.SelectedItems.Count > 0)
        {
            // 确认删除
            DialogResult result = MessageBox.Show(
                $"确定要删除选中的 {lbData.SelectedItems.Count} 个数据吗？",
                "确认删除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // 获取所有选中项（转换为列表，避免修改集合时的枚举错误）
                var selectedItems = lbData.SelectedItems.Cast<PushContentClass>().ToList();

                foreach (var md in selectedItems)
                {
                    PushContentClassBll.Instance.Delete(md.Id);
                }

                BindClass();
            }
        }
        else
        {
            MessageBox.Show("请先选择要删除的项目！");
        }
    }
}
