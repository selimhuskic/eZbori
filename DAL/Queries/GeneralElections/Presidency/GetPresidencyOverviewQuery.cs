using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Presidency;

public class GetPresidencyOverviewQuery : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; }
    public Application.Enum.Entity Entity { get; }

    public GetPresidencyOverviewQuery(int electionYear, Application.Enum.Entity entity) => (ElectionYear, Entity) = (electionYear, entity);
}

public class GetPresidencyOverviewQueryHandler(IPresidencyRepository repository) : IRequestHandler<GetPresidencyOverviewQuery, TableOverviewReadModel>
{
    private readonly IPresidencyRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetPresidencyOverviewQuery request, CancellationToken cancellationToken)
    {
        var presidencyOverviews = await _repository.GetPresidencyOverviewsAsync(request.ElectionYear, request.Entity).ConfigureAwait(false);

        return presidencyOverviews ?? throw new UserException($"No presidency overviews found for: {request.ElectionYear}!");
    }
}
