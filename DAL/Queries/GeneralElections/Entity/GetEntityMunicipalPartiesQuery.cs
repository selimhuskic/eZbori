using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Entity;

public class GetEntityMunicipalPartiesQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetEntityMunicipalPartiesQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetEntityMunicipalPartiesQueryHandler(IEntityRepository repository) : IRequestHandler<GetEntityMunicipalPartiesQuery, TableCandidateReadModel>
{
    private readonly IEntityRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetEntityMunicipalPartiesQuery request, CancellationToken cancellationToken)
    {
        var entityMunicipalPartiesResults =
            await _repository.GetEntityMunicipalPartiesAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return entityMunicipalPartiesResults ?? throw new UserException("No entity municipal parties results found!");
    }
}
