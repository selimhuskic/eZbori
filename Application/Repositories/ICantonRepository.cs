using Application.Enum;
using Application.Models;
using Application.Models.MachineLearning;
using Application.ReadModels;

namespace Application.Repositories;

public interface ICantonRepository
{
    Task StoreCantonElectoralUnitOverviewAsync(CantonElectoralUnitOverview model);
    Task<TableOverviewReadModel> GetCantonElectoralUnitOverviewsAsync(int electionYear, CantonParliamentElectoralUnit electoralUnit);
    Task StoreCantonElectoralUnitPartiesAsync(IEnumerable<CantonElectoralUnitParty> models);
    Task<TableCandidateReadModel> GetCantonElectoralUnitPartiesAsync(int electionYear, CantonParliamentElectoralUnit electoralUnit);
    Task StoreCantonMunicipalOverviewAsync(CantonMunicipalOverview model);
    Task<TableOverviewReadModel> GetCantonMunicipalOverviewsAsync(int electionYear, int municipalityCode);
    Task StoreCantonMunicipalPartiesAsync(IEnumerable<CantonMunicipalParty> models);
    Task<TableCandidateReadModel> GetCantonMunicipalPartiesAsync(int electionYear, int municipalityCode);
    Task<IEnumerable<int>> GetElectoralUnitOverviewElectionYearsAsync(IEnumerable<int> cantonCodes);
    Task<IEnumerable<int>> GetElectoralUnitPartyElectionYearsAsync(IEnumerable<int> cantonCodes);
    Task<IEnumerable<int>> GetMunicipalOverviewElectionYearsAsync(IEnumerable<int> municipalityCodes);
    Task<IEnumerable<int>> GetMunicipalPartyElectionYearsAsync(IEnumerable<int> municipalityCodes);
    Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync();
}
