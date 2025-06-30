using System.Linq.Expressions;

namespace PowerTaskManager.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    // Get methods
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null);
    Task<T> GetByIdAsync(object id);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
        string includeProperties = "");
    
    // Add methods
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    
    // Update methods
    void Update(T entity);
    
    // Remove methods
    Task RemoveAsync(object id);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    
    // Count method
    Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
}