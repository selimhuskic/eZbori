using MediatR;

namespace DAL.Queries.GeneralElections.Entity;

public class GetEntityElectoralUnitPartiesQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public EntityParliamentElectoralUnit ElectoralUnit { get; }

    public GetEntityElectoralUnitPartiesQuery(int electionYear, int electoralUnit)
        => (ElectionYear, ElectoralUnit) = (electionYear, (EntityParliamentElectoralUnit)electoralUnit);
}

public class GetEntityElectoralUnitPartiesQueryHandler(
    IEntityRepository repository,
    IMediator mediator) : ElectionYearHandlerValidation(mediator), IRequestHandler<GetEntityElectoralUnitPartiesQuery, TableCandidateReadModel>
{
    private readonly IEntityRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetEntityElectoralUnitPartiesQuery request, CancellationToken cancellationToken)
    {
        await Validate(request.ElectionYear, ElectionType.GeneralElection, cancellationToken);

        return await _repository.GetEntityElectoralUnitPartiesAsync(request.ElectionYear, request.ElectoralUnit).ConfigureAwait(false);
    }
}
