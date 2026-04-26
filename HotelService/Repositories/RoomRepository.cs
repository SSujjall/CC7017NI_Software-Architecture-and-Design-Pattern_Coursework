using BuildingBlocks.GenericRepo;
using HotelService.Data;
using HotelService.Models;
using HotelService.Repositories.Interfaces;

namespace HotelService.Repositories;

public class RoomRepository : GenericRepo<Rooms>, IRoomRepository
{
    public RoomRepository(HotelDbContext context) : base(context)
    {
    }
}