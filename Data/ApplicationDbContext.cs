using AdminPanel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<PersonalizationInfo> PersonalizationInfos { get; set; }
        public DbSet<LogsInfo> LogsInfos { get; set; }
        public DbSet<UserBackUpInfo> UserBackUpInfos { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            _ = builder.HasDefaultSchema("Identity");
            _ = builder.Entity<ApplicationUser>(entity =>
            {
                _ = entity.ToTable(name: "User");
            });
            _ = builder.Entity<ApplicationRole>(entity =>
            {
                _ = entity.ToTable(name: "Role");
            });
            _ = builder.Entity<ApplicationUserRole>(entity =>
            {
                _ = entity.ToTable(name: "UserRoles");
            });
            _ = builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                _ = entity.ToTable("UserClaims");
            });
            _ = builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                _ = entity.ToTable("UserLogins");
            });
            _ = builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                _ = entity.ToTable("RoleClaims");
            });
            _ = builder.Entity<IdentityUserToken<string>>(entity =>
            {
                _ = entity.ToTable("UserTokens");
            });
        }
    }
}