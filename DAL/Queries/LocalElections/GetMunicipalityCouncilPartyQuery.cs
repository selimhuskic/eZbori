using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.LocalElections;

public record GetMunicipalityCouncilPartyQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetMunicipalityCouncilPartyQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetMunicipalityCouncilPartyQueryHandler(IMunicipalityRepository repository) : IRequestHandler<GetMunicipalityCouncilPartyQuery, TableCandidateReadModel>
{
    private readonly IMunicipalityRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetMunicipalityCouncilPartyQuery request, CancellationToken cancellationToken)
    {
        var municipalityCouncilPartyResults =
            await _repository.GetMunicipalityCouncilPartiesAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return municipalityCouncilPartyResults ?? throw new UserException("No municipal council party results found!");
    }
}
