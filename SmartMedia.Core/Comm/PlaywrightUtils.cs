using Microsoft.Playwright;
using XS.Core2.FSO;

namespace SmartMedia.Core.Comm;

public static class PlaywrightUtils
{
    static public async Task<IBrowserContext> CreatePlaywrightAsync(string StateFullPath = "", bool IsHeadless = false)
    {

        var playwright = await Playwright.CreateAsync();
        var borwnconfigs = new BrowserTypeLaunchOptions();
        borwnconfigs.Headless = IsHeadless;//运行时是否禁止打开浏览器，默认为true
        borwnconfigs.Channel = "msedge"; // 默认使用google浏览器，可以这样指定使用edge浏览器 
        // 指定谷歌浏览器路径
        //borwnconfigs.ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

        var browser = await playwright.Chromium.LaunchAsync(borwnconfigs);


        // 模拟一些客户端的参数
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36", // 设置User-Agent
            ViewportSize = new ViewportSize { Width = 1366, Height = 768 } // 设置视窗大小
            ,
            StorageStatePath = StateFullPath
            ,
            AcceptDownloads = true,
        });
        return context;
    }
    static public async Task<string> GetState(string statePath)
    {
        if (!FObject.IsExist(statePath, FsoMethod.File))
            return "";
        var storageStateJson = await File.ReadAllTextAsync(statePath);
        return storageStateJson;
    }
    public static async void SaveState(string storageState, string statePath)
    {
        // 将存储状态保存到 SiteState 目录下
        XS.Core2.FSO.FObject.ExistsDirectory(statePath);
        // 将存储状态保存到文件中
        await File.WriteAllTextAsync(statePath, storageState);
    }
}
