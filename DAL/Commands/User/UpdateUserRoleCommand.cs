using Application.Repositories;
using MediatR;

namespace DAL.Commands.User;

public record UpdateUserRoleCommand(int UserId, int RoleId) : IRequest;

internal sealed class UpdateUserRoleCommandHandler(IUserRepository userRepository, IMediator mediator)
    : IRequestHandler<UpdateUserRoleCommand>
{
    public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        await userRepository.UpdateRoleAsync(request.UserId, request.RoleId);
        await mediator.Send(new DAL.Commands.Notification.CreateNotificationCommand(
            request.UserId, "Uloga promijenjena", "Vaša korisnička uloga je promijenjena."), cancellationToken);
    }
}
