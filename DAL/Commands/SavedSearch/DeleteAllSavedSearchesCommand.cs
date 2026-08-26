using Application.Repositories;
using MediatR;

namespace DAL.Commands.SavedSearch;

public record DeleteAllSavedSearchesCommand(int UserId) : IRequest;

internal sealed class DeleteAllSavedSearchesCommandHandler(ISavedSearchRepository repository)
    : IRequestHandler<DeleteAllSavedSearchesCommand>
{
    public Task Handle(DeleteAllSavedSearchesCommand request, CancellationToken cancellationToken)
        => repository.SoftDeleteAllAsync(request.UserId);
}
