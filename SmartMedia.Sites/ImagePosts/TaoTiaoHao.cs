using Microsoft.Playwright;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using System.Text.RegularExpressions;
using XS.Core2.XsExtensions;

namespace SmartMedia.Sites.ImagePosts;

public class TaoTiaoHao : ImagePushBase
{
    public override string PluginName => "微头条";
    override public Image IcoName => Resource.taotiaohao;
    protected override string UploadPage => "https://mp.toutiao.com/profile_v4/weitoutiao/publish";

    public override string DataPage => "https://mp.toutiao.com/profile_v4/analysis/works-overall/weitoutiao";

    override public async Task<string> LoginAsync()
    {
        return await LoginCustom("https://mp.toutiao.com/auth/page/login", "span:text('粉丝数')");
    }

    override protected Dictionary<string, SettingCtrBase> SiteCtrls
    {
        get
        {
            return new Dictionary<string, SettingCtrBase>()
            {
                //{ "SpecialName",BuildCtr<SettingTextBox>("合集名称","可为空，你在此平台上创建的合集名称") },
                //{ "IsPushToArticle",BuildCtr<SettingCheckBox>("同时发布为文章", "发布得更多收益", "false") },
                { "IsFirst",BuildCtr<SettingCheckBox>("头条首发声明", "符合首发质量标准且72小时内仅在头条发布的内容，可享额外激励分成", "true") },
                { "NoticeInfo",BuildCtr<SettingComboBox>("作品声明","默认为个人观点，如果需要声明，请选择声明项","个人观点，仅供参考","取材网络###虚构演绎，故事经历###个人观点，仅供参考###健康医疗分享，仅供参考###投资观点，仅供参考###引用AI") },
            };
        }
    }

    protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
    {
        CallBack("激活编辑框");
        await page.Locator(".publish-box").ClickAsync();
        await page.WaitForTimeoutAsync(1000);
        CallBack("输入文本");
        string targetText = "有什么新鲜事想告诉大家？";        
        await page.Locator("div").Filter(new() { HasTextRegex = new Regex($"^{targetText}$") }).Nth(1).FillAsync(model.Info);
        await page.WaitForTimeoutAsync(2000);

        await page.GetByRole(AriaRole.Button, new() { Name = "图片" }).ClickAsync();
        await page.WaitForTimeoutAsync(2000);

        CallBack("开始上传图片!");
        var ImgPaths = base.GetCoverPaths(model);
        if (ImgPaths.Length < 1)
            return "没有可上传的图片";

        // 选择视频文件
        await page.SetInputFilesAsync("input[type=file]", ImgPaths);
        await page.WaitForTimeoutAsync(3000);
        CallBack("图片上传完毕");
        await page.GetByRole(AriaRole.Button, new() { Name = "确定" }).ClickAsync();
        
        CallBack("头条首发设置");
        await page.WaitForTimeoutAsync(3000);

        var vIsFirst = base.GetCtrValue("IsFirst").ToBool();
        if (vIsFirst)
        {
            //await page.GetByText("头条首发").ClickAsync();
            await page.Locator("span").Filter(new() { HasText = "头条首发" }).First.ClickAsync();
            await page.WaitForTimeoutAsync(2000);
        }
        CallBack("作品声明设置");
        var vNoticeInfo = base.GetCtrValue("NoticeInfo");

        if (!string.IsNullOrWhiteSpace(vNoticeInfo))
        {
            await page.GetByText(vNoticeInfo).ClickAsync();
            await page.WaitForTimeoutAsync(2000);
        }
        CallBack("即将发布");
        await page.GetByRole(AriaRole.Button, new() { Name = "发布" }).ClickAsync();
        await page.WaitForTimeoutAsync(3000);

        return "";

    }

    public override string Help => @" 
";

}
