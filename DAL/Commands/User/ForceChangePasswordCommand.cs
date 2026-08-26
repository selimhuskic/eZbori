using MediatR;

namespace DAL.Commands.User;

public record ForceChangePasswordCommand(int UserId, string HashedPassword) : IRequest;

internal sealed class ForceChangePasswordCommandHandler(IUserRepository userRepository)
    : IRequestHandler<ForceChangePasswordCommand>
{
    public async Task Handle(ForceChangePasswordCommand request, CancellationToken cancellationToken)
    {
        await userRepository.ChangePasswordAsync(request.UserId, request.HashedPassword);
        await userRepository.ClearMustChangePasswordAsync(request.UserId);
    }
}
