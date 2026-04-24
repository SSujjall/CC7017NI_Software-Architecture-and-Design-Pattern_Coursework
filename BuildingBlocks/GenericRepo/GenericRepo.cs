using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.GenericRepo;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepo(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    
    public async Task<T> AddAsync(T entity)
    {
        var result = await _dbSet.AddAsync(entity);
        return result.Entity;
    }

    public Task<T> UpdateAsync(T entity)
    {
        var result = _dbSet.Update(entity);
        return Task.FromResult(result.Entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var result = await _dbSet.ToListAsync();
        return result;
    }

    public async Task<T?> GetByIdAsync(object entityId)
    {
        return await _dbSet.FindAsync(entityId);
    }

    public async Task<IEnumerable<T>> FindAllByConditionAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.Where(expression).ToListAsync();
    }

    public async Task<T> FindSingleByConditionAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.FirstOrDefaultAsync(expression);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}