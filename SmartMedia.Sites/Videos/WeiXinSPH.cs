using Microsoft.Playwright;
using SmartMedia.Core.Comm;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using System.Text.RegularExpressions;
using XS.Core2.XsExtensions;

namespace SmartMedia.Sites.Videos
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
                    { "picCoverImage2",BuildCtr<SettingCoverImage>("个人主页封面图","这里是个人主页列表展现时的封面图，要求最好是3:4的比例，而上面的是分享卡片图片，要求是4:3") },
                    { "SpecialName",BuildCtr<SettingTextBox>("合集名称","可为空，你在平台上创建的合集名称") }
                };
            }
        }

        //override public Dictionary<string, string> CustomCtrls() => new Dictionary<string, string>() {
        //     { "合集名称",""}
        //};
        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {
            // 短标题
            if (!IsValidTitle(model.Title))
            {
                return "标题为空，或长度不在6-16之间，或包含特殊字符，符号仅支持书名号、引号、冒号、加号、问号、百分号、摄氏度，逗号可用空格代替";
            }

            CallBack("正在上传视频...");
            // 选择视频文件
            await page.SetInputFilesAsync("input[type=file]", model.FilePath);

            // 等待元素出现

            await page.WaitForSelectorAsync("div.tag-inner:text('删除')", new()
            {
                State = WaitForSelectorState.Visible
            });

            CallBack("正在更改封面个人主页卡片封面...");
            await page.WaitForTimeoutAsync(3000);
            #region 更改封面

            string picCoverImage2 = base.GetCtrValue("picCoverImage2");
            //string ImgPath1 = base.GetCoverPath();
            if (!string.IsNullOrEmpty(picCoverImage2)) // 上传封面
            { 

                var upImg = new PlaywrightFileUploadHelper(page);
                bool isOk = await upImg.UploadImgFile(picCoverImage2, "image", "div.vertical-img-wrap");

                await page.WaitForTimeoutAsync(2000);
                await page.GetByText("确认").First.ClickAsync();
                await page.WaitForTimeoutAsync(2000); 

                //await page.WaitForTimeoutAsync(1000);
            }
            var picCoverImage = base.GetCoverPath();
            if (!string.IsNullOrEmpty(picCoverImage)) // 上传封面
            {
                 

                await page.Locator("div.horizon-img-wrap").ClickAsync();
                await page.WaitForTimeoutAsync(2000);
                var upImg = new PlaywrightFileUploadHelper(page);
                
                var editButton = page.GetByText("直接编辑");
                await editButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                await editButton.ClickAsync();
                await page.WaitForTimeoutAsync(2000);

                var isOk = await upImg.UploadImgFile(picCoverImage, "image");

                await page.WaitForTimeoutAsync(2000);
                await page.GetByText("确认").First.ClickAsync();
                await page.WaitForTimeoutAsync(2000);
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
                await page.Locator($".option-item:has-text('{special}')").ClickAsync();
                await page.WaitForTimeoutAsync(1000);
            }
            CallBack("正在设置标题...");
            await page.GetByPlaceholder("概括视频主要内容，字数建议6-16个字符").FillAsync(model.Title.CutStrLen(15));
            await page.WaitForTimeoutAsync(1000);


            //await DeclareOriginal(page);
            await page.GetByText("声明后，作品将展示原创标记，有机会获得广告收入。").ClickAsync();
            await page.WaitForTimeoutAsync(2000);
            await page.GetByText("我已阅读并同意《原创声明须知》和《使用条款》。如滥用声明，平台将驳回并予以相关处置。").First.ClickAsync();
            await page.WaitForTimeoutAsync(2000);
            await page.GetByRole(AriaRole.Button, new() { Name = "声明原创" }).ClickAsync();
            await page.WaitForTimeoutAsync(2000);
            CallBack("即将发布...");
            await page.GetByRole(AriaRole.Button, new() { Name = "发表", Exact = true }).ClickAsync();
            await page.WaitForTimeoutAsync(5000);


            return "";

        }

        
        private static bool IsValidTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return false;
            }

            // 检查长度
            if (title.Length < 6 || title.Length > 16)
            {
                return false;
            }

            // 使用正则表达式检查是否包含特殊字符
            // 匹配书名号、引号、冒号、加号、问号、百分号、摄氏度
            string pattern = @"[《》「」""“”:：+?？%℃]";

            return !Regex.IsMatch(title, pattern);
        }
        public override string Help => @"
