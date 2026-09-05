namespace Application.DTOs;

public record CreateSavedSearchRequest(
    byte ElectionType,
    short ElectionYear,
    byte? AnalysisSubject,
    int? ElectoralUnit,
    int? MunicipalityCode);
