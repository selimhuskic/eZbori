using Application.Repositories;
using MediatR;

namespace DAL.Commands.User;

public record SetPasswordCommand(string Email, string HashedPassword) : IRequest;

internal sealed class SetPasswordCommandHandler(IUserRepository userRepository)
    : IRequestHandler<SetPasswordCommand>
{
    public Task Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        => userRepository.SetPasswordAsync(request.Email, request.HashedPassword);
}
