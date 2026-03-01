using Microsoft.Playwright;
using SmartMedia.Core;
using SmartMedia.Core.Controls;
using SmartMedia.Core.SitesBase;
using SmartMedia.Core.SitesBase.DB; 

namespace SmartMedia.Sites.ImagePosts;

public class Twitter : ImagePushBase
{
    public override string PluginName => "Twitter";
    override public Image IcoName => Resource.twitter;
    protected override string UploadPage => "";
    /// <summary>
    /// UploadPage 与 DataPage为空，将视为API上传插件，打开平台时会有专门管理界面，否则将以平台后台页面为管理界面
    /// </summary>
    public override string DataPage => "";
    override protected Dictionary<string, SettingCtrBase> SiteCtrls
    {
        get
        {
            return new Dictionary<string, SettingCtrBase>() { };
        }
    }
    public Twitter()
    {
        OnChangeSetting();
    }
    override public void OnChangeSetting()
    {
        string appId = base.GetCf("ConsumerKey");
        string appSecret = base.GetCf("SecretKey");

        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(appSecret))
        {
            PrintLog("还没有公众号的配置或AppID");
        }

        //publisher = new WeChatArticlePublisher(appId, appSecret);
    }
    override public Dictionary<string, string> InitConfig() => new Dictionary<string, string>() {
            { "ApiKey",""},
            { "ApiSecret",""},
            { "AccessToken",""},
            { "AccessSecret",""}
        };
    override public async Task<string> LoginAsync()
    {
        var dlg = new PluginConfigs(this);
        dlg.ShowDialog();
        return ""; 
    }

 

    protected override async Task<string> ActionsAsync(PushInfo? model, IPage page, Action<string> CallBack)
    {
        CallBack("激活编辑框");
         

        return "";

    }

    public override string Help => @" 

# TwitterXClient 配置说明

> 适用于 · OAuth 1.0a · X API v2

---

## 一、配置密钥

 打开内容管理界面->点击【设置接口参数】
如果还没有申请开发者身份，请参考第三节。

| 构造函数参数 | 开发者后台名称 | 你手中的凭证 | 是否必须 | 说明 |
|---|---|---|---|---|
| `apiKey` | API Key (Consumer Key) | **Consumer Key** | ✅ 必须 | 应用唯一标识符 |
| `apiSecret` | API Key Secret | **Secret Key** | ✅ 必须 | Consumer Secret，用于签名 |
| `accessToken` | Access Token | ⚠️ 需手动生成 | ✅ 必须 | 代表用户的操作令牌 |
| `accessSecret` | Access Token Secret | ⚠️ 需手动生成 | ✅ 必须 | Access Token 对应的密钥 |

> **注意：** `Access Token` 和 `Access Token Secret` 初始不会自动提供，需在开发者后台手动点击生成，详见第三节。

---

## 二、你手中所有凭证的用途说明

开发者后台会提供多种凭证，以下是它们的完整说明：

| 凭证名称 | 作用 | 工具类使用情况 |
|---|---|---|
| Consumer Key | OAuth 1.0a 签名 — 应用标识 | ✅ 使用（`apiKey`） |
| Secret Key | OAuth 1.0a 签名 — 应用密钥 | ✅ 使用（`apiSecret`） |
| OAuth 1.0 Client Secret | 与 Secret Key 本质相同，后台不同页面的不同叫法 | ✅ 同上 |
| Access Token | 代表用户的操作令牌（需手动生成） | ✅ 使用（`accessToken`） |
| Access Token Secret | Access Token 对应密钥（需手动生成） | ✅ 使用（`accessSecret`） |
| Bearer Token | App-only 只读请求，无需用户授权 | — 未使用（可扩展） |
| OAuth 2.0 Client Secret | 第三方用户 OAuth 2.0 授权登录流程专用 | — 不需要 |

> **关于 OAuth 1.0 Client Secret：** 这只是 Secret Key（Consumer Secret）在后台某些页面的另一种叫法，本质是同一个值，不必困惑。

---

## 三、X 开发者账号注册与申请步骤

### 第一步：注册 / 登录 X 账号

