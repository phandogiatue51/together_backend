using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Together.Models
{
    public class TogetherDbContext : DbContext
    {
        public TogetherDbContext(DbContextOptions<TogetherDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Staff> Staff { get; set; }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<VolunteerApplication> VolunteerApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Staff>()
                .HasIndex(os => new { os.OrganizationId, os.AccountId })
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Organization)
                .WithMany(o => o.Projects)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Account)
                .WithMany(a => a.Certificates)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectCategory>()
                .HasIndex(pc => new { pc.ProjectId, pc.CategoryId })
                .IsUnique();

            modelBuilder.Entity<VolunteerApplication>()
                .HasIndex(va => new { va.ProjectId, va.VolunteerId })
                .IsUnique();

            modelBuilder.Entity<Account>()
                .Property(a => a.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Account>()
                .Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            modelBuilder.Entity<Staff>()
                .Property(s => s.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Project>()
                .Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Project>()
                .Property(p => p.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Certificate>()
                .Property(c => c.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<VolunteerApplication>()
                .Property(va => va.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Organization>()
                .Property(o => o.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Certificate>()
               .HasOne(c => c.Category)
               .WithMany()
               .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProjectCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VolunteerApplication>()
                .HasMany(va => va.SelectedCertificates)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ApplicationCertificate",
                    j => j.HasOne<Certificate>().WithMany().HasForeignKey("CertificateId"),
                    j => j.HasOne<VolunteerApplication>().WithMany().HasForeignKey("ApplicationId")
                );

        }
    }
}