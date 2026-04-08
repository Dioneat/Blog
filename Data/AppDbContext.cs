using Blog10.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext()
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=blog_database.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            modelBuilder.Entity<AdminAccount>().HasData(
                new AdminAccount { Id = 1, Username = "admin", Password = "password123" }
            );
        }
    }
}
