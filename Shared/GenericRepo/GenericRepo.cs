using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shared.GenericRepo;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    private readonly DbContext _context;

    public GenericRepo(DbContext context)
    {
        _context = context;
    }
    
    public Task<T> AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<T> UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<T> GetByIdAsync(object entityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FindAllByConditionAsync(Expression<Func<T, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task<T> FindSingleByConditionAsync(Expression<Func<T, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}