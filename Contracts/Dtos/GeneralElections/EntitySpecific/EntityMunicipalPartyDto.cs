namespace Contracts.Dtos.GeneralElections.EntitySpecific
{
    public class EntityMunicipalPartyDto
    {
        public string Code { get; set; }
        public int ElectoralUnitPartyResultId { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int TotalVotes { get; set; }
    }
}
