using MediatR;

namespace DAL.Queries.ForecastedResults;

public record GetAllForecastedResultsQuery : IRequest<IEnumerable<Application.Models.ForecastedResult>>;

internal sealed class GetAllForecastedResultsQueryHandler(IForecastedResultRepository repository)
    : IRequestHandler<GetAllForecastedResultsQuery, IEnumerable<Application.Models.ForecastedResult>>
{
    private readonly IForecastedResultRepository _repository = repository;

    public Task<IEnumerable<Application.Models.ForecastedResult>> Handle(GetAllForecastedResultsQuery request, CancellationToken cancellationToken)
        => _repository.GetAllAsync();
}
