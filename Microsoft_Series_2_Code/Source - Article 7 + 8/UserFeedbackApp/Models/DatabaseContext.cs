using Microsoft.EntityFrameworkCore;

namespace UserFeedbackApp.Models
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Review> Reviews => Set<Review>();
    }
}