前往 [x.com](https://x.com) 注册普通用户账号，如已有账号直接登录。建议先完善个人资料（头像、简介），有助于降低开发者申请被拒的概率。

---

### 第二步：申请开发者账号

1. 访问 [developer.x.com](https://developer.x.com)，点击右上角 **Sign up** 或 **Apply**。
2. 填写申请表单：
   - **使用目的** — 用中文或英文描述你的场景，例如：`个人项目，用于管理自己的账号、发布内容和获取统计数据`
   - **国家 / 地区** — 选择你所在的地区
3. 同意开发者协议，提交申请。

> Free 套餐通常即时审批通过，部分账号需等待 1–3 个工作日，审批结果会发送到注册邮箱。

---

### 第三步：创建项目和应用（App）

1. 进入开发者后台 → **Projects & Apps** → **Create Project**
2. 填写项目名称，选择使用场景
3. 填写 App 名称（全局唯一）
4. 创建成功后，后台会立即展示：
   - `API Key`（即 Consumer Key）
   - `API Key Secret`（即 Secret Key）

> ⚠️ **这两个值只显示一次，必须立即复制保存！** 关闭后无法再次查看，只能重新生成（重新生成会使旧凭证失效）。

---

### 第四步：配置应用权限（必须设为 Read and Write）

1. 进入 App → **Settings** → **User authentication settings** → **Edit**
2. 将 **App permissions** 设置为 **Read and Write**
   - 默认为只读（Read Only），**必须修改，否则无法发帖**
3. **Type of App** 选择 **Web App, Automated App or Bot**
4. **Callback URI** 填写任意地址，例如：`https://localhost/callback`
5. **Website URL** 填写你的网站或 GitHub 地址
6. 保存设置

> ⚠️ **重要：** 修改权限后，已生成的 Access Token 会立即失效，需要重新生成。

---

### 第五步：生成 Access Token 和 Access Token Secret

1. 进入 App → **Keys and Tokens** 页面
2. 找到 **Authentication Tokens** 区域下的 **Access Token and Secret**
3. 点击 **Generate** 按钮
4. 立即复制并保存：
   - `Access Token`（形如 `1234567890-xxxxxxxxxxxxxxxx`）
   - `Access Token Secret`（一串随机字符串）

> ⚠️ **同样只显示一次，立即保存！** 关闭后只能重新生成，旧 Token 随即失效。

---

## 四、开发者套餐说明

| 套餐 | 月费 | 每月发推限额 | 搜索推文 | 适用场景 |
|---|---|---|---|---|
| **Free** | 免费 | 1,500 条 | 仅限自己的推文 | 个人测试 |
| **Basic** | $100 / 月 | 3,000 条 | 最近 7 天 | 小型应用 |
| **Pro** | $5,000 / 月 | 300,000 条 | 最近 30 天 | 商业应用 |
| **Enterprise** | 定制 | 无限制 | 完整历史 | 企业级 |

工具类的所有功能在 **Free 套餐**下均可使用，但 `SearchTweetsAsync`（搜索推文）需要 **Basic 及以上套餐**，个人项目从 Free 开始即可。

---

## 五、安全注意事项

### 凭证保护

- **不要**将任何密钥硬编码在源代码中，尤其不能上传到 GitHub 等公开仓库
- 推荐使用**环境变量**或 `dotnet user-secrets` 管理凭证
- 使用 `.gitignore` 排除包含密钥的配置文件（如 `appsettings.Development.json`）

 

### 权限最小化

- 如果只需读取数据，将 App 权限设为 **Read Only**，不申请多余权限
- Access Token 的权限与生成时 App 的权限设置绑定，修改 App 权限后需重新生成 Token
- 建议定期轮换（重新生成）Access Token，降低泄露风险

---

## 六、快速检查清单

在开始使用工具类前，确认以下所有项目已完成：

- [ ] 已注册 X 账号并完善个人资料
- [ ] 已在 developer.x.com 申请开发者账号并通过审批
- [ ] 已创建项目和 App
- [ ] 已将 App 权限设置为 **Read and Write**
- [ ] 已获取并保存 `API Key`（Consumer Key）
- [ ] 已获取并保存 `API Key Secret`（Secret Key）
- [ ] 已生成并保存 `Access Token`
- [ ] 已生成并保存 `Access Token Secret`
- [ ] 凭证已通过环境变量或 user-secrets 管理，未硬编码在代码中

---

## 七、常用链接

| 链接 | 地址 |
|---|---|
| X 开发者门户 | https://developer.x.com |
| 申请开发者账号 | https://developer.x.com/en/portal/petition/essential/basic-info |
| X API v2 官方文档 | https://developer.x.com/en/docs/x-api |
| API 套餐与限额说明 | https://developer.x.com/en/docs/x-api/rate-limits |
| OAuth 1.0a 文档 | https://developer.x.com/en/docs/authentication/oauth-1-0a |

";

}
