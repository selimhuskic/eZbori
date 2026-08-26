using Application.DTOs;
using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.Analysis;

public record GetAnalysisOverviewQuery(AnalysisRequest AnalysisRequest) : IRequest<IEnumerable<BaseResultsOverview>>;

internal sealed class GetAnalysisOverviewQueryHandler(
    IAnalysisRepository analysisRepository) : IRequestHandler<GetAnalysisOverviewQuery, IEnumerable<BaseResultsOverview>>
{
    private readonly IAnalysisRepository _analysisRepository = analysisRepository;

    public async Task<IEnumerable<BaseResultsOverview>> Handle(GetAnalysisOverviewQuery request, CancellationToken cancellationToken)
    {
        var isGeneralElectionRequested = request.AnalysisRequest.ElectionType == ElectionType.GeneralElection;

        if (isGeneralElectionRequested)
            return await _analysisRepository.GetGeneralOverviewAsync(request.AnalysisRequest);

        return await _analysisRepository.GetLocalOverviewAsync(request.AnalysisRequest);
    }
}
