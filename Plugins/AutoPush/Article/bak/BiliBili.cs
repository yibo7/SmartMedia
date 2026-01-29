//using Microsoft.Playwright;
//using SmartMedia.Modules.PushContent.DB;
//using System.Text.RegularExpressions;
//using XS.Core2.XsExtensions;

//namespace SmartMedia.Plugins.AutoPush.Article
//{
//    public class BiliBili : ArticlePushBase
//    {
//        //override protected Dictionary<string, SettingCtrBase> SiteCtrls => new Dictionary<string, SettingCtrBase>() {

//        //    { "发布分类",new SettingItem(CtrlType.Categorie,"")},
//        //    { "文集名称",new SettingItem(CtrlType.TextBox,"可为空，你在此平台上创建的文集名称")}
//        //};


//        //override public Dictionary<string, string> CustomCtrls() => new Dictionary<string, string>() {
//        //    { "发布分类",""},{ "文集名称",""}
//        //};
//        override public int OrderIndex => 3;
//        override public string CategoryFileName => "BiliBili_A.json";
//        protected override string UploadPage => "https://member.bilibili.com/platform/upload/text/edit";
//        override public int CategoryLeve => 2;
//        public override string DataPage => "https://member.bilibili.com/platform/data-up/index";
//        public override string PluginName => "哔哩哔哩";
//        override public Image IcoName => Resource.bilibili;

//        override public async Task<string> LoginAsync()
//        {
//            return await LoginCustom("https://passport.bilibili.com/pc/passport/login", "span:has-text('消息')");
//        }

//        protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
//        {
//            var Iframe1 = page.FrameLocator("iframe[name=\"\\37 5\"]");

//            await page.WaitForTimeoutAsync(2000);
//            // 填写标题              
//            await Iframe1.GetByPlaceholder("请输入标题（建议30字以内）").FillAsync(model.Title.CutStrLen(30));
//            await page.WaitForTimeoutAsync(1000);

//            // 填写内容
//            var editorDiv = Iframe1.Locator("div[data-placeholder='请输入正文']");
//            if (await editorDiv.CountAsync() > 0)
//            {
//                await editorDiv.WaitForAsync();

//                List<string> oldImgs = ExtractImages(model.Info);

//                foreach (string oldimg in oldImgs)
//                {
//                    //// 开始等待文件选择器
//                    var waitForFileChooserTask = page.WaitForFileChooserAsync();
//                    //// 如果您有一个文件输入元素，需要设置文件，可以这样做：
//                    await Iframe1.GetByTitle("图片").ClickAsync();
//                    //// 等待文件选择器出现
//                    var fileChooser = await waitForFileChooserTask;
//                    //// 选择文件
//                    string src = GetImgSrc(oldimg);
//                    await fileChooser.SetFilesAsync(src);
//                    await page.WaitForTimeoutAsync(5000);
//                }

//                var imgHtml = await editorDiv.InnerHTMLAsync();
//                List<string> newImgs = ExtractImages(imgHtml);
//                string sNewContent = model.Info;
//                for (int i = 0; i < newImgs.Count; i++)
//                { // 替换成新上传的图片
//                    string NewSrc = newImgs[i];
//                    string OldSrc = oldImgs[i];
//                    sNewContent = sNewContent.Replace(OldSrc, NewSrc);
//                }
//                // 图片父级P标签要保留这个样式，normal-img\" contenteditable=\"false\，否则图片无法上传成功
//                sNewContent = sNewContent.Replace("class=\"edui-image-item edui-image-upload-item\"", "class=\"normal-img\" contenteditable=\"false\"");

//                //PrintLog(sNewContent);
//                await editorDiv.EvaluateAsync(@"(element, content) => {element.innerHTML = content;}", sNewContent);

//            }

//            await page.WaitForTimeoutAsync(1000);


//            // 选择分类 
//            var categoryList = GetCategoryList();


//            if (categoryList.Count == 2)
//            {
//                await Iframe1.GetByText("更多设置").ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await Iframe1.GetByRole(AriaRole.Button, new() { Name = categoryList[0] }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await Iframe1.GetByText(categoryList[1], new() { Exact = true }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }

