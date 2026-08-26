namespace Application.ReadModels
{
    public class PushpinReadModel
    {
        public int MunicipalityCode { get; set; }
        public double Lattitude { get; set; }
        public double Longitude { get; set; }
        public int? CantonParliamentElectoralUnit { get; set; }
        public int EntityParliamentElectoralUnit { get; set; }
        public int StatesParliamentElectoralUnit { get; set; }
        public string Description { get; set; }
    }
}
