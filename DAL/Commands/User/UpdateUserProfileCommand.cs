using Application.Repositories;
using MediatR;

namespace DAL.Commands.User;

public record UpdateUserProfileCommand(
    int UserId,
    string? Email,
    string? FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    int? MunicipalityId,
    bool ClearMunicipality,
    string? ProfileImageBase64) : IRequest;

internal sealed class UpdateUserProfileCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserProfileCommand>
{
    public Task Handle(UpdateUserProfileCommand request, CancellationToken ct)
        => userRepository.UpdateProfileAsync(
            request.UserId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.MunicipalityId,
            request.ClearMunicipality,
            request.ProfileImageBase64);
}
