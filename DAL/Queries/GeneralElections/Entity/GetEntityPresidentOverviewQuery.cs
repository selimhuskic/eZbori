using Application.ReadModels;
using Application.Repositories;
using DAL.Exceptions;
using MediatR;

namespace DAL.Queries.GeneralElections.Entity;

public class GetEntityPresidentOverviewQuery(int electionYear) : IRequest<TableOverviewReadModel>
{
    public int ElectionYear { get; } = electionYear;
}

public class GetEntityPresidentOverviewQueryHandler(IEntityRepository repository) : IRequestHandler<GetEntityPresidentOverviewQuery, TableOverviewReadModel>
{
    private readonly IEntityRepository _repository = repository;

    public async Task<TableOverviewReadModel> Handle(GetEntityPresidentOverviewQuery request, CancellationToken cancellationToken)
    {
        var entityPresidentOverviewResult =
            await _repository.GetEntityPresidentOverviewResultsAsync(request.ElectionYear).ConfigureAwait(false);

        return entityPresidentOverviewResult ?? throw new UserException("No entity president overview results found!");
    }
}
