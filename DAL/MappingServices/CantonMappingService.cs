using Application.Services;
using Contracts.Dtos.GeneralElections.CantonSpecific;
using DAL.Mapping;

namespace DAL.MappingServices;

public class CantonMappingService(EZboriMapper mapper) : ICantonMappingService
{
    public CantonElectoralUnitOverview MapCantonElectoralUnitOverview(
        CantonElectoralUnitOverviewDto cantonElectoralUnitOverview, int electionYear, int cantonCode)
    {
        var result = mapper.Map(cantonElectoralUnitOverview);
        result.ElectionYear = electionYear;
        result.CantonElectoralUnitCode = cantonCode;
        return result;
    }

    public IEnumerable<CantonElectoralUnitParty> MapCantonElectoralUnitParties(
        CantonElectoralUnitPartyDto[] cantonElectoralUnitParties, int electionYear, int cantonCode)
    {
        return cantonElectoralUnitParties.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.CantonElectoralUnitCode = cantonCode;
            return item;
        }).ToList();
    }

    public CantonMunicipalOverview MapCantonMunicipalOverview(
        CantonMunicipalOverviewDto cantonMunicipalOverviewDto, int electionYear, int cantonCode, int municipalityCode)
    {
        var result = mapper.Map(cantonMunicipalOverviewDto);
        result.ElectionYear = electionYear;
        result.CantonCode = cantonCode;
        result.MunicipalityCode = municipalityCode;
        return result;
    }

    public IEnumerable<CantonMunicipalParty> MapCantonMunicipalParties(
        CantonMunicipalPartyDto[] cantonMunicipalParties, int electionYear, int cantonCode, int municipalityCode)
    {
        return cantonMunicipalParties.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.CantonCode = cantonCode;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }
}
