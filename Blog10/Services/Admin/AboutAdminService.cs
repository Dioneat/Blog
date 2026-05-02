using Blog10.Models;
using Blog10.Services.Interfaces;
using System.Text.Json;

namespace Blog10.Services.Admin
{
    public interface IAboutAdminService
    {
        Task SaveAsync(
            AboutData model,
            List<AboutBlock> blocks,
            CancellationToken cancellationToken = default);

        void ReplaceHeroImage(AboutData model, string newUrl);
        void ReplaceProfileImage(AboutData model, string newUrl);
    }

    public sealed class AboutAdminService : IAboutAdminService
    {
        private readonly IAboutService _aboutService;
        private readonly IFileService _fileService;

        public AboutAdminService(
            IAboutService aboutService,
            IFileService fileService)
        {
            _aboutService = aboutService;
            _fileService = fileService;
        }

        public async Task SaveAsync(
            AboutData model,
            List<AboutBlock> blocks,
            CancellationToken cancellationToken = default)
        {
            model.AboutBlocksJson = JsonSerializer.Serialize(blocks);

            await _aboutService.UpdateAboutDataAsync(model);

            _fileService.RunGarbageCollectorInBackground();
        }

        public void ReplaceHeroImage(AboutData model, string newUrl)
        {
            _fileService.DeleteFile(model.MainHeroImageUrl);
            model.MainHeroImageUrl = newUrl;
        }

        public void ReplaceProfileImage(AboutData model, string newUrl)
        {
            _fileService.DeleteFile(model.ProfileImageUrl);
            model.ProfileImageUrl = newUrl;
        }
    }
}