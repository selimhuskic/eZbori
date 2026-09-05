using Application.Models;
using Application.Repositories;
using MediatR;

namespace DAL.Commands.ElectionCycles;

public record UpdateElectionCycleCommand(int Id, ElectionCycle Cycle) : IRequest;

internal sealed class UpdateElectionCycleCommandHandler(IElectionCycleRepository repository)
    : IRequestHandler<UpdateElectionCycleCommand>
{
    public Task Handle(UpdateElectionCycleCommand request, CancellationToken cancellationToken)
        => repository.UpdateAsync(request.Id, request.Cycle);
}
