using Application.Repositories;
using MediatR;

namespace DAL.Commands.User;

public record DeleteUserCommand(int UserId) : IRequest;

internal sealed class DeleteUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand>
{
    public Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        => userRepository.DeleteAsync(request.UserId);
}
