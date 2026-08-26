using Contracts.Dtos.GeneralElections.PresidencySpecific;

namespace External.CentralElectionCommiteeHttpClients
{
    public interface IPresidencyClient
    {
        Task<PresidencyResultsDto[]> GetPresidentialResultsAsync(string uri);
        Task<PresidencyOverviewDto> GetPresidentialOverviewAsync(string uri);
        Task<PresidencyResultsMunicipalLevelDto[]> GetPresidentialResultsMunicipalAsync(string uri);
        Task<PresidencyOverviewMunicipalLevelDto> GetPresidencyMunicipalOverviewAsync(string uri);
    }
}
