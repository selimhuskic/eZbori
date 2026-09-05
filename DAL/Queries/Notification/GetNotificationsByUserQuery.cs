using MediatR;

namespace DAL.Queries.Notification;

public record GetNotificationsByUserQuery(int UserId) : IRequest<IEnumerable<Application.Models.Notification>>;

internal sealed class GetNotificationsByUserQueryHandler(INotificationRepository repository)
    : IRequestHandler<GetNotificationsByUserQuery, IEnumerable<Application.Models.Notification>>
{
    public Task<IEnumerable<Application.Models.Notification>> Handle(GetNotificationsByUserQuery request, CancellationToken cancellationToken)
        => repository.GetByUserAsync(request.UserId);
}
