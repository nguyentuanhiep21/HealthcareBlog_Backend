namespace HealthCareBlog_Backend.Interfaces;

/// <summary>
/// Generic repository interface — định nghĩa các thao tác CRUD cơ bản cho mọi entity.
/// Infrastructure layer sẽ implement interface này với EF Core.
/// </summary>
public interface IRepository<T> where T : class
{
    // ========== READ ==========
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();

    // ========== WRITE ==========
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    // ========== SAVE ==========
    Task<int> SaveChangesAsync();
}
