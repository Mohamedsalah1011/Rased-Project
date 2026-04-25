using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rased.Core.Entities;
using Rased.Core.Identity;

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

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Complaint> Complaints => Set<Complaint>();

        public DbSet<Ad> Ads => Set<Ad>();

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

            // Seed default categories (optional - remove if you manage categories elsewhere)
            builder.Entity<Category>().HasData(
                new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "General Complaint" },
                new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Infrastructure" },
                new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Safety" }
            );
        }
    }
}