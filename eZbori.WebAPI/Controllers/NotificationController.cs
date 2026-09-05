using DAL.Commands.Notification;
using DAL.Queries.Notification;
using System.Security.Claims;

namespace eZbori.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        var results = await _mediator.Send(new GetNotificationsByUserQuery(userId.Value), cancellationToken);
        return Ok(results);
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        await _mediator.Send(new MarkNotificationAsReadCommand(id, userId.Value), cancellationToken);
        return NoContent();
    }

    private int? GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : null;
    }
}
