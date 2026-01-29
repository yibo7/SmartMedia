//using Microsoft.Playwright;
//using XS.Core2.XsExtensions;
//using SmartMedia.AtqCore;
//using SmartMedia.Modules.PushContent.DB;

//namespace SmartMedia.Plugins.AutoPush.Video
//{
//    public class ZhiHu : VideoPushBase
//    {
//        override public Dictionary<string, SettingItem> ConfigCtrls => new Dictionary<string, SettingItem>() {
//            { "发布分类",new SettingItem(CtrlType.Categorie,"")}
//        };


//        override public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
//            { "发布分类",""} 
//        };
//        public override string CategoryFileName => "ZhiHuVideo.json";
//        protected override string UploadPage => "https://www.zhihu.com/zvideo/upload-video";

//        public override string DataPage => "https://www.zhihu.com/creator/analytics/work/zvideo";

//        public override string PluginName => "知乎视频";
//        override public Image IcoName => Resource.zihu;
//        override public async Task<string> LoginAsync()
//        {
//            return await LoginCustom("https://www.zhihu.com/signin", "div:has-text('创作中心')");
//        }
//        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page)
//        {

//            // 选择视频文件
//            await page.SetInputFilesAsync("input[type=file]", model.file_path);

//            await page.WaitForSelectorAsync("text='选择视频封面'");
//            await page.WaitForTimeoutAsync(1000);


//            #region 更改封面


//            if (!string.IsNullOrEmpty(model.img_path)) // 上传封面
//            {
//                await page.GetByText("选择视频封面").ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByText("本地上传").ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Textbox).Nth(2).SetInputFilesAsync(model.img_path);
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Button, new() { Name = "确认选择" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }
//            #endregion


//            if (!string.IsNullOrEmpty(model.title))
//            {
//                await page.GetByPlaceholder("输入视频标题").FillAsync(model.title.CutStrLen(39)); 
//                await page.WaitForTimeoutAsync(1000);
//            }


//            #region 视频简介
//            if (!string.IsNullOrWhiteSpace(model.info))
//            {
//                await page.WaitForTimeoutAsync(1000);
//                // 输入内容
//                await page.GetByPlaceholder("填写视频简介，让更多人找到你的视频").FillAsync(model.info.CutStrLen(300));
//                await page.WaitForTimeoutAsync(1000);
//            }
//            #endregion

//            if (model.original == 1)
//            { 
//                await page.Locator("label").Filter(new() { HasText = "原创" }).GetByRole(AriaRole.Img).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }



//            //var searchInput = await page.GetByRole(AriaRole.Combobox, new() { Id = "rc_select_0" });
//            var class_names = GetCategoryList();
//            if (class_names.Count == 2)
//            {
//                await page.Locator("button#Popover8-toggle").ClickAsync();                 
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Option, new() { Name = class_names[0] }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByText("选择领域").ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Option, new() { Name = class_names[1] }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);

//            }

//            await page.GetByRole(AriaRole.Button, new() { Name = "发布视频", Exact = true }).ClickAsync();
//            await page.WaitForTimeoutAsync(1000);

//            return "";

//        }

//        public override string Help => @"
//# 关于西瓜中视频

//西瓜中视频是西瓜视频平台推出的一个计划，旨在鼓励和支持视频创作者通过原创内容获得收益。根据百度百科的介绍，加入西瓜中视频伙伴计划后，创作者可以通过西瓜创作平台、西瓜视频App、抖音中西瓜视频小程序、剪映中西瓜视频发布原创横屏视频（时长≥1分钟），在西瓜视频、抖音、今日头条三大平台产生的有效播放均可获得创作收益。此外，还有版权托管服务，收益可以翻倍，最高可达250%。

//> 为了方便，如果你达到以下条件，请加入加入西瓜中视频，发布后的视频会同步到抖音，今日头条。

//# 如何加入西瓜中视频

//1. **发布原创视频**：您需要通过西瓜创作平台、西瓜视频App、抖音中西瓜视频小程序、剪映中西瓜视频发布至少3篇公开可见的、原创横屏视频，每篇视频时长需达到或超过1分钟。

//2. **满足播放量要求**：这些视频在西瓜视频、抖音、今日头条三大平台的总播放量需要达到17000次。

//3. **提交申请**：完成上述任务后，将自动提交审核。西瓜视频的官方工作人员会根据您发布的视频是否符合原创标准来确定您是否能够加入该计划。

//4. **审核过程**：审核结果将通过计划介绍页面和消息通知公布。如果审核未通过，您可以在30天后再次提交审核申请。

//5. **申请入口**：您可以通过电脑访问 https://studio.ixigua.com/mvp 提交申请，或者在西瓜APP、抖音APP中搜索“中视频伙伴计划”，点击搜索结果进行申请。

//6. **注意同步选项**：如果您习惯在抖音发布视频，发布时要选择同步西瓜视频，并勾选版权托管选项。

//7. **收益获取**：一旦成功加入中视频伙伴计划，您发布的原创横屏视频在上述平台产生的有效播放均可获得创作收益。

//8. **独家发布选项**：经主办方评估符合独家条件并签署相关协议的作者，在发布视频时勾选独家发布按钮，收益可以翻倍。

//9. **提现收益**：所获得的收益可以在平台进行查看，并且设置了提现通道，在规定时间内创作者可进行收益提现。

//请注意，这些步骤可能会随着平台政策的变化而有所调整，建议在申请前访问西瓜视频的官方页面或联系客服获取最新信息。
//";

//    }
//}
