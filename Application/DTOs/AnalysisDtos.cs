using Application.Enum;
using System.ComponentModel;

namespace Application.DTOs;

public record AnalysisRequest(
    bool IsLoggedIn,
    ElectionType ElectionType,
    AnalysisSubject AnalysisSubject,
    int SelectedYear,
    int? ElectoralUnit,
    int? MunicipalityCode);


public abstract record BaseResultsOverview(
    int ElectoralUnit,
    string ElectoralUnitName,
    int ElectionYear,
    ElectionType ElectionType,
    AnalysisSubject AnalysisSubject);

public record OverviewResults(
    int ElectoralUnit,
    string ElectoralUnitName,
    int ElectionYear,
    ElectionType ElectionType,
    AnalysisSubject AnalysisSubject,
    int NumberOfVoters,
    int RegularVotes,
    int TotalMandates,
    int TotalVotes,
    int TotalNoVotes,
    int ValidVotes,
    int TotalInvalidVotes,
    int InvalidBlankBallots,
    int InvalidOtherBallots,
    decimal PercentageTotalVotes,
    decimal PercentageTotalNoVotes,
    int NumberOfCandidates,
    decimal ProcessedRegularVotes,
    decimal ProcessedValidVotes,
    decimal ProcessedPostOfficeVotes,
    decimal ProcessedAbsenceAndMobileTeamVotes
    ) : BaseResultsOverview(ElectoralUnit, ElectoralUnitName, ElectionYear, ElectionType, AnalysisSubject);


public record PartiesResults(
    int ElectoralUnit,
    string ElectoralUnitName,
    int ElectionYear,
    ElectionType ElectionType,
    AnalysisSubject AnalysisSubject,
    int AbsenceAndMobileTeamVotes,
    string Code,
    int CompensationMandates,
    int ConfirmedVotes,
    string PartyName,
    decimal Percentage,
    int PostOfficeVotes,
    int RegularMandates,
    int RegularVotes,
    int TotalVotes) : BaseResultsOverview(ElectoralUnit, ElectoralUnitName, ElectionYear, ElectionType, AnalysisSubject);

public record PartiesExportRow
{
    [DisplayName("Naziv stranke")]
    public string PartyName { get; init; }

    [DisplayName("Ukupni glasovi")]
    public int TotalVotes { get; init; }

    [DisplayName("Postotak")]
    public decimal Percentage { get; init; }

    [DisplayName("Redovni mandati")]
    public int RegularMandates { get; init; }

    [DisplayName("Kompenzacioni mandati")]
    public int CompensationMandates { get; init; }

    [DisplayName("Regularni glasovi")]
    public int RegularVotes { get; init; }

    public PartiesExportRow(PartiesResults p)
    {
        PartyName            = p.PartyName;
        TotalVotes           = p.TotalVotes;
        Percentage           = p.Percentage;
        RegularMandates      = p.RegularMandates;
        CompensationMandates = p.CompensationMandates;
        RegularVotes         = p.RegularVotes;
    }
}