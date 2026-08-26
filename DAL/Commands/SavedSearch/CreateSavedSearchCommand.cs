using Application.Models;
using Application.Repositories;
using MediatR;

namespace DAL.Commands.SavedSearch;

public record CreateSavedSearchCommand(Application.Models.SavedSearch Search)
    : IRequest<Application.Models.SavedSearch>;

internal sealed class CreateSavedSearchCommandHandler(ISavedSearchRepository repository)
    : IRequestHandler<CreateSavedSearchCommand, Application.Models.SavedSearch>
{
    public Task<Application.Models.SavedSearch> Handle(CreateSavedSearchCommand request, CancellationToken cancellationToken)
        => repository.CreateAsync(request.Search);
}
