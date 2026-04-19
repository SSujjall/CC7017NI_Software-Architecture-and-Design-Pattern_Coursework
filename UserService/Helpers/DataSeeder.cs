using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Enums;
using UserService.Models;

namespace UserService.Helpers;

public class DataSeeder
{
    public static void SeedData(ModelBuilder builder)
    {
        var userRoleId = Guid.NewGuid().ToString();

        // seed user role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = UserRoles.User.ToString(),
            NormalizedName = UserRoles.User.ToString().ToUpper(),
            Id = userRoleId,
            ConcurrencyStamp = userRoleId
        });
        
        var superadminUserId = Guid.NewGuid().ToString();
        var superadminRoleId = Guid.NewGuid().ToString();
        var adminRoleId = Guid.NewGuid().ToString();
        
        // seed superadmin role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = UserRoles.Superadmin.ToString(),
            NormalizedName = UserRoles.Superadmin.ToString().ToUpper(),
            Id = superadminRoleId,
            ConcurrencyStamp = superadminRoleId
        });

        // seed admin role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = UserRoles.Admin.ToString(),
            NormalizedName = UserRoles.Admin.ToString().ToUpper(),
            Id = adminRoleId,
            ConcurrencyStamp = adminRoleId
        });
        
        // create new superadmin 
        var adminUser = new Users()
        {
            Id = superadminUserId,
            Email = "superadmin@blog.com",
            UserName = "Superadmin",
            NormalizedEmail = "SUPERADMIN@BLOG.COM",
            NormalizedUserName = "SUPERADMIN",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
        };
        
        var passwordHash = new PasswordHasher<Users>();
        const string password = "Superadmin@123";

        adminUser.PasswordHash = passwordHash.HashPassword(adminUser, password);

        builder.Entity<Users>().HasData(adminUser);

        // set the superadmin role to the superadmin user
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = superadminRoleId,
            UserId = superadminUserId
        });
    }
}