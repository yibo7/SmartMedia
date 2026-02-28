//using Microsoft.Playwright;
//using XS.Core2.XsExtensions;
//using SmartMedia.AtqCore;
//using SmartMedia.Modules.PushContent.DB;
//using static System.Windows.Forms.AxHost;
//using System.ComponentModel;

//namespace SmartMedia.Plugins.AutoPush.Video
//{
//    public class TaoTiaoHao : VideoPushBase
//    {
//        override public int OrderIndex => 4;
//        public override string CategoryFileName => "TaoTiaoHaoVideo.json";
//        override public Dictionary<string, SettingItem> ConfigCtrls => new Dictionary<string, SettingItem>() {
//            { "是否同步",new SettingItem(CtrlType.CheckBox,"是否同步到到斗音")},
//            { "合集名称",new SettingItem(CtrlType.TextBox,"可为空，你在此平台上创建的合集名称")} 
//        };


//        override public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
//            { "是否同步",""},{ "合集名称",""}
//        };

//        public override string PluginName => "今日头条";
//        override public Image IcoName => Resource.taotiaohao;
//        protected override string UploadPage => "https://mp.toutiao.com/profile_v4/xigua/upload-video";

//        public override string DataPage => "https://mp.toutiao.com/profile_v4/analysis/works-overall/video";

//        override public async Task<string> LoginAsync()
//        {
//            return await LoginCustom("https://mp.toutiao.com/auth/page/login", "span:text('粉丝数')");
//        }
//        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page)
//        {

//            // 选择视频文件
//            await page.SetInputFilesAsync("input[type=file]", model.file_path);

//            await page.WaitForSelectorAsync("text='上传成功'");

//            // 输入标题
//            var inputTitle = await page.QuerySelectorAsync("input[placeholder='请输入 1～30 个字符']");
//            await inputTitle.ClickAsync();
//            await page.WaitForTimeoutAsync(1000);
//            await inputTitle.FillAsync(model.title.CutStrLen(60));
//            await page.WaitForTimeoutAsync(1000);

//            #region 添加话题


//            var TagList = model.TagList();
//            if (TagList.Count > 0)
//            {
//                var inputTags = await page.QuerySelectorAsync("input.arco-input-tag-input.arco-input-tag-input-size-default");
//                var tagCount = Math.Min(TagList.Count, 10);
//                for (int i = 0; i < tagCount; i++)
//                {
//                    await inputTags.TypeAsync($"{TagList[i]}");//FillAsync会替换原有内容
//                    await page.WaitForTimeoutAsync(2000);
//                    await inputTags.PressAsync("Enter");
//                    await page.WaitForTimeoutAsync(1000);
//                }
//                //await inputTags.PressAsync("Enter");
//                //await page.WaitForTimeoutAsync(1000);
//            }


//            #region 更改封面
//            await page.GetByText("上传封面").ClickAsync();
//            await page.WaitForTimeoutAsync(1000);

//            if (!string.IsNullOrEmpty(model.img_path)) // 上传封面
//            { 

//                await page.GetByText("本地上传").ClickAsync();
//                await page.WaitForTimeoutAsync(1000);

//                var imgUpload = await page.QuerySelectorAsync("input[type='file'][accept='image/jpg,image/jpeg,image/png,image/x-png,image/webp']");
//                // 使用SetInputFilesAsync方法设置文件输入的值
//                await imgUpload.SetInputFilesAsync(model.img_path);
//                await page.WaitForTimeoutAsync(1000);

//                await page.WaitForSelectorAsync("button:has-text('重选封面')", new()
//                {
//                    State = WaitForSelectorState.Visible
//                });
//                //await page.GetByText("完成裁剪").ClickAsync();
//                await page.GetByText("完成裁剪", new() { Exact = true }).ClickAsync();

//                await page.WaitForTimeoutAsync(1000); 

//                await page.GetByRole(AriaRole.Button, new() { Name = "确定" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Button, new() { Name = "确定" }).Nth(1).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }
//            else // 没有封面图片，要自己选择
//            {
//                await page.WaitForTimeoutAsync(1000);
//                await page.ClickAsync($"div.m-button.red:text('下一步')");
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Button, new() { Name = "确定" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Button, new() { Name = "确定" }).Nth(1).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }
//            #endregion

//            #region 视频简介
//            if (!string.IsNullOrWhiteSpace(model.info))
//            {
//                await page.FillAsync("textarea[placeholder='请输入视频简介']", model.info.CutStrLen(400));
//            }
//            #endregion

//            #region 是否同步

//            var isSy = GetCf("是否同步");

//            if (!string.IsNullOrEmpty(isSy) && isSy.ToBool())
//            {
//                await page.ClickAsync($"span.byte-checkbox-inner-text:text('同步到抖音')");
//                await page.WaitForTimeoutAsync(1000);
//            }
//            #endregion

//            // 选择原创
//            if (model.original == 1)
//            {
//                await page.ClickAsync(".byte-radio-inner-text:has-text('原创')");
//                await page.WaitForTimeoutAsync(1000);
//            }

//            await page.WaitForTimeoutAsync(1000);
//            var special = GetSpecialName();
//            if (!string.IsNullOrWhiteSpace(special))
//            {
//                //
//                // 合集
//                await page.ClickAsync("span:has-text('选择合集')");
//                await page.WaitForTimeoutAsync(1000);
//                await page.ClickAsync($"span.byte-radio-inner-text:text('{special}')");
//                await page.WaitForTimeoutAsync(1000);
//                //await page.ClickAsync($"button.byte-btn:text('确认')");
//                await page.Locator("button").Filter(new() { HasText = "确定" }).Nth(1).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }


//            #endregion 

//            // 发布视频
//            //await page.ClickAsync(".submit:has-text('发布')");
//            //await page.WaitForTimeoutAsync(10000);

//            await page.GetByRole(AriaRole.Button, new() { Name = "发布", Exact = true }).ClickAsync();
//            await page.WaitForTimeoutAsync(1000);

//            if (!string.IsNullOrEmpty(model.img_path)) // 上传封面
//            {
//                try
//                {
//                    await page.WaitForSelectorAsync("text='上传更清晰的封面'", new PageWaitForSelectorOptions
//                    {
//                        State = WaitForSelectorState.Attached,
//                        Timeout = 3000 // 5秒超时
//                    });
//                    return "上传失败，你的视频封面分辨率较低，清晰美观的封面有利于推荐，建议上传分辨率不低于1920*1080的封面。";
//                }
//                catch (TimeoutException)
//                {
//                    // 说明封面图片合格
//                }
//            }


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
