namespace DAL.Repositories;

public class StateRepository(eZboriDbContext dboContext) : IStateRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public async Task StoreElectoralUnitOverviewAsync(StateElectoralUnitOverview stateElectoralUnitOverview)
    {
        await _dbContext.StateElectoralUnitOverview.AddAsync(stateElectoralUnitOverview);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TableOverviewReadModel> GetStateElectoralUnitOverviewsTableData(int electionYear, StateParliamentElectoralUnit electoralUnit)
    {
        var stateElectoralUnitOverview = await _dbContext.StateElectoralUnitOverview
            .FirstAsync(x => electionYear == x.ElectionYear && x.ElectoralUnit == (int)electoralUnit);

        return new TableOverviewReadModel(
                electoralUnit.ToString(), 
                electionYear,
                GetEntityForElectoralUnit(electoralUnit).ToString(), 
                stateElectoralUnitOverview.NumberOfVoters,
                stateElectoralUnitOverview.TotalVotes, 
                stateElectoralUnitOverview.PartyNumber, 
                stateElectoralUnitOverview.CandidatesNumber,
                stateElectoralUnitOverview.TotalMandates, 
                stateElectoralUnitOverview.PercentageTotalVotes);
    }

    public async Task StoreStateElectoralUnitPartiesAsync(IEnumerable<StateElectoralUnitParty> stateElectoralUnitParty)
    {
        await _dbContext.StateElectoralUnitParty.AddRangeAsync(stateElectoralUnitParty);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TableCandidateReadModel> GetStateElectoralUnitPartiesAsync(int electionYear)
    {
        var partyTotals = await _dbContext.StateElectoralUnitParty
            .Where(x => x.ElectionYear == electionYear)
            .GroupBy(x => x.PartyName)
            .Select(g => new { PartyName = g.Key, Total = g.Sum(x => x.TotalVotes) })
            .ToListAsync();

        var candidateResults = partyTotals.ToDictionary(x => x.PartyName, x => x.Total);
        var votersVoted = partyTotals.Sum(x => x.Total);

        return new TableCandidateReadModel("Bosna i Hercegovina", null, votersVoted, electionYear, candidateResults);
    }

    public async Task StoreStateMunicipalOverviews(StateMunicipalOverview stateMunicipalOverview)
    {
        await _dbContext.StateMunicipalOverview.AddAsync(stateMunicipalOverview);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TableOverviewReadModel> GetStateMunicipalOverviewQueryAsync(int electionYear, int municipalityCode)
    {
        var stateMunicipalOverview = await _dbContext.StateMunicipalOverview
            .FirstAsync(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode);

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableOverviewReadModel(municipality.Name, electionYear, municipality.Entity.ToString(),
            stateMunicipalOverview.NumberOfVoters, stateMunicipalOverview.TotalVotes, stateMunicipalOverview.NumberOfParties,
            stateMunicipalOverview.PercentageTotalVotes, stateMunicipalOverview.InvalidBlankBallots, stateMunicipalOverview.InvalidOthersBallots);
    }

    public async Task StoreMunicipalPartiesAsync(IEnumerable<StateMunicipalParty> stateMunicipalParties)
    {
        await _dbContext.StateMunicipalParty.AddRangeAsync(stateMunicipalParties).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TableCandidateReadModel> GetStateMunicipalPartiesAsync(int electionYear, int municipalityCode)
    {
        var stateMunicipalPartyResult = await _dbContext.StateMunicipalParty
            .Where(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode)
            .ToListAsync();

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableCandidateReadModel(municipality.Name, null, stateMunicipalPartyResult.Sum(x => x.TotalVotes),
            electionYear, stateMunicipalPartyResult.ToDictionary(y => y.Name, z => z.TotalVotes));
    }

    public async Task<IEnumerable<int>> GetElectoralUnitOverviewElectionYearsAsync()
    {
        return await _dbContext
            .StateElectoralUnitOverview
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetElectoralUnitPartiesElectionYearsAsync()
    {
        return await _dbContext
            .StateElectoralUnitParty
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetElectoralUnitMunicipalOverviewElectionYearsAsync()
    {
        return await _dbContext
            .StateMunicipalOverview
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetElectoralUnitMunicipalPartiesElectionYearsAsync()
    {
        return await _dbContext
            .StateMunicipalParty
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync()
    {
        var electionYears = await _dbContext.StateElectoralUnitOverview
            .Select(x => x.ElectionYear)
            .Distinct()
            .Order()
            .ToListAsync();

        var yearsOverview       = await _dbContext.StateElectoralUnitOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsParty          = await _dbContext.StateElectoralUnitParty.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalOv    = await _dbContext.StateMunicipalOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalParty = await _dbContext.StateMunicipalParty.Select(r => r.ElectionYear).Distinct().ToListAsync();

        return yearsOverview.Select(y => new SearchRecommendationDto(0, typeof(StateElectoralUnitOverview).ToString(), y, y.GetRelevance(electionYears, typeof(StateElectoralUnitOverview))))
            .Concat(yearsParty.Select(y => new SearchRecommendationDto(0, typeof(StateElectoralUnitParty).ToString(), y, y.GetRelevance(electionYears, typeof(StateElectoralUnitParty)))))
            .Concat(yearsMunicipalOv.Select(y => new SearchRecommendationDto(0, typeof(StateMunicipalOverview).ToString(), y, y.GetRelevance(electionYears, typeof(StateMunicipalOverview)))))
            .Concat(yearsMunicipalParty.Select(y => new SearchRecommendationDto(0, typeof(StateMunicipalParty).ToString(), y, y.GetRelevance(electionYears, typeof(StateMunicipalParty)))));
    }

    public async Task DeleteElectoralUnitOverviewAsync(int year)
        => await _dbContext.StateElectoralUnitOverview
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeleteElectoralUnitPartiesAsync(int year)
        => await _dbContext.StateElectoralUnitParty
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeleteStateMunicipalOverviewAsync(int year)
        => await _dbContext.StateMunicipalOverview
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    public async Task DeleteStateMunicipalPartiesAsync(int year)
        => await _dbContext.StateMunicipalParty
            .Where(x => x.ElectionYear == year)
            .ExecuteDeleteAsync();

    private static Entity GetEntityForElectoralUnit(StateParliamentElectoralUnit electoralUnit) =>
        electoralUnit is StateParliamentElectoralUnit.F1 or StateParliamentElectoralUnit.F2 or StateParliamentElectoralUnit.F3 or StateParliamentElectoralUnit.F4 or StateParliamentElectoralUnit.F5
        ? Entity.Federation
        : Entity.RS;
}
