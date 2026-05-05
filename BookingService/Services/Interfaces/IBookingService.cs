using BookingService.Models;
using BookingService.Models.DTOs;
using BuildingBlocks.Models;

namespace BookingService.Services.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<Bookings>> CreateBooking(string userId, CreateBookingDTO dto);
    Task<ApiResponse<IEnumerable<Bookings>>> GetUserBookings(string userId);
    Task<ApiResponse<IEnumerable<Bookings>>> GetAllBookings();
    Task<ApiResponse<Bookings>> ClearBooking(ClearBookingDTO dto);
}