using DAL.Exceptions;

namespace DAL.Repositories;

public class UserRoleRepository(eZboriDbContext dboContext)
    : GenericRepository<Application.Models.UserRole>(dboContext), IUserRoleRepository
{
    public async Task AssignDefaultUserRoleAsync(int userId)
    {
        var userRole = new Application.Models.UserRole { RoleName = Application.Enum.UserRole.User.ToString(), Id = userId };
        await AddAsync(userRole);
    }

    public async Task<Application.Models.UserRole> GetUserRoleAsync(int roleId)
        => await GetByIdAsync(roleId)
            ?? throw new UserException($"User role with id {roleId} not found.");
}
