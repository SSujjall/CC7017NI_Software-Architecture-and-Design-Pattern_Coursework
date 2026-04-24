using System.Linq.Expressions;

namespace BuildingBlocks.GenericRepo;

public interface IGenericRepo<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object entityId);
    Task<IEnumerable<T>> FindAllByConditionAsync(Expression<Func<T, bool>> expression);
    Task<T> FindSingleByConditionAsync(Expression<Func<T, bool>> expression);
    Task SaveChangesAsync();
}