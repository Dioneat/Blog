namespace Blog10.Services
{
    public interface IAdminBootstrapService
    {
        Task EnsureAdminCreatedAsync(CancellationToken cancellationToken = default);
    }
}