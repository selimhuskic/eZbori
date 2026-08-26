using Application.Services;
using Contracts.Dtos.GeneralElections.EntitySpecific;
using DAL.Mapping;

namespace DAL.MappingServices;

public class EntityMappingService(EZboriMapper mapper) : IEntityMappingService
{
    public EntityElectoralUnitOverview MapEntityElectoralUnitOverview(
        EntityElectoralUnitOverviewDto entityElectoralUnitOverviewDto, int electionYear, int entityElectoralUnit)
    {
        var result = mapper.Map(entityElectoralUnitOverviewDto);
        result.ElectionYear = electionYear;
        result.ElectoralUnitCode = entityElectoralUnit;
        return result;
    }

    public IEnumerable<EntityElectoralUnitParty> MapEntityElectoralUnitParties(
        IEnumerable<EntityElectoralUnitPartyDto> entityElectoralUnitPartiesDto, int electionYear, int entityElectoralUnit)
    {
        return entityElectoralUnitPartiesDto.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.ElectoralUnitCode = entityElectoralUnit;
            return item;
        }).ToList();
    }

    public EntityPresidentOverview MapEntityPresidentOverview(
        EntityPresidentOverviewDto entityPresidentOverviewDto, int electionYear, Entity entity)
    {
        var result = mapper.Map(entityPresidentOverviewDto);
        result.ElectionYear = electionYear;
        result.Entity = entity;
        return result;
    }

    public IEnumerable<EntityPresidentMunicipalCandidate> MapPresidentMunicipal(
        EntityPresidentMunicipalCandidateDto[] entityPresidentMunicipal, int electionYear, int municipalityCode)
    {
        return entityPresidentMunicipal.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }

    public EntityMunicipalOverview MapEntityMunicipalOverview(
        EntityMunicipalOverviewDto entityPresidentOverview, int electionYear, int municipalityCode)
    {
        var result = mapper.Map(entityPresidentOverview);
        result.ElectionYear = electionYear;
        result.MunicipalityCode = municipalityCode;
        return result;
    }

    public IEnumerable<EntityMunicipalParty> MapEntityMunicipalParties(
        EntityMunicipalPartyDto[] entityMunicipalParties, int electionYear, int municipalityCode)
    {
        return entityMunicipalParties.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }
}
