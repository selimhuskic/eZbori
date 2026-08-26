namespace Contracts.Dtos.GeneralElections.CantonSpecific
{
    public class CantonMunicipalPartyDto
    {
        public string Code { get; set; }
        public int ElectoralUnitPartyResultId { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int TotalVotes { get; set; }
    }
}
