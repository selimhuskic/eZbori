using Application.Models;

namespace Application.Repositories;

public interface IForecastedResultRepository : IGenericRepository<ForecastedResult>
{
    Task<ForecastedResult> CreateAsync(ForecastedResult result);
    Task DeleteAsync(int id);
}
