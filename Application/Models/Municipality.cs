using Application.Enum;

namespace Application.Models;

public record Municipality
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Canton? Canton { get; set; }
    public Entity Entity { get; set; }
    public bool? District { get; set; }
    public StateParliamentElectoralUnit StateParliamentElectoralUnit { get; set; }
    public EntityParliamentElectoralUnit EntityParliamentElectoralUnit { get; set; }
    public CantonParliamentElectoralUnit? CantonParliamentElectoralUnit { get; set; }
    public decimal Lattitude { get; set; }
    public decimal Longittude { get; set; }
    public int Population { get; set; }
    public decimal Area { get; set; }
}
