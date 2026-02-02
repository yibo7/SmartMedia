using Microsoft.Playwright;
using SmartMedia.Controls;
using SmartMedia.Modules.PushContent.DB;
using System.Text.RegularExpressions;
using XS.Core2.XsExtensions;

namespace SmartMedia.Plugins.AutoPush.Video
{
    public class WeiXinSPH : VideoPushBase
    {
        override public int OrderIndex => 6;
        public override string CategoryFileName => "WeiXinSPH.json";
        protected override string UploadPage => "https://channels.weixin.qq.com/platform/post/create";

        public override string DataPage => "https://channels.weixin.qq.com/platform/statistic/post";

        public override string PluginName => "微信视频号";
        override public Image IcoName => Resource.weixinsph;
        override public async Task<string> LoginAsync()
        {
            return await LoginCustom("https://channels.weixin.qq.com", "button:has-text('发表视频')");
        }

   
        override protected Dictionary<string, SettingCtrBase> SiteCtrls
        {
            get
            {
                return new Dictionary<string, SettingCtrBase>()
                { 
                    { "SpecialName",BuildCtr<SettingTextBox>("合集名称","可为空，你在平台上创建的合集名称") }
                };
            }
        }

        //override public Dictionary<string, string> CustomCtrls() => new Dictionary<string, string>() {
        //     { "合集名称",""}
        //};
        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {

            CallBack("正在上传视频...");
            // 选择视频文件
            await page.SetInputFilesAsync("input[type=file]", model.FilePath);

            // 等待元素出现

            await page.WaitForSelectorAsync("div.tag-inner:text('删除')", new()
            {
                State = WaitForSelectorState.Visible
            });

            CallBack("正在更改封面...");
            await page.WaitForTimeoutAsync(3000);
            #region 更改封面


            if (!string.IsNullOrEmpty(model.ImgPath)) // 上传封面
            {
                var editCoverButton = page.GetByText("更换封面");
                //var editCoverButton = await page.QuerySelectorAsync("div.tag-inner:text('更换封面')");

                await editCoverButton.ClickAsync();
                await page.WaitForTimeoutAsync(10000);


                var imgUpload = await page.QuerySelectorAsync("input[type='file'][accept='image/jpeg,image/jpg,image/png']");

                await imgUpload.SetInputFilesAsync(model.ImgPath);
                await page.WaitForTimeoutAsync(2000);

                await page.GetByRole(AriaRole.Button, new() { Name = "确定" }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);
                await page.GetByRole(AriaRole.Button, new() { Name = "确认" }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);

                //await page.WaitForTimeoutAsync(1000);
            }
            #endregion

            CallBack("正在设置标签...");
            // 输入内容
            var contentInput = await page.QuerySelectorAsync(".input-editor");
            await contentInput.ClickAsync();
            #region 添加标签
            var TagList = model.TagList();
            if (TagList.Count > 0)
            {
                var tagCount = Math.Min(TagList.Count, 3);
                for (int i = 0; i < tagCount; i++)
                {
                    await contentInput.TypeAsync($"#{TagList[i]} ");//FillAsync会替换原有内容
                    //await contentInput.PressAsync("Enter");
                    await page.WaitForTimeoutAsync(1000);
                }
                await contentInput.PressAsync("Enter");
                await page.WaitForTimeoutAsync(1000);
            }
            #endregion 

            CallBack("正在设置简介...");
            #region 视频简介
            if (!string.IsNullOrWhiteSpace(model.Info))
            {
                // 请填写视频简介
                await contentInput.TypeAsync(model.Info.CutStrLen(900));
                await page.WaitForTimeoutAsync(1000);
            }
            #endregion

            CallBack("正在选择专集...");
            // 专题
            var special = GetSpecialName();
            if (!string.IsNullOrWhiteSpace(special))
            {
                await page.Locator("div").Filter(new() { HasTextRegex = new Regex("^选择合集$") }).Nth(1).ClickAsync();

                await page.WaitForTimeoutAsync(2000);

                var nameElement = page.GetByText(special).Locator(".."); // 获取父元素
                if (nameElement != null)
                {
                    await nameElement.ClickAsync();
                    await page.WaitForTimeoutAsync(1000);
                }
            }
            CallBack("正在设置标题...");
            // 短标题
            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                await page.GetByPlaceholder("概括视频主要内容，字数建议6-16个字符").FillAsync(model.Title.CutStrLen(15));
                await page.WaitForTimeoutAsync(1000);
            }
            CallBack("即将发布...");
            await page.GetByRole(AriaRole.Button, new() { Name = "发表", Exact = true }).ClickAsync();
            await page.WaitForTimeoutAsync(1000);


            return "";

        }

        public override string Help => @"
# 关于西瓜中视频

西瓜中视频是西瓜视频平台推出的一个计划，旨在鼓励和支持视频创作者通过原创内容获得收益。根据百度百科的介绍，加入西瓜中视频伙伴计划后，创作者可以通过西瓜创作平台、西瓜视频App、抖音中西瓜视频小程序、剪映中西瓜视频发布原创横屏视频（时长≥1分钟），在西瓜视频、抖音、今日头条三大平台产生的有效播放均可获得创作收益。此外，还有版权托管服务，收益可以翻倍，最高可达250%。

> 为了方便，如果你达到以下条件，请加入加入西瓜中视频，发布后的视频会同步到抖音，今日头条。

# 如何加入西瓜中视频

1. **发布原创视频**：您需要通过西瓜创作平台、西瓜视频App、抖音中西瓜视频小程序、剪映中西瓜视频发布至少3篇公开可见的、原创横屏视频，每篇视频时长需达到或超过1分钟。

2. **满足播放量要求**：这些视频在西瓜视频、抖音、今日头条三大平台的总播放量需要达到17000次。

3. **提交申请**：完成上述任务后，将自动提交审核。西瓜视频的官方工作人员会根据您发布的视频是否符合原创标准来确定您是否能够加入该计划。

4. **审核过程**：审核结果将通过计划介绍页面和消息通知公布。如果审核未通过，您可以在30天后再次提交审核申请。

5. **申请入口**：您可以通过电脑访问 https://studio.ixigua.com/mvp 提交申请，或者在西瓜APP、抖音APP中搜索“中视频伙伴计划”，点击搜索结果进行申请。

6. **注意同步选项**：如果您习惯在抖音发布视频，发布时要选择同步西瓜视频，并勾选版权托管选项。

7. **收益获取**：一旦成功加入中视频伙伴计划，您发布的原创横屏视频在上述平台产生的有效播放均可获得创作收益。

8. **独家发布选项**：经主办方评估符合独家条件并签署相关协议的作者，在发布视频时勾选独家发布按钮，收益可以翻倍。

9. **提现收益**：所获得的收益可以在平台进行查看，并且设置了提现通道，在规定时间内创作者可进行收益提现。

请注意，这些步骤可能会随着平台政策的变化而有所调整，建议在申请前访问西瓜视频的官方页面或联系客服获取最新信息。
";

    }
}
