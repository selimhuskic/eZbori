using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.State;

public class GetStateMunicipalOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetStateMunicipalOverviewQuery(int electionYear, int municipalityCode) => (ElectionYear, MunicipalityCode) =
        (electionYear, municipalityCode);
}

public class GetStateMunicipalOverviewQueryHandler(IStateRepository repository) : IRequestHandler<GetStateMunicipalOverviewQuery, TableOverviewReadModel>
{
    private readonly IStateRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetStateMunicipalOverviewQuery request, CancellationToken cancellationToken)
    {
        var stateMunicipalOverviewQuery = await _repository
            .GetStateMunicipalOverviewQueryAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return stateMunicipalOverviewQuery ?? throw new UserException("No state municipal overviews found!");
    }
}
