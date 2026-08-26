using MediatR;

namespace DAL.Queries.GeneralElections.Entity;

public class GetEntityElectoralUnitOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public EntityParliamentElectoralUnit ElectoralUnit { get; }

    public GetEntityElectoralUnitOverviewQuery(int electionYear, int electoralUnit)
        => (ElectionYear, ElectoralUnit) = (electionYear, (EntityParliamentElectoralUnit)electoralUnit);
}

public class GetEntityElectoralUnitOverviewQueryHandler(
    IEntityRepository repository,
    IMediator mediator) : ElectionYearHandlerValidation(mediator), IRequestHandler<GetEntityElectoralUnitOverviewQuery, TableOverviewReadModel>
{
    private readonly IEntityRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetEntityElectoralUnitOverviewQuery request, CancellationToken cancellationToken)
    {
        await Validate(request.ElectionYear, ElectionType.GeneralElection, cancellationToken);

        return await _repository.GetEntityElecetoralUnitResultsAsync(request.ElectionYear, request.ElectoralUnit).ConfigureAwait(false);
    }
}
