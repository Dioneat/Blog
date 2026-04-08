using Microsoft.AspNetCore.Mvc;

namespace Blog10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpPost("audio")]
        public async Task<IActionResult> UploadAudio(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Файл не выбран");

            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".mp3", ".wav", ".ogg", ".m4a" };
            if (!allowedExtensions.Contains(ext)) return BadRequest("Недопустимый формат аудио");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(_env.WebRootPath, "uploads", "audio", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Url = $"/uploads/audio/{fileName}" });
        }

        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Файл не выбран");

            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".mp4", ".webm", ".mov" };
            if (!allowedExtensions.Contains(ext)) return BadRequest("Недопустимый формат видео");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(_env.WebRootPath, "uploads", "video", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Url = $"/uploads/video/{fileName}" });
        }
        [HttpPost("document")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Файл не выбран");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".pdf") return BadRequest("Разрешены только PDF файлы");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(_env.WebRootPath, "uploads", "documents", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Url = $"/uploads/documents/{fileName}" });
        }
        [HttpPost("image")] 
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Файл не выбран");
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var imageUrl = $"/uploads/images/{uniqueFileName}";

                return Ok(new { Url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
    }
}
