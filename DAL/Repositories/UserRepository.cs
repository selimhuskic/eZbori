namespace DAL.Repositories;

public class UserRepository(eZboriDbContext dboContext)
    : GenericRepository<User>(dboContext), IUserRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public async Task<User> CreatNewUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserAsync(string username)
        => await _dbContext.Users
            .FirstOrDefaultAsync(x => x.UserName == username || x.Email == username)
            .ConfigureAwait(false);

    public async Task<User?> GetUserAsync(string email, string username)
        => await _dbContext.Users
            .FirstOrDefaultAsync(x => x.UserName == username || x.Email == email)
            .ConfigureAwait(false);

    public async Task UpdateRoleAsync(int userId, int roleId)
    {
        var user = await _dbContext.Users.FindAsync(userId)
            ?? throw new UserException("Korisnik nije pronađen.");
        user.UserRole = roleId;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId)
            ?? throw new UserException("Korisnik nije pronađen.");
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SetPasswordAsync(string email, string hashedPassword)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new UserException("Korisnik nije pronađen.");
        if (!string.IsNullOrEmpty(user.Password))
            throw new UserException("Lozinka je već postavljena.");
        user.Password = hashedPassword;
        user.UserVerified = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetProfileAsync(int userId)
        => await _dbContext.Users
            .Include(u => u.MunicipalityNavigation)
            .FirstOrDefaultAsync(u => u.Id == userId);

    public async Task UpdateProfileAsync(int userId, string? email, string? firstName,
        string? lastName, DateTime? dateOfBirth, int? municipalityId,
        bool clearMunicipality, string? profileImageBase64)
    {
        var user = await _dbContext.Users.FindAsync(userId)
            ?? throw new UserException("Korisnik nije pronađen.");
        user.ApplyProfileUpdate(email, firstName, lastName, dateOfBirth,
            municipalityId, clearMunicipality, profileImageBase64);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
        => await _dbContext.Users.FindAsync(userId);

    public async Task ChangePasswordAsync(int userId, string hashedPassword)
    {
        var user = await _dbContext.Users.FindAsync(userId)
            ?? throw new UserException("Korisnik nije pronađen.");
        user.Password = hashedPassword;
        await _dbContext.SaveChangesAsync();
    }

    public async Task SetResetTokenAsync(string email, string? token, DateTime? expiry)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new UserException("Korisnik nije pronađen.");
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = expiry;
        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearMustChangePasswordAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId)
            ?? throw new UserException("Korisnik nije pronađen.");
        user.MustChangePassword = false;
        await _dbContext.SaveChangesAsync();
    }
}
