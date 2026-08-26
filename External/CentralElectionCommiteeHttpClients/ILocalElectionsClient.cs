using Contracts.Dtos.LocalElections.CandidateSpecific;
using Contracts.Dtos.LocalElections.CouncilSpecific;

namespace External.CentralElectionCommiteeHttpClients
{

    public interface ILocalElectionsClient
    {
        Task<IEnumerable<MunicipalityCandidateDetailsDto>> GetMunicipalityCandidateDetailsAsync(string url);
        Task<MunicipalityCandidateOverviewDto?> GetMunicipalityCandidateOverviewAsync(string url);
        Task<MunicipalityCouncilOverviewDto?> GetMunicipalityCouncilOverviewAsync(string url);
        Task<IEnumerable<MunicipalityCouncilPartyDto>> GetMunicipalityCouncilPartiesAsync(string url);
        Task<IEnumerable<MunicipalityCouncilMinorityDto>> GetMunicipalityCouncilMinoritiesAsync(string url);
    }
}