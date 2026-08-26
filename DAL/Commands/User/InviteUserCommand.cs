using System.Security.Cryptography;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DAL.Commands.User;

public record InviteUserCommand(
    string FirstName,
    string LastName,
    string Email,
    int RoleId,
    string? Message) : IRequest;

internal sealed class InviteUserCommandHandler(
    IUserRepository userRepository,
    IUserInviteQueue inviteQueue,
    IPasswordHasher<Application.Models.User> passwordHasher) : IRequestHandler<InviteUserCommand>
{
    private static readonly char[] PasswordChars =
        "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789".ToCharArray();

    public async Task Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetUserAsync(request.Email, request.Email);
        if (existingUser is not null)
            throw new UserException("Korisnik s ovom email adresom već postoji.");

        var tempPassword = GenerateTempPassword(10);

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
            UserVerified = true,
            MustChangePassword = true,
        };
        newUser = newUser.WithHashedPassword(passwordHasher.HashPassword(newUser, tempPassword));
        await userRepository.CreatNewUserAsync(newUser);

        await inviteQueue.PublishAsync(new UserInviteMessage(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Message,
            tempPassword));
    }

    private static string GenerateTempPassword(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return new string(bytes.Select(b => PasswordChars[b % PasswordChars.Length]).ToArray());
    }
}
