namespace Application.Models
{
    public class PresidencyMunicipalResults
    {
        public int Id { get; set; }
        public int MunicipalityCode { get; set; }
        public int ElectionYear { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int TotalVotes { get; set; }
    }
}
