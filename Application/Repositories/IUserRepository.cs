using Application.Models;

namespace Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserAsync(string username);
    Task<User?> GetUserAsync(string email, string username);
    Task<User> CreatNewUserAsync(User user);
    Task UpdateRoleAsync(int userId, int roleId);
    Task DeleteAsync(int userId);
    Task<User?> GetProfileAsync(int userId);
    Task UpdateProfileAsync(int userId, string? email, string? firstName, string? lastName,
        DateTime? dateOfBirth, int? municipalityId, bool clearMunicipality,
        string? profileImageBase64);
    Task<User?> GetUserByIdAsync(int userId);
    Task ChangePasswordAsync(int userId, string hashedPassword);
    Task SetResetTokenAsync(string email, string? token, DateTime? expiry);
    Task ConfirmUserAsync(int userId);
}
