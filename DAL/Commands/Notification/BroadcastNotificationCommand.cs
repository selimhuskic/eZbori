using MediatR;

namespace DAL.Commands.Notification;

public record BroadcastNotificationCommand(string Title, string Body) : IRequest;

internal sealed class BroadcastNotificationCommandHandler(
    INotificationRepository notificationRepository,
    IUserRepository userRepository) : IRequestHandler<BroadcastNotificationCommand>
{
    public async Task Handle(BroadcastNotificationCommand request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync();
        var userIds = users.Select(u => u.Id);
        await notificationRepository.CreateForUsersAsync(userIds, request.Title, request.Body);
    }
}
