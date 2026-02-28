using Microsoft.Playwright;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB;
using System.Text.RegularExpressions;
using XS.Core2.XsExtensions;

namespace SmartMedia.Sites.Videos
{
    public class DouYin : VideoPushBase
    {
        override public int OrderIndex => 1;
        override public string CategoryFileName => "DouYin.json";
        protected override string UploadPage => "https://creator.douyin.com/creator-micro/content/upload";
        override public int CategoryLeve => 2;
        public override string DataPage => "https://creator.douyin.com/creator-micro/data-center/operation";

        public override string PluginName => "抖音";
        override public Image IcoName => Resource.douyin;
         
        override protected Dictionary<string, SettingCtrBase> SiteCtrls => new Dictionary<string, SettingCtrBase>() {
            { "是否同步",base.BuildCtr<SettingCheckBox>("是否同步","是否同步到到西瓜与头条","True")},
            { "SpecialName",BuildCtr<SettingTextBox>("合集名称","可为空，你在此平台上创建的合集名称") },
            { "CoverImageH",BuildCtr<SettingCoverImage>("设置竖封面","可为空，要求为3:4图片格式") }
        };


        override public async Task<string> LoginAsync()
        {
            return await LoginCustom("https://creator.douyin.com", "span:text('高清发布')");
        }

        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
        {
            CallBack($"开始处理【{this.PluginName}】");
            var err = base.CheckModel(model);

            if (!string.IsNullOrWhiteSpace(err))
                return err;

            CallBack("正在上传视频...");

            // 点击发布视频按钮
            //await page.ClickAsync("span:text('上传视频')");

            // 选择视频文件
            await page.SetInputFilesAsync("input[type=file]", model.FilePath); 
            // 等待视频上传
            await page.WaitForTimeoutAsync(1000);
            // 等待元素消失
            await page.WaitForSelectorAsync("p:text('取消上传')", new()
            {
                State = WaitForSelectorState.Detached // 等待元素从 DOM 中移除
            });

            CallBack("正在输入标题...");
            // 输入标题
            await page.ClickAsync("input[placeholder='填写作品标题，为作品获得更多流量']");
            await page.WaitForTimeoutAsync(1000);
            await page.FillAsync("input[placeholder='填写作品标题，为作品获得更多流量']", model.Title.CutStrLen(60));
            await page.WaitForTimeoutAsync(1000);

            CallBack("正在输入简介...");
            // 输入内容
            var contentInput = await page.QuerySelectorAsync("div[data-placeholder='添加作品简介']");
            await contentInput.ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            CallBack("正在输入标签...");
            #region 添加标签
            var TagList = model.TagList();
            if (TagList.Count > 0)
            {
                var tagCount = Math.Min(TagList.Count, 10);
                for (int i = 0; i < tagCount; i++)
                {
                    await contentInput.FillAsync($"#{TagList[i]}");//FillAsync会替换原有内容
                    await contentInput.PressAsync("Enter");
                    await page.WaitForTimeoutAsync(3000);
                }
                await contentInput.PressAsync("Enter");
                await page.WaitForTimeoutAsync(1000);
            }
            #endregion

            CallBack("开始填写内容");

            #region 视频简介
            string sDescription = model.Info;
            if (!string.IsNullOrWhiteSpace(sDescription))
            {
                // 请填写视频简介
                await contentInput.FillAsync(sDescription.CutStrLen(400));
                await page.WaitForTimeoutAsync(1000);
            }
            #endregion

            CallBack("正在更改封面...");

            #region 更改封面

            // 上传封面
            var sDefaultCover  = base.GetCoverPath();
            var sCoverH = base.GetCtrValue("CoverImageH");
            if (!string.IsNullOrEmpty(model.ImgPath) || !string.IsNullOrEmpty(sDefaultCover) || string.IsNullOrEmpty(sCoverH)) // 上传封面
            {
                
                //await page.ClickAsync("text='选择封面'");
                await page.Locator("div").Filter(new() { HasTextRegex = new Regex("^选择封面$") }).Nth(1).ClickAsync();

                await page.WaitForTimeoutAsync(2000);


                await page.GetByText("设置竖封面").First.ClickAsync();

                if (!string.IsNullOrEmpty(sCoverH)) {
                    await page.SetInputFilesAsync(".semi-upload-hidden-input", sCoverH);
                    await page.WaitForTimeoutAsync(5000);
                }

                await page.WaitForTimeoutAsync(2000);
                await page.GetByText("设置横封面").First.ClickAsync();


                if (!string.IsNullOrWhiteSpace(sDefaultCover))
                {
                    var sImgPath = string.IsNullOrEmpty(sDefaultCover) ? model.ImgPath : sDefaultCover;
                    await page.WaitForTimeoutAsync(2000);
                    await page.SetInputFilesAsync(".semi-upload-hidden-input", sImgPath);
                    await page.WaitForTimeoutAsync(5000);
                } 

                await page.GetByRole(AriaRole.Button, new() { Name = "完成" }).ClickAsync();
                await page.WaitForTimeoutAsync(1000);
                 
            }

            #endregion 
            var isSy = GetCtrValue("是否同步");

            if (!string.IsNullOrEmpty(isSy) && !isSy.ToBool()) // 因为默认是选中状态，所以加了！
            {
                await page.Locator("label").Filter(new() { HasText = "不同时发布" }).ClickAsync();
                await page.WaitForTimeoutAsync(1000);
            }

            CallBack("正在选择集合...");
            var special = GetSpecialName();
            if (!string.IsNullOrWhiteSpace(special))
            {
                // 合集
                await page.GetByText("请选择合集").ClickAsync();
                await page.WaitForTimeoutAsync(2000); 
                await page.Locator(".collection-option").Filter(new() { HasText = special }).ClickAsync();

                await page.WaitForTimeoutAsync(2000);
            }

            //// 发布视频
            await page.GetByRole(AriaRole.Button, new() { Name = "发布", Exact = true }).ClickAsync();
            await page.WaitForTimeoutAsync(2000);
             

            return "";

        }

        public override string Help => @" 
";

    }
}
