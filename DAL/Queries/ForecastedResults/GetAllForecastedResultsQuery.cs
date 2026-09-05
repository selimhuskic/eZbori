using MediatR;

namespace DAL.Queries.ForecastedResults;

public record GetAllForecastedResultsQuery(int Page, int PageSize)
    : IRequest<(IEnumerable<ForecastedResult> Items, int Total)>;

internal sealed class GetAllForecastedResultsQueryHandler(IForecastedResultRepository repository)
    : IRequestHandler<GetAllForecastedResultsQuery, (IEnumerable<Application.Models.ForecastedResult> Items, int Total)>
{
    private readonly IForecastedResultRepository _repository = repository;

    public Task<(IEnumerable<ForecastedResult> Items, int Total)> Handle(GetAllForecastedResultsQuery request, CancellationToken cancellationToken)
        => _repository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
}
