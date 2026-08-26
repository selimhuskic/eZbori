namespace Application.Models;

public class EntityElectoralUnitParty
{
    public int Id { get; set; }
    public int ElectionYear { get; set; }
    public int ElectoralUnitCode { get; set; }
    public int AbsenceAndMobileTeamVotes { get; set; }
    public string Code { get; set; }
    public int CompensationMandates { get; set; }
    public int ConfirmedVotes { get; set; }
    public int ElectoralUnitParentPartyResultId { get; set; }
    public string PartyName { get; set; }
    public decimal Percentage { get; set; }
    public int PostOfficeVotes { get; set; }
    public int RegularMandates { get; set; }
    public int RegularVotes { get; set; }
    public int TotalMandates { get; set; }
    public int TotalVotes { get; set; }
}
