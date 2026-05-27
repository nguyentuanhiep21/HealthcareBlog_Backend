using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic base repository — implement IRepository<T> với EF Core.
/// Mọi repository cụ thể (PostRepository, UserRepository,...) kế thừa class này.
/// </summary>
public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // ========== READ ==========

    public virtual async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    // ========== WRITE ==========

    public virtual async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public virtual void Update(T entity)
        => _dbSet.Update(entity);

    public virtual void Remove(T entity)
        => _dbSet.Remove(entity);

    public virtual void RemoveRange(IEnumerable<T> entities)
        => _dbSet.RemoveRange(entities);

    // ========== SAVE ==========

    public virtual async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
