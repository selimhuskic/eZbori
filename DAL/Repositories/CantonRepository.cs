namespace DAL.Repositories;

public class CantonRepository(eZboriDbContext dboContext) : ICantonRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public async Task StoreCantonElectoralUnitOverviewAsync(CantonElectoralUnitOverview model)
    {

        await _dbContext.CantonElectoralUnitOverview.AddAsync(model).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableOverviewReadModel> GetCantonElectoralUnitOverviewsAsync(int electionYear, CantonParliamentElectoralUnit electoralUnit)
    {
        var entityElectoralUnitResults = await _dbContext.CantonElectoralUnitOverview
            .FirstAsync(x => electionYear == x.ElectionYear && x.CantonElectoralUnitCode == (int)electoralUnit);

        return new TableOverviewReadModel(electoralUnit.ToString(), electionYear, Entity.Federation.ToString(), entityElectoralUnitResults.NumberOfVoters,
            entityElectoralUnitResults.TotalVotes, entityElectoralUnitResults.NumberOfParties, entityElectoralUnitResults.PercentageTotalVotes,
            entityElectoralUnitResults.InvalidBlankBallots, entityElectoralUnitResults.InvalidOthersBallots);
    }

    public async Task StoreCantonElectoralUnitPartiesAsync(IEnumerable<CantonElectoralUnitParty> models)
    {
        await _dbContext.CantonElectoralUnitParties.AddRangeAsync(models).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public Task<TableCandidateReadModel> GetCantonElectoralUnitPartiesAsync(int electionYear, CantonParliamentElectoralUnit electoralUnit)
    {
        var stateElectoralUnitParties = _dbContext.EntityElectoralUnitParty
            .Where(x => electionYear == x.ElectionYear);

        return Task.FromResult(new TableCandidateReadModel(electoralUnit.ToString(), null, stateElectoralUnitParties.Sum(x => x.TotalVotes),
            electionYear, stateElectoralUnitParties.ToDictionary(y => y.PartyName, z => z.TotalVotes)));
    }

    public async Task StoreCantonMunicipalOverviewAsync(CantonMunicipalOverview model)
    {
        await _dbContext.CantonMunicipalOverview.AddAsync(model).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableOverviewReadModel> GetCantonMunicipalOverviewsAsync(int electionYear, int municipalityCode)
    {
        var cantonMunicipalOverview = await _dbContext.EntityMunicipalOverview
            .FirstAsync(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode);

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableOverviewReadModel(municipality.Name, electionYear, municipality.Entity.ToString(),
            cantonMunicipalOverview.NumberOfVoters, cantonMunicipalOverview.TotalVotes, cantonMunicipalOverview.NumberOfParties,
            cantonMunicipalOverview.PercentageTotalVotes, cantonMunicipalOverview.InvalidBlankBallots, cantonMunicipalOverview.InvalidOthersBallots);
    }

    public async Task StoreCantonMunicipalPartiesAsync(IEnumerable<CantonMunicipalParty> models)
    {
        await _dbContext.CantonMunicipalParties.AddRangeAsync(models).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableCandidateReadModel> GetCantonMunicipalPartiesAsync(int electionYear, int municipalityCode)
    {
        var cantonMunicipalPartyResult = _dbContext.EntityMunicipalParty
            .Where(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode);

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableCandidateReadModel(municipality.Name, null, cantonMunicipalPartyResult.Sum(x => x.TotalVotes),
            electionYear, cantonMunicipalPartyResult.ToDictionary(y => y.Name, z => z.TotalVotes));
    }

    public async Task<IEnumerable<int>> GetElectoralUnitOverviewElectionYearsAsync(IEnumerable<int> cantonCodes)
    {
        return await _dbContext
            .CantonElectoralUnitOverview
            .Where(ceuo => cantonCodes.Contains(ceuo.CantonElectoralUnitCode))
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetElectoralUnitPartyElectionYearsAsync(IEnumerable<int> cantonCodes)
    {
        return await _dbContext
           .CantonElectoralUnitParties
           .Where(ceup => cantonCodes.Contains(ceup.CantonElectoralUnitCode))
           .Select(x => x.ElectionYear)
           .Distinct()
           .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetMunicipalOverviewElectionYearsAsync(IEnumerable<int> municipalityCodes)
    {
        return await _dbContext
          .CantonMunicipalOverview
          .Where(cmo => municipalityCodes.Contains(cmo.MunicipalityCode))
          .Select(x => x.ElectionYear)
          .Distinct()
          .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetMunicipalPartyElectionYearsAsync(IEnumerable<int> municipalityCodes)
    {
        return await _dbContext
         .CantonMunicipalParties
         .Where(cmp => municipalityCodes.Contains(cmp.MunicipalityCode))
         .Select(x => x.ElectionYear)
         .Distinct()
         .ToArrayAsync();
    }

    public async Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync()
    {
        var electionYears = await _dbContext.CantonElectoralUnitOverview
            .Select(x => x.ElectionYear)
            .Distinct()
            .Order()
            .ToListAsync();

        var yearsOverview       = await _dbContext.CantonElectoralUnitOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsParty          = await _dbContext.CantonElectoralUnitParties.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalOv    = await _dbContext.CantonMunicipalOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalParty = await _dbContext.CantonMunicipalParties.Select(r => r.ElectionYear).Distinct().ToListAsync();

        return yearsOverview.Select(y => new SearchRecommendationDto(0, typeof(CantonElectoralUnitOverview).ToString(), y, y.GetRelevance(electionYears, typeof(CantonElectoralUnitOverview))))
            .Concat(yearsParty.Select(y => new SearchRecommendationDto(0, typeof(CantonElectoralUnitParty).ToString(), y, y.GetRelevance(electionYears, typeof(CantonElectoralUnitParty)))))
            .Concat(yearsMunicipalOv.Select(y => new SearchRecommendationDto(0, typeof(CantonMunicipalOverview).ToString(), y, y.GetRelevance(electionYears, typeof(CantonMunicipalOverview)))))
            .Concat(yearsMunicipalParty.Select(y => new SearchRecommendationDto(0, typeof(CantonMunicipalParty).ToString(), y, y.GetRelevance(electionYears, typeof(CantonMunicipalParty)))));
    }
}
