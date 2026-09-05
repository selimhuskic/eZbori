using Application.Repositories;
using MediatR;

namespace DAL.Commands.Notification;

public record CreateNotificationCommand(int UserId, string Title, string Body) : IRequest;

internal sealed class CreateNotificationCommandHandler(INotificationRepository repository)
    : IRequestHandler<CreateNotificationCommand>
{
    public Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        => repository.CreateAsync(request.UserId, request.Title, request.Body);
}
