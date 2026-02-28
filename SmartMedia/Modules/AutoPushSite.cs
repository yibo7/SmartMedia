
using Microsoft.Web.WebView2.Core;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.UIForms;
using SmartMedia.MCore;
 
using XS.Core2.XsExtensions;

namespace SmartMedia.Modules
{
    public partial class AutoPushSite : XsDockContent
    {
        public string Title => "分发平台站点";//要实现模块名称
        public System.Drawing.Image Ico => Resource.books; //要实现模块图标
        //VisualStudioToolStripExtender vsToolStripExtender1;
        private PushBase AutoPush;
        public AutoPushSite(PushBase autoPush)
        {
            AutoPush = autoPush;
            CloseButtonVisible = false; // 隐藏关闭按钮 
            InitializeComponent();

            // 开始异步初始化 WebView2
            //webBox.EnsureCoreWebView2Async(null);
            //// 等待 WebView2 初始化完成
            webBox.CoreWebView2InitializationCompleted += (sender, e) =>
            {
                webBox.CoreWebView2.WebMessageReceived += (object? sender, CoreWebView2WebMessageReceivedEventArgs e) =>
                {
                    if (e.TryGetWebMessageAsString() == "GoToLogin")
                    {
                        GoToLogin();
                    }
                };
            };
            OpenUrlAsync();
            webBox.NavigationStarting += webBox_NavigationStarting;
        }

        private async void GoToLogin()
        {
            string err = await AutoPush.LoginAsync();
            if (string.IsNullOrWhiteSpace(err))
            {
                await OpenUrlAsync();

            }
            else
            {
                XS.Core2.LogHelper.Error<AutoPushSite>($"登录发生异常：{err}");
                MessageBox.Show("登录发生异常,具体原因可查看日志！");
            }
        }
        private void webBox_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // 检查即将导航到的网址
            string uri = e.Uri;
            if (!Equals(uri, AutoPush.DataPage))
            {
                // 如果需要取消这次导航，可以调用e.Cancel = true;
                e.Cancel = true;

                string script = $"document.documentElement.innerHTML = `{htmlContent.Replace("`", "\\`")}`;";
                webBox.CoreWebView2.ExecuteScriptAsync(script);

            }

        }
        private async Task OpenUrlAsync()
        {
            // 等待 WebView2 环境准备就绪
            await webBox.EnsureCoreWebView2Async(null);
            string sJson = AutoPush.GetStateJsonStr();
            if (!string.IsNullOrEmpty(sJson))
            {
                var dJson = sJson.ToJsonDynamic();

                string sDomain = GetShortDomain(AutoPush.DataPage);

                foreach (var ck in dJson.cookies)
                {
                    string cookie_name = ck.name;
                    string cookie_value = ck.value;
                    CoreWebView2Cookie cookie = webBox.CoreWebView2.CookieManager.CreateCookie(cookie_name, cookie_value, sDomain, null);
                    //cookie.IsHttpOnly = false;
                    cookie.IsSecure = true;
                    webBox.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
                }

            }
            // 加载网页
            //webBox.Source = new Uri(AutoPush.DataPage);
            webBox.CoreWebView2.Navigate(AutoPush.DataPage);
        }
        private string GetShortDomain(string url)
        {
            Uri uri = new Uri(url);
            string host = uri.Host; // 获取主机名
            int dotIndex = host.IndexOf('.');
            if (dotIndex != -1)
            {
                // 从第一个点开始截取，获取短域名部分
                return host.Substring(dotIndex);
            }
            return host; // 如果没有点，返回整个主机名
        }


        private void btnShowReport_Click(object sender, EventArgs e)
        {
            base.OpenHelpDockToMain($"{AutoPush.PluginName}使用帮助", AutoPush.Help);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            GoToLogin();
        }



        private string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            flex-direction: column;
        }
        .center-content {
            text-align: center;
            font-size: 16px;
            color:#ff0000;
            padding:10px;
        }
 
        .blue-button {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 12px 24px;
            font-size: 16px;
            cursor: pointer;
            border-radius: 5px;
            transition: all 0.3s ease;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        .blue-button:hover {
            background-color: #0056b3;
            box-shadow: 0 6px 8px rgba(0, 0, 0, 0.15);
        }
        .blue-button:active {
            background-color: #004085;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            transform: translateY(2px);
        }

    </style>
</head>
<body>
    <div class='center-content'>
        您还没有登录网站或登录状态已过期，请点击右上角的【登录网站】
    </div>
     <!-- <button class='blue-button' onclick=""window.chrome.webview.postMessage('GoToLogin')"">立即登录</button> -->
    
    <script>
        console.log(""JavaScript loaded"");
    </script>
</body>
</html>

";

    }


}
