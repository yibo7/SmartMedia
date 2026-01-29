//using Microsoft.Playwright;
//using SmartMedia.Modules.PushContent.DB;
//using XS.Core2.XsExtensions;

//namespace SmartMedia.Plugins.AutoPush.Article
//{
//    public class TaoTiaoHao : ArticlePushBase
//    { 
//        public override string PluginName => "今日头条";
//        override public Image IcoName => Resource.taotiaohao;
//        protected override string UploadPage => "https://mp.toutiao.com/profile_v4/graphic/publish";

//        public override string DataPage => "https://mp.toutiao.com/profile_v4/analysis/works-overall/article";

//        override public async Task<string> LoginAsync()
//        {
//            return await LoginCustom("https://mp.toutiao.com/auth/page/login", "span:text('粉丝数')");
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
//";

//    }
//}
