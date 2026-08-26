using Application.Enum;
using Application.Models;
using Application.Models.MachineLearning;
using Application.ReadModels;

namespace Application.Repositories;

public interface IStateRepository
{
    Task StoreElectoralUnitOverviewAsync(StateElectoralUnitOverview stateElectoralUnitOverview);
    Task<TableOverviewReadModel> GetStateElectoralUnitOverviewsTableData(int electionYear, StateParliamentElectoralUnit electoralUnit);
    Task StoreStateElectoralUnitPartiesAsync(IEnumerable<StateElectoralUnitParty> stateElectoralUnitParty);
    Task<TableCandidateReadModel> GetStateElectoralUnitPartiesAsync(int electionYear);
    Task StoreStateMunicipalOverviews(StateMunicipalOverview stateMunicipalOverview);
    Task<TableOverviewReadModel> GetStateMunicipalOverviewQueryAsync(int electionYear, int municipalityCode);
    Task StoreMunicipalPartiesAsync(IEnumerable<StateMunicipalParty> stateMunicipalParties);
    Task<TableCandidateReadModel> GetStateMunicipalPartiesAsync(int electionYear, int municipalityCode);
    Task<IEnumerable<int>> GetElectoralUnitOverviewElectionYearsAsync();
    Task<IEnumerable<int>> GetElectoralUnitPartiesElectionYearsAsync();
    Task<IEnumerable<int>> GetElectoralUnitMunicipalOverviewElectionYearsAsync();
    Task<IEnumerable<int>> GetElectoralUnitMunicipalPartiesElectionYearsAsync();
    Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync();
}
