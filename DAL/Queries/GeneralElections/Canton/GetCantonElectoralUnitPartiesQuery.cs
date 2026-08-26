using MediatR;

namespace DAL.Queries.GeneralElections.Canton;

public class GetCantonElectoralUnitPartiesQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public CantonParliamentElectoralUnit ElectoralUnit { get; }

    public GetCantonElectoralUnitPartiesQuery(int electionYear, int electoralUnit)
        => (ElectionYear, ElectoralUnit) = (electionYear, (CantonParliamentElectoralUnit)electoralUnit);
}

public class GetCantonElectoralUnitPartiesQueryHandler(
    ICantonRepository repository,
    IMediator mediator) : ElectionYearHandlerValidation(mediator), IRequestHandler<GetCantonElectoralUnitPartiesQuery, TableCandidateReadModel>
{
    private readonly ICantonRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetCantonElectoralUnitPartiesQuery request, CancellationToken cancellationToken)
    {
        await Validate(request.ElectionYear, ElectionType.GeneralElection, cancellationToken);

        return await _repository.GetCantonElectoralUnitPartiesAsync(request.ElectionYear, request.ElectoralUnit).ConfigureAwait(false);
    }
}
