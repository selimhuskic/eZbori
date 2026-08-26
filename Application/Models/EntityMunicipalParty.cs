namespace Application.Models;

public class EntityMunicipalParty
{
    public int Id { get; set; }
    public int ElectionYear { get; set; }
    public int MunicipalityCode { get; set; }
    public string Code { get; set; }
    public int ElectoralUnitPartyResultId { get; set; }
    public string Name { get; set; }
    public decimal Percentage { get; set; }
    public int TotalVotes { get; set; }
}
