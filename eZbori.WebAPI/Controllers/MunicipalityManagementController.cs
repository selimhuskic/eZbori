using Application.DTOs;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMunicipalityRequest request, CancellationToken cancellationToken)
    {
        var municipality = new Municipality
        {
            Id = request.Id,
            Name = request.Name,
            Entity = request.Entity,
            Population = request.Population,
            StateParliamentElectoralUnit = request.StateParliamentElectoralUnit,
            EntityParliamentElectoralUnit = request.EntityParliamentElectoralUnit,
            CantonParliamentElectoralUnit = request.CantonParliamentElectoralUnit,
        };
        var created = await _mediator.Send(new CreateMunicipalityCommand(municipality), cancellationToken);
        
        return CreatedAtAction(nameof(GetAll), created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMunicipalityRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateMunicipalityCommand(id, request.Name, request.Population), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteMunicipalityCommand(id), cancellationToken);
        return NoContent();
    }
}
