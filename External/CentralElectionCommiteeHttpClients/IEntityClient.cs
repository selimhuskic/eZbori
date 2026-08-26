using Contracts.Dtos.GeneralElections.EntitySpecific;

namespace External.CentralElectionCommiteeHttpClients
{
    public interface IEntityClient
    {
        Task<EntityElectoralUnitOverviewDto> GetEntityElectoralUnitOverviewAsync(string uri);
        Task<EntityElectoralUnitPartyDto[]> GetEntityElectoralUnitPartiesAsync(string uri);
        Task<EntityPresidentOverviewDto> GetEntityPresidentOverviewAsync(string uri);
        Task<EntityPresidentMunicipalCandidateDto[]> GetEntityPresidentMunicipalCandidateAsync(string uri);
        Task<EntityMunicipalOverviewDto> GetEntityMunicipalOverviewAsync(string uri);
        Task<EntityMunicipalPartyDto[]> GetEntityMunicipalPartyAsync(string url);
    }
}
