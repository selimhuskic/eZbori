using Application.Services;
using MediatR;

namespace DAL.Queries.RankSearch;

public record GetRankedSearchSuggestionsQuery(int Top = 10, int? UserId = null) : IRequest<IEnumerable<SearchRecommendationDto>>;

internal class GetRankedSearchSuggestionsQueryHandler(IRankingService rankingService)
    : IRequestHandler<GetRankedSearchSuggestionsQuery, IEnumerable<SearchRecommendationDto>>
{
    private readonly IRankingService _rankingService = rankingService;

    public async Task<IEnumerable<SearchRecommendationDto>> Handle(GetRankedSearchSuggestionsQuery request, CancellationToken cancellationToken)
    {
        return await _rankingService.GetSuggestedSearchesRankedAsync(request.Top, request.UserId);
    }
}
