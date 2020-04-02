using UserManagementService.Models.Database;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersistedGrant>().HasKey(x => x.Key);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }
    }
}
