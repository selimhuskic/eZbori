using Application.Services;
using MediatR;
using System.Security.Cryptography;

namespace DAL.Commands.User;

public record ForgotPasswordCommand(string Email) : IRequest<string>;

internal sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetQueue passwordResetQueue)
    : IRequestHandler<ForgotPasswordCommand, string>
{
    private const string Message = "Ako vaša adresa postoji u sistemu, poslat ćemo vam kod za resetovanje.";

    public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserAsync(request.Email, request.Email);

        if (user is not null)
        {
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
            await userRepository.SetResetTokenAsync(request.Email, token, DateTime.UtcNow.AddMinutes(30));
            await passwordResetQueue.PublishAsync(new PasswordResetMessage(user.FirstName ?? "", user.Email!, token));
        }

        return Message;
    }
}
