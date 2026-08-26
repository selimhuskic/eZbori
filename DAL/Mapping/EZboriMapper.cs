using Contracts.Dtos.GeneralElections.CantonSpecific;
using Contracts.Dtos.GeneralElections.EntitySpecific;
using Contracts.Dtos.GeneralElections.PresidencySpecific;
using Contracts.Dtos.GeneralElections.StateSpecific;
using Contracts.Dtos.LocalElections.CandidateSpecific;
using Contracts.Dtos.LocalElections.CouncilSpecific;
using Riok.Mapperly.Abstractions;

namespace DAL.Mapping;

[Mapper]
public partial class EZboriMapper
{
    // Presidency
    [MapProperty(nameof(PresidencyResultsDto.HaveMandates), nameof(PresidencyResults.MandateWon))]
    [MapProperty(nameof(PresidencyResultsDto.Name), nameof(PresidencyResults.CandidateName))]
    public partial PresidencyResults Map(PresidencyResultsDto dto);

    [MapProperty(nameof(PresidencyOverviewDto.NumberPatry), nameof(PresidencyOverview.PartyNumber))]
    [MapProperty(nameof(PresidencyOverviewDto.NumberOfVoters), nameof(PresidencyOverview.TotalVoters))]
    [MapProperty(nameof(PresidencyOverviewDto.PercentageProcessedPollingStations), nameof(PresidencyOverview.ProcessedPollingStationsPercentage))]
    [MapProperty(nameof(PresidencyOverviewDto.NumberCandidates), nameof(PresidencyOverview.CandidatesNumber))]
    public partial PresidencyOverview Map(PresidencyOverviewDto dto);

    public partial PresidencyMunicipalResults Map(PresidencyResultsMunicipalLevelDto dto);

    [MapProperty(nameof(PresidencyOverviewMunicipalLevelDto.NumberOfVoters), nameof(PresidencyMunicipalOverview.TotalVoters))]
    [MapProperty(nameof(PresidencyOverviewMunicipalLevelDto.NumberParty), nameof(PresidencyMunicipalOverview.PartyNumber))]
    [MapProperty(nameof(PresidencyOverviewMunicipalLevelDto.PercentageProcessedPollingStations), nameof(PresidencyMunicipalOverview.ProcessedPollingStationsPercentage))]
    [MapProperty(nameof(PresidencyOverviewMunicipalLevelDto.NumberCandidates), nameof(PresidencyMunicipalOverview.CandidatesNumber))]
    public partial PresidencyMunicipalOverview Map(PresidencyOverviewMunicipalLevelDto dto);

    // State
    [MapProperty(nameof(StateElectoralUnitOverviewDto.NumberPatry), nameof(StateElectoralUnitOverview.PartyNumber))]
    [MapProperty(nameof(StateElectoralUnitOverviewDto.NumberCandidates), nameof(StateElectoralUnitOverview.CandidatesNumber))]
    [MapProperty(nameof(StateElectoralUnitOverviewDto.ElectoralUnitParentId), nameof(StateElectoralUnitOverview.ElectoralUnit))]
    public partial StateElectoralUnitOverview Map(StateElectoralUnitOverviewDto dto);

    [MapProperty(nameof(StateElectoralUnitPartyDto.Name), nameof(StateElectoralUnitParty.PartyName))]
    public partial StateElectoralUnitParty Map(StateElectoralUnitPartyDto dto);

    [MapProperty(nameof(StateMunicipalOverviewDto.NumberPatry), nameof(StateMunicipalOverview.NumberOfParties))]
    [MapProperty(nameof(StateMunicipalOverviewDto.NumberCandidates), nameof(StateMunicipalOverview.NumberOfCandidates))]
    public partial StateMunicipalOverview Map(StateMunicipalOverviewDto dto);

    public partial StateMunicipalParty Map(StateMunicipalPartyDto dto);

    // Entity
    [MapProperty(nameof(EntityElectoralUnitOverviewDto.NumberParty), nameof(EntityElectoralUnitOverview.NumberOfParties))]
    [MapProperty(nameof(EntityElectoralUnitOverviewDto.NumberCandidates), nameof(EntityElectoralUnitOverview.NumberOfCandidates))]
    public partial EntityElectoralUnitOverview Map(EntityElectoralUnitOverviewDto dto);

    [MapProperty(nameof(EntityElectoralUnitPartyDto.Name), nameof(EntityElectoralUnitParty.PartyName))]
    public partial EntityElectoralUnitParty Map(EntityElectoralUnitPartyDto dto);

    [MapProperty(nameof(EntityPresidentOverviewDto.NumberMunicipality), nameof(EntityPresidentOverview.NumberOfMunicipalities))]
    [MapProperty(nameof(EntityPresidentOverviewDto.NumberCandidates), nameof(EntityPresidentOverview.NumberOfCandidates))]
    public partial EntityPresidentOverview Map(EntityPresidentOverviewDto dto);

    public partial EntityPresidentMunicipalCandidate Map(EntityPresidentMunicipalCandidateDto dto);

    [MapProperty(nameof(EntityMunicipalOverviewDto.NumberCandidates), nameof(EntityMunicipalOverview.NumberOfCandidates))]
    [MapProperty(nameof(EntityMunicipalOverviewDto.NumberParty), nameof(EntityMunicipalOverview.NumberOfParties))]
    public partial EntityMunicipalOverview Map(EntityMunicipalOverviewDto dto);

    public partial EntityMunicipalParty Map(EntityMunicipalPartyDto dto);

    // Canton
    [MapProperty(nameof(CantonElectoralUnitOverviewDto.NumberCandidates), nameof(CantonElectoralUnitOverview.NumberOfCandidates))]
    [MapProperty(nameof(CantonElectoralUnitOverviewDto.NumberParty), nameof(CantonElectoralUnitOverview.NumberOfParties))]
    public partial CantonElectoralUnitOverview Map(CantonElectoralUnitOverviewDto dto);

    public partial CantonElectoralUnitParty Map(CantonElectoralUnitPartyDto dto);

    [MapProperty(nameof(CantonMunicipalOverviewDto.NumberCandidates), nameof(CantonMunicipalOverview.NumberOfCandidates))]
    [MapProperty(nameof(CantonMunicipalOverviewDto.NumberParty), nameof(CantonMunicipalOverview.NumberOfParties))]
    public partial CantonMunicipalOverview Map(CantonMunicipalOverviewDto dto);

    public partial CantonMunicipalParty Map(CantonMunicipalPartyDto dto);

    // Municipality
    [MapProperty(nameof(MunicipalityCandidateDetailsDto.HaveManadates), nameof(MunicipalityCandidateDetails.HaveMandates))]
    public partial MunicipalityCandidateDetails Map(MunicipalityCandidateDetailsDto dto);

    public partial MunicipalityCandidateOverview Map(MunicipalityCandidateOverviewDto dto);

    public partial MunicipalityCouncilOverview Map(MunicipalityCouncilOverviewDto dto);

    public partial MunicipalityCouncilParty Map(MunicipalityCouncilPartyDto dto);

    public partial MunicipalityCouncilMinority Map(MunicipalityCouncilMinorityDto dto);
}
