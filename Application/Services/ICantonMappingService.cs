using Application.Models;
using Contracts.Dtos.GeneralElections.CantonSpecific;

namespace Application.Services;

public interface ICantonMappingService
{
    CantonElectoralUnitOverview MapCantonElectoralUnitOverview(CantonElectoralUnitOverviewDto cantonElectoralUnitOverview, int electionYear, int cantonCode);
    IEnumerable<CantonElectoralUnitParty> MapCantonElectoralUnitParties(CantonElectoralUnitPartyDto[] cantonElectoralUnitParties, int electionYear, int cantonCode);
    CantonMunicipalOverview MapCantonMunicipalOverview(CantonMunicipalOverviewDto cantonMunicipalOverviewDto, int electionYear, int cantonCode, int municipalityCode);
    IEnumerable<CantonMunicipalParty> MapCantonMunicipalParties(CantonMunicipalPartyDto[] cantonMunicipalParties, int electionYear, int cantonCode, int municipalityCode);
}
