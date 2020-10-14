using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();
                userRole.HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();
            });
            modelBuilder.Entity<Like>().HasKey(k => new { k.LikerId, k.LikeeId });

            modelBuilder.Entity<Like>().HasOne(l => l.Liker).WithMany(l => l.Likees).HasForeignKey(l => l.LikerId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Like>().HasOne(l => l.Likee).WithMany(l => l.Likers).HasForeignKey(l => l.LikeeId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>().HasOne(l => l.Sender).WithMany(l => l.MessagesSent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().HasOne(l => l.Recipient).WithMany(l => l.MessagesReceived).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);

        }
    }
}