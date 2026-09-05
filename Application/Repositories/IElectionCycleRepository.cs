using Application.Enum;
using Application.Models;

namespace Application.Repositories;

public interface IElectionCycleRepository : IGenericRepository<ElectionCycle>
{
    int[] GetYearsForType(ElectionType electionType);
    int[] GetAllYearsForType(ElectionType electionType);
    Task<int[]> GetYearsForTypeAsync(ElectionType electionType);
    Task<int[]> GetAllYearsForTypeAsync(ElectionType electionType);
    Task<ElectionCycle> CreateAsync(ElectionCycle cycle);
    Task UpdateAsync(int id, ElectionCycle cycle);
    Task DeleteAsync(int id);
    Task<ElectionCycle> GetByYearAndTypeAsync(short year, ElectionType electionType);
    Task MarkImportedAsync(short year, ElectionType electionType);
}
