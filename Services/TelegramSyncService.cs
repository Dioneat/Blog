using System.Text.Json;

namespace Blog10.Services
{
    public class TelegramSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly SettingsService _settings;
        private readonly AppLogService _logService; 

        public TelegramSyncService(HttpClient httpClient, SettingsService settings, AppLogService logService)
        {
            _httpClient = httpClient;
            _settings = settings;
            _logService = logService;
        }

        public async Task<(bool Success, string Message)> PublishToTelegramAsync(string title, string shortDescription, string articleUrl)
        {
            var token = await _settings.GetSettingAsync("TgBotToken");
            var chatId = await _settings.GetSettingAsync("TgChannelId");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(chatId))
            {
                await _logService.LogWarningAsync("Telegram API", "Попытка публикации отменена: не настроены ключи в админке.");
                return (false, "Токен бота или ID канала Telegram не настроены.");
            }

            string text = $"<b>{title}</b>\n\n{shortDescription}\n\n👉 <a href=\"{articleUrl}\">Читать полную статью на сайте</a>";

            var payload = new
            {
                chat_id = chatId,
                text = text,
                parse_mode = "HTML",
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"https://api.telegram.org/bot{token}/sendMessage", payload);

                if (response.IsSuccessStatusCode)
                {
                    await _logService.LogInfoAsync("Telegram API", $"Статья '{title}' успешно опубликована в канале.");
                    return (true, "Статья успешно опубликована в Telegram!");
                }

                var errorJson = await response.Content.ReadAsStringAsync();

                await _logService.LogErrorAsync("Telegram API", $"Ошибка ответа от серверов Telegram: {errorJson}");
                return (false, $"Ошибка Telegram: {errorJson}");
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Telegram API", "Сбой сети при попытке отправить сообщение в Telegram.", ex);
                return (false, $"Системная ошибка отправки: {ex.Message}");
            }
        }
    }
}