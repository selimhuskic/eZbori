using Application.Repositories;
using MediatR;

namespace DAL.Commands.User;

public record ChangePasswordCommand(int UserId, string HashedPassword) : IRequest;

internal sealed class ChangePasswordCommandHandler(IUserRepository userRepository)
    : IRequestHandler<ChangePasswordCommand>
{
    public Task Handle(ChangePasswordCommand request, CancellationToken ct)
        => userRepository.ChangePasswordAsync(request.UserId, request.HashedPassword);
}
