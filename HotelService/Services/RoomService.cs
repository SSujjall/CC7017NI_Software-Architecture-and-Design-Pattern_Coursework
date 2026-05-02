using System.Net;
using BuildingBlocks.Models;
using HotelService.Models;
using HotelService.Models.DTOs;
using HotelService.Repositories.Interfaces;
using HotelService.Services.Interfaces;

namespace HotelService.Services;

public class RoomService(
    IRoomRepository _roomRepo
) : IRoomService
{
    public async Task<ApiResponse<IEnumerable<Rooms>>> GetAllRoomsOfHotel(int hotelId)
    {
        var rooms = await _roomRepo.FindAllByConditionAsync(x => x.HotelId == hotelId);
        return ApiResponse<IEnumerable<Rooms>>.Success(rooms, "Rooms found");
    }

    public async Task<ApiResponse<Rooms>> GetRoomByHotelAndId(int hotelId, int roomId)
    {
        var room = await _roomRepo.FindSingleByConditionAsync(
            x => x.HotelId == hotelId && x.Id == roomId
        );
        if (room == null)
        {
            return ApiResponse<Rooms>.Failed(
                new Dictionary<string, string> { { "Room", "Room not found" } },
                "Room not found",
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<Rooms>.Success(room, "Room fetched");
    }

    public Task<ApiResponse<Rooms>> AddRoomForHotel(CreateRoomDTO model)
    {
        throw new NotImplementedException();
    }
}