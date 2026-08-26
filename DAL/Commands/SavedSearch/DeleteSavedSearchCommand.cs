using Application.Repositories;
using MediatR;

namespace DAL.Commands.SavedSearch;

public record DeleteSavedSearchCommand(int Id, int UserId) : IRequest;

internal sealed class DeleteSavedSearchCommandHandler(ISavedSearchRepository repository)
    : IRequestHandler<DeleteSavedSearchCommand>
{
    public Task Handle(DeleteSavedSearchCommand request, CancellationToken cancellationToken)
        => repository.SoftDeleteAsync(request.Id, request.UserId);
}
