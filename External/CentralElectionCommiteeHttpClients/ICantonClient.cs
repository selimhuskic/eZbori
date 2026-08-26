using Contracts.Dtos.GeneralElections.CantonSpecific;

namespace External.CentralElectionCommiteeHttpClients
{
    public interface ICantonClient
    {
        Task<CantonElectoralUnitOverviewDto> GetCantonElectoralUnitOverviewAsync(string url);
        Task<CantonElectoralUnitPartyDto[]> GetCantonElectoralUnitPartiesAsync(string url);
        Task<CantonMunicipalOverviewDto> GetCantonMunicipalOverviewAsync(string url);
        Task<CantonMunicipalPartyDto[]> GetCantonMunicipalPartiesAsync(string url);
    }
}