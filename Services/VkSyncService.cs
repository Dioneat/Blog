using Blog10.Data;
using Blog10.Models;
using Blog10.Utils;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Blog10.Services
{
    public class VkSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly SettingsService _settings;
        private readonly AppLogService _logService; 

        public VkSyncService(HttpClient httpClient, AppDbContext dbContext, SettingsService settings, AppLogService logService)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _settings = settings;
            _logService = logService;
        }

        public async Task<(int Imported, string Message)> SyncPostsAsync(int count = 5)
        {
            var token = await _settings.GetSettingAsync("VkServiceToken");
            var domain = await _settings.GetSettingAsync("VkDomain");
            var version = "5.131";

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(domain))
            {
                await _logService.LogWarningAsync("VK Sync", "Синхронизация отменена: не настроены сервисный токен или домен.");
                return (0, "Ключи ВК (Сервисный токен или Домен) не настроены в параметрах.");
            }

            string url = $"https://api.vk.com/method/wall.get?domain={domain}&count={count}&v={version}&access_token={token}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                {
                    string errMsg = errorProp.GetProperty("error_msg").GetString() ?? "Неизвестная ошибка";
                    await _logService.LogErrorAsync("VK Sync", $"Ошибка API ВКонтакте при загрузке стены: {errMsg}");
                    return (0, $"Ошибка ВК API: {errMsg}");
                }

                var items = doc.RootElement.GetProperty("response").GetProperty("items");
                int importedCount = 0;

                foreach (var item in items.EnumerateArray())
                {
                    long vkId = item.GetProperty("id").GetInt64();

                    bool exists = await _dbContext.Articles.AnyAsync(a => a.VkPostId == vkId);
                    if (exists) continue;

                    string text = item.GetProperty("text").GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(text)) continue;

                    string title = GenerateTitleFromText(text);
                    string? imageUrl = GetLargestPhotoUrl(item);

                    var newPost = new BlogPost
                    {
                        VkPostId = vkId,
                        Title = title,
                        Slug = SlugGenerator.Generate(title),
                        ShortDescription = text.Length > 150 ? text.Substring(0, 147) + "..." : text,
                        Content = FormatVkTextToHtml(text),
                        CoverImageUrl = imageUrl,
                        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("date").GetInt64()).LocalDateTime,
                        IsDraft = true
                    };

                    _dbContext.Articles.Add(newPost);
                    importedCount++;
                }

                await _dbContext.SaveChangesAsync();

                await _logService.LogInfoAsync("VK Sync", $"Синхронизация завершена. Загружено новых постов: {importedCount}");
                return (importedCount, $"Загружено новых постов: {importedCount}. Проверьте их в черновиках.");
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("VK Sync", "Системный сбой при попытке загрузить посты из ВКонтакте", ex);
                return (0, $"Системная ошибка: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> PublishToVkAsync(string title, string shortDescription, string articleUrl)
        {
            var token = await _settings.GetSettingAsync("VkUserToken");
            var groupId = await _settings.GetSettingAsync("VkGroupId");
            var version = "5.131";

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(groupId))
            {
                await _logService.LogWarningAsync("VK API", "Публикация отменена: не настроен токен пользователя или ID группы.");
                return (false, "Токен пользователя или ID группы не настроены.");
            }

            string message = $"{title}\n\n{shortDescription}\n\nЧитать полную статью с интерактивами на сайте 👇";

            var parameters = new Dictionary<string, string>
            {
                { "owner_id", $"-{groupId}" },
                { "from_group", "1" },
                { "message", message },
                { "access_token", token },
                { "v", version }
            };

            if (!articleUrl.Contains("localhost"))
            {
                parameters.Add("attachments", articleUrl);
            }
            else
            {
                parameters["message"] += $"\n{articleUrl}";
            }

            try
            {
                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync("https://api.vk.com/method/wall.post", content);
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                {
                    string errMsg = errorProp.GetProperty("error_msg").GetString() ?? "Неизвестная ошибка";
                    await _logService.LogErrorAsync("VK API", $"ВКонтакте отклонил публикацию статьи '{title}': {errMsg}");
                    return (false, $"Ошибка ВК: {errMsg}");
                }

                await _logService.LogInfoAsync("VK API", $"Статья '{title}' успешно опубликована в группе ВКонтакте.");
                return (true, "Опубликовано ВКонтакте!");
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("VK API", "Ошибка сети при отправке запроса на публикацию ВКонтакте", ex);
                return (false, $"Ошибка сети: {ex.Message}");
            }
        }

        private string GenerateTitleFromText(string text)
        {
            var firstLine = text.Split(new[] { '\r', '\n', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(firstLine)) return "Новая статья";
            return firstLine.Length > 60 ? firstLine.Substring(0, 57) + "..." : firstLine.Trim();
        }

        private string FormatVkTextToHtml(string text)
        {
            var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("", paragraphs.Select(p => $"<p>{p.Replace("\n", "<br/>")}</p>"));
        }

        private string? GetLargestPhotoUrl(JsonElement item)
        {
            if (!item.TryGetProperty("attachments", out var attachments)) return null;
            foreach (var att in attachments.EnumerateArray())
            {
                if (att.GetProperty("type").GetString() == "photo")
                {
                    var sizes = att.GetProperty("photo").GetProperty("sizes").EnumerateArray();
                    var largest = sizes.OrderByDescending(s => s.GetProperty("width").GetInt32()).FirstOrDefault();
                    return largest.GetProperty("url").GetString();
                }
            }
            return null;
        }
    }
}