using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.LocalElections;

public record GetMunicipalityCouncilOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetMunicipalityCouncilOverviewQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetMunicipalityCouncilOverviewQueryHandler(IMunicipalityRepository repository) : IRequestHandler<GetMunicipalityCouncilOverviewQuery, TableOverviewReadModel>
{
    private readonly IMunicipalityRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetMunicipalityCouncilOverviewQuery request, CancellationToken cancellationToken)
    {
        var municipalityCouncilOverviews =
            await _repository.GetMunicipalityCouncilOverviewsAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return municipalityCouncilOverviews ?? throw new UserException("No municipal council overview results!");
    }
}
