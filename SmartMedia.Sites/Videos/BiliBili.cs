using Microsoft.Playwright;
using SmartMedia.Core.Comm;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;

namespace SmartMedia.Sites.Videos
{
    public class BiliBili : VideoPushBase
    {
        override public int OrderIndex => 3;
        override public string CategoryFileName => "BiliBili.json";
        protected override string UploadPage => "https://member.bilibili.com/platform/upload/video/frame";
        override public int CategoryLeve => 2;
        public override string DataPage => "https://member.bilibili.com/platform/data-up/video/dataCenter/video";
        public override string PluginName => "哔哩哔哩";
        override public Image IcoName => Resource.bilibili;

        override public async Task<string> LoginAsync()
        {
            return await LoginCustom("https://passport.bilibili.com/pc/passport/login", "span:has-text('消息')");
        }

        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {

            CallBack("准备上传视频");
            // 选择文件上传 
            await page.SetInputFilesAsync("input[type=file]", model.FilePath);
            await page.WaitForTimeoutAsync(1000);
            await page.WaitForSelectorAsync("text='上传完成'");
            CallBack("上传完成，开始上传封面");

            #region 更改封面
            if (!string.IsNullOrEmpty(model.ImgPath))
            { 
                var upImg = new PlaywrightFileUploadHelper(page);
                bool isOk = await upImg.UploadImgFile(model.ImgPath, "image", "span:text('封面设置')");
                if (!isOk)
                {
                    CallBack("封面上传失败!"); 
                }
                await page.GetByText("完成", new PageGetByTextOptions { Exact = true }).ClickAsync();
                await page.WaitForTimeoutAsync(2000);

            }

            #endregion


            // 填写视频标题 
            CallBack("开始输入标题，内容，分类，专题信息!");

            await page.Locator("input[type='text'][placeholder=\"请输入稿件标题\"]").FillAsync(model.Title);
            await page.WaitForTimeoutAsync(2000);
             
            await page.GetByText("自制").Nth(0).ClickAsync();

            await page.WaitForTimeoutAsync(2000);

            // 选择分类 
            var categoryList = GetCategoryList();


            if (categoryList.Count < 1)
            {
                CallBack("失败，没有分类设置!");
                return "没有分类设置，请在json中设置分类信息或者在当前任务的自定义配置中修改ClassList节点的值";
            }
            CallBack("选择专题!");
            // 1. 首先点击展开下拉列表
            await page.ClickAsync("div.select-container");
            await page.WaitForTimeoutAsync(5000);
              
            // 2. 选择"音乐"选项
            await page.Locator($".drop-list-v2-item[title='{categoryList[0]}']").ClickAsync();
            await page.WaitForTimeoutAsync(2000);

            //await page.ClickAsync("div.select-container");
            //await page.WaitForTimeoutAsync(2000);

            //await page.ClickAsync($"p.f-item-content:text('{categoryList[0]}')");
            //await page.WaitForTimeoutAsync(2000); 

            #region 添加标签
            CallBack("开始添加标签!");
            var Tags = model.TagList();
            if (Tags.Count > 0)
            {
                var tagInput = await page.QuerySelectorAsync(".tag-input-wrp .input-instance input");

                var tagCount = Math.Min(Tags.Count, 10);
                for (int i = 0; i < tagCount; i++)
                {
                    await tagInput.FillAsync(Tags[i]);
                    await tagInput.PressAsync("Enter");
                    await page.WaitForTimeoutAsync(1000);
                }
                await page.WaitForTimeoutAsync(2000);
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(model.Info))
            {
                // 填写简介 
                await page.FillAsync("div.ql-editor", model.Info);
                await page.WaitForTimeoutAsync(2000);
            }
            var special = GetSpecialName();
            if (!string.IsNullOrWhiteSpace(special))
            {
                //
                // 合集
                await page.ClickAsync("div.season-select");
                await page.WaitForTimeoutAsync(2000);
                // 
                await page.ClickAsync($"p.season-item-title:text('{special}')");
                await page.WaitForTimeoutAsync(2000);
            }

            // 滚动到底部
            await base.ScrollToBottom(page);

            // 点击立即投稿
            await page.GetByText("立即投稿").ClickAsync();
            await page.WaitForTimeoutAsync(2000); 
            return "";

        }

        public override string Help => @"
# 关于BiliBili(哔哩哔哩)

BiliBili，通常被称为哔哩哔哩，是一个以年轻人为主要用户群体的视频分享网站，成立于2009年。它最初是一个以动画、漫画和游戏（ACG）文化为主题的社区，但随着时间的发展，BiliBili已经扩展成为一个多元化的内容平台，涵盖了各种类型的内容，包括但不限于音乐、舞蹈、科技、娱乐、生活、时尚、教育等领域。



# 如何成为BiliBili（哔哩哔哩）的UP主
要成为BiliBili（哔哩哔哩）的UP主，你可以按照以下步骤操作：

1. **注册账号**：首先，使用浏览器搜索哔哩哔哩（B站）官网，进入官网注册并登录哔哩哔哩账号。

> https://www.bilibili.com/

2. **完成认证**：点击左上角头像完成bilibili认证。

3. **投稿**：认证完成后，点击右上角的【投稿】按钮，并选择你想要投稿的类型。

4. **上传视频**：点击【上传视频】，选择你想要上传的视频文件。

5. **编辑信息**：等待视频上传完成后，选择封面（可以自行上传或选择系统自动生成的封面），选择视频的类型与分区，并填写视频的标题、标签、参加活动信息以及简介。

6. **提交审核**：完成上述步骤后，点击【立即投稿】提交你的视频，之后需要等待稿件审核完毕。

7. **审核通过**：一旦审核通过，你的视频就可以在B站上被大家看到，这样你就正式成为了一名UP主。
";

    }
}
