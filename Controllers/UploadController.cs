using Microsoft.AspNetCore.Mvc;

namespace Blog10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private const long MaxImageSize = 5 * 1024 * 1024;     // 5 МБ
        private const long MaxDocumentSize = 10 * 1024 * 1024; // 10 МБ
        private const long MaxAudioSize = 25 * 1024 * 1024;    // 25 МБ
        private const long MaxVideoSize = 100 * 1024 * 1024;   // 100 МБ

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("audio")]
        [RequestSizeLimit(MaxAudioSize + 1024)] 
        public async Task<IActionResult> UploadAudio(IFormFile file)
        {
            return await ProcessUploadAsync(file, "audio", new[] { ".mp3", ".wav", ".ogg", ".m4a" }, MaxAudioSize);
        }

        [HttpPost("video")]
        [RequestSizeLimit(MaxVideoSize + 1024)]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            return await ProcessUploadAsync(file, "video", new[] { ".mp4", ".webm", ".mov" }, MaxVideoSize);
        }

        [HttpPost("document")]
        [RequestSizeLimit(MaxDocumentSize + 1024)]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            return await ProcessUploadAsync(file, "documents", new[] { ".pdf" }, MaxDocumentSize);
        }

        [HttpPost("image")]
        [RequestSizeLimit(MaxImageSize + 1024)]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            return await ProcessUploadAsync(file, "images", new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" }, MaxImageSize);
        }

        private async Task<IActionResult> ProcessUploadAsync(IFormFile file, string folderName, string[] allowedExtensions, long maxSize)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Файл не выбран");

                if (file.Length > maxSize)
                {
                    var maxMb = maxSize / (1024 * 1024);
                    return BadRequest($"Файл слишком большой. Максимальный размер: {maxMb} МБ.");
                }

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                    return BadRequest($"Недопустимый формат файла. Разрешены: {string.Join(", ", allowedExtensions)}");

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);

                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var fileUrl = $"/uploads/{folderName}/{uniqueFileName}";

                return Ok(new { Url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Внутренняя ошибка сервера при сохранении файла.");
            }
        }
    }
}