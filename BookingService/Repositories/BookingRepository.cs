using BookingService.Data;
using BookingService.Models;
using BookingService.Repositories.Interfaces;
using BuildingBlocks.GenericRepo;

namespace BookingService.Repositories;

public class BookingRepository : GenericRepo<Bookings>, IBookingRepository
{
    public BookingRepository(BookingDbContext context) : base(context)
    {
    }
}