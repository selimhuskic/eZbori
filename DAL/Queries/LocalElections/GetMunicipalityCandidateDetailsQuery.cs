using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.LocalElections;

public record GetMunicipalityCandidateDetailsQuery : IRequest<TableCandidateReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCodes { get; }

    public GetMunicipalityCandidateDetailsQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCodes) = (electionYear, municipalityCode);
}

public class GetMunicipalityCandidateDetailsQueryHandler(IMunicipalityRepository repository) : IRequestHandler<GetMunicipalityCandidateDetailsQuery, TableCandidateReadModel>
{
    private readonly IMunicipalityRepository _repository = repository;

    public async Task<TableCandidateReadModel> Handle(GetMunicipalityCandidateDetailsQuery request, CancellationToken cancellationToken)
    {
        var municipalCandidateDetailResults =
            await _repository.GetMunicipalityCandidateDetailsAsync(request.ElectionYear, request.MunicipalityCodes).ConfigureAwait(false);

        return municipalCandidateDetailResults ?? throw new UserException("No municipal candidate details results found!");
    }
}
