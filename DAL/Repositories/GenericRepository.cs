using System.Linq.Expressions;

namespace DAL.Repositories;

public class GenericRepository<TModel> : IGenericRepository<TModel>
    where TModel : class
{
    private readonly eZboriDbContext _context;
    private readonly DbSet<TModel> _table;

    public GenericRepository(eZboriDbContext dboContext)
    {
        _context = dboContext;
        _table = _context.Set<TModel>();
    }

    public async Task AddAsync(TModel item)
    {
        await _table.AddAsync(item).ConfigureAwait(false);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<TModel> range)
    {
        await _table.AddRangeAsync(range).ConfigureAwait(false);
        await _context.SaveChangesAsync();
    }

    public async Task<TModel?> GetByIdAsync(int id) =>
        await _table.FindAsync(id);

    public async Task<IEnumerable<TModel>> GetAllAsync() =>
        await _table.ToListAsync();

    public async Task UpdateAsync(TModel item)
    {
        _table.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TModel item)
    {
        _table.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<TModel, bool>> predicate) =>
        await _table.AnyAsync(predicate);
}
