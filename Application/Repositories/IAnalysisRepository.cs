using Application.DTOs;

namespace Application.Repositories;

public interface IAnalysisRepository
{
    Task<IEnumerable<OverviewResults>> GetGeneralOverviewAsync(AnalysisRequest analysisRequest);
    Task<IEnumerable<OverviewResults>> GetLocalOverviewAsync(AnalysisRequest analysisRequest);
    Task<IEnumerable<PartiesResults>> GetPartiesAsync(AnalysisRequest analysisRequest, CancellationToken ct);
}
