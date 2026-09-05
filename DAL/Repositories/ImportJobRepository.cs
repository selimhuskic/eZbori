using Application.Enum;

namespace DAL.Repositories;

public class ImportJobRepository(eZboriDbContext dbContext) : IImportJobRepository
{
    private readonly eZboriDbContext _dbContext = dbContext;

    public async Task<ImportJob> CreateAsync(int electionType, short year)
    {
        var job = new ImportJob
        {
            Id = Guid.NewGuid(),
            ElectionType = electionType,
            Year = year,
            Status = ImportJobStatus.Queued,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _dbContext.ImportJobs.AddAsync(job);
        await _dbContext.SaveChangesAsync();
        return job;
    }

    public async Task SetRunningAsync(Guid id)
    {
        await _dbContext.ImportJobs
            .Where(j => j.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(j => j.Status, ImportJobStatus.Running)
                .SetProperty(j => j.UpdatedAt, DateTime.UtcNow));
    }

    public async Task SetCompletedAsync(Guid id)
    {
        await _dbContext.ImportJobs
            .Where(j => j.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(j => j.Status, ImportJobStatus.Completed)
                .SetProperty(j => j.UpdatedAt, DateTime.UtcNow));
    }

    public async Task SetFailedAsync(Guid id, string errorMessage)
    {
        await _dbContext.ImportJobs
            .Where(j => j.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(j => j.Status, ImportJobStatus.Failed)
                .SetProperty(j => j.ErrorMessage, errorMessage)
                .SetProperty(j => j.UpdatedAt, DateTime.UtcNow));
    }

    public async Task<ImportJob?> GetByIdAsync(Guid id)
        => await _dbContext.ImportJobs.FindAsync(id);
}
