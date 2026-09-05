using Application.Models;

namespace Application.Repositories;

public interface IImportJobRepository
{
    Task<ImportJob> CreateAsync(int electionType, short year);
    Task SetRunningAsync(Guid id);
    Task SetCompletedAsync(Guid id);
    Task SetFailedAsync(Guid id, string errorMessage);
    Task<ImportJob?> GetByIdAsync(Guid id);
}
