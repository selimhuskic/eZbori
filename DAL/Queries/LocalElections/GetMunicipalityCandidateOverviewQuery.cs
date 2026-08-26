using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.LocalElections;

public record GetMunicipalityCandidateOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetMunicipalityCandidateOverviewQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetMunicipalityCandidateOverviewQueryHandler(IMunicipalityRepository repository) : IRequestHandler<GetMunicipalityCandidateOverviewQuery, TableOverviewReadModel>
{
    private readonly IMunicipalityRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetMunicipalityCandidateOverviewQuery request, CancellationToken cancellationToken)
    {
        var municipalityCandidateOverviews =
            await _repository.GetMunicipalityCandidateOverviewsAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return municipalityCandidateOverviews ?? throw new UserException("No municipal candidate overview results found!");
    }
}
