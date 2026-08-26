using Application.Models;
using Contracts.Dtos.GeneralElections.StateSpecific;

namespace Application.Services;

public interface IStateMappingService
{
    StateElectoralUnitOverview MapStateElectoralUnitOverview(StateElectoralUnitOverviewDto presidencyMunicipalOverviewDto, int electionYear);
    IEnumerable<StateElectoralUnitParty> MapStateElectoralUnitParties(IEnumerable<StateElectoralUnitPartyDto> presidencyMunicipalPartiesDto, int electionYear, int electoralUnit);
    StateMunicipalOverview MapStateMunicipalOverview(StateMunicipalOverviewDto stateMunicipalOverviewDto, int electionYear, int municipalityCode);
    IEnumerable<StateMunicipalParty> MapStateMunicipalParties(StateMunicipalPartyDto[] stateMunicipalPartiesDto, int electionYear, int municipalityCode);
}
