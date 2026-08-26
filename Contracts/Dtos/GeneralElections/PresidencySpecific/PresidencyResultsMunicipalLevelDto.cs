namespace Contracts.Dtos.GeneralElections.PresidencySpecific
{
    public class PresidencyResultsMunicipalLevelDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int TotalVotes { get; set; }
    }
}
