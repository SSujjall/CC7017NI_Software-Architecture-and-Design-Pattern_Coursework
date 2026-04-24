using System.Net;
using BuildingBlocks.Models;
using HotelService.Models;
using HotelService.Models.DTOs;
using HotelService.Repositories.Interfaces;
using HotelService.Services.Interfaces;

namespace HotelService.Services;

public class HotelService(
    IHotelRepository _hotelRepo
) : IHotelService
{
    public async Task<ApiResponse<IEnumerable<Hotels>>> GetAllHotels()
    {
        var hotels = await _hotelRepo.GetAllAsync();
        if (hotels.Count() == 0)
        {
            return ApiResponse<IEnumerable<Hotels>>.Success(
                null,
                "There are no hotels in the database.",
                HttpStatusCode.NoContent
            );
        }

        return ApiResponse<IEnumerable<Hotels>>.Success(
            hotels,
            "Hotels listed",
            HttpStatusCode.OK
        );
    }

    public async Task<ApiResponse<Hotels>> GetHotelById(int id)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id);
        return ApiResponse<Hotels>.Success(
            hotel,
            "Hotel found"
        );
    }

    public async Task<ApiResponse<Hotels>> AddHotel(AddHotelDTO dto)
    {
        var hotelModel = new Hotels
        {
            Name = dto.Name,
            Location = dto.Location
        };

        var addHotelResponse = await _hotelRepo.AddAsync(hotelModel);
        await _hotelRepo.SaveChangesAsync();
        return ApiResponse<Hotels>.Success(
            addHotelResponse,
            "Hotel added",
            HttpStatusCode.Created
        );
    }
}