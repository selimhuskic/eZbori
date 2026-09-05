using MediatR;

namespace DAL.Queries;

public record GetElectionYearsQuery(int ElectionType) : IRequest<IEnumerable<int>>;

public class GetElectionYearsQueryHandler(IElectionCycleRepository electionCycleRepository)
    : IRequestHandler<GetElectionYearsQuery, IEnumerable<int>>
{
    public async Task<IEnumerable<int>> Handle(GetElectionYearsQuery request, CancellationToken cancellationToken)
        => await electionCycleRepository.GetYearsForTypeAsync((Application.Enum.ElectionType)request.ElectionType);
}
