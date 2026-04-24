using BuildingBlocks.GenericRepo;
using HotelService.Data;
using HotelService.Models;
using HotelService.Repositories.Interfaces;

namespace HotelService.Repositories;

public class HotelRepository : GenericRepo<Hotels>, IHotelRepository
{
    public HotelRepository(HotelDbContext context) : base(context)
    {
    }
}