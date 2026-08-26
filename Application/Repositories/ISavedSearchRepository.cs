using Application.Models;

namespace Application.Repositories;

public interface ISavedSearchRepository
{
    Task<IEnumerable<SavedSearch>> GetByUserAsync(int userId);
    Task<SavedSearch> CreateAsync(SavedSearch search);
    Task SoftDeleteAsync(int id, int userId);
    Task SoftDeleteAllAsync(int userId);
}
