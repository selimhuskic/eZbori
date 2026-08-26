using Application.Enum;
using Application.Models;
using Contracts.Dtos.GeneralElections.EntitySpecific;

namespace Application.Services;

public interface IEntityMappingService
{
    EntityElectoralUnitOverview MapEntityElectoralUnitOverview(EntityElectoralUnitOverviewDto municipalityResultsDto, int electionYear, int entityElectoralUnit);
    IEnumerable<EntityElectoralUnitParty> MapEntityElectoralUnitParties(IEnumerable<EntityElectoralUnitPartyDto> entityElectoralUnitPartiesDto, int electionYear, int entityElectoralUnit);
    EntityPresidentOverview MapEntityPresidentOverview(EntityPresidentOverviewDto entityPresidentOverviewDto, int electionYear, Entity entity);
    IEnumerable<EntityPresidentMunicipalCandidate> MapPresidentMunicipal(EntityPresidentMunicipalCandidateDto[] entityPresidentMunicipal, int electionYear, int municipalityCode);
    EntityMunicipalOverview MapEntityMunicipalOverview(EntityMunicipalOverviewDto entityPresidentOverview, int electionYear, int municipalityCode);
    IEnumerable<EntityMunicipalParty> MapEntityMunicipalParties(EntityMunicipalPartyDto[] entityMunicipalParties, int electionYear, int municipalityCode);
}
