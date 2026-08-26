using MediatR;

namespace DAL.Commands.User;

public record ResetPasswordCommand(string Email, string Token, string HashedPassword) : IRequest<ResetPasswordResult>;

public record ResetPasswordResult(bool Success, string Message);

internal sealed class ResetPasswordCommandHandler(IUserRepository userRepository)
    : IRequestHandler<ResetPasswordCommand, ResetPasswordResult>
{
    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserAsync(request.Email, request.Email);

        if (user is null)
            return new ResetPasswordResult(false, "Korisnik nije pronađen.");

        if (user.PasswordResetToken is null
            || !string.Equals(user.PasswordResetToken, request.Token, StringComparison.OrdinalIgnoreCase)
            || user.PasswordResetTokenExpiry is null
            || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return new ResetPasswordResult(false, "Kod je nevažeći ili je istekao.");

        await userRepository.ChangePasswordAsync(user.Id, request.HashedPassword);
        await userRepository.SetResetTokenAsync(request.Email, null, null);
        await userRepository.ClearMustChangePasswordAsync(user.Id);

        return new ResetPasswordResult(true, "Lozinka uspješno promijenjena.");
    }
}
