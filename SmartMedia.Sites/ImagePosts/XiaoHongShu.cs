using Microsoft.Playwright;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using XS.Core2.XsExtensions;
namespace SmartMedia.Sites.ImagePosts;

public class XiaoHongShu : ImagePushBase
{
   
     
    protected override string UploadPage => "https://creator.xiaohongshu.com/publish/publish?from=menu&target=image";

    public override string DataPage => "https://creator.xiaohongshu.com/new/home";

    public override string PluginName => "小红书图文";
    override public Image IcoName => Resource.xiaohongshu;
    override public async Task<string> LoginAsync()
    {
        return await LoginCustom("https://creator.xiaohongshu.com/login", "text=发布笔记");
    }
    protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
    {
        CallBack("开始上传图片!");
        var ImgPaths = base.GetCoverPaths(model);
        if (ImgPaths.Length < 1)
            return "没有可上传的图片";

        // 选择视频文件
        await page.SetInputFilesAsync("input[type=file]", ImgPaths);

        //await page.WaitForSelectorAsync("text='清空并重新上传'");
        await page.WaitForTimeoutAsync(3000);

        CallBack("开始设置标题!");
        if (!string.IsNullOrEmpty(model.Title))
        {
            var titleInput = page.GetByPlaceholder("填写标题会有更多赞哦");
            titleInput.ClickAsync();
            await page.WaitForTimeoutAsync(1000);
            titleInput.FillAsync(model.Title.CutStrLen(20));
            await page.WaitForTimeoutAsync(1000);

            //await page.GetByPlaceholder("填写标题，可能会有更多赞哦～").FillAsync(model.Title.CutStrLen(20));
            //await page.GetByRole(AriaRole.Textbox, new() { Name = "填写标题会有更多赞哦～" }).ClickAsync();
            //await page.WaitForTimeoutAsync(1000);
            //await page.GetByRole(AriaRole.Textbox, new() { Name = "填写标题会有更多赞哦～" }).FillAsync(model.Title.CutStrLen(20));

            //await page.WaitForTimeoutAsync(1000);

        } 
        
        await page.WaitForTimeoutAsync(1000);
        // 输入内容

        CallBack("开始设置简介!");

        #region 视频简介
        if (!string.IsNullOrWhiteSpace(model.Info))
        {
            string sContent = model.Info.CutStrLen(1000);
            if (!string.IsNullOrWhiteSpace(model.Tags))
            {
                sContent = $"{sContent}\n\r{model.FormatTags()}"; 
            }
            // 请填写视频简介 
            await page.GetByRole(AriaRole.Textbox).Nth(1).ClickAsync();
            await page.WaitForTimeoutAsync(1000);
            await page.GetByRole(AriaRole.Textbox).Nth(1).FillAsync(sContent);
            await page.WaitForTimeoutAsync(1000);
        }
        #endregion



        CallBack("开始设置专题!");
        string specialName = GetSpecialName();

        if (!string.IsNullOrWhiteSpace(specialName))
        {
            await page.GetByText("添加合集").Nth(1).ClickAsync();
            await page.WaitForTimeoutAsync(1000);
            await page.GetByText(specialName, new() { Exact = true }).ClickAsync();

        }

        CallBack("开始设置声明，即将发布!");
        await page.Locator(".custom-switch-switch").First.ClickAsync();

        await page.WaitForTimeoutAsync(1000);
        await page.GetByText("《原创声明须知》").First.ClickAsync();
        await page.WaitForTimeoutAsync(1000);
        await page.GetByText("声明原创").ClickAsync(); 
        await page.WaitForTimeoutAsync(2000);

        await page.GetByRole(AriaRole.Button, new() { Name = "发布" }).ClickAsync();
        await page.WaitForTimeoutAsync(5000);


        return "";

    }

    public override string Help => @" 
";

}
