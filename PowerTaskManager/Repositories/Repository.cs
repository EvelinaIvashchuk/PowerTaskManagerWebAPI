using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PowerTaskManager.Data;
using PowerTaskManager.Repositories.Interfaces;

namespace PowerTaskManager.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : class
{
    internal readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }
    
    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null)
    {
        IQueryable<T> query = DbSet;
        
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
        return await DbSet.FindAsync(id);
    }
    
    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "")
    {
        IQueryable<T> query = DbSet;
        
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
        await DbSet.AddAsync(entity);
    }
    
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await DbSet.AddRangeAsync(entities);
    }
    
    public void Update(T entity)
    {
        DbSet.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
    }
    
    public async Task RemoveAsync(object id)
    {
        T entityToRemove = await DbSet.FindAsync(id);
        Remove(entityToRemove);
    }
    
    public void Remove(T entity)
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }
        DbSet.Remove(entity);
    }
    
    public void RemoveRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
    }
    
    public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
    {
        IQueryable<T> query = DbSet;
        
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        return await query.CountAsync();
    }
}