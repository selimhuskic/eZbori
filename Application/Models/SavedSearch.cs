namespace Application.Models;

public class SavedSearch
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public byte ElectionType { get; set; }
    public short ElectionYear { get; set; }
    public byte? AnalysisSubject { get; set; }
    public int? ElectoralUnit { get; set; }
    public int? MunicipalityCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
