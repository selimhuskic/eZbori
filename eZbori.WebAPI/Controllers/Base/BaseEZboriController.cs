namespace eZbori.Web.Controllers.Base;

[Authorize]
public class BaseEZboriController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;
}
