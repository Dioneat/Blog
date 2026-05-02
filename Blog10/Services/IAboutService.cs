using Blog10.Models;

namespace Blog10.Services
{
    public interface IAboutService
    {
        Task<AboutData> GetAboutDataAsync();
        Task UpdateAboutDataAsync(AboutData data);
    }
}