namespace Contracts.Dtos.GeneralElections.PresidencySpecific;

public record PresidencyResultsDto
{
    public int AbsenceAndMobileTeamVotes { get; set; }
    public string Code { get; set; }
    public int ConfirmedVotes { get; set; }
    public bool? HaveMandates { get; set; }
    public string Name { get; set; }
    public decimal Percentage { get; set; }
    public int PostOfficeVote { get; set; }
    public int RegularVotes { get; set; }
    public int TotalVotes { get; set; }
}
