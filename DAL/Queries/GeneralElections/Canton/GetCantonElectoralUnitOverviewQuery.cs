using MediatR;

namespace DAL.Queries.GeneralElections.Canton;

public class GetCantonElectoralUnitOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public CantonParliamentElectoralUnit ElectoralUnit { get; }

    public GetCantonElectoralUnitOverviewQuery(int electionYear, int electoralUnit)
        => (ElectionYear, ElectoralUnit) = (electionYear, (CantonParliamentElectoralUnit)electoralUnit);
}

public class GetCantonElectoralUnitOverviewQueryHandler(
    ICantonRepository repo,
    IMediator mediator) : ElectionYearHandlerValidation(mediator), IRequestHandler<GetCantonElectoralUnitOverviewQuery, TableOverviewReadModel>
{        
    private readonly ICantonRepository _repo = repo;

    public async Task<TableOverviewReadModel> Handle(GetCantonElectoralUnitOverviewQuery request, CancellationToken cancellationToken)
    {
        await Validate(request.ElectionYear, ElectionType.GeneralElection, cancellationToken);

        return await _repo.GetCantonElectoralUnitOverviewsAsync(request.ElectionYear, request.ElectoralUnit).ConfigureAwait(false);
    }
}
