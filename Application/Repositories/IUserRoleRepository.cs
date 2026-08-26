using Application.Models;

namespace Application.Repositories;

public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<UserRole> GetUserRoleAsync(int roleId);
    Task AssignDefaultUserRoleAsync(int userId);
}
