using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Presidency;

public class GetPresidencyMunicipalOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public int MunicipalityCode { get; }

    public GetPresidencyMunicipalOverviewQuery(int electionYear, int municipalityCode)
        => (ElectionYear, MunicipalityCode) = (electionYear, municipalityCode);
}

public class GetPresidencyMunicipalOverviewQueryHandler(IPresidencyRepository repository) : IRequestHandler<GetPresidencyMunicipalOverviewQuery, TableOverviewReadModel>
{
    private readonly IPresidencyRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetPresidencyMunicipalOverviewQuery request, CancellationToken cancellationToken)
    {
        var presidencyMunicipalOverview = await _repository
            .GetPresidencyMunicipalOverviewsAsync(request.ElectionYear, request.MunicipalityCode).ConfigureAwait(false);

        return presidencyMunicipalOverview ?? throw new UserException("No presidency municipal overviews found!");
    }
}
