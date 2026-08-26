using Application.DTOs;
using Application.Enum;

namespace Application.Interfaces;

public interface IPartiesResultsStrategy
{
    AnalysisSubject Subject { get; }
    Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct);
}
