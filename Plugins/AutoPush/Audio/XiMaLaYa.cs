using Microsoft.Playwright;
using SmartMedia.Controls;
using SmartMedia.Modules.PushContent.DB;
using SmartMedia.Plugins.AutoPush.Article;
using System.Text.RegularExpressions;
using XS.Core2.XsExtensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartMedia.Plugins.AutoPush.Audio
{
    public class XiMaLaYa : AudioPushBase
    {

        protected override string UploadPage => "https://studio.ximalaya.com/upload";

        public override string DataPage => "https://studio.ximalaya.com/dataPreviewV2";

        public override string PluginName => "喜马拉雅";
        override public Image IcoName => Resource.ximalaya;
        override public async Task<string> LoginAsync()
        {
            return await LoginCustom("https://passport.ximalaya.com/page/web/login?fromUri=https%3A%2F%2Fstudio.ximalaya.com", "div:has-text('核心数据')");
        }

        //override protected Dictionary<string, SettingCtrBase> SiteCtrls => new Dictionary<string, SettingCtrBase>() {
        //    { "IsAiMake",base.BuildCtr<SettingCheckBox>("是否AI生成","","False")},
        //    { "SpecialName",BuildCtr<SettingTextBox>("合集名称","可为空，你在此平台上创建的合集名称") },
        //    { "CoverImageH",BuildCtr<SettingCoverImage>("设置竖封面","可为空，要求为3:4图片格式") }
        //};

        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {

            PrintLog($"开始处理【{this.PluginName}】");
            var err = base.CheckModel(model);

            if (!string.IsNullOrWhiteSpace(err))
                return err;
            // 等待 iframe 加载
            //await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            //// 选择视频文件
            ////await page.SetInputFilesAsync("input[type=file]", model.FilePath);
            //await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "上传音频" }).ClickAsync();
            //await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "上传音频" }).SetInputFilesAsync(model.FilePath);

            // 确保在正确的 iframe 中
            var frame = page.Locator("#contentWrapper").ContentFrame;

            // 方法1：直接查找文件输入
            var fileInput = frame.Locator("input[type='file']").First;

            if (await fileInput.CountAsync() > 0)
            {
                await fileInput.SetInputFilesAsync(model.FilePath);
            }
            else
            {
                // 方法2：点击按钮后等待文件输入
                await frame.GetByRole(AriaRole.Button, new() { Name = "上传音频" }).ClickAsync();

                // 等待文件输入出现
                await frame.Locator("input[type='file']")
                    .First
                    .WaitForAsync(new() { State = WaitForSelectorState.Attached });

                await frame.Locator("input[type='file']")
                    .First
                    .SetInputFilesAsync(model.FilePath);
            }

            // 等待上传
            //await page.WaitForSelectorAsync(":has-text('上传成功')");


            await page.WaitForTimeoutAsync(20000);
            var special = GetSpecialName();
            if (!string.IsNullOrWhiteSpace(special))
            {
                await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "选择专辑" }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);
                await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = special }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);
            }
            
            await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Textbox, new() { Name = "请输入声音标题" }).ClickAsync();
            await page.WaitForTimeoutAsync(2000);
            await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Textbox, new() { Name = "请输入声音标题" }).FillAsync(model.Title);
            await page.WaitForTimeoutAsync(2000);

            //await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Radio, new() { Name = "是" }).CheckAsync();
            //await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Radio, new() { Name = "否" }).CheckAsync();

            await page.Locator("#contentWrapper").ContentFrame.Locator("iframe").First.ContentFrame.Locator("body").ClickAsync();
            await page.WaitForTimeoutAsync(2000);
            await page.Locator("#contentWrapper").ContentFrame.Locator("iframe").First.ContentFrame.Locator("body").FillAsync(model.Info);


            #region 添加标签
            var TagList = model.TagList();
            if (TagList.Count > 0)
            {
                var tagCount = Math.Min(TagList.Count, 10);
                for (int i = 0; i < tagCount; i++)
                { 
                    await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Textbox, new() { Name = "回车键创建" }).ClickAsync();
                    await page.WaitForTimeoutAsync(1000);
                    await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Textbox, new() { Name = "回车键创建" }).FillAsync(TagList[i]);
                    await page.WaitForTimeoutAsync(1000);
                    await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Textbox, new() { Name = "回车键创建" }).PressAsync("Enter");
                    await page.WaitForTimeoutAsync(2000);
                }  
            }
            #endregion
             
            
            string lrcContent = GetLrcContent();
            if (!string.IsNullOrWhiteSpace(lrcContent))
            {
                await page.Locator("#contentWrapper").ContentFrame.Locator("div").Filter(new() { HasTextRegex = new Regex("^更多设置$") }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);
                await page.Locator("#contentWrapper").ContentFrame.GetByText("编辑字幕").ClickAsync();
                await page.WaitForTimeoutAsync(2000);
                await page.Locator("#contentWrapper").ContentFrame.Locator("span").Filter(new() { HasText = "字幕文件编辑字幕字幕添加技巧确认提交请填写带有时间轴的" }).GetByRole(AriaRole.Textbox).ClickAsync();

                await page.WaitForTimeoutAsync(2000);
                await page.Locator("#contentWrapper").ContentFrame.Locator("span").Filter(new() { HasText = "字幕文件编辑字幕字幕添加技巧确认提交请填写带有时间轴的" }).GetByRole(AriaRole.Textbox).FillAsync(lrcContent);
                await page.WaitForTimeoutAsync(2000);
                await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "确认提交字幕" }).ClickAsync();
               
            }
            await page.WaitForTimeoutAsync(2000);
            await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "确认发布" }).ClickAsync();
            await page.WaitForTimeoutAsync(5000);

            return "";

        }

        public override string Help => @" 
";

    }
}
