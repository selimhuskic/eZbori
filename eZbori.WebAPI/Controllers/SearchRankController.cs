using System.Security.Claims;
using Application.Models.MachineLearning;
using DAL.Queries.RankSearch;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchRankController(IMediator mediator) : BaseEZboriController(mediator)
{
    [AllowAnonymous]
    [HttpGet("suggestions")]
    public async Task<ActionResult<IEnumerable<SearchRecommendationDto>>> GetSuggestions([FromQuery] int top = 10)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claim, out var id))
                userId = id;
        }

        var result = await _mediator.Send(new GetRankedSearchSuggestionsQuery(top, userId));
        return Ok(result);
    }
}
