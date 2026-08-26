namespace Application.Models;

public class EntityMunicipalOverview
{
    public int Id { get; set; }
    public int ElectionYear { get; set; }
    public int MunicipalityCode { get; set; }
    public int ElectoralUnitId { get; set; }
    public int InvalidBlankBallots { get; set; }
    public int InvalidOthersBallots { get; set; }
    public int NumberOfCandidates { get; set; }
    public int NumberOfVoters { get; set; }
    public int NumberOfParties { get; set; }
    public decimal PercentageProcessedPollingStations { get; set; }
    public decimal PercentageTotalNoVotes { get; set; }
    public decimal PercentageTotalVotes { get; set; }
    public decimal ProcessedInvalidBlankBallots { get; set; }
    public decimal ProcessedInvalidOthersBallots { get; set; }
    public decimal ProcessedPollingStations { get; set; }
    public decimal ProcessedTotalInvalidVotes { get; set; }
    public decimal ProcessedValidVotes { get; set; }
    public int TotalInvalidVotes { get; set; }
    public int TotalNoVotes { get; set; }
    public int TotalPollingStations { get; set; }
    public int TotalVotes { get; set; }
    public int ValidVotes { get; set; }
}
