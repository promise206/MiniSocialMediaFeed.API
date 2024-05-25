using Microsoft.EntityFrameworkCore;
using MiniSocialMediaFeed.Domain.Entities;

namespace MiniSocialMediaFeed.Infrastructure
{
    public class MiniSocialMediaFeedDbContext : DbContext
    {
        public MiniSocialMediaFeedDbContext(DbContextOptions<MiniSocialMediaFeedDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Follow>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.NoAction); // Disable cascade delete for the Follower relationship

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Followee)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.NoAction); // Disable cascade delete for the Followee relationship
        }
    }
}
