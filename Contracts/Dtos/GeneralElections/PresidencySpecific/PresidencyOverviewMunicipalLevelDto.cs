namespace Contracts.Dtos.GeneralElections.PresidencySpecific
{
    public class PresidencyOverviewMunicipalLevelDto
    {
        public int InvalidBlankBallots { get; set; }
        public int InvalidOthersBallots { get; set; }
        public int NumberCandidates { get; set; }
        public int NumberOfVoters { get; set; }
        public int NumberParty { get; set; }
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
}
