using Application.DTOs;
using MediatR;

namespace DAL.Queries.Analysis;

public record GetAnalysisPartiesQuery(AnalysisRequest AnalysisRequest) : IRequest<IEnumerable<PartiesResults>>;

internal sealed class GetAnalysisPartiesQueryHandler(
    IAnalysisRepository analysisRepository) : IRequestHandler<GetAnalysisPartiesQuery, IEnumerable<PartiesResults>>
{
    private readonly IAnalysisRepository _analysisRepository = analysisRepository;

    public Task<IEnumerable<PartiesResults>> Handle(GetAnalysisPartiesQuery request, CancellationToken cancellationToken)
        => _analysisRepository.GetPartiesAsync(request.AnalysisRequest, cancellationToken);
}
