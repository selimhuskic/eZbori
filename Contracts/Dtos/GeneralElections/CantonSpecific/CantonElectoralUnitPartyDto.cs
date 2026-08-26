namespace Contracts.Dtos.GeneralElections.CantonSpecific
{
    public class CantonElectoralUnitPartyDto
    {
        public int AbsenceAndMobileTeamVotes { get; set; }
        public string Code { get; set; }
        public int ConfirmedVotes { get; set; }
        public int ElectoralUnitParentPartyResultId { get; set; }
        public int Mandates { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int PostOfficeVotes { get; set; }
        public int RegularVotes { get; set; }
        public int TotalVotes { get; set; }
    }
}
