namespace Contracts.Dtos.LocalElections.CouncilSpecific
{
    public class MunicipalityCouncilPartyDto
    {
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public string Code { get; set; }
        public int ElectoralUnitPartyResultId { get; set; }
        public int? Mandates { get; set; }
        public int AbsenceAndMobileTeamVotes { get; set; }
        public int ConfirmedVotes { get; set; }
        public int PostOfficeVotes { get; set; }
        public int TotalVotes { get; set; }
        public int RegularVotes { get; set; }
    }
}