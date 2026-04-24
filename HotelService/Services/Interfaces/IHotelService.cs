using BuildingBlocks.Models;
using HotelService.Models;
using HotelService.Models.DTOs;

namespace HotelService.Services.Interfaces;

public interface IHotelService
{
    Task<ApiResponse<IEnumerable<Hotels>>> GetAllHotels();
    Task<ApiResponse<Hotels>> GetHotelById(int id);
    Task<ApiResponse<Hotels>> AddHotel(AddHotelDTO dto);
}