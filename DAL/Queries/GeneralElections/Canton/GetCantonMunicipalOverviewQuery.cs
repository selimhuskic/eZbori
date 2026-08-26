using MediatR;

namespace DAL.Queries.GeneralElections.Canton;

public class GetCantonMunicipalOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetCantonMunicipalOverviewQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetCantonMunicipalOverviewQueryHandler(
    ICantonRepository repository,
    IMediator mediator) : ElectionYearHandlerValidation(mediator), IRequestHandler<GetCantonMunicipalOverviewQuery, TableOverviewReadModel>
{
    private readonly ICantonRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetCantonMunicipalOverviewQuery request, CancellationToken cancellationToken)
    {
        await Validate(request.ElectionYear, ElectionType.GeneralElection, cancellationToken);

        return await _repository.GetCantonMunicipalOverviewsAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);
    }
}
