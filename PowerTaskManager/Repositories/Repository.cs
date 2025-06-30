using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PowerTaskManager.Data;
using PowerTaskManager.Repositories.Interfaces;

namespace PowerTaskManager.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    internal DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null)
    {
        IQueryable<T> query = _dbSet;
        
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        foreach (var includeProperty in includeProperties.Split
                     (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }
        
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        
        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }
        
        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }
        
        return await query.ToListAsync();
    }
    
    public async Task<T> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;
        
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        foreach (var includeProperty in includeProperties.Split
                     (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }
        
        return await query.FirstOrDefaultAsync();
    }
    
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }
    
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }
    
    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
    
    public async Task RemoveAsync(object id)
    {
        T entityToRemove = await _dbSet.FindAsync(id);
        Remove(entityToRemove);
    }
    
    public void Remove(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }
    
    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    
    public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
    {
        IQueryable<T> query = _dbSet;
        
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        return await query.CountAsync();
    }
}