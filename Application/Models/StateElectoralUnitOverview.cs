namespace Application.Models;

public record StateElectoralUnitOverview
{
    public int Id { get; set; }
    public int ElectionYear { get; set; }
    public int AbsenceAndMobileTeamVotes { get; set; }
    public int ConfirmedVotes { get; set; }
    public DateTime DataFrom { get; set; }
    public int ElectoralUnit { get; set; }
    public int InvalidBlankBallots { get; set; }
    public int InvalidOthersBallots { get; set; }
    public int CandidatesNumber { get; set; }
    public int NumberOfVoters { get; set; }
    public int PartyNumber { get; set; }
    public decimal PercentageProcessedPollingStations { get; set; }
    public decimal PercentageTotalNoVotes { get; set; }
    public decimal PercentageTotalVotes { get; set; }
    public decimal PostOfficeVotes { get; set; }
    public decimal ProcessedAbsenceAndMobileTeamVotes { get; set; }
    public decimal ProcessedConfirmedVotes { get; set; }
    public decimal ProcessedInvalidBlankBallots { get; set; }
    public decimal ProcessedInvalidOthersBallots { get; set; }
    public int ProcessedPollingStations { get; set; }
    public decimal ProcessedPostOfficeVotes { get; set; }
    public decimal ProcessedRegularVotes { get; set; }
    public decimal ProcessedValidVotes { get; set; }
    public int RegularVotes { get; set; }
    public int TotalInvalidVotes { get; set; }
    public int TotalMandates { get; set; }
    public int TotalNoVotes { get; set; }
    public int TotalPollingStations { get; set; }
    public int TotalVotes { get; set; }
    public int ValidVotes { get; set; }
}
