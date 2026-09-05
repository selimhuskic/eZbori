using Application.Repositories;
using MediatR;

namespace DAL.Commands.User;

public record ChangePasswordCommand(int UserId, string HashedPassword) : IRequest;

internal sealed class ChangePasswordCommandHandler(IUserRepository userRepository, IMediator mediator)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        await userRepository.ChangePasswordAsync(request.UserId, request.HashedPassword);
        await mediator.Send(new DAL.Commands.Notification.CreateNotificationCommand(
            request.UserId, "Lozinka promijenjena", "Vaša lozinka je uspješno promijenjena."), ct);
    }
}
