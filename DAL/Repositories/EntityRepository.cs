using Application.Enum;
using Application.Models.MachineLearning;
using DAL.Helpers;

namespace DAL.Repositories;

public class EntityRepository(eZboriDbContext dboContext) : IEntityRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public async Task StoreElectoralUnitOverviewAsync(EntityElectoralUnitOverview entityElectoralUnitOverview)
    {
        await _dbContext.EntityElectoralUnitOverview.AddAsync(entityElectoralUnitOverview).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableOverviewReadModel> GetEntityElecetoralUnitResultsAsync(int electionYear, EntityParliamentElectoralUnit electoralUnit)
    {
        var entityElectoralUnitResults = await _dbContext.EntityElectoralUnitOverview
            .FirstAsync(x => electionYear == x.ElectionYear && x.ElectoralUnitCode == (int)electoralUnit);

        return new TableOverviewReadModel(electoralUnit.ToString(), electionYear, GetEntityForElectoralUnit(electoralUnit).ToString(),
            entityElectoralUnitResults.NumberOfVoters, entityElectoralUnitResults.TotalVotes, entityElectoralUnitResults.NumberOfParties,
            entityElectoralUnitResults.PercentageTotalVotes, entityElectoralUnitResults.InvalidBlankBallots, entityElectoralUnitResults.InvalidOthersBallots);
    }

    public async Task StoreElectoralUnitPartiesAsync(IEnumerable<EntityElectoralUnitParty> models)
    {
        await _dbContext.EntityElectoralUnitParty.AddRangeAsync(models).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableCandidateReadModel> GetEntityElectoralUnitPartiesAsync(int electionYear, EntityParliamentElectoralUnit electoralUnit)
    {
        var parties = await _dbContext.EntityElectoralUnitParty
            .Where(x => electionYear == x.ElectionYear)
            .ToListAsync();

        return new TableCandidateReadModel(electoralUnit.ToString(), null,
            parties.Sum(x => x.TotalVotes),
            electionYear,
            parties.ToDictionary(y => y.PartyName, z => z.TotalVotes));
    }

    public async Task StoreEntityMunicipalOverviewAsync(EntityMunicipalOverview entityMunicipalOverview)
    {
        await _dbContext.EntityMunicipalOverview.AddAsync(entityMunicipalOverview).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableOverviewReadModel> GetEntityMunicipalOverviewAsync(int electionYear, int municipalityCode)
    {
        var entityMunicipalOverview = await _dbContext.EntityMunicipalOverview
            .FirstAsync(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode);

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableOverviewReadModel(municipality.Name, electionYear, municipality.Entity.ToString(),
            entityMunicipalOverview.NumberOfParties, entityMunicipalOverview.TotalVotes, entityMunicipalOverview.NumberOfParties,
            entityMunicipalOverview.PercentageTotalVotes, entityMunicipalOverview.InvalidBlankBallots, entityMunicipalOverview.InvalidOthersBallots);
    }

    public async Task StoreMunicipalPartyResultsAsync(IEnumerable<EntityMunicipalParty> models)
    {
        await _dbContext.EntityMunicipalParty.AddRangeAsync(models).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableCandidateReadModel> GetEntityMunicipalPartiesAsync(int electionYear, int municipalityCode)
    {
        var parties = await _dbContext.EntityMunicipalParty
            .Where(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode)
            .ToListAsync();

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableCandidateReadModel(municipality.Name, null,
            parties.Sum(x => x.TotalVotes),
            electionYear,
            parties.ToDictionary(y => y.Name, z => z.TotalVotes));
    }

    public async Task StorePresidentMunicipalAsync(IEnumerable<EntityPresidentMunicipalCandidate> models)
    {
        await _dbContext.EntityPresidentMunicipalCandidate.AddRangeAsync(models).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableCandidateReadModel> GetEntityPresidentMunicipalResultsAsync(int electionYear, int municipalityCode)
    {
        var candidates = await _dbContext.EntityPresidentMunicipalCandidate
            .Where(x => electionYear == x.ElectionYear && municipalityCode == x.MunicipalityCode)
            .ToListAsync();

        var municipality = await _dbContext.Municipalities.FirstAsync(x => municipalityCode == x.Id);

        return new TableCandidateReadModel(municipality.Name, null,
            candidates.Sum(x => x.TotalVotes),
            electionYear,
            candidates.ToDictionary(y => y.Name, z => z.TotalVotes));
    }

    public async Task StoreEntityPresidentOverviewAsync(EntityPresidentOverview entityPresidentOverview)
    {
        await _dbContext.EntityPresidentOverview.AddAsync(entityPresidentOverview).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<TableOverviewReadModel> GetEntityPresidentOverviewResultsAsync(int electionYear)
    {
        var entityPresidentOverviews = await _dbContext.EntityPresidentOverview
            .FirstAsync(x => electionYear == x.ElectionYear);

        return new TableOverviewReadModel(Entity.RS.ToString(), electionYear, Entity.RS.ToString(),
            entityPresidentOverviews.NumberOfVoters, entityPresidentOverviews.TotalVotes,
            entityPresidentOverviews.NumberOfCandidates, entityPresidentOverviews.PercentageTotalVotes,
            entityPresidentOverviews.InvalidBlankBallots, entityPresidentOverviews.InvalidOthersBallots);
    }

    private static Entity GetEntityForElectoralUnit(EntityParliamentElectoralUnit electoralUnit) =>
        electoralUnit is EntityParliamentElectoralUnit.F1 or EntityParliamentElectoralUnit.F2 or EntityParliamentElectoralUnit.F3 or EntityParliamentElectoralUnit.F4 or
            EntityParliamentElectoralUnit.F5 or EntityParliamentElectoralUnit.F6 or EntityParliamentElectoralUnit.F7 or EntityParliamentElectoralUnit.F8 or
            EntityParliamentElectoralUnit.F9 or EntityParliamentElectoralUnit.F10 or EntityParliamentElectoralUnit.F11 or EntityParliamentElectoralUnit.F12
            ? Entity.Federation
            : Entity.RS;

    public async Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync()
    {
        var electionYears = await _dbContext.EntityElectoralUnitOverview
            .Select(pr => pr.ElectionYear)
            .Distinct()
            .Order()
            .ToListAsync();

        var yearsParty          = await _dbContext.EntityElectoralUnitParty.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsOverview       = await _dbContext.EntityElectoralUnitOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalParty = await _dbContext.EntityMunicipalParty.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsMunicipalOv    = await _dbContext.EntityMunicipalOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsPresidentOv    = await _dbContext.EntityPresidentOverview.Select(r => r.ElectionYear).Distinct().ToListAsync();
        var yearsPresidentMun   = await _dbContext.EntityPresidentMunicipalCandidate.Select(r => r.ElectionYear).Distinct().ToListAsync();

        return yearsParty.Select(y => new SearchRecommendationDto(0, typeof(EntityElectoralUnitParty).ToString(), y, y.GetRelevance(electionYears, typeof(EntityElectoralUnitParty))))
            .Concat(yearsOverview.Select(y => new SearchRecommendationDto(0, typeof(EntityElectoralUnitOverview).ToString(), y, y.GetRelevance(electionYears, typeof(EntityElectoralUnitOverview)))))
            .Concat(yearsMunicipalParty.Select(y => new SearchRecommendationDto(0, typeof(EntityMunicipalParty).ToString(), y, y.GetRelevance(electionYears, typeof(EntityMunicipalParty)))))
            .Concat(yearsMunicipalOv.Select(y => new SearchRecommendationDto(0, typeof(EntityMunicipalOverview).ToString(), y, y.GetRelevance(electionYears, typeof(EntityMunicipalOverview)))))
            .Concat(yearsPresidentOv.Select(y => new SearchRecommendationDto(0, typeof(EntityPresidentOverview).ToString(), y, y.GetRelevance(electionYears, typeof(EntityPresidentOverview)))))
            .Concat(yearsPresidentMun.Select(y => new SearchRecommendationDto(0, typeof(EntityPresidentMunicipalCandidate).ToString(), y, y.GetRelevance(electionYears, typeof(EntityPresidentMunicipalCandidate)))));
    }

    public async Task<IEnumerable<int>> GetElectoralUnitOverviewElectionYearsAsync(IEnumerable<int> electoralUnits)
    {
        return await _dbContext
            .EntityElectoralUnitOverview
            .Where(eeuo => electoralUnits.Contains(eeuo.ElectoralUnitCode))
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetElectoralUnitPartiesElectionYearsAsync(IEnumerable<int> electoralUnits)
    {
        return await _dbContext
            .EntityElectoralUnitParty
            .Where(eeuo => electoralUnits.Contains(eeuo.ElectoralUnitCode))
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetEntityMunicipalPartyElectionYearsAsync(IEnumerable<int> municipalityCodes)
    {
        return await _dbContext
            .EntityMunicipalParty
            .Where(empe => municipalityCodes.Contains(empe.MunicipalityCode))
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetEntityMunicipalOverviewElectionYearsAsync(IEnumerable<int> municipalityCodes)
    {
        return await _dbContext
            .EntityMunicipalOverview
            .Where(epmc => municipalityCodes.Contains(epmc.MunicipalityCode))
            .Select(x => x.ElectionYear)
            .Distinct()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetEntityPresidentMunicipalElectionYearsAsync(IEnumerable<int> municipalityCodes)
    {
        return await _dbContext
           .EntityPresidentMunicipalCandidate
           .Where(epmc => municipalityCodes.Contains(epmc.MunicipalityCode))
           .Select(x => x.ElectionYear)
           .Distinct()
           .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetEntityPresidentOverviewElectionYearsAsync(Entity entity)
    {
        return await _dbContext
           .EntityPresidentOverview
           .Where(eeuo => eeuo.Entity == entity)
           .Select(x => x.ElectionYear)
           .Distinct()
           .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetMunicipalityOverviewYearsAsync()
    {
        return await _dbContext
           .EntityMunicipalOverview
           .Select(x => x.ElectionYear)
           .Distinct()
           .ToArrayAsync();
    }
}
