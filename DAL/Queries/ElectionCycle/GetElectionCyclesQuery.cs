using Application.Repositories;
using MediatR;

namespace DAL.Queries.ElectionCycle;

public record GetElectionCyclesQuery : IRequest<IEnumerable<Application.Models.ElectionCycle>>;

internal sealed class GetElectionCyclesQueryHandler(IElectionCycleRepository repository)
    : IRequestHandler<GetElectionCyclesQuery, IEnumerable<Application.Models.ElectionCycle>>
{
    public Task<IEnumerable<Application.Models.ElectionCycle>> Handle(GetElectionCyclesQuery request, CancellationToken cancellationToken)
        => repository.GetAllAsync();
}
