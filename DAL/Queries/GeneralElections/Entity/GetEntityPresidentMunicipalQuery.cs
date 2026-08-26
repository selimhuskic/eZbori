using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Entity;

public class GetEntityPresidentMunicipalQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetEntityPresidentMunicipalQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetEntityPresidentMunicipalQueryHandler(IEntityRepository repository) : IRequestHandler<GetEntityPresidentMunicipalQuery, TableCandidateReadModel>
{
    private readonly IEntityRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetEntityPresidentMunicipalQuery request, CancellationToken cancellationToken)
    {
        var entityPresidentMunicipalCandidateResults =
            await _repository.GetEntityPresidentMunicipalResultsAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return entityPresidentMunicipalCandidateResults ?? throw new UserException("No entity president municipal candidate results found!");
    }
}
