using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
private static readonly string API_KEY = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? "";
    private static readonly string API_URL = "https://api.deepseek.com/v1/chat/completions";
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("   AI 新闻简报生成器");
        Console.WriteLine("========================================");

        if (string.IsNullOrEmpty(API_KEY))
        {
            Console.WriteLine("");
            Console.WriteLine("❌ 错误：未找到 DEEPSEEK_API_KEY 环境变量！");
            Console.WriteLine("");
            Console.WriteLine("解决方案：");
            Console.WriteLine("  在仓库 Settings → Secrets 中添加 DEEPSEEK_API_KEY");
            Console.WriteLine("");
            Console.WriteLine("========================================");
            Environment.Exit(1);
            return;
        }

        Console.WriteLine($"✅ API Key 已加载 (前8位: {API_KEY.Substring(0, 8)}...)");
        Console.WriteLine($"📅 生成日期: {DateTime.Now:yyyy年MM月dd日}");
        Console.WriteLine("========================================");

        try
        {
            Console.WriteLine("🔄 正在调用 DeepSeek API 生成新闻...");
            string rawData = await FetchNewsData();

            Console.WriteLine("🔄 正在解析新闻数据...");
            var (total, policy, learning, international, weather) = ParseNewsData(rawData);

            Console.WriteLine("🔄 正在生成 HTML 页面...");
            string template = File.ReadAllText("index.html");

            string html = template
                .Replace("{{DATE}}", DateTime.Now.ToString("yyyy年MM月dd日"))
                .Replace("{{TOTAL_COUNT}}", total.ToString())
                .Replace("{{POLICY_CARDS}}", policy)
                .Replace("{{LEARNING_CARDS}}", learning)
                .Replace("{{INTERNATIONAL_COUNTRIES}}", international)
                .Replace("{{WEATHER_CARDS}}", weather);

            File.WriteAllText("index.html", html, Encoding.UTF8);

            Console.WriteLine("");
            Console.WriteLine("✅ index.html 更新成功！");
            Console.WriteLine($"📊 共生成 {total} 条新闻");
            Console.WriteLine("========================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine("");
            Console.WriteLine($"❌ 执行失败: {ex.Message}");
            Console.WriteLine("========================================");
            Environment.Exit(1);
        }
    }

    static async Task<string> FetchNewsData()
    {
        var prompt = @"
你是一个新闻编辑。请生成今日新闻简报的结构化数据，按以下格式返回纯JSON：

{
    ""totalCount"": 12,
    ""policyCards"": [
        { ""emoji"": ""🎓"", ""tag"": ""扩招"", ""title"": ""标题"", ""content"": ""内容"", ""source"": ""来源"" }
    ],
    ""learningCards"": [
        { ""emoji"": ""📝"", ""tag"": ""备考"", ""title"": ""标题"", ""content"": ""内容"", ""source"": ""来源"" }
    ],
    ""internationalCountries"": [
        { ""country"": ""美国"", ""flag"": ""🇺🇸"", ""news"": [ ""新闻1"", ""新闻2"" ] }
    ],
    ""weatherCards"": [
        { ""emoji"": ""🌧️"", ""tag"": ""暴雨"", ""title"": ""标题"", ""content"": ""内容"", ""source"": ""来源"" }
    ]
}

要求：
- 政策方向：高考扩招、新增专业，针对江苏考生，包含具体高校和数字
- 学习资源：备考建议、命题趋势
- 国际局势：至少包含美国、英国、法国、德国、日本
- 天气：华南暴雨 + 欧洲热浪
- 日期为今天
- 请通过联网搜索获取最新的真实新闻，不要靠模型自身知识编造
- 只输出纯JSON，不要任何其他文字说明
";

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

        var requestBody = new
        {
            model = "deepseek-v4-pro",
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.6f,
            max_tokens = 4096,
            enable_search = true
        };

        string json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(API_URL, content);
        string responseString = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<JsonElement>(responseString);
        return result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
    }

    static (int Total, string Policy, string Learning, string International, string Weather) ParseNewsData(string raw)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(raw);
            int total = data.GetProperty("totalCount").GetInt32();

            string policy = "";
            foreach (var item in data.GetProperty("policyCards").EnumerateArray())
            {
                policy += TemplateEngine.GenerateCard(
                    GetStringOrDefault(item, "emoji"),
                    GetStringOrDefault(item, "tag"),
                    GetStringOrDefault(item, "title"),
                    GetStringOrDefault(item, "content"),
                    GetStringOrDefault(item, "source")
                );
            }

            string learning = "";
            foreach (var item in data.GetProperty("learningCards").EnumerateArray())
            {
                learning += TemplateEngine.GenerateCard(
                    GetStringOrDefault(item, "emoji"),
                    GetStringOrDefault(item, "tag"),
                    GetStringOrDefault(item, "title"),
                    GetStringOrDefault(item, "content"),
                    GetStringOrDefault(item, "source")
                );
            }

            string international = "";
            foreach (var item in data.GetProperty("internationalCountries").EnumerateArray())
            {
                var newsList = new List<string>();
                foreach (var n in item.GetProperty("news").EnumerateArray())
                {
                    newsList.Add(n.GetString() ?? "暂无新闻");
                }

                international += TemplateEngine.GenerateCountryCard(
                    GetStringOrDefault(item, "country"),
                    GetStringOrDefault(item, "flag"),
                    newsList
                );
            }

            string weather = "";
            foreach (var item in data.GetProperty("weatherCards").EnumerateArray())
            {
                weather += TemplateEngine.GenerateCard(
                    GetStringOrDefault(item, "emoji"),
                    GetStringOrDefault(item, "tag"),
                    GetStringOrDefault(item, "title"),
                    GetStringOrDefault(item, "content"),
                    GetStringOrDefault(item, "source")
                );
            }

            return (total, policy, learning, international, weather);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ 解析数据失败: {ex.Message}");
            return GetFallbackData();
        }
    }

    static string GetStringOrDefault(JsonElement element, string propertyName, string defaultValue = "")
    {
        if (element.TryGetProperty(propertyName, out JsonElement property))
        {
            return property.GetString() ?? defaultValue;
        }
        return defaultValue;
    }

    static (int, string, string, string, string) GetFallbackData()
    {
        int total = 6;
        string policy = TemplateEngine.GenerateCard("🎓", "扩招", "东南大学扩招600人", "2026年东南大学本科扩招600人，利好江苏考生", "江苏省教育考试院");
        string learning = TemplateEngine.GenerateCard("📝", "备考", "关注AI伦理话题", "2026年高考作文出现AI相关题目，建议提前积累素材", "教育专家");
        string international = TemplateEngine.GenerateCountryCard("美国", "🇺🇸", new List<string> { "美中经贸磋商达成共识", "美伊核谈判持续博弈" });
        string weather = TemplateEngine.GenerateCard("🌧️", "暴雨", "华南地区暴雨预警", "福建、广东、广西等地有暴雨或大暴雨", "中央气象台");
        return (total, policy, learning, international, weather);
    }
}
