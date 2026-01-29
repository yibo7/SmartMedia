//using Microsoft.Playwright;
//using XS.Core2.XsExtensions;
//using SmartMedia.AtqCore;
//using SmartMedia.Modules.PushContent.DB;
//using static System.Windows.Forms.AxHost;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using System.Text.RegularExpressions;

//namespace SmartMedia.Plugins.AutoPush.Video
//{
//    public class BaiJiaHao : VideoPushBase
//    {
//        override public int OrderIndex => 5;
//        override public string CategoryFileName => "BaiJiaHao.json";
//        protected override string UploadPage => "https://baijiahao.baidu.com/builder/rc/edit?type=videoV2";
//        override public int CategoryLeve => 2;
//        public override string DataPage => "https://baijiahao.baidu.com/builder/rc/analysiscontent";

//        public override string PluginName => "百家号";
//        override public Image IcoName => Resource.baijiahao;

//        override public async Task<string> LoginAsync()
//        {
//            return await LoginCustom("https://baijiahao.baidu.com/builder/theme/bjh/login", "div:has-text('累计阅读量')");
//        }

//        override public Dictionary<string, SettingItem> ConfigCtrls => new Dictionary<string, SettingItem>() {
//            { "发布分类",new SettingItem(CtrlType.Categorie,"")}
//        };


//        override public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
//            { "发布分类",""}
//        };

//        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page)
//        {

//            // 选择视频文件
//            await page.SetInputFilesAsync("input[type=file]", model.file_path);

//            // 等待元素消失
//            await page.WaitForSelectorAsync("span:text('加载中，可先下滑填写发布信息')", new()
//            {
//                State = WaitForSelectorState.Detached // 等待元素从 DOM 中移除
//            });

//            // 输入标题
//            if (!string.IsNullOrEmpty(model.title) && model.title.Length > 8)
//            {
//                await page.GetByPlaceholder("请输入标题").FillAsync(model.title.CutStrLen(30));
//                await page.WaitForTimeoutAsync(100);
//            }
//            else
//            {
//                return "标题的长度要大于8";
//            }


//            #region 更改封面

//            // 上传封面 

//            if (!string.IsNullOrEmpty(model.img_path)) // 上传封面
//            {
//                //await page.Locator(".op-remove").First.ClickAsync();
//                //await page.WaitForTimeoutAsync(5000);
//                await page.Locator("div").Filter(new() { HasTextRegex = new Regex("^添加$") }).First.ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Tab, new() { Name = "本地图片" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await page.SetInputFilesAsync("input[type='file'][accept='image/*']", model.img_path);
//                await page.WaitForTimeoutAsync(1000);
//                await page.GetByRole(AriaRole.Button, new() { Name = "确 认" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }

//            #endregion

//            var class_names = GetCategoryList();
//            if (class_names.Count==2)
//            {
//                await page.ClickAsync("input.cheetah-select-selection-search-input");
//                await page.WaitForTimeoutAsync(1000); 
//                await page.ClickAsync($"li.cheetah-cascader-menu-item-expand[title='{class_names[0]}']");
//                await page.WaitForTimeoutAsync(2000);
//                await page.ClickAsync($"li.cheetah-cascader-menu-item[title='{class_names[1]}']");
//                await page.WaitForTimeoutAsync(2000);

//            }
//            var TagList = model.TagList();
//            if (TagList.Count > 0)
//            {
//                var tagInput = await page.QuerySelectorAsync("input.cheetah-ui-pro-tag-input-container-tag-input"); 
//                var tagCount = Math.Min(TagList.Count, 10);
//                for (int i = 0; i < tagCount; i++)
//                {
//                    await tagInput.TypeAsync(TagList[i]);//FillAsync会替换原有内容
//                    await tagInput.PressAsync("Enter");
//                    await page.WaitForTimeoutAsync(1000);
//                }

//            }
//            // 视频简介
//            await page.FillAsync("textarea[placeholder='让别人更懂你']", model.info.CutStrLen(100));
//            // 发布视频
//            await page.GetByRole(AriaRole.Button).Nth(1).ClickAsync(); ;
//            await page.WaitForTimeoutAsync(2000);


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
