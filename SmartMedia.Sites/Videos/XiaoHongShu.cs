using Microsoft.Playwright;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using XS.Core2.XsExtensions;
namespace SmartMedia.Sites.Videos
{
    public class XiaoHongShu : VideoPushBase
    {
        override protected Dictionary<string, SettingCtrBase> SiteCtrls => new Dictionary<string, SettingCtrBase>()
        {
        };
         
        protected override string UploadPage => "https://creator.xiaohongshu.com/publish/publish";

        public override string DataPage => "https://creator.xiaohongshu.com/new/home";

        public override string PluginName => "小红书";
        override public Image IcoName => Resource.xiaohongshu;
        override public async Task<string> LoginAsync()
        {
            return await LoginCustom("https://creator.xiaohongshu.com/login", "text=发布");
        }
        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {
            CallBack("视频上传中...");
            // 选择视频文件
            await page.SetInputFilesAsync("input[type=file]", model.FilePath);

            await page.WaitForSelectorAsync("text='替换视频'");

            #region 更改封面
            
            CallBack("正在更改封面...");

            string coverPath = base.GetCoverPath();
            coverPath = string.IsNullOrWhiteSpace(coverPath) ? model.ImgPath : coverPath;

            if (!string.IsNullOrEmpty(coverPath) && File.Exists(coverPath)) // 上传封面
            {
                //await page.GetByRole(AriaRole.Button, new() { Name = "设置封面" }).ClickAsync();
                await page.Locator("div.upload-cover > div.text:has-text('设置封面')").ClickAsync();
                await page.WaitForTimeoutAsync(3000);


                // 定位第二个文件输入（封面图片的input）
                var coverImageInput = page.Locator("input[type='file'][accept*='image']");

                if (await coverImageInput.CountAsync() > 0)
                {
                    PrintLog("找到封面图片上传input");
                    await coverImageInput.SetInputFilesAsync(coverPath);
                    PrintLog("封面图片上传成功");
                    await page.WaitForTimeoutAsync(1000);
                }
                else
                {
                    PrintLog("未找到封面图片上传input，可能元素不存在或已隐藏");
                }
                 
                await page.WaitForTimeoutAsync(1000);
                await page.GetByRole(AriaRole.Button, new() { Name = "确定" }).ClickAsync();
                await page.WaitForTimeoutAsync(1000);
            }
            else
            {
                PrintLog($"封面图片不存在或地址为空：{coverPath}");
            }
            #endregion
            CallBack("正在输入标题...");
            if (!string.IsNullOrEmpty(model.Title))
            {
                //await page.GetByPlaceholder("填写标题，可能会有更多赞哦～").FillAsync(model.Title.CutStrLen(20));
                await page.GetByRole(AriaRole.Textbox, new() { Name = "填写标题会有更多赞哦～" }).ClickAsync();
                await page.WaitForTimeoutAsync(1000);
                await page.GetByRole(AriaRole.Textbox, new() { Name = "填写标题会有更多赞哦～" }).FillAsync(model.Title.CutStrLen(20));

                await page.WaitForTimeoutAsync(1000);

            }



            CallBack("正在输入内容...");
            await page.WaitForTimeoutAsync(1000);
            // 输入内容
             
            #region 视频简介
            if (!string.IsNullOrWhiteSpace(model.Info))
            {
                // 请填写视频简介 
                await page.GetByRole(AriaRole.Textbox).Nth(1).ClickAsync();
                await page.WaitForTimeoutAsync(1000);
                await page.GetByRole(AriaRole.Textbox).Nth(1).FillAsync(model.Info.CutStrLen(1000));
                await page.WaitForTimeoutAsync(1000);
            }
            #endregion

            CallBack("正在设置专题...");
            string specialName = GetSpecialName();

            if (!string.IsNullOrWhiteSpace(specialName))
            {
                await page.GetByText("添加合集").Nth(1).ClickAsync();
                await page.WaitForTimeoutAsync(1000);
                await page.GetByText(specialName, new() { Exact = true }).ClickAsync();

            }

            CallBack("声明...");
            await page.GetByText("去声明").ClickAsync();
            await page.WaitForTimeoutAsync(1000);
            await page.Locator(".d-checkbox-indicator").ClickAsync();
            await page.WaitForTimeoutAsync(1000);
            await page.GetByRole(AriaRole.Button, new() { Name = "声明原创" }).ClickAsync();

            CallBack("即将发布...");
            await page.WaitForTimeoutAsync(1000);
            await page.GetByRole(AriaRole.Button, new() { Name = "发布" }).ClickAsync();
            await page.WaitForTimeoutAsync(5000);


            return "";

        }

        public override string Help => @" 
";

    }
}
