using Application.Enum;
using Application.Models;
using Contracts.Dtos.GeneralElections.PresidencySpecific;

namespace Application.Services
{
    public interface IPresidencyMappingService
    {
        IEnumerable<PresidencyResults> MapPresidencyResults(IEnumerable<PresidencyResultsDto> presidencyResultsDto, int electionYear, Constituency constituency);
        IEnumerable<PresidencyMunicipalResults> MapPresidencyMunicipalResults(IEnumerable<PresidencyResultsMunicipalLevelDto> presidencyResultsMunicipal, int electionYear, int municipalityCode);
        PresidencyOverview MapPresidencyOverview(PresidencyOverviewDto presidencyOverviewDto, int electionYear, Entity entity);
        PresidencyMunicipalOverview MapPresidencyMunicipalOverview(PresidencyOverviewMunicipalLevelDto presidencyMunicipalOverviewDto, int electionYear, Entity entity, int municipalityCode);
    }
}
