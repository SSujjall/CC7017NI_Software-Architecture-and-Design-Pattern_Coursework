using BuildingBlocks.GenericRepo;
using UserService.Data;
using UserService.Models;
using UserService.Repositories.Interfaces;

namespace UserService.Repositories;

public class UserRepository : GenericRepo<Users>, IUserRepository
{
    private readonly UserDbContext _dbContext;
    public UserRepository(UserDbContext context) : base(context)
    {
        _dbContext = context;
    }
}