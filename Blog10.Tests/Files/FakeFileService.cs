using Blog10.Services.Interfaces;

namespace Blog10.Tests.Fakes;

public sealed class FakeFileService : IFileService
{
    public List<string> DeletedFiles { get; } = new();

    public int GarbageCollectorRunCount { get; private set; }

    public void DeleteFile(string? relativeUrl)
    {
        if (!string.IsNullOrWhiteSpace(relativeUrl))
        {
            DeletedFiles.Add(relativeUrl);
        }
    }

    public Task RunGarbageCollectorAsync(CancellationToken cancellationToken = default)
    {
        GarbageCollectorRunCount++;
        return Task.CompletedTask;
    }

    public void RunGarbageCollectorInBackground()
    {
        GarbageCollectorRunCount++;
    }
}