//            #region 上传封面
//            if (!string.IsNullOrEmpty(model.ImgPath))
//            {
//                //// 开始等待文件选择器
//                var waitForFileChooserTask = page.WaitForFileChooserAsync();
//                //// 如果您有一个文件输入元素，需要设置文件，可以这样做：
//                await Iframe1.Locator(".bre-settings__coverbox__img").ClickAsync();
//                //// 等待文件选择器出现
//                var fileChooser = await waitForFileChooserTask;
//                //// 选择文件
//                await fileChooser.SetFilesAsync(model.ImgPath);

//                await page.WaitForTimeoutAsync(1000);
//                await Iframe1.GetByRole(AriaRole.Button, new() { Name = "确认" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);

//            }

//            #endregion

//            await Iframe1.GetByRole(AriaRole.Checkbox, new() { Name = "我声明此文章为原创" }).ClickAsync();
//            await page.WaitForTimeoutAsync(1000);
//            await Iframe1.Locator(".bre-modal__close > svg").ClickAsync();
//            await page.WaitForTimeoutAsync(1000);
//            #region 添加标签
//            var Tags = model.TagList();
//            if (Tags.Count > 0)
//            {
//                var tagInput = Iframe1.Locator("form");

//                var tagCount = Math.Min(Tags.Count, 10);
//                for (int i = 0; i < tagCount; i++)
//                {
//                    await tagInput.GetByRole(AriaRole.Textbox).FillAsync(Tags[i]);
//                    await tagInput.PressAsync("Enter");
//                    await page.WaitForTimeoutAsync(1000);
//                }
//                await page.WaitForTimeoutAsync(2000);
//            }
//            #endregion



//            var special = GetSpecialName("文集名称");
//            if (!string.IsNullOrWhiteSpace(special))
//            {
//                await Iframe1.GetByRole(AriaRole.Button, new() { Name = "选择文集" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await Iframe1.Locator("div").Filter(new() { HasTextRegex = new Regex($"^{special}$") }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//                await Iframe1.GetByRole(AriaRole.Button, new() { Name = "确认" }).ClickAsync();
//                await page.WaitForTimeoutAsync(1000);
//            }

//            //await page.WaitForTimeoutAsync(100000);
//            // 点击立即投稿
//            await Iframe1.GetByRole(AriaRole.Button, new() { Name = "提交文章" }).ClickAsync();
//            await page.WaitForTimeoutAsync(100000);


//            return "";

//        }

//        public override string Help => @"
//# 关于BiliBili(哔哩哔哩)

//BiliBili，通常被称为哔哩哔哩，是一个以年轻人为主要用户群体的视频分享网站，成立于2009年。它最初是一个以动画、漫画和游戏（ACG）文化为主题的社区，但随着时间的发展，BiliBili已经扩展成为一个多元化的内容平台，涵盖了各种类型的内容，包括但不限于音乐、舞蹈、科技、娱乐、生活、时尚、教育等领域。



//# 如何成为BiliBili（哔哩哔哩）的UP主
//要成为BiliBili（哔哩哔哩）的UP主，你可以按照以下步骤操作：

//1. **注册账号**：首先，使用浏览器搜索哔哩哔哩（B站）官网，进入官网注册并登录哔哩哔哩账号。

//> https://www.bilibili.com/

//2. **完成认证**：点击左上角头像完成bilibili认证。

//3. **投稿**：认证完成后，点击右上角的【投稿】按钮，并选择你想要投稿的类型。

//4. **上传视频**：点击【上传视频】，选择你想要上传的视频文件。

//5. **编辑信息**：等待视频上传完成后，选择封面（可以自行上传或选择系统自动生成的封面），选择视频的类型与分区，并填写视频的标题、标签、参加活动信息以及简介。

//6. **提交审核**：完成上述步骤后，点击【立即投稿】提交你的视频，之后需要等待稿件审核完毕。

//7. **审核通过**：一旦审核通过，你的视频就可以在B站上被大家看到，这样你就正式成为了一名UP主。
//";

//    }
//}
