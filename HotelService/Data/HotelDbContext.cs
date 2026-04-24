using HotelService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed default hotels and respective rooms
        modelBuilder.Entity<Hotels>().HasData(
            new Hotels
            {
                Id = 1,
                Name = "Mount Hotel Fuji",
                Location = "Kathmandu",
            }
        );

        modelBuilder.Entity<Rooms>().HasData(
            new Rooms { Id = 1, HotelId = 1, RoomNumber = "101", Price = 1000 },
            new Rooms { Id = 2, HotelId = 1, RoomNumber = "102", Price = 1500 }
        );
    }

    public DbSet<Hotels> Hotels { get; set; }
    public DbSet<Rooms> Rooms { get; set; }
}