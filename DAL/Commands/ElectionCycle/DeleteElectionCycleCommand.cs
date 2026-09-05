using Application.Repositories;
using MediatR;

namespace DAL.Commands.ElectionCycle;

public record DeleteElectionCycleCommand(int Id) : IRequest;

internal sealed class DeleteElectionCycleCommandHandler(IElectionCycleRepository repository)
    : IRequestHandler<DeleteElectionCycleCommand>
{
    public Task Handle(DeleteElectionCycleCommand request, CancellationToken cancellationToken)
        => repository.DeleteAsync(request.Id);
}
