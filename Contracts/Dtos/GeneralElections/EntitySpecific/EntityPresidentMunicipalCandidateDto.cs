namespace Contracts.Dtos.GeneralElections.EntitySpecific
{
    public class EntityPresidentMunicipalCandidateDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int TotalVotes { get; set; }
    }
}
