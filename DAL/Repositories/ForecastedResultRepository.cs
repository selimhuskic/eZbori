namespace DAL.Repositories;

public class ForecastedResultRepository(eZboriDbContext dbContext)
    : GenericRepository<ForecastedResult>(dbContext), IForecastedResultRepository
{
    public async Task<ForecastedResult> CreateAsync(ForecastedResult result)
    {
        await AddAsync(result);
        return result;
    }

    public async Task DeleteAsync(int id)
    {
        var result = await GetByIdAsync(id);
        if (result is not null)
            await DeleteAsync(result);
    }
}
