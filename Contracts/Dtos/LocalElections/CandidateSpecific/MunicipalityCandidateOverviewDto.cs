namespace Contracts.Dtos.LocalElections.CandidateSpecific
{

    public class MunicipalityCandidateOverviewDto
    {
        public int InvalidOtherBallots { get; set; }
        public int NumberOfCandidates { get; set; }
        public decimal ProcessedConfirmedVotes { get; set; }
        public int AbsenceAndMobileTeamVotes { get; set; }
        public int ConfirmedVotes { get; set; }
        public int PostOfficeVotes { get; set; }
        public int TotalVotes { get; set; }
        public int RegularVotes { get; set; }
        public decimal ProcessedAbsenceAndMobileTeamVotes { get; set; }
        public int InvalidBlankBallots { get; set; }
        public int NumberOfVoters { get; set; }
        public decimal PercentageTotalNoVotes { get; set; }
        public decimal PercentageTotalVotes { get; set; }
        public int TotalNoVotes { get; set; }
        public int TotalPollingStations { get; set; }
        public int ValidVotes { get; set; }
        public int TotalInvalidVotes { get; set; }
        public decimal ProcessedPollingStations { get; set; }
        public decimal ProcessedPostOfficeVotes { get; set; }
        public decimal ProcessedRegularVotes { get; set; }
        public decimal ProcessedValidVotes { get; set; }
        public decimal ProcessedInvalidBlankBallots { get; set; }
        public decimal ProcessedInvalidOthersBallots { get; set; }
        public decimal PercentageProcessedPollingStations { get; set; }
    }
}