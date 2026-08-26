using Application.Models;
using Application.Repositories;
using MediatR;

namespace DAL.Queries.ElectionCycles;

public record GetElectionCyclesQuery : IRequest<IEnumerable<ElectionCycle>>;

internal sealed class GetElectionCyclesQueryHandler(IElectionCycleRepository repository)
    : IRequestHandler<GetElectionCyclesQuery, IEnumerable<ElectionCycle>>
{
    public Task<IEnumerable<ElectionCycle>> Handle(GetElectionCyclesQuery request, CancellationToken cancellationToken)
        => repository.GetAllAsync();
}
