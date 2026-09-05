namespace DAL.Repositories;

public class MunicipalityRepository(eZboriDbContext dboContext) : IMunicipalityRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public async Task StoreMunicipalityCouncilOverviewAsync(MunicipalityCouncilOverview model)
    {
        await _dbContext.MunicipalityCouncilOverview.AddAsync(model).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableOverviewReadModel> GetMunicipalityCouncilOverviewsAsync(int electionYear, int municipalityCode)
    {
        var councilOverview = await _dbContext.MunicipalityCouncilOverview
            .FirstOrDefaultAsync(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode);

        if (councilOverview == null)
            throw new UserException($"Ne postoji pregled općinskog vijeća za općinu {municipalityCode}, godina {electionYear}.");

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableOverviewReadModel(municipality.Name, electionYear, municipality.Entity.ToString(),
            councilOverview.NumberOfVoters, councilOverview.TotalVotes,
            null, councilOverview.PercentageTotalVotes, councilOverview.InvalidBlankBallots, councilOverview.ProcessedInvalidOthersBallots);
    }

    public async Task StoreMunicipalityCandidateDetailsAsync(IEnumerable<MunicipalityCandidateDetails> models)
    {
        await _dbContext.MunicipalityCandidateDetails.AddRangeAsync(models).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableCandidateReadModel> GetMunicipalityCandidateDetailsAsync(int electionYear, int municipalityCode)
    {
        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        var municipalityCandidates = await _dbContext.MunicipalityCandidateDetails
                                                  .Where(x => electionYear == x.ElectionYear && x.MunicipalityCode == municipalityCode)
                                                  .ToListAsync();

        return new TableCandidateReadModel(municipality.Name,
                         null, municipalityCandidates.Sum(x => x.TotalVotes),
                         electionYear, municipalityCandidates.ToDictionary(x => x.Name, x => x.TotalVotes));
    }

    public async Task StoreMunicipalityCandidateOverviewAsync(MunicipalityCandidateOverview model)
    {
        await _dbContext.MunicipalityCandidateOverview.AddRangeAsync(model).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TableOverviewReadModel> GetMunicipalityCandidateOverviewsAsync(int electionYear, int municipalityCode)
    {
        var municipalityCandidateOverviews = await _dbContext.MunicipalityCandidateOverview
            .Where(x => x.ElectionYear == electionYear && x.MunicipalityCode == municipalityCode)
            .ToListAsync();

        var municipality = await _dbContext.Municipalities.FirstAsync(x => x.Id == municipalityCode);

        var municipalityCandidateOverview = municipalityCandidateOverviews.FirstOrDefault(x => x.MunicipalityCode == municipality.Id);

        if (municipalityCandidateOverview == null)
            throw new UserException($"Ne postoji pregled kandidata za općinu {municipalityCode}, godina {electionYear}.");

        return new TableOverviewReadModel(municipality.Name,
            electionYear,
            municipality.Entity.ToString(),
            municipalityCandidateOverview.NumberOfVoters,
            municipalityCandidateOverview.TotalVotes,
            municipalityCandidateOverview.NumberOfCandidates,
            municipalityCandidateOverview.PercentageTotalVotes,
            municipalityCandidateOverview.ProcessedInvalidBlankBallots,
            municipalityCandidateOverview.ProcessedInvalidOthersBallots);

    }

    public async Task StoreMunicipalityCouncilMinoritiesAsync(IEnumerable<MunicipalityCouncilMinority> models)
    {
        await _dbContext.MunicipalityCouncilMinorities.AddRangeAsync(models).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableCandidateReadModel> GetMunicipalityCouncilMinoritiesAsync(int electionYear, int municipalityCode)
    {
        var municipalityMinorityCandidates = await _dbContext.MunicipalityCouncilMinorities
            .Where(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode)
            .ToListAsync();

        var municipality = await
            _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableCandidateReadModel(municipality.Name, null, municipalityMinorityCandidates.Sum(x => x.TotalVotes),
            electionYear, municipalityMinorityCandidates.ToDictionary(y => y.Name, z => z.TotalVotes));
    }

    public async Task StoreMunicipalityCouncilPartiesAsync(IEnumerable<MunicipalityCouncilParty> models)
    {
        await _dbContext.MunicipalityCouncilParties.AddRangeAsync(models).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableCandidateReadModel> GetMunicipalityCouncilPartiesAsync(int electionYear, int municipalityCode)
    {
        var municipalityCouncilParties = await _dbContext.MunicipalityCouncilParties
            .Where(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode)
            .ToListAsync();

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableCandidateReadModel(municipality.Name, null, municipalityCouncilParties.Sum(x => x.TotalVotes),
            electionYear, municipalityCouncilParties.ToDictionary(y => y.Name, z => z.TotalVotes));
    }

    public async Task<IEnumerable<int>> GetElectionYears()
    {
        return await _dbContext.MunicipalityCandidateOverview.Select(x => x.ElectionYear).Distinct().ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetCandidateDetailsElectionYearsAsync()
    {
        return await _dbContext
          .MunicipalityCandidateDetails
          .Select(x => x.ElectionYear)
          .Distinct()
          .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetCandidateOverviewElectionYearsAsync()
    {
        return await _dbContext
         .MunicipalityCandidateOverview
         .Select(x => x.ElectionYear)
         .Distinct()
         .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetMunicipalCouncilOverviewElectionYearsAsync()
    {
        return await _dbContext
         .MunicipalityCouncilOverview
         .Select(x => x.ElectionYear)
         .Distinct()
         .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetMunicipalCouncilPartyElectionYearsAsync()
    {
        return await _dbContext
        .MunicipalityCouncilParties
        .Select(x => x.ElectionYear)
        .Distinct()
        .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetMunicipalCouncilMinirotiesElectionYearsAsync()
    {
        return await _dbContext
        .MunicipalityCouncilMinorities
        .Select(x => x.ElectionYear)
        .Distinct()
        .ToArrayAsync();
    }

    public async Task DeleteCandidateDetailsByYearAsync(int year)
        => await _dbContext.MunicipalityCandidateDetails
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeleteCandidateOverviewByYearAsync(int year)
        => await _dbContext.MunicipalityCandidateOverview
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeleteCouncilOverviewByYearAsync(int year)
        => await _dbContext.MunicipalityCouncilOverview
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeleteCouncilPartiesByYearAsync(int year)
        => await _dbContext.MunicipalityCouncilParties
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeleteCouncilMinoritiesByYearAsync(int year)
        => await _dbContext.MunicipalityCouncilMinorities
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync()
    {
        var electionYears = await _dbContext.MunicipalityCouncilOverview
            .Select(x => x.ElectionYear)
            .Distinct()
            .Order()
            .ToListAsync();

        var yearsCandidateDetails  = await _dbContext.MunicipalityCandidateDetails.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsCandidateOverview = await _dbContext.MunicipalityCandidateOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsCouncilOverview   = await _dbContext.MunicipalityCouncilOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsCouncilParties    = await _dbContext.MunicipalityCouncilParties.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsCouncilMinorities = await _dbContext.MunicipalityCouncilMinorities.Select(r => r.ElectionYear).Distinct().ToListAsync();

        return yearsCandidateDetails.Select(y => new SearchRecommendationDto(0, typeof(MunicipalityCandidateDetails).ToString(), y, y.GetRelevance(electionYears, typeof(MunicipalityCandidateDetails))))
            .Concat(yearsCandidateOverview.Select(y => new SearchRecommendationDto(0, typeof(MunicipalityCandidateOverview).ToString(), y, y.GetRelevance(electionYears, typeof(MunicipalityCandidateOverview)))))
            .Concat(yearsCouncilOverview.Select(y => new SearchRecommendationDto(0, typeof(MunicipalityCouncilOverview).ToString(), y, y.GetRelevance(electionYears, typeof(MunicipalityCouncilOverview)))))
            .Concat(yearsCouncilParties.Select(y => new SearchRecommendationDto(0, typeof(MunicipalityCouncilParty).ToString(), y, y.GetRelevance(electionYears, typeof(MunicipalityCouncilParty)))))
            .Concat(yearsCouncilMinorities.Select(y => new SearchRecommendationDto(0, typeof(MunicipalityCouncilMinority).ToString(), y, y.GetRelevance(electionYears, typeof(MunicipalityCouncilMinority)))));
    }
}
