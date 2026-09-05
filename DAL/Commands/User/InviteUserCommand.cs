using System.Security.Cryptography;
using Application.Services;
using MediatR;

namespace DAL.Commands.User;

public record InviteUserCommand(
    string FirstName,
    string LastName,
    string Email,
    int RoleId,
    string? Message) : IRequest;

internal sealed class InviteUserCommandHandler(
    IUserRepository userRepository,
    IUserInviteQueue inviteQueue) : IRequestHandler<InviteUserCommand>
{
    public async Task Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetUserAsync(request.Email, request.Email);
        if (existingUser is not null)
            throw new UserException("Korisnik s ovom email adresom već postoji. Koristite opciju za ponovno slanje pozivnice.");

        var newUser = new Application.Models.User
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = null,
            CreatedAt = DateTime.UtcNow,
            UserRole = request.RoleId,
            Password = string.Empty,
            UserVerified = false,
        };
        await userRepository.CreatNewUserAsync(newUser);

        var otp = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
        await userRepository.SetResetTokenAsync(request.Email, otp, DateTime.UtcNow.AddMinutes(30));

        await inviteQueue.PublishAsync(new UserInviteMessage(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Message,
            otp));
    }
}
