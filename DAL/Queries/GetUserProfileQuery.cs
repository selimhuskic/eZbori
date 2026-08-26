using Application.Requests;
using MediatR;

namespace DAL.Queries;

public record GetUserProfileQuery(int UserId) : IRequest<UserProfileDto?>;

public sealed class GetUserProfileQueryHandler(eZboriDbContext dbContext)
    : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    public Task<UserProfileDto?> Handle(GetUserProfileQuery request, CancellationToken ct)
        => dbContext.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new UserProfileDto(
                u.Email,
                u.UserName,
                u.FirstName,
                u.LastName,
                u.DateOfBirth,
                dbContext.UserRoles.Where(r => r.Id == u.UserRole).Select(r => r.RoleName).FirstOrDefault(),
                u.MunicipalityId,
                u.MunicipalityNavigation != null ? u.MunicipalityNavigation.Name : null,
                u.ProfileImageBase64))
            .FirstOrDefaultAsync(ct);
}
