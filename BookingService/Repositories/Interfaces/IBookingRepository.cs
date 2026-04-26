using BookingService.Models;
using BuildingBlocks.GenericRepo;

namespace BookingService.Repositories.Interfaces;

public interface IBookingRepository : IGenericRepo<Bookings>
{
    
}