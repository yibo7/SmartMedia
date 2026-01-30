# Facebook & Instagram API 凭据获取完整指南

## 📋 目录

1. [前置准备](#前置准备)
2. [Facebook Page 凭据获取](#facebook-page-凭据获取)
3. [Instagram Business 凭据获取](#instagram-business-凭据获取)
4. [权限配置](#权限配置)
5. [Token 管理](#token-管理)
6. [常见问题](#常见问题)
7. [安全最佳实践](#安全最佳实践)

---

## 前置准备

### 必备条件

在开始之前，您需要确保：

- ✅ 拥有一个 **Facebook Page**（公共主页）
- ✅ 拥有一个 **Instagram Business Account**（商业账户）
- ✅ Instagram 账户已关联到 Facebook Page
- ✅ Facebook 开发者账号
- ✅ 创建 Facebook App

### 账户要求

| 平台 | 账户类型要求 | 说明 |
|------|-------------|------|
| Facebook | Facebook Page (公共主页) | 必须是 Page 管理员 |
| Instagram | Business Account (商业账户) | 不能是个人账户 |
| 关联要求 | Instagram 必须关联到 Facebook Page | 在 Instagram 设置中完成 |

---

## Facebook Page 凭据获取

### 第一步：创建 Facebook App

#### 1. 访问 Facebook 开发者平台

🔗 访问：https://developers.facebook.com/

#### 2. 创建应用

```
1. 点击右上角 "My Apps" → "Create App"
2. 选择应用类型：
   - 对于内容发布，选择 "Business" 或 "Consumer"
3. 填写应用信息：
   - App Display Name: 你的应用名称（如 "Social Media Manager"）
   - App Contact Email: 你的邮箱
   - Business Account: 选择或创建商务账号（可选）
4. 点击 "Create App"
```

![创建应用流程]
```
Facebook Developers
    └── My Apps
        └── Create App
            ├── Choose an app type
            │   └── [Business] or [Consumer]
            └── Tell us about your app
                ├── App Display Name: ___________
                ├── App Contact Email: ___________
                └── [Create App]
```

#### 3. 记录 App ID 和 App Secret

创建成功后，在应用面板的 **Settings** → **Basic** 中：

```
App ID: 123456789012345          ← 复制这个
App Secret: [Show] → abc123...   ← 点击 Show 并复制
```

**⚠️ 重要**: App Secret 是敏感信息，不要公开分享！

---

### 第二步：配置应用权限

#### 1. 添加 Facebook Login 产品

```
应用面板左侧菜单:
    └── Add Products
        └── Facebook Login
            └── [Set Up]
```

#### 2. 配置 OAuth 重定向 URI

在 **Facebook Login** → **Settings** 中：

```
Valid OAuth Redirect URIs: 
    https://your-domain.com/auth/callback
    http://localhost:8080/auth/callback  (用于本地测试)
```

#### 3. 添加所需产品

在左侧菜单 **Add Products** 中添加：
- ✅ **Facebook Login** (用于获取 Token)
- ✅ **Pages** (用于管理 Facebook Page)
- ✅ **Instagram** (用于管理 Instagram)

---

### 第三步：获取 Facebook Page ID

#### 方法1：通过 Facebook Page 设置（最简单）

```
1. 访问你的 Facebook Page
2. 点击左侧菜单 "About" (关于)
3. 向下滚动到 "Page ID"
4. 复制该 ID

示例 Page ID: 102736478291756
```

#### 方法2：通过 Page URL

如果你的 Page URL 是：
```
https://www.facebook.com/YourPageName
```

使用 Graph API Explorer 查询：
```
GET https://graph.facebook.com/YourPageName
```

返回结果包含：
```json
{
  "id": "102736478291756",  ← 这就是你的 Page ID
  "name": "Your Page Name"
}
```

#### 方法3：通过 Meta Business Suite

```
1. 访问 https://business.facebook.com/
2. 选择你的 Page
3. 点击 "Settings" (设置)
4. 在 "Page Info" 中查看 Page ID
```

---

### 第四步：获取 Facebook Access Token

#### 选项A：使用 Graph API Explorer（适合测试）

🔗 访问：https://developers.facebook.com/tools/explorer/

```
1. 选择你的应用（右上角下拉菜单）
2. 点击 "Generate Access Token"
3. 选择所需权限：
   ✅ pages_manage_posts
   ✅ pages_read_engagement
   ✅ pages_manage_metadata
   ✅ pages_read_user_content
   ✅ business_management (如果管理多个 Page)
4. 点击 "Generate Access Token"
5. Facebook 会要求你授权 - 点击 "Continue" 和 "OK"
6. 复制生成的 User Access Token
```

⚠️ **注意**: 这个 Token 是短期的（1-2小时），需要转换为长期 Token。

#### 选项B：使用 OAuth 流程（生产环境推荐）

**1. 构建授权 URL**

```csharp
// C# 代码示例
var appId = "YOUR_APP_ID";
var redirectUri = "https://your-domain.com/auth/callback";
var scope = "pages_manage_posts,pages_read_engagement,pages_manage_metadata";

var authUrl = $"https://www.facebook.com/v19.0/dialog/oauth?" +
              $"client_id={appId}&" +
              $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
              $"scope={scope}&" +
              $"response_type=code";

// 将用户重定向到这个 URL
```

**2. 用户授权后，交换 Code 获取 Token**

```csharp
// 用户授权后，Facebook 会重定向到你的 callback URL，带上 code 参数
// https://your-domain.com/auth/callback?code=AUTHORIZATION_CODE

var code = "AUTHORIZATION_CODE"; // 从回调 URL 获取
var appId = "YOUR_APP_ID";
var appSecret = "YOUR_APP_SECRET";
var redirectUri = "https://your-domain.com/auth/callback";

var tokenUrl = $"https://graph.facebook.com/v19.0/oauth/access_token?" +
               $"client_id={appId}&" +
               $"client_secret={appSecret}&" +
               $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
               $"code={code}";

// 发送 GET 请求获取 Access Token
var response = await httpClient.GetStringAsync(tokenUrl);

// 返回格式：
// {
//   "access_token": "USER_ACCESS_TOKEN",
//   "token_type": "bearer"
// }
```

#### 选项C：使用 Facebook SDK（最简单的生产方式）

```bash
# 安装 Facebook SDK
dotnet add package Facebook
```

```csharp
using Facebook;

var fb = new FacebookClient();
fb.AppId = "YOUR_APP_ID";
fb.AppSecret = "YOUR_APP_SECRET";

// 使用 OAuth 获取 Token
// 具体实现参考 Facebook SDK 文档
```

---

### 第五步：转换为长期 Page Access Token

#### 1. 将短期 User Token 转换为长期 User Token

```http
GET https://graph.facebook.com/v19.0/oauth/access_token
    ?grant_type=fb_exchange_token
    &client_id=YOUR_APP_ID
    &client_secret=YOUR_APP_SECRET
    &fb_exchange_token=SHORT_LIVED_USER_TOKEN
```

**C# 实现示例**:

```csharp
public async Task<string> GetLongLivedUserTokenAsync(string shortLivedToken)
{
    var appId = "YOUR_APP_ID";
    var appSecret = "YOUR_APP_SECRET";
    
    var url = $"https://graph.facebook.com/v19.0/oauth/access_token?" +
              $"grant_type=fb_exchange_token&" +
              $"client_id={appId}&" +
              $"client_secret={appSecret}&" +
              $"fb_exchange_token={shortLivedToken}";
    
    var response = await httpClient.GetStringAsync(url);
    var result = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
    
    return result["access_token"].ToString(); // 有效期 60 天
}
```

#### 2. 使用长期 User Token 获取 Page Access Token

```http
GET https://graph.facebook.com/v19.0/me/accounts
    ?access_token=LONG_LIVED_USER_TOKEN
```

**C# 实现示例**:

```csharp
public async Task<string> GetPageAccessTokenAsync(string userAccessToken, string pageId)
{
    var url = $"https://graph.facebook.com/v19.0/me/accounts" +
              $"?access_token={userAccessToken}";
    
    var response = await httpClient.GetStringAsync(url);
    var result = JsonSerializer.Deserialize<FacebookAccountsResponse>(response);
    
    // 找到对应的 Page
    var page = result.Data.FirstOrDefault(p => p.Id == pageId);
    
    if (page == null)
        throw new Exception($"Page {pageId} not found");
    
    return page.AccessToken; // 这个 Token 永不过期（除非密码更改或权限撤销）
}

// 响应模型
public class FacebookAccountsResponse
{
    [JsonPropertyName("data")]
    public List<PageInfo> Data { get; set; }
}

public class PageInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } // ← Page Access Token
    
    [JsonPropertyName("category")]
    public string Category { get; set; }
}
```

返回示例：
```json
{
  "data": [
    {
      "access_token": "EAABwzLixnjY...", // ← 这就是你要的 Page Access Token
      "category": "Community",
      "category_list": [...],
      "name": "Your Page Name",
      "id": "102736478291756",           // ← 你的 Page ID
      "tasks": ["ANALYZE", "ADVERTISE", "MODERATE", "CREATE_CONTENT"]
    }
  ]
}
```

---

## Instagram Business 凭据获取

### 前置条件检查

在获取 Instagram 凭据前，确保：

1. ✅ Instagram 账户已转换为 **Business Account** 或 **Creator Account**
2. ✅ Instagram Business Account 已关联到 Facebook Page
3. ✅ 你是该 Facebook Page 的管理员

---

### 第一步：将 Instagram 转换为 Business Account

#### 在 Instagram App 中操作：

```
1. 打开 Instagram App
2. 进入 Profile (个人主页)
3. 点击右上角 ≡ (菜单)
4. 选择 "Settings" (设置)
5. 选择 "Account" (账户)
6. 选择 "Switch to Professional Account" (切换到专业账户)
7. 选择类别 → 选择 "Business" (商业)
8. 完成设置流程
```

---

### 第二步：关联 Instagram 到 Facebook Page

#### 方法1：在 Instagram App 中关联

```
1. Instagram App → Profile → ≡ → Settings
2. 选择 "Business" 或 "Account"
3. 选择 "Linked Accounts" (关联账户)
4. 选择 "Facebook"
5. 登录你的 Facebook 账户
6. 选择要关联的 Facebook Page
7. 确认关联
```

#### 方法2：在 Facebook Page 中关联

```
1. 访问你的 Facebook Page
2. 进入 "Settings" (设置)
3. 左侧菜单选择 "Instagram"
4. 点击 "Connect Account" (关联账户)
5. 登录 Instagram 账户
6. 授权关联
```

#### 方法3：通过 Meta Business Suite

```
1. 访问 https://business.facebook.com/
2. 选择你的 Business Portfolio
3. 点击 "Settings" → "Accounts" → "Instagram accounts"
4. 点击 "Add" → "Connect an Instagram account"
5. 登录并授权
```

---

### 第三步：获取 Instagram Business Account ID

#### 方法1：使用 Graph API（推荐）

使用你的 **Facebook Page Access Token**：

```http
GET https://graph.facebook.com/v19.0/{PAGE_ID}
    ?fields=instagram_business_account
    &access_token={PAGE_ACCESS_TOKEN}
```

**C# 实现示例**:

```csharp
public async Task<string> GetInstagramBusinessAccountIdAsync(
    string pageId, 
    string pageAccessToken)
{
    var url = $"https://graph.facebook.com/v19.0/{pageId}" +
              $"?fields=instagram_business_account" +
              $"&access_token={pageAccessToken}";
    
    var response = await httpClient.GetStringAsync(url);
    var result = JsonSerializer.Deserialize<PageInstagramResponse>(response);
    
    return result.InstagramBusinessAccount.Id;
}

// 响应模型
public class PageInstagramResponse
{
    [JsonPropertyName("instagram_business_account")]
    public InstagramBusinessAccount InstagramBusinessAccount { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
}

public class InstagramBusinessAccount
{
    [JsonPropertyName("id")]
    public string Id { get; set; } // ← Instagram Business Account ID
}
```

返回示例：
```json
{
  "instagram_business_account": {
    "id": "17841405309211844"  // ← 这就是你的 Instagram Business Account ID
  },
  "id": "102736478291756"
}
```

#### 方法2：使用 Graph API Explorer

🔗 访问：https://developers.facebook.com/tools/explorer/

```
1. 选择你的应用
2. 选择你的 Page Access Token
3. 在查询框输入：
   {your-page-id}?fields=instagram_business_account
4. 点击 "Submit"
5. 查看返回的 instagram_business_account.id
```

---

### 第四步：Instagram Access Token

**好消息**：Instagram API 使用的是 **Facebook Page Access Token**！

也就是说：
```csharp
private const string InstagramAccessToken = "your-facebook-access-token";
// ↑ 这个和 Facebook Page Access Token 是同一个！
```

你不需要单独为 Instagram 获取 Token，只需要：
- ✅ 使用 Facebook Page Access Token
- ✅ 确保 Page 已关联 Instagram Business Account
- ✅ Token 拥有必要的 Instagram 权限

---

## 权限配置

### Facebook Page 所需权限

在 Facebook App 设置中，确保申请以下权限：

#### 必需权限

| 权限名称 | 用途 | 审核要求 |
|---------|------|---------|
| `pages_manage_posts` | 发布内容到 Page | ✅ 需要审核 |
| `pages_read_engagement` | 读取互动数据 | ✅ 需要审核 |
| `pages_manage_metadata` | 管理 Page 元数据 | ✅ 需要审核 |

#### 可选权限

| 权限名称 | 用途 | 审核要求 |
|---------|------|---------|
| `pages_read_user_content` | 读取用户生成的内容 | ✅ 需要审核 |
| `pages_show_list` | 获取 Page 列表 | ❌ 无需审核 |
| `business_management` | 管理多个 Business | ✅ 需要审核 |

### Instagram 所需权限

| 权限名称 | 用途 | 审核要求 |
|---------|------|---------|
| `instagram_basic` | 基础 Instagram 功能 | ✅ 需要审核 |
| `instagram_content_publish` | 发布内容 | ✅ 需要审核 |
| `instagram_manage_comments` | 管理评论 | ✅ 需要审核 |
| `instagram_manage_insights` | 查看分析数据 | ✅ 需要审核 |

### 申请权限审核流程

```
1. 访问 Facebook App 控制台
2. 左侧菜单 → "App Review" → "Permissions and Features"
3. 找到需要的权限，点击 "Request Advanced Access"
4. 填写审核问卷：
   - 说明应用用途
   - 提供应用截图/演示视频
   - 解释为什么需要该权限
5. 提交审核（通常 1-7 天）
```

**审核提示**：
- ✅ 提供详细的应用说明
- ✅ 准备应用演示视频
- ✅ 说明使用场景和必要性
- ❌ 不要请求不需要的权限

---

## Token 管理

### Token 类型和有效期

| Token 类型 | 有效期 | 用途 |
|-----------|--------|------|
| User Access Token (短期) | 1-2 小时 | 初始授权 |
| User Access Token (长期) | 60 天 | 获取 Page Token |
| Page Access Token | 永久* | 发布内容 |

*永久有效，除非：
- 用户更改 Facebook 密码
- 用户撤销应用权限
- Facebook 检测到安全问题

### Token 刷新策略

#### 实现自动刷新逻辑

```csharp
public class TokenManager
{
    private string _pageAccessToken;
    private DateTime _lastRefreshed;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromDays(30);
    
    public async Task<string> GetValidPageTokenAsync()
    {
        // Page Token 理论上永不过期，但建议定期验证
        if (DateTime.Now - _lastRefreshed > _refreshInterval)
        {
            await ValidateAndRefreshTokenAsync();
        }
        
        return _pageAccessToken;
    }
    
    private async Task ValidateAndRefreshTokenAsync()
    {
        // 验证 Token 是否仍然有效
        var isValid = await ValidateTokenAsync(_pageAccessToken);
        
        if (!isValid)
        {
            // Token 无效，需要用户重新授权
            throw new InvalidOperationException(
                "Page Access Token is invalid. User re-authorization required.");
        }
        
        _lastRefreshed = DateTime.Now;
    }
    
    private async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var url = $"https://graph.facebook.com/v19.0/me" +
                      $"?access_token={token}";
            
            var response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
```

### Token 调试工具

🔗 Facebook Access Token Debugger:  
https://developers.facebook.com/tools/debug/accesstoken/

输入你的 Token 可以看到：
- ✅ Token 类型
- ✅ 过期时间
- ✅ 关联的应用
- ✅ 授予的权限
- ✅ 是否有效

---

## 常见问题

### Q1: 为什么我的 Page 没有 instagram_business_account 字段？

**可能原因**：
1. ❌ Instagram 账户不是 Business 或 Creator 账户
2. ❌ Instagram 没有关联到这个 Facebook Page
3. ❌ 关联后需要等待几分钟同步

**解决方案**：
```bash
# 检查 Page 字段
GET https://graph.facebook.com/v19.0/{PAGE_ID}
    ?fields=id,name,instagram_business_account
    &access_token={TOKEN}

# 如果没有 instagram_business_account，重新关联
```

### Q2: Token 获取后立即失效？

**可能原因**：
1. ❌ 应用处于 Development Mode
2. ❌ 使用了错误的 Token 类型
3. ❌ 权限未审核通过

**解决方案**：
```
1. 检查应用模式：App Dashboard → Settings → Basic → App Mode
   - Development: 只有测试用户可用
   - Live: 所有用户可用
   
2. 将应用切换到 Live 模式（需要通过审核）
```

### Q3: 获取 Page Token 时返回空数组？

```json
{
  "data": []  // 空的！
}
```

**可能原因**：
1. ❌ User Token 没有 `pages_show_list` 权限
2. ❌ 用户不是任何 Page 的管理员
3. ❌ Page 被禁用或删除

**解决方案**：
```
1. 在 Graph API Explorer 重新生成 Token，确保勾选 pages_show_list
2. 检查用户在 Facebook 上是否是 Page 管理员
```

### Q4: Instagram API 返回 "Invalid user id"？

**可能原因**：
1. ❌ 使用了 Instagram User ID 而不是 Business Account ID
2. ❌ Token 没有 Instagram 权限

**解决方案**：
```csharp
// ❌ 错误：使用 Instagram 用户名或个人账户 ID
var wrongId = "your_instagram_username";

// ✅ 正确：使用通过 Graph API 获取的 Business Account ID
var correctId = "17841405309211844";
```

### Q5: 如何在本地开发测试？

**最佳实践**：

```csharp
// appsettings.Development.json
{
  "Facebook": {
    "AppId": "your-app-id",
    "AppSecret": "your-app-secret",
    "PageId": "your-test-page-id",
    "PageAccessToken": "test-token-from-graph-explorer"
  }
}

// 使用 Graph API Explorer 获取测试 Token
// 注意：这些 Token 是短期的，适合开发测试
```

---

## 安全最佳实践

### 1. 永不硬编码敏感信息

```csharp
// ❌ 危险：硬编码
public class FacebookPublisher
{
    private const string PageId = "123456789";
    private const string AccessToken = "EAABwz...";  // 绝对不要这样做！
}

// ✅ 安全：使用配置
public class FacebookPublisher
{
    private readonly string _pageId;
    private readonly string _accessToken;
    
    public FacebookPublisher(IConfiguration config)
    {
        _pageId = config["Facebook:PageId"];
        _accessToken = config["Facebook:AccessToken"];
    }
}
```

### 2. 使用环境变量

```bash
# .env 文件（添加到 .gitignore）
FB_PAGE_ID=102736478291756
FB_ACCESS_TOKEN=EAABwzLixnjY...
IG_ACCOUNT_ID=17841405309211844
```

```csharp
// 读取环境变量
var pageId = Environment.GetEnvironmentVariable("FB_PAGE_ID");
var token = Environment.GetEnvironmentVariable("FB_ACCESS_TOKEN");
```

### 3. 使用 Azure Key Vault 或 AWS Secrets Manager

```csharp
// Azure Key Vault 示例
var client = new SecretClient(
    new Uri("https://your-vault.vault.azure.net/"),
    new DefaultAzureCredential());

var secret = await client.GetSecretAsync("FacebookPageAccessToken");
var token = secret.Value.Value;
```

### 4. Token 轮换策略

```csharp
public class SecureTokenManager
{
    // 定期轮换 Token
    public async Task RotateTokensAsync()
    {
        // 1. 生成新的 Token
        var newToken = await GenerateNewTokenAsync();
        
        // 2. 验证新 Token
        await ValidateTokenAsync(newToken);
        
        // 3. 更新存储
        await UpdateTokenStorageAsync(newToken);
        
        // 4. 废弃旧 Token（如果可能）
        await RevokeOldTokenAsync();
    }
}
```

### 5. 监控和日志

```csharp
// 记录 Token 使用但不记录实际值
logger.LogInformation("Facebook API called with token ending in ...{TokenSuffix}", 
    token.Substring(token.Length - 6));

// 监控异常 Token 使用
if (response.StatusCode == HttpStatusCode.Unauthorized)
{
    logger.LogWarning("Token may be expired or invalid");
    await NotifyAdminAsync();
}
```

---

## 完整的获取流程总结

### 📝 检查清单

完成以下步骤即可获得所有必需的凭据：

#### Facebook Page 部分

- [ ] 1. 创建 Facebook App (获得 App ID 和 App Secret)
- [ ] 2. 配置 Facebook Login 和必要的产品
- [ ] 3. 获取 Facebook Page ID（从 Page 设置）
- [ ] 4. 通过 OAuth 流程获取 User Access Token
- [ ] 5. 转换为长期 User Access Token
- [ ] 6. 使用长期 User Token 获取 Page Access Token
- [ ] 7. 申请必要权限并通过审核

#### Instagram Business 部分

- [ ] 1. 将 Instagram 转换为 Business Account
- [ ] 2. 关联 Instagram 到 Facebook Page
- [ ] 3. 使用 Graph API 获取 Instagram Business Account ID
- [ ] 4. 使用相同的 Page Access Token（无需单独 Token）

#### 最终获得的凭据

```csharp
// ✅ 你应该有这些值
private const string FacebookPageId = "102736478291756";
private const string FacebookAccessToken = "EAABwzLixnjYBAO...";  // Page Access Token
private const string InstagramBusinessAccountId = "17841405309211844";
private const string InstagramAccessToken = "EAABwzLixnjYBAO...";  // 同上，使用 Page Token
```

---

## 🎯 快速测试你的凭据

使用以下代码测试凭据是否正确：

```csharp
public class CredentialTester
{
    private readonly HttpClient _httpClient = new HttpClient();
    
    public async Task TestAllCredentialsAsync(
        string fbPageId,
        string fbToken,
        string igAccountId)
    {
        Console.WriteLine("Testing credentials...\n");
        
        // 测试 Facebook Page
        Console.WriteLine("1. Testing Facebook Page Access...");
        var fbValid = await TestFacebookPageAsync(fbPageId, fbToken);
        Console.WriteLine(fbValid ? "   ✅ Facebook Page: OK" : "   ❌ Facebook Page: FAILED");
        
        // 测试 Instagram Account
        Console.WriteLine("\n2. Testing Instagram Account Access...");
        var igValid = await TestInstagramAccountAsync(igAccountId, fbToken);
        Console.WriteLine(igValid ? "   ✅ Instagram Account: OK" : "   ❌ Instagram Account: FAILED");
        
        // 测试权限
        Console.WriteLine("\n3. Testing Permissions...");
        await TestPermissionsAsync(fbToken);
    }
    
    private async Task<bool> TestFacebookPageAsync(string pageId, string token)
    {
        try
        {
            var url = $"https://graph.facebook.com/v19.0/{pageId}" +
                      $"?fields=id,name,fan_count" +
                      $"&access_token={token}";
            
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                Console.WriteLine($"   Page Name: {result["name"]}");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error: {ex.Message}");
            return false;
        }
    }
    
    private async Task<bool> TestInstagramAccountAsync(string accountId, string token)
    {
        try
        {
            var url = $"https://graph.facebook.com/v19.0/{accountId}" +
                      $"?fields=id,username,name" +
                      $"&access_token={token}";
            
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                Console.WriteLine($"   Instagram Username: @{result["username"]}");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error: {ex.Message}");
            return false;
        }
    }
    
    private async Task TestPermissionsAsync(string token)
    {
        var url = $"https://graph.facebook.com/v19.0/me/permissions" +
                  $"?access_token={token}";
        
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PermissionsResponse>(content);
        
        var requiredPermissions = new[]
        {
            "pages_manage_posts",
            "pages_read_engagement",
            "instagram_basic",
            "instagram_content_publish"
        };
        
        foreach (var perm in requiredPermissions)
        {
            var granted = result.Data.Any(p => 
                p.Permission == perm && p.Status == "granted");
            
            Console.WriteLine(granted 
                ? $"   ✅ {perm}" 
                : $"   ❌ {perm} - NOT GRANTED");
        }
    }
}

public class PermissionsResponse
{
    [JsonPropertyName("data")]
    public List<Permission> Data { get; set; }
}

public class Permission
{
    [JsonPropertyName("permission")]
    public string Permission { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
}
```

---

## 📚 参考资源

- 📖 [Facebook Graph API 文档](https://developers.facebook.com/docs/graph-api)
- 📖 [Facebook Login 文档](https://developers.facebook.com/docs/facebook-login)
- 📖 [Instagram Graph API 文档](https://developers.facebook.com/docs/instagram-api)
- 📖 [Access Token 文档](https://developers.facebook.com/docs/facebook-login/guides/access-tokens)
- 🛠️ [Graph API Explorer](https://developers.facebook.com/tools/explorer/)
- 🛠️ [Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken/)

---

**祝您配置顺利！如有问题，欢迎参考本文档的常见问题部分。** 🎉
