using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rased.Core.Entities;
using Rased.Core.Identity;
using System;

namespace Rased_Project
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public ApplicationDbContext()
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<Ad> Ads => Set<Ad>();

        // 🆕 الجداول الجديدة الخاصة بمشاركة الموقع وأماكن التجمع
        public DbSet<UserLiveLocation> UserLiveLocations => Set<UserLiveLocation>();
        public DbSet<GatheringSpot> GatheringSpots => Set<GatheringSpot>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Complaint>(e =>
            {
                e.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(c => c.SerialNumber).IsUnique();
            });

            // Configure 1-to-1 relationship between ApplicationUser and UserProfile
            builder.Entity<UserProfile>(e =>
            {
                e.HasOne(up => up.User)
                    .WithOne(u => u.UserProfile)
                    .HasForeignKey<UserProfile>(up => up.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(up => up.FullName).IsRequired().HasMaxLength(100);
            });

            // 🆕 إعدادات جدول المواقع الحية للمستخدمين
            builder.Entity<UserLiveLocation>(e =>
            {
                // ربط علاقة الموقع بالمستخدم (لو المستخدم اتحذف، موقعه الحي بيتحذف تلقائياً Cascade)
                e.HasOne(l => l.User)
                    .WithMany()
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}