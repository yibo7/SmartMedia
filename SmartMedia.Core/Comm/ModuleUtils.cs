
using System.Reflection;
using XS.Core2;

namespace SmartMedia.Core.Comm;

public class ModuleUtils
{

    // 声明事件，使用内置的 EventHandler 委托类型
    static public event EventHandler<EventArgsOnShowWin> EvShowToRight;

    // 触发事件的方法
    static public void OnEvShowToRight(EventArgsOnShowWin ev)
    {
        if (EvShowToRight != null)
        {
            // 触发事件
            EvShowToRight?.Invoke(null, ev);
        }
    }

    public static Dictionary<string, IModules> LoadModules()
    {
        Dictionary<string, IModules> dic = new Dictionary<string, IModules>();
        List<IModules> lst = getModuleList();
        foreach (var model in lst)
        {
            dic.Add(model.Title, model);
        }
        return dic;

    }
    private static List<IModules> getModuleList()
    {
        List<Assembly> assModule = new List<Assembly>();
        List<Assembly> assAll = new List<Assembly>();
        try
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            assAll = assModule.Union(asms).ToList();//Concat保留重复项,Union去重

        }
        catch (Exception ex)
        {
            LogHelper.Error<ModuleUtils>($"默认模块加载发生异常:【{ex.Message}】");
        }

        var modules = LoadInterface<IModules>(assAll);

        modules = modules.OrderBy(m => m.OrderIndex).ToList();

        return modules;
    }

    public static List<T> LoadInterface<T>(List<Assembly> asms) where T : class
    {
        List<T> lstModules = new List<T>();
        foreach (var asm in asms)
        {
            try
            {
                var tTypes = asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(T))).ToArray();
                foreach (var type in tTypes)
                {
                    if (type.IsClass)
                    {
                        try
                        {
                            T md = Activator.CreateInstance(type) as T;
                            if (!Equals(md, null))
                            {
                                lstModules.Add(md);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<ModuleUtils>($"模块类型转换发生异常:【{ex.Message}】");
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        return lstModules;
    }
}
