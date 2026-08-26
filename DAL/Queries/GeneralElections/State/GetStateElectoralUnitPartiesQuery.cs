using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.State;

public class GetStateElectoralUnitPartiesQuery(int electionYear) : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; } = electionYear;
}

public class GetStateElectoralUnitPartiesQueryHandler(IStateRepository repository) : IRequestHandler<GetStateElectoralUnitPartiesQuery, TableCandidateReadModel>
{
    private readonly IStateRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetStateElectoralUnitPartiesQuery request, CancellationToken cancellationToken)
    {
        var stateElectoralUnitParties = await _repository.GetStateElectoralUnitPartiesAsync(request.ElectionYear).ConfigureAwait(false);

        return stateElectoralUnitParties ?? throw new UserException("No state electoral unit parties found!");
    }
}
