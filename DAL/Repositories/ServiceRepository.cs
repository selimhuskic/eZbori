namespace DAL.Repositories;

public class ServiceRepository(eZboriDbContext dboContext) : IServiceRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public async Task<IEnumerable<int>> GetGeneralElectionYearsAsync()
    {
        return await _dbContext.PresidencyResults.Select(x => x.ElectionYear).Distinct().ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetLocalElectionYearsAsync()
    {
        return await _dbContext.MunicipalityCandidateOverview.Select(x => x.ElectionYear).Distinct().ToArrayAsync();
    }
}
