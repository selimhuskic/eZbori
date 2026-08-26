using Contracts.Dtos.GeneralElections.StateSpecific;

namespace External.CentralElectionCommiteeHttpClients
{
    public interface IStateClient
    {        
        Task<StateElectoralUnitOverviewDto> GetElectoralUnitOverviewAsync(string url);
        Task<StateElectoralUnitPartyDto[]> GetElectoralUnitPartiesAsync(string uri);
        Task<StateMunicipalOverviewDto> GetStateMunicipalOverviewsAsync(string uri);
        Task<StateMunicipalPartyDto[]> GetStateMunicipalPartiesAsync(string uri);
    }
}
