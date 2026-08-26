using Application.Repositories;
using MediatR;

namespace DAL.Commands.User;

public record UpdateUserRoleCommand(int UserId, int RoleId) : IRequest;

internal sealed class UpdateUserRoleCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserRoleCommand>
{
    public Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        => userRepository.UpdateRoleAsync(request.UserId, request.RoleId);
}
