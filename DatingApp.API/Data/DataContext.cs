using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>().HasKey(k => new { k.LikerId, k.LikeeId });

            modelBuilder.Entity<Like>().HasOne(l => l.Liker).WithMany(l => l.Likees).HasForeignKey(l => l.LikerId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Like>().HasOne(l => l.Likee).WithMany(l => l.Likers).HasForeignKey(l => l.LikeeId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>().HasOne(l => l.Sender).WithMany(l => l.MessagesSent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().HasOne(l => l.Recipient).WithMany(l => l.MessagesReceived).OnDelete(DeleteBehavior.Restrict);

        }
    }
}