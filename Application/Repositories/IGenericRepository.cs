using System.Linq.Expressions;

namespace Application.Repositories;

public interface IGenericRepository<TModel>
    where TModel : class
{
    Task AddAsync(TModel item);
    Task AddRangeAsync(IEnumerable<TModel> range);
    Task<TModel?> GetByIdAsync(int id);
    Task<IEnumerable<TModel>> GetAllAsync();
    Task<(IEnumerable<TModel> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task UpdateAsync(TModel item);
    Task DeleteAsync(TModel item);
    Task<bool> ExistsAsync(Expression<Func<TModel, bool>> predicate);
}
