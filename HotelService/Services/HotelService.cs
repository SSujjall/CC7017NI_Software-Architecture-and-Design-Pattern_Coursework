using System.Net;
using BuildingBlocks.Cache;
using BuildingBlocks.Models;
using HotelService.Models;
using HotelService.Models.DTOs;
using HotelService.Repositories.Interfaces;
using HotelService.Services.Interfaces;

namespace HotelService.Services;

public class HotelService(
    IHotelRepository _hotelRepo,
    ICacheService _cache
) : IHotelService
{
    private const string AllHotelsCacheKey = "hotels:all";

    public async Task<ApiResponse<IEnumerable<Hotels>>> GetAllHotels()
    {
        var cached = await _cache.GetAsync<IEnumerable<Hotels>>(AllHotelsCacheKey);
        if (cached != null)
            return ApiResponse<IEnumerable<Hotels>>.Success(cached, "Hotels listed", HttpStatusCode.OK);

        var hotels = await _hotelRepo.GetAllAsync();
        if (!hotels.Any())
            return ApiResponse<IEnumerable<Hotels>>.Success(null, "There are no hotels in the database.", HttpStatusCode.NoContent);

        await _cache.SetAsync(AllHotelsCacheKey, hotels, TimeSpan.FromMinutes(15));
        return ApiResponse<IEnumerable<Hotels>>.Success(hotels, "Hotels listed");
    }

    public async Task<ApiResponse<Hotels>> GetHotelById(int id)
    {
        var cacheKey = $"hotels:{id}";
        var cached = await _cache.GetAsync<Hotels>(cacheKey);
        if (cached != null)
            return ApiResponse<Hotels>.Success(cached, "Hotel found");

        var hotel = await _hotelRepo.GetByIdAsync(id);
        if (hotel != null)
            await _cache.SetAsync(cacheKey, hotel, TimeSpan.FromMinutes(15));

        return ApiResponse<Hotels>.Success(hotel, "Hotel found");
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

        await _cache.RemoveAsync(AllHotelsCacheKey);

        return ApiResponse<Hotels>.Success(addHotelResponse, "Hotel added", HttpStatusCode.Created);
    }
}