using MediatR;

namespace DAL.Commands.ForecastedResults;

public record CreateForecastedResultCommand(Application.Models.ForecastedResult Result)
    : IRequest<Application.Models.ForecastedResult>;

internal sealed class CreateForecastedResultCommandHandler(IForecastedResultRepository repository)
    : IRequestHandler<CreateForecastedResultCommand, Application.Models.ForecastedResult>
{
    private readonly IForecastedResultRepository _repository = repository;

    public Task<Application.Models.ForecastedResult> Handle(CreateForecastedResultCommand request, CancellationToken cancellationToken)
        => _repository.CreateAsync(request.Result);
}
