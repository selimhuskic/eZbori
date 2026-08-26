using Application.Enum;
using Application.Models;
using Application.Models.MachineLearning;
using Application.ReadModels;

namespace Application.Repositories;

public interface IEntityRepository
{
    Task StoreElectoralUnitOverviewAsync(EntityElectoralUnitOverview entityElectoralUnitOverview);
    Task<TableOverviewReadModel> GetEntityElecetoralUnitResultsAsync(int electionYear, EntityParliamentElectoralUnit electoralUnit);
    Task StoreElectoralUnitPartiesAsync(IEnumerable<EntityElectoralUnitParty> models);
    Task<TableCandidateReadModel> GetEntityElectoralUnitPartiesAsync(int electionYear, EntityParliamentElectoralUnit electoralUnit);
    Task StoreEntityMunicipalOverviewAsync(EntityMunicipalOverview entityMunicipalOverview);
    Task<TableOverviewReadModel> GetEntityMunicipalOverviewAsync(int electionYear, int municipalityCode);
    Task StoreMunicipalPartyResultsAsync(IEnumerable<EntityMunicipalParty> model);
    Task<TableCandidateReadModel> GetEntityMunicipalPartiesAsync(int electionYear, int municipalityCode);
    Task StorePresidentMunicipalAsync(IEnumerable<EntityPresidentMunicipalCandidate> model);
    Task<TableCandidateReadModel> GetEntityPresidentMunicipalResultsAsync(int electionYear, int municipalityCode);
    Task StoreEntityPresidentOverviewAsync(EntityPresidentOverview entityPresidentOverview);
    Task<TableOverviewReadModel> GetEntityPresidentOverviewResultsAsync(int electionYear);
    Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync();
    Task<IEnumerable<int>> GetElectoralUnitOverviewElectionYearsAsync(IEnumerable<int> electoralUnits);
    Task<IEnumerable<int>> GetElectoralUnitPartiesElectionYearsAsync(IEnumerable<int> electoralUnits);
    Task<IEnumerable<int>> GetEntityMunicipalPartyElectionYearsAsync(IEnumerable<int> municipalityCodes);
    Task<IEnumerable<int>> GetEntityMunicipalOverviewElectionYearsAsync(IEnumerable<int> municipalityCodes);
    Task<IEnumerable<int>> GetEntityPresidentMunicipalElectionYearsAsync(IEnumerable<int> municipalityCodes);
    Task<IEnumerable<int>> GetEntityPresidentOverviewElectionYearsAsync(Entity entity);
    Task<IEnumerable<int>> GetMunicipalityOverviewYearsAsync();
}
