using MediatR;

namespace DAL.Queries.GeneralElections.Canton;

public class GetCantonMunicipalPartiesQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetCantonMunicipalPartiesQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetCantonMunicipalPartiesQueryHandler(
    ICantonRepository repository,
    IMediator mediator) : ElectionYearHandlerValidation(mediator), IRequestHandler<GetCantonMunicipalPartiesQuery, TableCandidateReadModel>
{
    private readonly ICantonRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetCantonMunicipalPartiesQuery request, CancellationToken cancellationToken)
    {
        await Validate(request.ElectionYear, ElectionType.GeneralElection, cancellationToken);

        return await _repository.GetCantonMunicipalPartiesAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);
    }
}
