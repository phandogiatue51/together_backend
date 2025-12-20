using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormQuestion> FormQuestions { get; set; }
        public DbSet<FormResponse> FormResponses { get; set; }
        public DbSet<FormAnswer> FormAnswers { get; set; }
        public DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Staff>()
                .HasIndex(os => new { os.OrganizationId, os.AccountId })
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Organization)
                .WithMany(o => o.Projects)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Form>()
                .HasOne(f => f.Project)
                .WithMany(p => p.Forms)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
