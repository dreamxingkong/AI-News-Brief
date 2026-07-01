using System;
using System.Text;
using System.Collections.Generic;

public static class TemplateEngine
{
    public static string GenerateCard(string emoji, string tag, string title, string content, string source = "")
    {
        var sb = new StringBuilder();
        sb.AppendLine("<div class=\"card\">");
        if (!string.IsNullOrEmpty(emoji))
            sb.AppendLine($"    <span class=\"emoji-icon\">{emoji}</span>");
        if (!string.IsNullOrEmpty(tag))
            sb.AppendLine($"    <span class=\"tag\">{tag}</span>");
        sb.AppendLine($"    <h3>{title}</h3>");
        sb.AppendLine($"    <p>{content}</p>");
        if (!string.IsNullOrEmpty(source))
        {
            sb.AppendLine("    <div class=\"meta\">");
            sb.AppendLine($"        <span class=\"source\">📌 {source}</span>");
            sb.AppendLine("    </div>");
        }
        sb.AppendLine("</div>");
        return sb.ToString();
    }

    public static string GenerateCountryCard(string country, string flag, List<string> newsItems)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<div class=\"country-card\">");
        sb.AppendLine($"    <div class=\"flag\">{flag} {country} <span>📍 最新动态</span></div>");
        sb.AppendLine("    <ul>");
        foreach (var item in newsItems)
        {
            sb.AppendLine($"        <li>• {item}</li>");
        }
        sb.AppendLine("    </ul>");
        sb.AppendLine("</div>");
        return sb.ToString();
    }
}