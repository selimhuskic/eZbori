namespace Application.Models;

public class ForecastedResult
{
    public int Id { get; set; }
    public short? MunicipalCode { get; set; }
    public short? CantonCode { get; set; }
    public short? EntityCode { get; set; }
    public bool IsStateCouncil { get; set; }
    public double? ForecastedNumberOfVotes { get; set; }
    public string PartyName { get; set; } = string.Empty;
    public short? ElectionYear { get; set; }
}
