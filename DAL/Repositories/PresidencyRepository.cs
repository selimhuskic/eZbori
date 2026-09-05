namespace DAL.Repositories;

public class PresidencyRepository(eZboriDbContext dboContext) : IPresidencyRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public async Task StoreOverviewAsync(PresidencyMunicipalOverview overview)
    {
        await _dbContext.PresidencyMunicipalOverview.AddAsync(overview);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TableOverviewReadModel> GetPresidencyMunicipalOverviewsAsync(int electionYear, int municipalityCode)
    {
        var presidencyMunicipalOverview = await _dbContext.PresidencyMunicipalOverview
            .FirstAsync(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode);

        var municipality = await _dbContext.Municipalities.FirstAsync(x => x.Id == municipalityCode);

        return new TableOverviewReadModel(municipality.Name, electionYear, municipality.Entity.ToString(),
            presidencyMunicipalOverview.TotalVoters, presidencyMunicipalOverview.TotalVotes,
            presidencyMunicipalOverview.PartyNumber, presidencyMunicipalOverview.PercentageTotalVotes,
            presidencyMunicipalOverview.InvalidBlankBallots, presidencyMunicipalOverview.InvalidOthersBallots);
    }

    public async Task StorePresidencyOverviewAsync(PresidencyOverview overview)
    {
        await _dbContext.PresidencyOverview.AddAsync(overview).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableOverviewReadModel> GetPresidencyOverviewsAsync(int electionYear, Entity entity)
    {
        var presidencyOverview = await _dbContext.PresidencyOverview
            .FirstAsync(x => electionYear == x.ElectionYear && x.Entity == entity);

        return new TableOverviewReadModel(entity.ToString(), electionYear, entity.ToString(),
            presidencyOverview.TotalVoters, presidencyOverview.TotalVotes, presidencyOverview.PartyNumber,
            presidencyOverview.PercentageTotalVotes, presidencyOverview.InvalidBlankBallots, presidencyOverview.InvalidOthersBallots);
    }

    public async Task StorePresidencyResultsMunicipalAsync(IEnumerable<PresidencyMunicipalResults> municipalResults)
    {
        await _dbContext.PresidencyMunicipalResults.AddRangeAsync(municipalResults);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TableCandidateReadModel> GetPresidencyMunicipalResultsAsync(int electionYear, int municipalityCode)
    {
        var results = await _dbContext.PresidencyMunicipalResults
            .Where(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode)
            .ToListAsync();

        var municipality = await _dbContext.Municipalities.FirstAsync(x => x.Id == municipalityCode);

        return new TableCandidateReadModel(municipality.Name, null,
            results.Sum(x => x.TotalVotes),
            electionYear,
            results.ToDictionary(y => y.Name, z => z.TotalVotes));
    }

    public async Task StorePresidencyResultsAsync(IEnumerable<PresidencyResults> presidencyResults)
    {
        await _dbContext.PresidencyResults.AddRangeAsync(presidencyResults).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<int>> GetPresidencyResultsElectionYearsAsync(Constituency constituency)
    {
        return await _dbContext
            .PresidencyResults
            .Where(pr => pr.Constituency == constituency)
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<TableCandidateReadModel> GetPresidencyResultsAsync(int electionYear, Constituency constituency)
    {
        var results = await _dbContext.PresidencyResults
            .Where(x => electionYear == x.ElectionYear && x.Constituency == constituency)
            .ToListAsync();

        return new TableCandidateReadModel(
            constituency == Constituency.Serb ? Entity.RS.ToString() : Entity.Federation.ToString(),
            null,
            results.Sum(x => x.TotalVotes),
            electionYear,
            results.ToDictionary(x => x.CandidateName, y => y.TotalVotes));
    }

    public async Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync()
    {
        var electionYears = await _dbContext.PresidencyResults
            .Select(pr => pr.ElectionYear)
            .Distinct()
            .Order()
            .ToListAsync();

        var yearsResults         = await _dbContext.PresidencyResults.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsOverview        = await _dbContext.PresidencyOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalResults  = await _dbContext.PresidencyMunicipalResults.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalOverview = await _dbContext.PresidencyMunicipalOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();

        return yearsResults.Select(y => new SearchRecommendationDto(0, typeof(PresidencyResults).ToString(), y, y.GetRelevance(electionYears, typeof(PresidencyResults))))
            .Concat(yearsOverview.Select(y => new SearchRecommendationDto(0, typeof(PresidencyOverview).ToString(), y, y.GetRelevance(electionYears, typeof(PresidencyOverview)))))
            .Concat(yearsMunicipalResults.Select(y => new SearchRecommendationDto(0, typeof(PresidencyMunicipalResults).ToString(), y, y.GetRelevance(electionYears, typeof(PresidencyMunicipalResults)))))
            .Concat(yearsMunicipalOverview.Select(y => new SearchRecommendationDto(0, typeof(PresidencyMunicipalOverview).ToString(), y, y.GetRelevance(electionYears, typeof(PresidencyMunicipalOverview)))));
    }

    public IEnumerable<PresidencyOverview> GetAllOverviews()
    {
        return _dbContext.PresidencyOverview.Select(x => x);
    }

    public async Task<IEnumerable<int>> GetPresidencyOverviewMunicipalElectionYearsAsync()
    {
        return await _dbContext
            .PresidencyMunicipalOverview
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetPresidencyResultsMunicipalLevelElectionYearsAsync()
    {
        return await _dbContext
            .PresidencyMunicipalResults
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task DeletePresidencyOverviewAsync(int year, Entity entity)
        => await _dbContext.PresidencyOverview
            .Where(x => x.ElectionYear == year && x.Entity == entity)
            .ExecuteDeleteAsync();

    public async Task DeletePresidencyResultsAsync(int year, Constituency constituency)
        => await _dbContext.PresidencyResults
            .Where(x => x.ElectionYear == year && x.Constituency == constituency)
            .ExecuteDeleteAsync();

    public async Task DeletePresidencyMunicipalOverviewAsync(int year)
        => await _dbContext.PresidencyMunicipalOverview
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeletePresidencyMunicipalResultsAsync(int year)
        => await _dbContext.PresidencyMunicipalResults
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();
}
