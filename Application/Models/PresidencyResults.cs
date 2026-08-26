using Application.Enum;

namespace Application.Models;

public record PresidencyResults
{
    public int Id { get; set; }
    public int ElectionYear { get; set; }
    public string CandidateName { get; set; }
    public Constituency Constituency { get; set; }
    public string Code { get; set; }
    public int TotalVotes { get; set; }
    public decimal Percentage { get; set; }
    public int AbsenceAndMobileTeamVotes { get; set; }
    public int RegularVotes { get; set; }
    public int ConfirmedVotes { get; set; }
    public bool MandateWon { get; set; }
}
