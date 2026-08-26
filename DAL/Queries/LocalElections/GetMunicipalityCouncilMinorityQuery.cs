using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.LocalElections;

public record GetMunicipalityCouncilMinorityQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetMunicipalityCouncilMinorityQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetMunicipalityCouncilMinorityQueryHandler(IMunicipalityRepository repository) : IRequestHandler<GetMunicipalityCouncilMinorityQuery, TableCandidateReadModel>
{
    private readonly IMunicipalityRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetMunicipalityCouncilMinorityQuery request, CancellationToken cancellationToken)
    {
        var municipalityCouncilMinorities =
            await _repository.GetMunicipalityCouncilMinoritiesAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return municipalityCouncilMinorities ?? throw new UserException("No municipal council minorities results found!");
    }
}
