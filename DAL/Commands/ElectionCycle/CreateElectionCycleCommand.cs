using DAL.Validation;
using MediatR;

namespace DAL.Commands.ElectionCycle;

public record CreateElectionCycleCommand(Application.Models.ElectionCycle Cycle) : IRequest<Application.Models.ElectionCycle>;

internal sealed class CreateElectionCycleCommandHandler(IElectionCycleRepository repository)
    : IRequestHandler<CreateElectionCycleCommand, Application.Models.ElectionCycle>
{
    public async Task<Application.Models.ElectionCycle> Handle(CreateElectionCycleCommand request, CancellationToken cancellationToken)
    {
        await ElectionCycleValidator.ValidateAsync(request.Cycle);
        return await repository.CreateAsync(request.Cycle);
    }
}
