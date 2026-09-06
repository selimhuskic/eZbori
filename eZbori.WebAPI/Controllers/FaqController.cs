using DAL.Queries.Faq;

namespace eZbori.Web.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class FaqController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FaqItem>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetFaqItemsQuery(), cancellationToken);
        return Ok(items);
    }
}
