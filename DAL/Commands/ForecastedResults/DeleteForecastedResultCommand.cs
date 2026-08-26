using MediatR;

namespace DAL.Commands.ForecastedResults;

public record DeleteForecastedResultCommand(int Id) : IRequest;

internal sealed class DeleteForecastedResultCommandHandler(IForecastedResultRepository repository)
    : IRequestHandler<DeleteForecastedResultCommand>
{
    private readonly IForecastedResultRepository _repository = repository;

    public Task Handle(DeleteForecastedResultCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(request.Id);
}
