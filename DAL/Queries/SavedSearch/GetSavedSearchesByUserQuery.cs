using Application.ReadModels;
using MediatR;

namespace DAL.Queries.SavedSearch;

public record GetSavedSearchesByUserQuery(int UserId) : IRequest<IEnumerable<SavedSearchReadModel>>;

internal sealed class GetSavedSearchesByUserQueryHandler(ISavedSearchRepository repository)
    : IRequestHandler<GetSavedSearchesByUserQuery, IEnumerable<SavedSearchReadModel>>
{
    public Task<IEnumerable<SavedSearchReadModel>> Handle(GetSavedSearchesByUserQuery request, CancellationToken cancellationToken)
        => repository.GetByUserAsync(request.UserId);
}
