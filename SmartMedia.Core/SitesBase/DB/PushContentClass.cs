using SmartMedia.Core; 
using XS.Data2.LiteDBBase;

namespace SmartMedia.Modules.PushContent.DB;
 

[LiteTable("PushContentClass")] // 指定表名称
public class PushContentClass : LiteModelBaseInt
{
    public string ClassName { get; set; }
    public int IType { get; set; }  // 1.视频，2.音频, 3.文章， 4.图文  

}


public class PushContentClassBll : LiteDbInt<PushContentClass>
{
    public static readonly PushContentClassBll Instance = new PushContentClassBll();

    public List<PushContentClass> FindByTypeId(int itype)
    {
        return base.Find($"IType={itype}", 1000);
    }
}