# 关于微信视频号

微信视频号是微信生态中不可或缺的短视频和直播平台，它深度整合在微信内，为用户提供了一个记录和分享生活、表达观点、展示创作才华的窗口。与其它短视频平台不同，视频号的核心优势在于其强大的社交关系链，您的作品不仅可以被陌生人看到，更能轻松触达您的微信好友，通过朋友点赞等社交推荐机制实现裂变式传播。您可以在“发现”页面找到并进入视频号，浏览海量视频，或是开通自己的频道，发布最长1小时的高清视频（封面尺寸建议为1080*1260），与广阔世界建立连接。

> 为了方便，如果您希望在微信生态内打造个人品牌、推广业务或通过内容创作获得收益，请加入微信视频号，发布的视频有机会通过社交推荐和算法推荐获得海量曝光。

# 如何开通和使用微信视频号

1. **开通视频号**：首先，请确保您的微信已更新至最新版本。进入微信的“发现”页面，点击“视频号”。如果您是首次使用，按照页面提示选择“创建视频号”，并绑定您的个人微信号即可完成开通。一个微信账号只能创建一个视频号。

2. **明确账号定位**：在发布内容前，建议先明确您的视频号定位。思考您想分享哪个领域的内容（如美食、旅行、科技、生活感悟等），并以此为基础完善头像、昵称和简介，打造一个专业且吸引人的账号形象。

3. **准备并发布第一条视频**：点击个人主页右上角的“发表”按钮（照相机图标）。您可以从相册中选择预先拍摄并剪辑好的视频（时长建议灵活，从几秒到1分钟内的短视频起步较为合适），或直接拍摄。发布时，需要为视频配上一段精彩的描述文字，并可以添加话题标签（如#旅行日记）和地理位置，以增加曝光机会。发布后，您的微信好友可能会在“朋友”板块看到您的动态。

4. **掌握内容创作要点**：
    *   **视频尺寸**：视频号支持多种横竖屏比例，但竖屏（6:7比例，常见如1080x1260）在手机端展示效果最佳，能最大化利用屏幕空间。
    *   **内容质量**：原创、有价值、能引起共鸣或讨论的内容更容易获得推荐。清晰稳定的画面和清晰的音频是基础。
    *   **互动引导**：在视频或文案中，可以适当引导观众进行点赞、评论和转发，积极的互动数据有助于内容进入更高级别的流量池。

5. **探索视频号的功能**：视频号不仅仅是短视频。您还可以开启**直播**，与粉丝实时互动，甚至进行直播带货。同时，视频号与**微信小商店**深度打通，让您能轻松在主页和直播间展示并销售商品，实现商业变现。

6. **使用“推荐”机制增加曝光**：视频号的推荐页基于算法，为您可能感兴趣的内容进行推荐。发布高质量、高完播率的视频，能增加被算法推荐给更多潜在用户的概率。

7. **利用社交关系链传播**：视频号的一大特色是“朋友”板块。当您的微信好友为某个视频点赞后，您的好友也更容易在“朋友”板块看到这个视频，这是视频号冷启动和快速传播的关键。

8. **关注数据与运营**：在您的视频号主页，可以查看作品的点赞、评论、转发等数据，了解粉丝的喜好。持续分析数据，优化内容方向，与评论区粉丝积极互动，是运营好视频号的长期之道。

9. **收益与变现途径**：视频号为创作者提供了多元的变现方式。除了上述的直播带货、小商店卖货，当账号具备一定影响力后，还可以通过**视频号互选平台**承接品牌广告合作，获得收益。

请注意，视频号的各项功能和政策（如直播带货的门槛、互选平台的准入要求等）会随着微信生态的演进而不断更新。建议您在创作过程中，多关注微信视频号的官方动态和公告，以获取最新信息。
";

    }
}
