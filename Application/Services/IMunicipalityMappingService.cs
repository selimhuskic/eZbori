using Application.Models;
using Contracts.Dtos.LocalElections.CandidateSpecific;
using Contracts.Dtos.LocalElections.CouncilSpecific;

namespace Application.Services
{
    public interface IMunicipalityMappingService
    {
        IEnumerable<MunicipalityCandidateDetails> MapMunicipalityCandidateDetails(IEnumerable<MunicipalityCandidateDetailsDto> municipalityCandidateDetails, int electionYear, int municipalityCode);
        MunicipalityCandidateOverview MapMunicipalityCandidateOverview(MunicipalityCandidateOverviewDto municipalityCandidateDetails, int electionYear, int municipalityCode);
        MunicipalityCouncilOverview MapMunicipalityCouncilOverview(MunicipalityCouncilOverviewDto municipalityCandidateDetails, int electionYear, int municipalityCode);
        IEnumerable<MunicipalityCouncilParty> MapMunicipalityCouncilParties(IEnumerable<MunicipalityCouncilPartyDto> municipalityCouncilParties, int electionYear, int municipalityCode);
        IEnumerable<MunicipalityCouncilMinority> MapMunicipalityCouncilMinorities(IEnumerable<MunicipalityCouncilMinorityDto> municipalityCouncilMinorities, int electionYear, int municipalityCode);
    }
}
