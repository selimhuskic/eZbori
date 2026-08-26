using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.State;

public class GetStateElectoralUnitOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public StateParliamentElectoralUnit ElectoralUnit { get; set; }

    public GetStateElectoralUnitOverviewQuery(int electionYear, int electoralUnit)
        => (ElectionYear, ElectoralUnit) = (electionYear, (StateParliamentElectoralUnit)electoralUnit);
}

public class GetStateElectoralUnitOverviewQueryHandler(IStateRepository repository) : IRequestHandler<GetStateElectoralUnitOverviewQuery, TableOverviewReadModel>
{
    private readonly IStateRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetStateElectoralUnitOverviewQuery request, CancellationToken cancellationToken)
    {
        var stateElectoralUnitOverviews =
            await _repository.GetStateElectoralUnitOverviewsTableData(request.ElectionYear, request.ElectoralUnit).ConfigureAwait(false);

        return stateElectoralUnitOverviews ?? throw new UserException("No state electoral unit overviews found!");
    }
}
