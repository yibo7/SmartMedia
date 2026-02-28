using Microsoft.Playwright;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;

namespace SmartMedia.Sites.Videos
{
    public class KuaiSou : VideoPushBase
    {
        override public int OrderIndex => 5;
        override public string CategoryFileName => "KuaiSou.json";
        protected override string UploadPage => "https://cp.kuaishou.com/article/publish/video";
        override public int CategoryLeve => 2;
        public override string DataPage => "https://cp.kuaishou.com/statistics/works";

        public override string PluginName => "快手";
        override public Image IcoName => Resource.kuaisou;

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
        override public async Task<string> LoginAsync()
        {
            return await LoginCustom("https://passport.kuaishou.com/pc/account/login", "div:has-text('上传作品')");
        }
        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {
            CallBack("正在上传视频...");
            var err = base.CheckModel(model);

            if(!string.IsNullOrWhiteSpace(err))
                return err;
             
            await page.SetInputFilesAsync("input[type=file]", model.FilePath);

            await page.WaitForSelectorAsync("text='编辑画布'");

            await page.WaitForTimeoutAsync(1000);
            await page.GetByRole(AriaRole.Button, new() { Name = "Skip" }).ClickAsync();

            CallBack("正在输入标题...");
            await page.WaitForTimeoutAsync(1000);
            string sDescription = $"{model.Title} \n\n {model.FormatTags()}\n\n{model.Info}";
            await page.WaitForTimeoutAsync(1000);
            await page.Locator("#work-description-edit").ClickAsync();
            await page.WaitForTimeoutAsync(1000);
            await page.Locator("#work-description-edit").FillAsync(sDescription);

            CallBack("正在更改封面...");
            if (!string.IsNullOrWhiteSpace(model.ImgPath))
            {

                await page.WaitForTimeoutAsync(1000);
                await page.GetByText("封面设置").Nth(1).ClickAsync();

                await page.WaitForTimeoutAsync(2000);
                await page.GetByText("上传封面").ClickAsync();

                //await page.WaitForTimeoutAsync(1000);
                //await page.Locator("._upload-icon_1i0wh_53").ClickAsync();

                await page.WaitForTimeoutAsync(2000); 
                await page.GetByRole(AriaRole.Dialog).Locator("input[type='file']").SetInputFilesAsync(new[] { model.ImgPath });

                await page.WaitForTimeoutAsync(2000);
                await page.GetByRole(AriaRole.Button, new() { Name = "确认" }).ClickAsync(); 
            }

            CallBack("正在选择专题...");
            var special = GetSpecialName();
            if (!string.IsNullOrWhiteSpace(special))
            {
                await page.WaitForTimeoutAsync(1000);
                await page.Locator("#rc_select_5").ClickAsync();

                await page.WaitForTimeoutAsync(1000);
                await page.GetByText(special, new() { Exact = true }).ClickAsync();
                //await page.Locator(".ant-select-selection-search").GetByText(new Regex(special)).ClickAsync();
            }
            CallBack("即将发布...");
            await page.WaitForTimeoutAsync(2000); 
            await page.GetByText("发布", new() { Exact = true }).ClickAsync();

            return "";

        }

        public override string Help => @"  

### 📱 如何成为快手创作者并成长
与所有内容平台一样，在快手发布内容非常简单。但要成为一名能持续吸引粉丝并实现成长的创作者，关键在于后续的运营。下面的流程和表格说明了关键的步骤与策略：

1.  **基础起步：发布你的第一个作品**
    操作非常直观：在快手APP首页点击底部的“**+**”号，即可选择拍摄新视频或从手机相册上传已有视频。上传后，你可以添加音乐、文字、特效等进行简单编辑，并填写描述和话题标签，最后点击发布。

2.  **进阶成长：从“发布者”到“创作者”**
    发布视频只是开始，要获得更多关注和平台支持，你需要有策略地运营：
    *   **明确人设与定位**：确定一个你擅长且能持续产出内容的领域（如美食、生活、知识分享），并在个人资料中清晰地展示出来，这有助于吸引精准粉丝。
    *   **持续输出优质原创内容**：平台鼓励原创，高质量的视频更容易获得流量推荐。可以关注热门话题，但结合自己的特色进行创作。
    *   **积极互动与数据分析**：及时回复评论、发起投票等能有效提升粉丝粘性。同时，关注后台的播放量、完播率等数据，了解什么内容更受欢迎，并据此优化。

3.  **寻求认证：获取官方身份与资源**
    当你积累了一定作品和粉丝后，可以申请官方认证，以获得更多曝光和功能。主要的认证路径有：
    | 认证类型 | 主要目的与价值 | 关键申请条件（参考） |
    | :--- | :--- | :--- |
    | **快手达人认证** | 获取直播、带货等高级功能权限的基础。 | 账号状态正常、持续创作能力、一定的粉丝基础（如数千粉丝）和互动指数。 |
    | **快手原石号认证** | 面向**优质原创作者**的扶持计划，提供流量倾斜和专属权益。 | 通常要求**实名认证**、账号活跃、粉丝数达标（如1000以上）、持续产出原创内容。 |

    > 注：以上认证的具体条件可能随平台政策调整，申请时请以快手APP“设置”或“创作者服务中心”内的官方说明为准。

4.  **商业变现：探索多元收入模式**
    快手为创作者提供了成熟的商业化路径：
    *   **直播打赏与带货**：完成达人认证后，可申请开通直播带货功能。将商品链接添加至视频或直播中，可以直接促成销售。
    *   **电商与商家服务**：如果想更专业地运营电商，可以考虑开通“快手小店”或升级为商家号，以获得更全面的店铺管理、营销工具和数据分析功能。
    *   **广告与平台合作**：随着影响力提升，可以通过参与平台官方活动、承接品牌广告等方式获得收益。

### 💡 给你的核心建议
*   **内容为本**：无论选择哪个领域，**真实、有趣、有价值**的内容是长久吸引粉丝的根本。
*   **保持活跃**：稳定的更新频率（如每周2-3次）有助于维持账号热度。
*   **遵守规则**：务必了解并遵守社区规范，避免搬运抄袭、虚假宣传等行为，保护账号安全。

如果你已经想好了要在哪个具体领域（比如美食、美妆、游戏或知识分享）开始创作，我可以为你提供更聚焦的运营建议。";

    }
}
