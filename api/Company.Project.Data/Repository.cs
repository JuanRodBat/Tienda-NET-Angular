using Microsoft.EntityFrameworkCore;
using Company.Project.Data.Interfaces;

namespace Company.Project.Data;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _ctx;
    private readonly DbSet<T> _set;
    public Repository(AppDbContext ctx)
    {
        _ctx = ctx;
        _set = _ctx.Set<T>();
    }

    public Task<List<T>> GetAllAsync() => _set.AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(params object[] key)
    {
        return await _set.FindAsync(key);
    }

    public async Task<T> AddAsync(T entity)
    {
        _set.Add(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await _ctx.SaveChangesAsync();
    }
}

