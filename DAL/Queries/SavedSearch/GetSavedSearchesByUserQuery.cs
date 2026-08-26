using Application.Models;
using Application.Repositories;
using MediatR;

namespace DAL.Queries.SavedSearch;

public record GetSavedSearchesByUserQuery(int UserId) : IRequest<IEnumerable<Application.Models.SavedSearch>>;

internal sealed class GetSavedSearchesByUserQueryHandler(ISavedSearchRepository repository)
    : IRequestHandler<GetSavedSearchesByUserQuery, IEnumerable<Application.Models.SavedSearch>>
{
    public Task<IEnumerable<Application.Models.SavedSearch>> Handle(GetSavedSearchesByUserQuery request, CancellationToken cancellationToken)
        => repository.GetByUserAsync(request.UserId);
}
