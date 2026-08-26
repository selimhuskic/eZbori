using MediatR;

namespace DAL.Queries;

public record GetElectionYearsQuery(int ElectionType) : IRequest<IEnumerable<int>>;

public class GetElectionYearsQueryHandler(IElectionCycleRepository electionCycleRepository)
    : IRequestHandler<GetElectionYearsQuery, IEnumerable<int>>
{
    public Task<IEnumerable<int>> Handle(GetElectionYearsQuery request, CancellationToken cancellationToken)
        => Task.FromResult<IEnumerable<int>>(
            electionCycleRepository.GetYearsForType((Application.Enum.ElectionType)request.ElectionType));
}
