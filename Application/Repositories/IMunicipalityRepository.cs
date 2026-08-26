using Application.Models;
using Application.Models.MachineLearning;
using Application.ReadModels;

namespace Application.Repositories;

public interface IMunicipalityRepository
{
    Task StoreMunicipalityCandidateDetailsAsync(IEnumerable<MunicipalityCandidateDetails> model);
    Task<TableCandidateReadModel> GetMunicipalityCandidateDetailsAsync(int electionYear, int municipalityCode);
    Task StoreMunicipalityCandidateOverviewAsync(MunicipalityCandidateOverview model);
    Task<TableOverviewReadModel> GetMunicipalityCandidateOverviewsAsync(int electionYear, int municipalityCode);
    Task StoreMunicipalityCouncilMinoritiesAsync(IEnumerable<MunicipalityCouncilMinority> models);
    Task<TableCandidateReadModel> GetMunicipalityCouncilMinoritiesAsync(int electionYear, int municipalityCode);
    Task StoreMunicipalityCouncilOverviewAsync(MunicipalityCouncilOverview model);
    Task<TableOverviewReadModel> GetMunicipalityCouncilOverviewsAsync(int electionYear, int municipalityCode);
    Task StoreMunicipalityCouncilPartiesAsync(IEnumerable<MunicipalityCouncilParty> models);
    Task<TableCandidateReadModel> GetMunicipalityCouncilPartiesAsync(int electionYear, int municipalityCode);
    Task<IEnumerable<int>> GetElectionYears();
    Task<IEnumerable<int>> GetCandidateDetailsElectionYearsAsync();
    Task<IEnumerable<int>> GetCandidateOverviewElectionYearsAsync();
    Task<IEnumerable<int>> GetMunicipalCouncilOverviewElectionYearsAsync();
    Task<IEnumerable<int>> GetMunicipalCouncilPartyElectionYearsAsync();
    Task<IEnumerable<int>> GetMunicipalCouncilMinirotiesElectionYearsAsync();
    Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync();
}
