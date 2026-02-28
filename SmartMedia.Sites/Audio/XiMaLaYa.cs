using Microsoft.Playwright;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using System.Text.RegularExpressions;

namespace SmartMedia.Sites.Audio
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
 

        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {            

            CallBack($"开始处理【{this.PluginName}】");
             
            var err = base.CheckModel(model);

            if (!string.IsNullOrWhiteSpace(err))
                return err;
            
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
            CallBack("音频上传完毕，开始设置专题!"); 

            // 直接等待包含"上传成功"文本的元素出现
            await frame.GetByText("上传成功").WaitForAsync(new LocatorWaitForOptions
            {
                Timeout = 90000
            });

            await page.WaitForTimeoutAsync(2000);
            var special = GetSpecialName();
            if (!string.IsNullOrWhiteSpace(special))
            {
                await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "选择专辑" }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);
                await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Button, new() { Name = special }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);
            }
            CallBack("开始设置标题!");
            await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Textbox, new() { Name = "请输入声音标题" }).ClickAsync();
            await page.WaitForTimeoutAsync(2000);
            await page.Locator("#contentWrapper").ContentFrame.GetByRole(AriaRole.Textbox, new() { Name = "请输入声音标题" }).FillAsync(model.Title);
            await page.WaitForTimeoutAsync(2000);
             

            await page.Locator("#contentWrapper").ContentFrame.Locator("iframe").First.ContentFrame.Locator("body").ClickAsync();
            await page.WaitForTimeoutAsync(2000);
            await page.Locator("#contentWrapper").ContentFrame.Locator("iframe").First.ContentFrame.Locator("body").FillAsync(model.Info);
            CallBack("开始添加标签!");

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

            CallBack("设置字幕!");
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
            await page.WaitForTimeoutAsync(10000);

            return "";

        }

        public override string Help => @" 
";

    }
}
