using Application.Models;
using Application.ReadModels;

namespace Application.Repositories;

public interface ISavedSearchRepository
{
    Task<IEnumerable<SavedSearchReadModel>> GetByUserAsync(int userId);
    Task<IEnumerable<SavedSearch>> GetByUserIncludingDeletedAsync(int userId);
    Task<SavedSearch> CreateAsync(SavedSearch search);
    Task SoftDeleteAsync(int id, int userId);
    Task SoftDeleteAllAsync(int userId);
}
