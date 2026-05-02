using BuildingBlocks.Models;
using HotelService.Models;
using HotelService.Models.DTOs;

namespace HotelService.Services.Interfaces;

public interface IRoomService
{
    Task<ApiResponse<IEnumerable<Rooms>>> GetAllRoomsOfHotel(int hotelId);
    Task<ApiResponse<Rooms>> GetRoomByHotelAndId(int hotelId, int roomId);
    Task<ApiResponse<Rooms>> AddRoomForHotel(CreateRoomDTO model);
}