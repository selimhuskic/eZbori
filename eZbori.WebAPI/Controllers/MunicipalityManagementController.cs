using DAL.Commands.Municipality;
using DAL.Queries;

namespace eZbori.Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]")]
[ApiController]
public class MunicipalityManagementController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetMunicipalitiesQuery(), cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMunicipalityRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateMunicipalityCommand(id, request.Name, request.Population), cancellationToken);
        return NoContent();
    }

}

public record UpdateMunicipalityRequest(string Name, int Population);
