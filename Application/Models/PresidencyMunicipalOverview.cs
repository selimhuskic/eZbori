using Application.Enum;

namespace Application.Models
{
    public class PresidencyMunicipalOverview
    {
        public int Id { get; set; }
        public int ElectionYear { get; set; }
        public Entity Entity { get; set; }
        public int InvalidBlankBallots { get; set; }
        public int InvalidOthersBallots { get; set; }
        public int TotalVoters { get; set; }
        public int PartyNumber { get; set; }
        public int TotalVotes { get; set; }
        public int TotalNoVotes { get; set; }
        public int ValidVotes { get; set; }
        public int TotalInvalidVotes { get; set; }
        public decimal ProcessedPollingStationsPercentage { get; set; }
        public decimal PercentageTotalVotes { get; set; }
        public decimal PercentageTotalNoVotes { get; set; }
        public decimal ProcessedTotalInvalidVotes { get; set; }
        public decimal ProcessedInvalidBlankBallots { get; set; }
        public decimal ProcessedInvalidOthersBallots { get; set; }
        public int TotalPollingStations { get; set; }
        public int ProcessedPollingStations { get; set; }
        public int CandidatesNumber { get; set; }
        public int MunicipalityCode { get; set; }
    }
}
