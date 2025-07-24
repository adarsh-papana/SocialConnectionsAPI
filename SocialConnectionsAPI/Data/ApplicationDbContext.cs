using Microsoft.EntityFrameworkCore;
using SocialConnectionsAPI.Models;

namespace SocialConnectionsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets represent tables in our database
        public DbSet<User> Users { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique index for UserStrId in User table
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserStrId)
                .IsUnique();

            // Configure unique composite index for Connection table
            // This enforces the rule: (User1StrId, User2StrId) must be unique
            modelBuilder.Entity<Connection>()
                .HasIndex(c => new { c.User1StrId, c.User2StrId })
                .IsUnique();

            // Optional: Define foreign key relationships if navigation properties were used
            // modelBuilder.Entity<Connection>()
            //     .HasOne<User>()
            //     .WithMany()
            //     .HasForeignKey(c => c.User1StrId)
            //     .HasPrincipalKey(u => u.UserStrId)
            //     .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // modelBuilder.Entity<Connection>()
            //     .HasOne<User>()
            //     .WithMany()
            //     .HasForeignKey(c => c.User2StrId)
            //     .HasPrincipalKey(u => u.UserStrId)
            //     .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
        }
    }
}
