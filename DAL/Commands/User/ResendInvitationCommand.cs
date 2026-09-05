using System.Security.Cryptography;
using Application.Services;
using MediatR;

namespace DAL.Commands.User;

public record ResendInvitationCommand(int UserId) : IRequest;

internal sealed class ResendInvitationCommandHandler(
    IUserRepository userRepository,
    IUserInviteQueue inviteQueue) : IRequestHandler<ResendInvitationCommand>
{
    public async Task Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(request.UserId)
            ?? throw new UserException("Korisnik nije pronađen.");

        if (user.UserVerified)
            throw new UserException("Korisnik je već potvrdio nalog.");

        var otp = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
        await userRepository.SetResetTokenAsync(user.Email, otp, DateTime.UtcNow.AddMinutes(30));

        await inviteQueue.PublishAsync(new UserInviteMessage(
            user.FirstName, user.LastName, user.Email, null, otp));
    }
}
