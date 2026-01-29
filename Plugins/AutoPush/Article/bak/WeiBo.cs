//using Microsoft.Playwright;
//using SmartMedia.Modules.PushContent.DB;
//using XS.Core2.XsExtensions;

//namespace SmartMedia.Plugins.AutoPush.Article
//{
//    public class WeiBo : ArticlePushBase
//    {
//        //override protected Dictionary<string, SettingItem> SiteCtrls => new Dictionary<string, SettingItem>() {
//        //    { "专栏设置",new SettingItem(CtrlType.TextBox,"可为空，你在此平台上创建的专栏设置名称")}
//        //};


//        //override public Dictionary<string, string> CustomCtrls() => new Dictionary<string, string>() {
//        //     { "专栏设置",""}
//        //};
//        //public override string CategoryFileName => "WeiboVideo.json";
//        protected override string UploadPage => "https://card.weibo.com/article/v3/editor";

//        public override string DataPage => "https://me.weibo.com/data/overview";

//        public override string PluginName => "新浪微博";
//        override public Image IcoName => Resource.xinlangweibo;
//        override public async Task<string> LoginAsync()
//        {
//            return await LoginCustom("https://passport.weibo.com", "a:has-text('消息')");
//        }
//        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
//        {

//            // 点击发布视频按钮
//            await page.ClickAsync("div:text('发布视频')");

//            // 选择视频文件
//            await page.SetInputFilesAsync("input[type=file]", model.FilePath);

//            // 等待视频上传
//            await page.WaitForTimeoutAsync(2000);

//            // 输入标题
//            await page.ClickAsync("div[data-contents]");
//            await page.WaitForTimeoutAsync(2000);


//            await page.FillAsync("div[data-contents]", model.Title.CutStrLen(60));
//            await page.WaitForTimeoutAsync(2000);

//            #region 添加标签
//            var TagList = model.TagList();
//            if (TagList.Count > 0)
//            {
//                var tagInput = await page.QuerySelectorAsync("input.arco-input-tag-input");
//                await tagInput.ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                var tagCount = Math.Min(TagList.Count, 10);
//                for (int i = 0; i < tagCount; i++)
//                {
//                    await tagInput.FillAsync(TagList[i]);
//                    await tagInput.PressAsync("Enter");
//                    await page.WaitForTimeoutAsync(2000);
//                }
//                await page.WaitForTimeoutAsync(2000);
//            }
//            #endregion

//            #region 更改封面

//            // 上传封面
//            await page.ClickAsync("span:has-text('上传封面')");
//            await page.WaitForTimeoutAsync(2000);
//            if (!string.IsNullOrEmpty(model.ImgPath)) // 上传封面
//            {
//                await page.ClickAsync("li[text()='本地上传']");
//                await page.SetInputFilesAsync(".upload-trigger-tip", model.ImgPath);
//                await page.ClickAsync(".byte-upload-trigger-tip");
//                await page.WaitForSelectorAsync(".upload_paths_inputted");
//            }
//            else // 默认封面
//            {
//                //await page.ClickAsync("(//div[@class='m-system-i'])[1]");
//                //await page.WaitForTimeoutAsync(2000);

//            }


//            // 点击确定按钮
//            await page.ClickAsync("div.m-button:has-text('下一步')");
//            await page.WaitForTimeoutAsync(2000);
//            await page.ClickAsync("button.btn-sure:has-text('确定')");
//            await page.WaitForTimeoutAsync(2000);
//            await page.ClickAsync("button.m-button.red:has-text('确定')");
//            await page.WaitForTimeoutAsync(2000);
//            await page.WaitForSelectorAsync("span.image-modify-btn:has-text('编辑')");
//            await page.WaitForTimeoutAsync(2000);
//            //await page.WaitForSelectorAsync("div[data-contents]", new WaitForSelectorOptions { State = WaitForSelectorState.Hidden });
//            #endregion

//            // 选择原创
//            await page.ClickAsync(".byte-radio-inner-text:has-text('原创')");
//            await page.WaitForTimeoutAsync(2000);

//            if (!string.IsNullOrWhiteSpace(model.Info))
//            {
//                var tagInputParent = await page.QuerySelectorAsync("//div[.='请填写视频简介']/../.."); // 获取内容等于某个元素的上级元素
//                var tagInput = await tagInputParent.QuerySelectorAsync("div[data-contents]");

//                tagInput.ClickAsync();

//                tagInput.FillAsync(model.Info.CutStrLen(400));

//                //RecorderUtils.OnCopy("Hello World");
//            }
//            await page.WaitForTimeoutAsync(2000);
//            var special = GetSpecialName();
//            if (!string.IsNullOrWhiteSpace(special))
//            {
//                //
//                // 合集
//                await page.ClickAsync("span:has-text('添加至合集')");
//                await page.WaitForTimeoutAsync(2000);
//                // 
//                await page.ClickAsync($"div.m-item-title:text('{special}')");
//                await page.WaitForTimeoutAsync(2000);

//                await page.ClickAsync($"button.m-button:text('确认')");
//                await page.WaitForTimeoutAsync(2000);
//            }

//            // 发布视频
//            await page.ClickAsync(".submit:has-text('发布')");
//            await page.WaitForTimeoutAsync(2000);


//            return $"西瓜视频上传成功 {DateTime.Now}";

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
