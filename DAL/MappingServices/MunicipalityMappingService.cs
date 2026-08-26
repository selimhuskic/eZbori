using Application.Services;
using Contracts.Dtos.LocalElections.CandidateSpecific;
using Contracts.Dtos.LocalElections.CouncilSpecific;
using DAL.Mapping;

namespace DAL.MappingServices;

public class MunicipalityMappingService(EZboriMapper mapper) : IMunicipalityMappingService
{
    public IEnumerable<MunicipalityCandidateDetails> MapMunicipalityCandidateDetails(
        IEnumerable<MunicipalityCandidateDetailsDto> municipalityCandidateDetails, int electionYear, int municipalityCode)
    {
        return municipalityCandidateDetails.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }

    public MunicipalityCandidateOverview MapMunicipalityCandidateOverview(
        MunicipalityCandidateOverviewDto municipalityCandidateDetails, int electionYear, int municipalityCode)
    {
        var result = mapper.Map(municipalityCandidateDetails);
        result.ElectionYear = electionYear;
        result.MunicipalityCode = municipalityCode;
        return result;
    }

    public MunicipalityCouncilOverview MapMunicipalityCouncilOverview(
        MunicipalityCouncilOverviewDto municipalityCouncilDetails, int electionYear, int municipalityCode)
    {
        var result = mapper.Map(municipalityCouncilDetails);
        result.ElectionYear = electionYear;
        result.MunicipalityCode = municipalityCode;
        return result;
    }

    public IEnumerable<MunicipalityCouncilParty> MapMunicipalityCouncilParties(
        IEnumerable<MunicipalityCouncilPartyDto> municipalityCouncilParties, int electionYear, int municipalityCode)
    {
        return municipalityCouncilParties.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }

    public IEnumerable<MunicipalityCouncilMinority> MapMunicipalityCouncilMinorities(
        IEnumerable<MunicipalityCouncilMinorityDto> municipalityCouncilMinorities, int electionYear, int municipalityCode)
    {
        return municipalityCouncilMinorities.Select(dto =>
        {
            var item = mapper.Map(dto);
            item.ElectionYear = electionYear;
            item.MunicipalityCode = municipalityCode;
            return item;
        }).ToList();
    }
}
