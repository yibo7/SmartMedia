using System.Text.RegularExpressions;

namespace SmartMedia.Core.SitesBase;

abstract public class ArticlePushBase : PushBase
{
    protected List<string> ExtractImages(string html)
    {
        // 正则表达式匹配整个img标签
        // 注意：这个正则表达式假设img标签没有嵌套引号，并且属性名和值之间没有空格
        string pattern = @"<img\s+([^>]*?)\s*>";

        // 创建Regex对象，使用Singleline模式使.匹配换行符
        Regex regex = new Regex(pattern, RegexOptions.Singleline);
        List<string> lst = new List<string>();
        // 匹配所有img标签
        MatchCollection matches = regex.Matches(html);

        // 遍历所有匹配项
        foreach (Match match in matches)
        {
            // 提取整个img标签
            string imgNode = match.Value;
            lst.Add(imgNode);
        }
        return lst;
    }

    protected string GetImgSrc(string imgtag)
    {
        // 正则表达式匹配第一个img标签的src属性
        string pattern = @"<img[^>]*?src=['""](.*?)['""]";

        Regex regex = new Regex(pattern);

        Match match = regex.Match(imgtag);

        if (match.Success)
        {
            // match.Groups[1].Value 是src的值
            string srcValue = match.Groups[1].Value;
            return srcValue;
        }
        return "";
    }
}
