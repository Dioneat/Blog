namespace Blog10.Services.Interfaces
{
    public interface IFileService
    {
        void DeleteFile(string? relativeUrl);

        Task RunGarbageCollectorAsync(CancellationToken cancellationToken = default);

        void RunGarbageCollectorInBackground();
    }
}