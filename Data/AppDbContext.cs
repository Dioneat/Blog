using Blog10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Blog10.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<FlashcardSet> FlashcardSets { get; set; }
        public DbSet<AboutData> AboutPage { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> Articles { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<AppSetting> Settings { get; set; }
        public DbSet<AdminAccount> AdminAccounts { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tagsComparer = new ValueComparer<List<string>>(
                (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()),
                c => c == null
                    ? 0
                    : c.Aggregate(0, (a, v) => HashCode.Combine(a, v == null ? 0 : v.GetHashCode())),
                c => c == null ? new List<string>() : c.ToList());

            modelBuilder.Entity<BlogPost>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => string.IsNullOrWhiteSpace(v)
                        ? new List<string>()
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                    tagsComparer
                );

            modelBuilder.Entity<BlogPost>()
                .HasIndex(a => a.Slug)
                .IsUnique();

            modelBuilder.Entity<BlogPost>()
                .HasIndex(a => new { a.IsDraft, a.CreatedAt });

            modelBuilder.Entity<AppSetting>()
                .HasIndex(s => s.Key)
                .IsUnique();

            modelBuilder.Entity<AdminAccount>().HasData(
                new AdminAccount
                {
                    Id = 1,
                    Username = "admin",
                    Password = "password1234"
                }
            );
        }
    }
}