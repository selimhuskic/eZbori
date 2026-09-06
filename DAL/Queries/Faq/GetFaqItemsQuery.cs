using Application.Models;
using Application.Repositories;
using MediatR;

namespace DAL.Queries.Faq;

public record GetFaqItemsQuery : IRequest<IEnumerable<FaqItem>>;

internal sealed class GetFaqItemsQueryHandler(IFaqRepository repository)
    : IRequestHandler<GetFaqItemsQuery, IEnumerable<FaqItem>>
{
    public Task<IEnumerable<FaqItem>> Handle(GetFaqItemsQuery request, CancellationToken cancellationToken)
        => repository.GetAllOrderedAsync();
}
