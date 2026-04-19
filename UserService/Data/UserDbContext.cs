using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Helpers;
using UserService.Models;

namespace UserService.Data;

public class UserDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Seed Data
        DataSeeder.SeedData(builder);
    }
    
    public DbSet<Users> Users { get; set; }
}