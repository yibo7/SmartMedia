using Microsoft.Playwright;

namespace SmartMedia.AtqCore
{
    public class AutoImgFromBing
    {

        static public async Task<string> InserImgsAsync(string NewsTitle, string NewsContent)
        {
            List<string> imgs = await GetAImgBase64ListAsync(NewsTitle, XS.Core2.XsUtils.GetRandNum(3));
            string imgcontent = NewsContent;
            for (int i = 0; i < imgs.Count; i++)
            {
                imgcontent = InsertTextAfterDot(imgcontent, i + 1, imgs[i]);
            }

            return imgcontent;

        }

        /// <summary>
        /// 获取一个图片
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="iIndex">获取第几个图片，索引号，可以从0开始</param>
        /// <returns></returns>
        static private async Task<string> GetAImgBase64Async(string keyword, int iIndex)
        {
            await using var context = await PlaywrightUtils.CreatePlaywrightAsync("", true);
            var page = await context.NewPageAsync();
            await page.GotoAsync($"https://www.bing.com/images/search?q={keyword}");
            var imgs = await page.QuerySelectorAllAsync("div.imgpt");

            var ImgBox = imgs[0];
            if (imgs.Count > iIndex)
                ImgBox = imgs[iIndex];
            else
                return "";

            await ImgBox.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await page.WaitForTimeoutAsync(2000);
            await page.ReloadAsync();
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            // 查找第一个具有class为"figure-result"的div元素
            var resultDiv = await page.QuerySelectorAsync("//div[@class='imgContainer']");

            // 查找第一个img标签
            var img_box = await resultDiv.QuerySelectorAsync("img");


            // 截取屏幕截图
            var screenshot = await img_box.ScreenshotAsync();
            // 将截图保存为 PNG 文件
            //File.WriteAllBytes("D:\\screenshot2.png", screenshot);
            // 将字节数组转换为 Base64 编码字符串
            string base64String = Convert.ToBase64String(screenshot);
            if (!string.IsNullOrEmpty(base64String))
                return $"<br/><img src=\"data:image/png;base64,{base64String}\" ><br/>";
            return string.Empty;


        }
        /// <summary>
        /// 一次性获取多张图片
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="num">图片数量</param>
        /// <returns></returns>
        static private async Task<List<string>> GetAImgBase64ListAsync(string keyword, int num)
        {
            List<string> lst = new List<string>();
            for (int i = 0; i < num; i++)
            {
                var img = await GetAImgBase64Async(keyword, i);
                if (!string.IsNullOrEmpty(img))
                    lst.Add(img);
                Thread.Sleep(2000);
            }
            return lst;
        }

        /// <summary>
        /// 向第几个句号插入图片
        /// </summary>
        /// <param name="input">内容</param>
        /// <param name="dotPosition">第几个句号</param>
        /// <param name="textToInsert">插入的图片</param>
        /// <returns></returns>
        static private string InsertTextAfterDot(string input, int dotPosition, string textToInsert)
        {
            if (dotPosition >= 2) // 如果数量大于2就直接将图片加在后面，因为排版问题还没解决暂时先这样
            {
                return $"{input}{textToInsert}";
            }
            int index = input.IndexOf('。');
            for (int i = 1; i < dotPosition && index >= 0; i++)
            {
                index = input.IndexOf('。', index + 1);
            }

            if (index >= 0)
            {
                return input.Insert(index + 1, textToInsert);
            }
            else
            {
                return input;
            }
        }
    }
}
