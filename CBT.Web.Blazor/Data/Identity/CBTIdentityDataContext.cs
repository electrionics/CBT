using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MedExpert.Domain.Identity;

namespace CBT.Web.Blazor.Data.Identity;

public class CBTIdentityDataContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public CBTIdentityDataContext(DbContextOptions<CBTIdentityDataContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region Identity

        builder.Entity<User>().ToTable("_AspNetUser", "dbo");
        builder.Entity<Role>().ToTable("_AspNetRole", "dbo");
        builder.Entity<RoleClaim>().ToTable("_AspNetRoleClaim", "dbo");
        builder.Entity<UserClaim>().ToTable("_AspNetUserClaim", "dbo");
        builder.Entity<UserRole>().ToTable("_AspNetUserRole", "dbo");
        builder.Entity<UserLogin>().ToTable("_AspNetUserLogin", "dbo");
        builder.Entity<UserToken>().ToTable("_AspNetUserToken", "dbo");

        #endregion
    }
}
