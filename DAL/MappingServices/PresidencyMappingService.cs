using Application.Services;
using Contracts.Dtos.GeneralElections.PresidencySpecific;
using DAL.Mapping;

namespace DAL.MappingServices;

public class PresidencyMappingService(EZboriMapper mapper) : IPresidencyMappingService
{
    public IEnumerable<PresidencyMunicipalResults> MapPresidencyMunicipalResults(
        IEnumerable<PresidencyResultsMunicipalLevelDto> presidencyResultsMunicipal, int electionYear, int municipalityCode)
    {
        return presidencyResultsMunicipal.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }

    public IEnumerable<PresidencyResults> MapPresidencyResults(
        IEnumerable<PresidencyResultsDto> presidencyResultsDto, int electionYear, Constituency constituency)
    {
        return presidencyResultsDto.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.Constituency = constituency;
            return item;
        }).ToList();
    }

    public PresidencyMunicipalOverview MapPresidencyMunicipalOverview(
        PresidencyOverviewMunicipalLevelDto presidencyMunicipalOverview, int electionYear, Entity entity, int municipalityCode)
    {
        var result = mapper.Map(presidencyMunicipalOverview);
        result.ElectionYear = electionYear;
        result.Entity = entity;
        result.MunicipalityCode = municipalityCode;
        return result;
    }

    public PresidencyOverview MapPresidencyOverview(PresidencyOverviewDto presidencyOverviewDto, int electionYear, Entity entity)
    {
        var result = mapper.Map(presidencyOverviewDto);
        result.ElectionYear = electionYear;
        result.Entity = entity;
        return result;
    }
}
