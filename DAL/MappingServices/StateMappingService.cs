using Application.Services;
using Contracts.Dtos.GeneralElections.StateSpecific;
using DAL.Mapping;

namespace DAL.MappingServices;

public class StateMappingService(EZboriMapper mapper) : IStateMappingService
{
    public StateElectoralUnitOverview MapStateElectoralUnitOverview(
        StateElectoralUnitOverviewDto presidencyMunicipalOverview, int electionYear)
    {
        var result = mapper.Map(presidencyMunicipalOverview);
        result.ElectionYear = electionYear;
        return result;
    }

    public IEnumerable<StateElectoralUnitParty> MapStateElectoralUnitParties(
        IEnumerable<StateElectoralUnitPartyDto> presidencyMunicipalParties, int electionYear, int electoralUnit)
    {
        return presidencyMunicipalParties.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.ElectoralUnit = electoralUnit;
            return item;
        }).ToList();
    }

    public StateMunicipalOverview MapStateMunicipalOverview(
        StateMunicipalOverviewDto stateMunicipalOverviewDtos, int electionYear, int municipalityCode)
    {
        var result = mapper.Map(stateMunicipalOverviewDtos);
        result.ElectionYear = electionYear;
        result.MunicipalityCode = municipalityCode;
        return result;
    }

    public IEnumerable<StateMunicipalParty> MapStateMunicipalParties(
        StateMunicipalPartyDto[] stateMunicipalPartiesDtos, int electionYear, int municipalityCode)
    {
        return stateMunicipalPartiesDtos.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }
}
