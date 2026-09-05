namespace Application.ReadModels;

public class SavedSearchReadModel
{
    public int Id { get; }
    public byte ElectionType { get; }
    public short ElectionYear { get; }
    public byte? AnalysisSubject { get; }
    public int? ElectoralUnit { get; }
    public int? MunicipalityCode { get; }
    public DateTime CreatedAt { get; }

    public SavedSearchReadModel(int id, byte electionType, short electionYear,
        byte? analysisSubject, int? electoralUnit, int? municipalityCode, DateTime createdAt)
        => (Id, ElectionType, ElectionYear, AnalysisSubject, ElectoralUnit, MunicipalityCode, CreatedAt)
            = (id, electionType, electionYear, analysisSubject, electoralUnit, municipalityCode, createdAt);
}
