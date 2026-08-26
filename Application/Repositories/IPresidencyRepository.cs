using Application.Enum;
using Application.Models;
using Application.Models.MachineLearning;
using Application.ReadModels;

namespace Application.Repositories;

public interface IPresidencyRepository
{
    Task StoreOverviewAsync(PresidencyMunicipalOverview overview);
    IEnumerable<PresidencyOverview> GetAllOverviews();
    Task<TableOverviewReadModel> GetPresidencyMunicipalOverviewsAsync(int electionYear, int municipalityCode);
    Task StorePresidencyOverviewAsync(PresidencyOverview presidencyResults);
    Task<TableOverviewReadModel> GetPresidencyOverviewsAsync(int electionYear, Entity entity);
    Task StorePresidencyResultsMunicipalAsync(IEnumerable<PresidencyMunicipalResults> municipalResults);
    Task<TableCandidateReadModel> GetPresidencyMunicipalResultsAsync(int electionYear, int municipalityCode);
    Task StorePresidencyResultsAsync(IEnumerable<PresidencyResults> presidencyResults);
    Task<TableCandidateReadModel> GetPresidencyResultsAsync(int electionYear, Constituency constituency);
    Task<IEnumerable<SearchRecommendationDto>> GetSearchRecommendationAsync();
    Task<IEnumerable<int>> GetPresidencyResultsElectionYearsAsync(Constituency constituency);
    Task<IEnumerable<int>> GetPresidencyOverviewMunicipalElectionYearsAsync();
    Task<IEnumerable<int>> GetPresidencyResultsMunicipalLevelElectionYearsAsync();
}
