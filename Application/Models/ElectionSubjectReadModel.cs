namespace Application.Models
{
    public class ElectionSubjectReadModel
    {
        public int TotalValidVotes { get; set; }
        public decimal Percentage { get; set; }
        public string Name { get; set; }
    }
}
