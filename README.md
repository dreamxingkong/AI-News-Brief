# AI 科技简报

每日自动生成 HTML 新闻简报，包含政策方向、学习资源、国际局势、天气四个板块。

## 🔒 安全说明（重要）

**本项目 API 密钥完全安全分离：**
- ❌ 代码中**不包含**任何硬编码密钥
- ✅ 密钥通过 **GitHub Secrets** 安全存储
- ✅ 仅从环境变量读取，不写入任何文件
- ✅ 密钥泄露风险 = 0

## 特性

- 🎨 薄荷奶青毛玻璃主题 + 8 套配色一键切换
- 🎬 CSS 3D 流畅动画（卡片悬停翻转 + 入场动画）
- 🤖 DeepSeek AI 驱动内容生成
- ⏰ 每天北京时间 8:00 自动更新
- 📱 响应式设计，手机/电脑完美适配
- 🔒 API 密钥完全安全分离

## 部署步骤

### 1. Fork 或下载本仓库

### 2. 添加 DeepSeek API Key

仓库 Settings → Secrets and variables → Actions → New repository secret
- **Name**: `DEEPSEEK_API_KEY`
- **Secret**: 你的 DeepSeek API 密钥

> ⚠️ 密钥仅存储在 GitHub Secrets 中，代码里**没有任何硬编码密钥**。

### 3. 启用 GitHub Pages

Settings → Pages → Source 选择 `Deploy from a branch` → `main` 分支根目录 → Save

### 4. 手动触发测试

Actions → "Daily News Update" → Run workflow

等待 2-3 分钟，访问 `https://你的用户名.github.io/AI-News-Brief/`

### 5. 之后每天自动更新

无需任何操作，每天北京时间 8:00 自动运行。

## 成本

| 项目 | 费用 |
|------|------|
| DeepSeek API | 约 0.03 元/次，每月约 0.9 元 |
| GitHub 托管 | 完全免费 |

## 安全措施

| 措施 | 状态 |
|------|------|
| API Key 存储在 GitHub Secrets | ✅ 已实施 |
| 代码中无硬编码密钥 | ✅ 已实施 |
| 启动时检查密钥是否存在 | ✅ 已实施 |
| 日志不打印完整密钥 | ✅ 已实施 |
| DeepSeek 平台设置 1 元停用 | ⚠️ 请自行设置 |

## License

本项目采用 **CC BY-NC-SA 4.0 国际许可证**。