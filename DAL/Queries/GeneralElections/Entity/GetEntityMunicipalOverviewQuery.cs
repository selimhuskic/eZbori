using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Entity;

public class GetEntityMunicipalOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetEntityMunicipalOverviewQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetEntityMunicipalOverviewQueryHandler(IEntityRepository repository) : IRequestHandler<GetEntityMunicipalOverviewQuery, TableOverviewReadModel>
{
    private readonly IEntityRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetEntityMunicipalOverviewQuery request, CancellationToken cancellationToken)
    {
        var entityMunicipalOverviewResults =
            await _repository.GetEntityMunicipalOverviewAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return entityMunicipalOverviewResults ?? throw new UserException("No entity municipal overview results found!");
    }
}
