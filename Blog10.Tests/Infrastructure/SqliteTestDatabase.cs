using Blog10.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Tests.Infrastructure;

public sealed class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public DbContextOptions<AppDbContext> Options { get; }

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var dbContext = CreateDbContext();
        dbContext.Database.EnsureCreated();
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(Options);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}