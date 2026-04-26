using BookingService.Models;
using BookingService.Models.DTOs;
using BuildingBlocks.Models;

namespace BookingService.Services.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<Bookings>> CreateBooking(string userId, CreateBookingDTO dto);
}