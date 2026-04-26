using System.Net;
using BookingService.Models;
using BookingService.Models.DTOs;
using BookingService.Repositories.Interfaces;
using BookingService.Services.Interfaces;
using BuildingBlocks.Events;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using MassTransit;
using UserService.Helpers;

namespace BookingService.Services;

public class BookingService(
    IBookingRepository _bookingRepo,
    IPublishEndpoint _publishEndpoint,
    IHttpClientFactory _httpClientFactory,
    IHttpContextAccessor _httpContextAccessor
) : IBookingService
{
    public async Task<ApiResponse<Bookings>> CreateBooking(string userId, CreateBookingDTO dto)
    {
        var existingRoom = await GetExistingRooms(dto.HotelId, dto.RoomId);
        if (!existingRoom.IsAvailable)
        {
            return ApiResponse<Bookings>.Failed(
                new Dictionary<string, string> { { "Room", "Room not available" } },
                "Booking failed",
                HttpStatusCode.Conflict
            );
        }
        var existingBooking = await _bookingRepo.FindAllByConditionAsync(
            x => x.HotelId == dto.HotelId &&
                 x.RoomId == dto.RoomId &&
                 (x.Status == BookingStatus.Pending || x.Status == BookingStatus.Confirmed)
        );
        if (existingBooking.Any())
        {
            return ApiResponse<Bookings>.Failed(
                new Dictionary<string, string> { { "Room", "Room already booked" } },
                "Booking failed",
                HttpStatusCode.Conflict
            );
        }

        var bookingModel = new Bookings
        {
            UserId = userId,
            HotelId = dto.HotelId,
            RoomId = dto.RoomId,
            Amount = existingRoom.Price,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now
        };

        var createdBooking = await _bookingRepo.AddAsync(bookingModel);
        await _bookingRepo.SaveChangesAsync();

        var superAdminUserId = await GetSuperAdminId();

        await _publishEndpoint.Publish(new BookingCreatedEvent
        {
            BookingId = createdBooking.Id,
            UserId = userId,
            Amount = existingRoom.Price,
            OwnerUserId = superAdminUserId
        });

        return ApiResponse<Bookings>.Success(createdBooking, "Booking created, awaiting payment", HttpStatusCode.Created);
    }

    #region Helper private methods
    private async Task<ExistingRoomDTO> GetExistingRooms(int hotelId, int roomId)
    {
        var client = _httpClientFactory.CreateClient("RoomService");
        ForwardAuthHeader(client);

        var response = await client.GetAsync($"get-by-hotel-room/{hotelId}/{roomId}");

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                throw new ServiceException(
                    new Dictionary<string, string> { { "HotelService", "Hotel service unavailable" } },
                    HttpStatusCode.ServiceUnavailable
                );

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new ServiceException(
                    new Dictionary<string, string> { { "Room", "Room not found" } },
                    HttpStatusCode.NotFound
                );

            throw new ServiceException(
                new Dictionary<string, string> { { "HotelService", "Unexpected error" } },
                HttpStatusCode.InternalServerError
            );
        }

        var mappedRes = await response.Content.ReadFromJsonAsync<ApiResponse<ExistingRoomDTO>>();
        if (mappedRes?.Data == null)
            throw new ServiceException(
                new Dictionary<string, string> { { "Room", "Room not found" } },
                HttpStatusCode.NotFound
            );

        return mappedRes.Data;
    }

    private async Task<string?> GetSuperAdminId()
    {
        var client = _httpClientFactory.CreateClient("UserService");
        ForwardAuthHeader(client);

        var response = await client.GetAsync("superadmin-id");
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return result?.Data;
    }

    private void ForwardAuthHeader(HttpClient client)
    {
        var token = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"]
            .ToString();

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", token);
        }
    }
    #endregion
}
