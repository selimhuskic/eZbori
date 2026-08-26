namespace Application.Models
{
    public class MunicipalityCouncilMinority
    {
        public int Id { get; set; }
        public int ElectionYear { get; set; }
        public int MunicipalityCode { get; set; }
        public string Code { get; set; }
        public bool HaveMandates { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int AbsenceAndMobileTeamVotes { get; set; }
        public int ConfirmedVotes { get; set; }
        public int PostOfficeVotes { get; set; }
        public int TotalVotes { get; set; }
        public int RegularVotes { get; set; }
    }
}