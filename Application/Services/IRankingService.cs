using Application.Models.MachineLearning;

namespace Application.Services;

public interface IRankingService
{
    Task<IEnumerable<SearchRecommendationDto>> GetSuggestedSearchesRankedAsync(int top, int? userId = null);
}
