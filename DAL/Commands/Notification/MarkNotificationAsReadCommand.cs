using MediatR;

namespace DAL.Commands.Notification;

public record MarkNotificationAsReadCommand(int NotificationId, int UserId) : IRequest;

internal sealed class MarkNotificationAsReadCommandHandler(INotificationRepository repository)
    : IRequestHandler<MarkNotificationAsReadCommand>
{
    public Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        => repository.MarkAsReadAsync(request.NotificationId, request.UserId);
}
