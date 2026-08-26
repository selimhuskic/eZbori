namespace Contracts.Dtos.GeneralElections.StateSpecific
{
    public class StateElectoralUnitPartyDto
    {
        public int AbsenceAndMobileTeamVotes { get; set; }
        public string Code { get; set; }
        public int CompensationMandates { get; set; }
        public int ConfirmedVotes { get; set; }
        public int ElectoralUnitParentPartyResultId { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int PostOfficeVotes { get; set; }
        public int RegularMandates { get; set; }
        public int RegularVotes { get; set; }
        public int TotalVotes { get; set; }
        public int ElectoralUnitParentId { get; set; }
    }
}
