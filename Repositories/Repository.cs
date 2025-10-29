using APIPedidos.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _ctx;
    protected readonly DbSet<T> _set;
    public Repository(AppDbContext ctx) { _ctx = ctx; _set = ctx.Set<T>(); }

    public Task<T?> GetByIdAsync(int id) => _set.FindAsync(id).AsTask();

    public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate is null ? await _set.AsNoTracking().ToListAsync()
                             : await _set.AsNoTracking().Where(predicate).ToListAsync();

    public Task AddAsync(T entity) => _set.AddAsync(entity).AsTask();
    public void Update(T entity) => _set.Update(entity);
    public void Delete(T entity) => _set.Remove(entity);
    public Task SaveChangesAsync() => _ctx.SaveChangesAsync();
}
