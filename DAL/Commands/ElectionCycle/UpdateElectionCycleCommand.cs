using DAL.Validation;
using MediatR;

namespace DAL.Commands.ElectionCycle;

public record UpdateElectionCycleCommand(int Id, Application.Models.ElectionCycle Cycle) : IRequest;

internal sealed class UpdateElectionCycleCommandHandler(IElectionCycleRepository repository)
    : IRequestHandler<UpdateElectionCycleCommand>
{
    public async Task Handle(UpdateElectionCycleCommand request, CancellationToken cancellationToken)
    {
        await ElectionCycleValidator.ValidateAsync(request.Cycle);
        await repository.UpdateAsync(request.Id, request.Cycle);
    }
}
