namespace DAL.Repositories;

public class RefreshTokenRepository(eZboriDbContext dbContext)
    : GenericRepository<RefreshToken>(dbContext), IRefreshTokenRepository
{
    private readonly eZboriDbContext _context = dbContext;

    public async Task<RefreshToken?> GetRefreshToken(int userId)
        => await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);

    public async Task<RefreshToken?> GetRefreshToken(string refreshToken)
        => await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

    public async Task<RefreshToken> UpsertToken(RefreshToken refreshToken)
    {
        if (refreshToken?.Id > 0)
            _context.RefreshTokens.Update(refreshToken);
        else
            await _context.RefreshTokens.AddAsync(refreshToken);

        await _context.SaveChangesAsync();

        return refreshToken;
    }
}
