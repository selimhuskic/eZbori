using Application.Models;
using Application.Repositories;
using MediatR;

namespace DAL.Commands.ElectionCycles;

public record CreateElectionCycleCommand(ElectionCycle Cycle) : IRequest<ElectionCycle>;

internal sealed class CreateElectionCycleCommandHandler(IElectionCycleRepository repository)
    : IRequestHandler<CreateElectionCycleCommand, ElectionCycle>
{
    public Task<ElectionCycle> Handle(CreateElectionCycleCommand request, CancellationToken cancellationToken)
        => repository.CreateAsync(request.Cycle);
}
